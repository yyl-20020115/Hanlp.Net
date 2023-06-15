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
 * 台湾繁体转繁体
 * @author hankcs
 */
public class TaiwanToTraditionalChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<String> trie = new AhoCorasickDoubleArrayTrie<String>();
    static
    {
        long start = DateTime.Now.Microsecond;
        String datPath = HanLP.Config.tcDictionaryRoot + "tw2t";
        if (!loadDat(datPath, trie))
        {
            TreeMap<String, String> tw2t = new TreeMap<String, String>();
            if (!load(tw2t, true, HanLP.Config.tcDictionaryRoot + "t2tw.txt"))
            {
                throw new IllegalArgumentException("台湾繁体转繁体加载失败");
            }
            trie.build(tw2t);
            saveDat(datPath, trie, tw2t.entrySet());
        }
        logger.info("台湾繁体转繁体加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static String convertToTraditionalChinese(String traditionalTaiwanChineseString)
    {
        return segLongest(traditionalTaiwanChineseString.ToCharArray(), trie);
    }

    public static String convertToTraditionalChinese(char[] traditionalTaiwanChineseString)
    {
        return segLongest(traditionalTaiwanChineseString, trie);
    }
}
