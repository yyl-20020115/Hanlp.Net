/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-08-30 AM10:29</create-date>
 *
 * <copyright file="SimplifiedToTaiwanChineseDictionary.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;

namespace com.hankcs.hanlp.dictionary.ts;




/**
 * 简体转台湾繁体
 * @author hankcs
 */
public class SimplifiedToTaiwanChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();
    static SimplifiedToTaiwanChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        string datPath = HanLP.Config.tcDictionaryRoot + "s2tw";
        if (!loadDat(datPath, trie))
        {
            Dictionary<string, string> s2t = new Dictionary<string, string>();
            Dictionary<string, string> t2tw = new Dictionary<string, string>();
            if (!load(s2t, false, HanLP.Config.tcDictionaryRoot + "s2t.txt") ||
                    !load(t2tw, false, HanLP.Config.tcDictionaryRoot + "t2tw.txt"))
            {
                throw new ArgumentException("简体转台湾繁体词典加载失败");
            }
            combineChain(s2t, t2tw);
            trie.Build(s2t);
            saveDat(datPath, trie, s2t.entrySet());
        }
        logger.info("简体转台湾繁体词典加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToTraditionalTaiwanChinese(string simplifiedChineseString)
    {
        return segLongest(simplifiedChineseString.ToCharArray(), trie);
    }

    public static string convertToTraditionalTaiwanChinese(char[] simplifiedChinese)
    {
        return segLongest(simplifiedChinese, trie);
    }
}
