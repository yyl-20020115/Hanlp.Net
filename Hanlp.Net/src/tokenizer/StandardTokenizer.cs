/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/9 18:39</create-date>
 *
 * <copyright file="StandTokenizer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.tokenizer;



/**
 * 标准分词器
 * @author hankcs
 */
public class StandardTokenizer
{
    /**
     * 预置分词器
     */
    public static readonly Segment SEGMENT = HanLP.newSegment();

    /**
     * 分词
     * @param text 文本
     * @return 分词结果
     */
    public static List<Term> segment(string text)
    {
        return SEGMENT.seg(text.ToCharArray());
    }

    /**
     * 分词
     * @param text 文本
     * @return 分词结果
     */
    public static List<Term> segment(char[] text)
    {
        return SEGMENT.seg(text);
    }

    /**
     * 切分为句子形式
     * @param text 文本
     * @return 句子列表
     */
    public static List<List<Term>> seg2sentence(string text)
    {
        return SEGMENT.seg2sentence(text);
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
