/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 11:06</create-date>
 *
 * <copyright file="CoNllLine.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.corpus.dependency.CoNll;

/**
 * CoNLL语料中的一行
 * @author hankcs
 */
public class CoNllLine
{
    /**
     * 十个值
     */
    public string[] value = new string[10];

    /**
     * 第一个值化为id
     */
    public int id;

    public CoNllLine(params string[] args)
    {
        int Length = Math.Min(args.Length, value.Length);
        for (int i = 0; i < Length; ++i)
        {
            value[i] = args[i];
        }
        id = int.parseInt(value[0]);
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (string value in this.value)
        {
            sb.Append(value);
            sb.Append('\t');
        }
        return sb.deleteCharAt(sb.Length - 1).ToString();
    }
}
