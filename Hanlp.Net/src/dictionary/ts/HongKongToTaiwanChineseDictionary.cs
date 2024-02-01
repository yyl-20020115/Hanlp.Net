/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-08-30 AM10:29</create-date>
 *
 * <copyright file="SimplifiedToHongKongChineseDictionary.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;

namespace com.hankcs.hanlp.dictionary.ts;




/**
 * 香港繁体转台湾繁体
 *
 * @author hankcs
 */
public class HongKongToTaiwanChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();

    static HongKongToTaiwanChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "hk2tw";
        if (!loadDat(datPath, trie))
        {
            Dictionary<string, string> t2tw = new Dictionary<string, string>();
            Dictionary<string, string> hk2t = new Dictionary<string, string>();
            if (!load(t2tw, false, HanLP.Config.tcDictionaryRoot + "t2tw.txt") ||
                    !load(hk2t, true, HanLP.Config.tcDictionaryRoot + "t2hk.txt"))
            {
                throw new ArgumentException("香港繁体转台湾繁体词典加载失败");
            }
            combineReverseChain(t2tw, hk2t, false);
            trie.Build(t2tw);
            saveDat(datPath, trie, t2tw.entrySet());
        }
        logger.info("香港繁体转台湾繁体词典加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToTraditionalTaiwanChinese(string traditionalHongKongChinese)
    {
        return segLongest(traditionalHongKongChinese.ToCharArray(), trie);
    }

    public static string convertToTraditionalTaiwanChinese(char[] traditionalHongKongChinese)
    {
        return segLongest(traditionalHongKongChinese, trie);
    }
}
