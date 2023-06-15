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
    {
        super(term, frequency);
    }

    protected PairFrequency(string term)
    {
        super(term);
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
        PairFrequency pairFrequency = new PairFrequency(first + delimiter + second);
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
    public string toString()
    {
        final StringBuilder sb = new StringBuilder();
        sb.Append(first);
        sb.Append(isRight() ? '→' : '←');
        sb.Append(second);
        sb.Append('=');
        sb.Append(" tf=");
        sb.Append(getValue());
        sb.Append(' ');
        sb.Append("mi=");
        sb.Append(mi);
        sb.Append(" le=");
        sb.Append(le);
        sb.Append(" re=");
        sb.Append(re);
        sb.Append(" score=");
        sb.Append(score);
        return sb.toString();
    }
}
