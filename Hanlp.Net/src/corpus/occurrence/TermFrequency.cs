/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/8 1:16</create-date>
 *
 * <copyright file="TermFrequency.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.MDAG;

namespace com.hankcs.hanlp.corpus.occurrence;


/**
 * 词与词频的简单封装
 * @author hankcs
 */
public class TermFrequency : AbstractMap<string,int>.SimpleEntry<string, int> , IComparable<TermFrequency>
{
    public TermFrequency(string term)
        : this(term, 1)
    {
        ;
    }
    public TermFrequency(string term, int frequency)
        : base(term, frequency)
    {
        ;
    }


    /**
     * 频次增加若干
     * @param number
     * @return
     */
    public int increase(int number)
    {
        setValue(Value + number);
        return Value;
    }

    public string getTerm()
    {
        return Key;
    }

    public int getFrequency()
    {
        return Value;
    }

    /**
     * 频次加一
     * @return
     */
    public int increase()
    {
        return increase(1);
    }

    //@Override
    public int CompareTo(TermFrequency? o)
    {
        if (this.getFrequency().CompareTo(o.getFrequency()) == 0) 
            return Key.CompareTo(o.Key);
        return this.getFrequency().CompareTo(o.getFrequency());
    }
}
