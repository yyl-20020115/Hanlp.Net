/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 20:20</create-date>
 *
 * <copyright file="NLPTokenizer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.tokenizer;



/**
 * 繁体中文分词器
 *
 * @author hankcs
 */
public class TraditionalChineseTokenizer
{
    /**
     * 预置分词器
     */
    public static Segment SEGMENT = HanLP.newSegment();

    private static List<Term> segSentence(string text)
    {
        string sText = CharTable.convert(text);
        List<Term> termList = SEGMENT.seg(sText);
        int offset = 0;
        foreach (Term term in termList)
        {
            term.offset = offset;
            term.word = text.substring(offset, offset + term.Length);
            offset += term.Length;
        }

        return termList;
    }

    public static List<Term> segment(string text)
    {
        List<Term> termList = new ();
        foreach (string sentence in SentencesUtil.toSentenceList(text))
        {
            termList.AddRange(segSentence(sentence));
        }

        return termList;
    }

    /**
     * 分词
     *
     * @param text 文本
     * @return 分词结果
     */
    public static List<Term> segment(char[] text)
    {
        return segment(CharTable.convert(text));
    }

    /**
     * 切分为句子形式
     *
     * @param text 文本
     * @return 句子列表
     */
    public static List<List<Term>> seg2sentence(string text)
    {
        List<List<Term>> resultList = new ();
        {
            foreach (string sentence in SentencesUtil.toSentenceList(text))
            {
                resultList.Add(segment(sentence));
            }
        }

        return resultList;
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
}
