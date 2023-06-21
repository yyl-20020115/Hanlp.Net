/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/9 20:57</create-date>
 *
 * <copyright file="FeatureFunction.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.model.crf;



/**
 * 特征函数，其实是tag.size个特征函数的集合
 * @author hankcs
 */
public class FeatureFunction : ICacheAble
{
    /**
     * 环境参数
     */
    public char[] o;
    /**
     * 标签参数
     */
//    string s;

    /**
     * 权值，按照index对应于tag的id
     */
    public double[] w;

    public FeatureFunction(char[] o, int tagSize)
    {
        this.o = o;
        w = new double[tagSize];
    }

    public FeatureFunction()
    {
    }

    public FeatureFunction(string o, int tagSize)
        : this(o.ToCharArray(), tagSize)
    {
        ;
    }

    //@Override
    public void save(Stream _out)
    {
        _out.writeInt(o.Length);
        foreach (char c in o)
        {
            _out.writeChar(c);
        }
        _out.writeInt(w.Length);
        foreach (double v in w)
        {
            _out.writeDouble(v);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        int size = byteArray.Next();
        o = new char[size];
        for (int i = 0; i < size; ++i)
        {
            o[i] = byteArray.nextChar();
        }
        size = byteArray.Next();
        w = new double[size];
        for (int i = 0; i < size; ++i)
        {
            w[i] = byteArray.nextDouble();
        }
        return true;
    }
}
