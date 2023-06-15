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
 * 繁体转台湾繁体
 * @author hankcs
 */
public class TraditionalToTaiwanChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();
    static
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "t2tw";
        if (!loadDat(datPath, trie))
        {
            TreeMap<string, string> t2tw = new TreeMap<string, string>();
            if (!load(t2tw, false, HanLP.Config.tcDictionaryRoot + "t2tw.txt"))
            {
                throw new IllegalArgumentException("繁体转台湾繁体加载失败");
            }
            trie.build(t2tw);
            saveDat(datPath, trie, t2tw.entrySet());
        }
        logger.info("繁体转台湾繁体加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToTaiwanChinese(string traditionalTaiwanChineseString)
    {
        return segLongest(traditionalTaiwanChineseString.ToCharArray(), trie);
    }

    public static string convertToTaiwanChinese(char[] traditionalTaiwanChineseString)
    {
        return segLongest(traditionalTaiwanChineseString, trie);
    }
}
