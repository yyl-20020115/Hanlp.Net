/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2015/3/13 18:36</create-date>
 *
 * <copyright file="CommonSuffixExtractor.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * 公共后缀提取工具
 * @author hankcs
 */
public class CommonSuffixExtractor
{
    TFDictionary tfDictionary;

    public CommonSuffixExtractor()
    {
        tfDictionary = new TFDictionary();
    }

    public void add(string key)
    {
        tfDictionary.add(key);
    }

    public List<string> extractSuffixExtended(int Length, int size)
    {
        return extractSuffix(Length, size, true);
    }

    /**
     * 提取公共后缀
     * @param Length 公共后缀长度
     * @param size 频率最高的前多少个公共后缀
     * @param extend 长度是否拓展为从1到Length为止的后缀
     * @return 公共后缀列表
     */
    public List<string> extractSuffix(int Length, int size, bool extend)
    {
        TFDictionary suffixTreeSet = new TFDictionary();
        for (string key : tfDictionary.keySet())
        {
            if (key.Length > Length)
            {
                suffixTreeSet.add(key.substring(key.Length - Length, key.Length));
                if (extend)
                {
                    for (int l = 1; l < Length; ++l)
                    {
                        suffixTreeSet.add(key.substring(key.Length - l, key.Length));
                    }
                }
            }
        }

        if (extend)
        {
            size *= Length;
        }

        return extract(suffixTreeSet, size);
    }

    private static List<string> extract(TFDictionary suffixTreeSet, int size)
    {
        List<string> suffixList = new ArrayList<string>(size);
        for (TermFrequency termFrequency : suffixTreeSet.values())
        {
            if (suffixList.size() >= size) break;
            suffixList.add(termFrequency.getKey());
        }

        return suffixList;
    }

    /**
     * 此方法认为后缀一定是整个的词语，所以Length是以词语为单位的
     * @param Length
     * @param size
     * @param extend
     * @return
     */
    public List<string> extractSuffixByWords(int Length, int size, bool extend)
    {
        TFDictionary suffixTreeSet = new TFDictionary();
        for (string key : tfDictionary.keySet())
        {
            List<Term> termList = StandardTokenizer.segment(key);
            if (termList.size() > Length)
            {
                suffixTreeSet.add(combine(termList.subList(termList.size() - Length, termList.size())));
                if (extend)
                {
                    for (int l = 1; l < Length; ++l)
                    {
                        suffixTreeSet.add(combine(termList.subList(termList.size() - l, termList.size())));
                    }
                }
            }
        }

        return extract(suffixTreeSet, size);
    }


    private static string combine(List<Term> termList)
    {
        StringBuilder sbResult = new StringBuilder();
        for (Term term : termList)
        {
            sbResult.Append(term.word);
        }

        return sbResult.toString();
    }
}
