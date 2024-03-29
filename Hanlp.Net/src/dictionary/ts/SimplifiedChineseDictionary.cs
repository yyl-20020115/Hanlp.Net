/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 23:04</create-date>
 *
 * <copyright file="SimplifiedChineseDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.ts;



/**
 * 简体=繁体词典
 * @author hankcs
 */
public class SimplifiedChineseDictionary : BaseChineseDictionary
{
    /**
     * 简体=繁体
     */
    static collection.AhoCorasick.AhoCorasickDoubleArrayTrie<string> trie = new collection.AhoCorasick.AhoCorasickDoubleArrayTrie<string>();
    
    static SimplifiedChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        if (!load(HanLP.Config.tcDictionaryRoot + "s2t.txt", trie, false))
        {
            throw new ArgumentException("简繁词典" + HanLP.Config.tcDictionaryRoot + "s2t.txt" + Predefine.BIN_EXT + "加载失败");
        }

        logger.info("简繁词典" + HanLP.Config.tcDictionaryRoot + "s2t.txt" + Predefine.BIN_EXT + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToTraditionalChinese(string simplifiedChineseString)
    {
        return segLongest(simplifiedChineseString.ToCharArray(), trie);
    }

    public static string convertToTraditionalChinese(char[] simplifiedChinese)
    {
        return segLongest(simplifiedChinese, trie);
    }

    public static string getTraditionalChinese(string simplifiedChinese)
    {
        return trie.Get(simplifiedChinese);
    }
}
