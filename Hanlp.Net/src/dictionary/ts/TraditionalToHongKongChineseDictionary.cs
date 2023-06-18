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
 * 繁体转香港繁体
 * @author hankcs
 */
public class TraditionalToHongKongChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();
    static
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "t2hk";
        if (!loadDat(datPath, trie))
        {
            Dictionary<string, string> t2hk = new Dictionary<string, string>();
            if (!load(t2hk, false, HanLP.Config.tcDictionaryRoot + "t2hk.txt"))
            {
                throw new ArgumentException("繁体转香港繁体加载失败");
            }
            trie.build(t2hk);
            saveDat(datPath, trie, t2hk.entrySet());
        }
        logger.info("繁体转香港繁体加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToHongKongTraditionalChinese(string traditionalChineseString)
    {
        return segLongest(traditionalChineseString.ToCharArray(), trie);
    }

    public static string convertToHongKongTraditionalChinese(char[] traditionalHongKongChineseString)
    {
        return segLongest(traditionalHongKongChineseString, trie);
    }
}
