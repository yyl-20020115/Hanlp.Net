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
 * 香港繁体转简体
 * @author hankcs
 */
public class HongKongToSimplifiedChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();
    static HongKongToSimplifiedChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "hk2s";
        if (!loadDat(datPath, trie))
        {
            Dictionary<string, string> t2s = new Dictionary<string, string>();
            Dictionary<string, string> hk2t = new Dictionary<string, string>();
            if (!load(t2s, false, HanLP.Config.tcDictionaryRoot + "t2s.txt") ||
                    !load(hk2t, true, HanLP.Config.tcDictionaryRoot + "t2hk.txt"))
            {
                throw new ArgumentException("香港繁体转简体加载失败");
            }
            combineReverseChain(t2s, hk2t, true);
            trie.build(t2s);
            saveDat(datPath, trie, t2s.entrySet());
        }
        logger.info("香港繁体转简体加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToSimplifiedChinese(string traditionalHongKongChineseString)
    {
        return segLongest(traditionalHongKongChineseString.ToCharArray(), trie);
    }

    public static string convertToSimplifiedChinese(char[] traditionalHongKongChineseString)
    {
        return segLongest(traditionalHongKongChineseString, trie);
    }
}
