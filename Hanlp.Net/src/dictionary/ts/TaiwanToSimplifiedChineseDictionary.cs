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
 * 台湾繁体转简体
 * @author hankcs
 */
public class TaiwanToSimplifiedChineseDictionary : BaseChineseDictionary
{
    static AhoCorasickDoubleArrayTrie<String> trie = new AhoCorasickDoubleArrayTrie<String>();
    static TaiwanToSimplifiedChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        String datPath = HanLP.Config.tcDictionaryRoot + "tw2s";
        if (!loadDat(datPath, trie))
        {
            var t2s = new Dictionary<String, String>();
            var tw2t = new Dictionary<String, String>();
            if (!load(t2s, false, HanLP.Config.tcDictionaryRoot + "t2s.txt") ||
                    !load(tw2t, true, HanLP.Config.tcDictionaryRoot + "t2tw.txt"))
            {
                throw new IllegalArgumentException("台湾繁体转简体词典加载失败");
            }
            combineReverseChain(t2s, tw2t, true);
            trie.build(t2s);
            saveDat(datPath, trie, t2s.entrySet());
        }
        logger.info("台湾繁体转简体词典加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static String convertToSimplifiedChinese(String traditionalTaiwanChinese)
    {
        return segLongest(traditionalTaiwanChinese.ToCharArray(), trie);
    }

    public static String convertToSimplifiedChinese(char[] traditionalTaiwanChinese)
    {
        return segLongest(traditionalTaiwanChinese, trie);
    }
}
