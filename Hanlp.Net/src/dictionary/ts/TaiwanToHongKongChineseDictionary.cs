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
namespace com.hankcs.hanlp.dictionary.ts;




/**
 * 台湾繁体转香港繁体
 *
 * @author hankcs
 */
public class TaiwanToHongKongChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();

    static
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "tw2hk";
        if (!loadDat(datPath, trie))
        {
            TreeMap<string, string> t2hk = new TreeMap<string, string>();
            TreeMap<string, string> tw2t = new TreeMap<string, string>();
            if (!load(t2hk, false, HanLP.Config.tcDictionaryRoot + "t2hk.txt") ||
                    !load(tw2t, true, HanLP.Config.tcDictionaryRoot + "t2tw.txt"))
            {
                throw new IllegalArgumentException("台湾繁体转香港繁体词典加载失败");
            }
            combineReverseChain(t2hk, tw2t, false);
            trie.build(t2hk);
            saveDat(datPath, trie, t2hk.entrySet());
        }
        logger.info("台湾繁体转香港繁体词典加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToTraditionalHongKongChinese(string traditionalTaiwanChinese)
    {
        return segLongest(traditionalTaiwanChinese.ToCharArray(), trie);
    }

    public static string convertToTraditionalHongKongChinese(char[] traditionalTaiwanChinese)
    {
        return segLongest(traditionalTaiwanChinese, trie);
    }
}
