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
 *
 * @author hankcs
 */
public class TriaFrequency : PairFrequency
{
    public string third;

    private TriaFrequency(string term, int frequency)
        :base(term, frequency)
    {
        ;
    }

    private TriaFrequency(string term)
        : base(term)
    {
       ;
    }

    /**
     * 构造一个三阶接续，正向
     *
     * @param first
     * @param second
     * @param third
     * @param delimiter 一般使用RIGHT！
     * @return
     */
    public static TriaFrequency create(string first, char delimiter, string second, string third)
    {
        TriaFrequency triaFrequency = new TriaFrequency(first + delimiter + second + Occurrence.RIGHT + third);
        triaFrequency.first = first;
        triaFrequency.second = second;
        triaFrequency.third = third;
        triaFrequency.delimiter = delimiter;
        return triaFrequency;
    }

    /**
     * 构造一个三阶接续，逆向
     * @param second
     * @param third
     * @param delimiter 一般使用LEFT
     * @param first
     * @return
     */
    public static TriaFrequency create(string second, string third, char delimiter, string first)
    {
        TriaFrequency triaFrequency = new TriaFrequency(second + Occurrence.RIGHT + third + delimiter + first);
        triaFrequency.first = first;
        triaFrequency.second = second;
        triaFrequency.third = third;
        triaFrequency.delimiter = delimiter;
        return triaFrequency;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Key.replace(Occurrence.LEFT, '←').replace(Occurrence.RIGHT, '→'));
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
        return sb.ToString();
    }
}
