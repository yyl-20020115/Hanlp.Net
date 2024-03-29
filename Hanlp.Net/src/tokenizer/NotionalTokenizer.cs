/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/8 1:58</create-date>
 *
 * <copyright file="NotionalTokenizer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dictionary.stopword;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.tokenizer;



/**
 * 实词分词器，自动移除停用词
 *
 * @author hankcs
 */
public class NotionalTokenizer
{
    /**
     * 预置分词器
     */
    public static Segment SEGMENT = HanLP.newSegment();

    public static List<Term> segment(string text)
    {
        return segment(text.ToCharArray());
    }

    /**
     * 分词
     *
     * @param text 文本
     * @return 分词结果
     */
    public static List<Term> segment(char[] text)
    {
        List<Term> resultList = SEGMENT.seg(text);
        IEnumerator<Term> listIterator = resultList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            if (!CoreStopWordDictionary.shouldInclude(listIterator.next()))
            {
                listIterator.Remove();
            }
        }

        return resultList;
    }

    /**
     * 切分为句子形式
     *
     * @param text
     * @return
     */
    public static List<List<Term>> seg2sentence(string text)
    {
        List<List<Term>> sentenceList = SEGMENT.seg2sentence(text);
        foreach (List<Term> sentence in sentenceList)
        {
            IEnumerator<Term> listIterator = sentence.GetEnumerator();
            while (listIterator.MoveNext())
            {
                if (!CoreStopWordDictionary.shouldInclude(listIterator.next()))
                {
                    listIterator.Remove();
                }
            }
        }

        return sentenceList;
    }

    /**
     * 分词断句 输出句子形式
     *
     * @param text     待分词句子
     * @param shortest 是否断句为最细的子句（将逗号也视作分隔符）
     * @return 句子列表，每个句子由一个单词列表组成
     */
    public List<List<Term>> seg2sentence(string text, bool shortest)
    {
        return SEGMENT.seg2sentence(text, shortest);
    }

    /**
     * 切分为句子形式
     *
     * @param text
     * @param filterArrayChain 自定义过滤器链
     * @return
     */
    public static List<List<Term>> seg2sentence(string text, params Filter[] filterArrayChain)
    {
        List<List<Term>> sentenceList = SEGMENT.seg2sentence(text);
        foreach (List<Term> sentence in sentenceList)
        {
            IEnumerator<Term> listIterator = sentence.GetEnumerator();
            while (listIterator.MoveNext())
            {
                if (filterArrayChain != null)
                {
                    Term term = listIterator.next();
                    foreach (Filter filter in filterArrayChain)
                    {
                        if (!filter.shouldInclude(term))
                        {
                            listIterator.Remove();
                            break;
                        }
                    }
                }
            }
        }

        return sentenceList;
    }
}
