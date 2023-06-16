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
 * 香港繁体转繁体
 * @author hankcs
 */
public class HongKongToTraditionalChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();
    static HongKongToTraditionalChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "hk2t";
        if (!load(datPath, trie))
        {
            var hk2t = new Dictionary<string, string>();
            if (!load(hk2t, true, HanLP.Config.tcDictionaryRoot + "t2hk.txt"))
            {
                throw new IllegalArgumentException("香港繁体转繁体加载失败");
            }
            trie.build(hk2t);
            saveDat(datPath, trie, hk2t.ToHashSet());
        }
        logger.info("香港繁体转繁体加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToTraditionalChinese(string traditionalHongKongChineseString)
    {
        return segLongest(traditionalHongKongChineseString.ToCharArray(), trie);
    }

    public static string convertToTraditionalChinese(char[] traditionalHongKongChineseString)
    {
        return segLongest(traditionalHongKongChineseString, trie);
    }
}
