/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/8 17:00</create-date>
 *
 * <copyright file="PairFrequency.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.seg.common;
using System.Text;

namespace com.hankcs.hanlp.corpus.occurrence;

/**
 * 一个二元的词串的频度
 * @author hankcs
 */
public class PairFrequency : TermFrequency
{
    /**
     * 互信息值
     */
    public double mi;
    /**
     * 左信息熵
     */
    public double le;
    /**
     * 右信息熵
     */
    public double re;
    /**
     * 分数
     */
    public double score;
    public string first;
    public string second;
    public char delimiter;

    protected PairFrequency(string term, int frequency)
        :base(term, frequency)
    {
    }

    protected PairFrequency(string term)
        :base(term)
    {
    }

    /**
     * 构造一个pf
     * @param first
     * @param delimiter
     * @param second
     * @return
     */
    public static PairFrequency create(string first, char delimiter ,string second)
    {
        var pairFrequency = new PairFrequency(first + delimiter + second);
        pairFrequency.first = first;
        pairFrequency.delimiter = delimiter;
        pairFrequency.second = second;
        return pairFrequency;
    }

    /**
     * 该共现是否统计的是否是从左到右的顺序
     * @return
     */
    public bool isRight()
    {
        return delimiter == Occurrence.RIGHT;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(first);
        sb.Append(isRight() ? '→' : '←');
        sb.Append(second);
        sb.Append('=');
        sb.Append(" tf=");
        sb.Append(Value);
        sb.Append(' ');
        sb.Append("mi=");
        sb.Append(mi);
        sb.Append(" le=");
        sb.Append(le);
        sb.Append(" re=");
        sb.Append(re);
        sb.Append(" score=");
        sb.Append(score);
        return sb.ToString();
    }
}
