/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/9 21:34</create-date>
 *
 * <copyright file="Table.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.crf;

/**
 * 给一个实例生成一个元素表
 * @author hankcs
 */
public class Table
{
    /**
     * 真实值，请不要直接读取
     */
    public string[][] v;
    static readonly string HEAD = "_B";

    //@Override
    public override string ToString()
    {
        if (v == null) return "null";
        StringBuilder sb = new StringBuilder(v.Length * v[0].Length * 2);
        for (string[] line : v)
        {
            for (string element : line)
            {
                sb.Append(element).Append('\t');
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    /**
     * 获取表中某一个元素
     * @param x
     * @param y
     * @return
     */
    public string get(int x, int y)
    {
        if (x < 0) return HEAD + x;
        if (x >= v.Length) return HEAD + "+" + (x - v.Length + 1);

        return v[x][y];
    }

    public void setLast(int x, string t)
    {
        v[x][v[x].Length - 1] = t;
    }

    public int size()
    {
        return v.Length;
    }
}
