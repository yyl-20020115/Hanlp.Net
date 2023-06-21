/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/11 10:35</create-date>
 *
 * <copyright file="InputWrapper.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.seg.common.wrapper;



/**
 * 一个将TextReader wrap进来的类
 *
 * @author hankcs
 */
public class SegmentWrapper
{
    TextReader br;
    Segment segment;
    /**
     * 因为next是单个term出去的，所以在这里做一个记录
     */
    Term[] termArray;
    /**
     * termArray下标
     */
    int index;

    public SegmentWrapper(TextReader br, Segment segment)
    {
        this.br = br;
        this.segment = segment;
    }

    /**
     * 重置分词器
     *
     * @param br
     */
    public void reset(TextReader br)
    {
        this.br = br;
        termArray = null;
        index = 0;
    }

    public Term next() 
    {
        if (termArray != null && index < termArray.Length) return termArray[index++];
        string line = br.ReadLine();
        while (TextUtility.isBlank(line))
        {
            if (line == null) return null;
            line = br.ReadLine();
        }

        List<Term> termList = segment.seg(line);
        if (termList.Count == 0) return null;
        termArray = termList.ToArray();
        index = 0;

        return termArray[index++];
    }
}
