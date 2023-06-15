/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM5:28</create-date>
 *
 * <copyright file="Tag.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.common;

namespace com.hankcs.hanlp.model.perceptron.tagset;


/**
 * @author hankcs
 */
public class CWSTagSet : TagSet
{
    public readonly int B;
    public readonly int M;
    public readonly int E;
    public readonly int S;

    public CWSTagSet(int b, int m, int e, int s)
        :base(TaskType.CWS)
    {
        B = b;
        M = m;
        E = e;
        S = s;
        string[] id2tag = new string[4];
        id2tag[b] = "B";
        id2tag[m] = "M";
        id2tag[e] = "E";
        id2tag[s] = "S";
        foreach (string tag in id2tag)
        {
            add(tag);
        }
        _lock();
    }

    public CWSTagSet()
        : base(TaskType.CWS)
    {
        B = add("B");
        M = add("M");
        E = add("E");
        S = add("S");
        _lock();
    }
}
