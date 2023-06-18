/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 21:04</create-date>
 *
 * <copyright file="TraditionalChineseDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dictionary.ts;



/**
 * 繁简词典，提供简繁转换
 * @author hankcs
 */
public class TraditionalChineseDictionary : BaseChineseDictionary
{
    /**
     * 繁体=简体
     */
    public static AhoCorasickDoubleArrayTrie<string> trie = new AhoCorasickDoubleArrayTrie<string>();

    static TraditionalChineseDictionary()
    {
        long start = DateTime.Now.Microsecond;
        if (!load(HanLP.Config.tcDictionaryRoot + "t2s.txt", trie, false))
        {
            throw new ArgumentException("繁简词典" + HanLP.Config.tcDictionaryRoot + "t2s.txt" + "加载失败");
        }

        logger.info("繁简词典" + HanLP.Config.tcDictionaryRoot + "t2s.txt" + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    public static string convertToSimplifiedChinese(string traditionalChineseString)
    {
        return segLongest(traditionalChineseString.ToCharArray(), trie);
    }

    public static string convertToSimplifiedChinese(char[] traditionalChinese)
    {
        return segLongest(traditionalChinese, trie);
    }

}
