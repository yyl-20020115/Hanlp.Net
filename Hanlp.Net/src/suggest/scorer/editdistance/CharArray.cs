/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/5 17:01</create-date>
 *
 * <copyright file="CharArray.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.suggest.scorer.editdistance;


/**
 * 对字符数组的封装，可以代替string
 * @author hankcs
 */
public class CharArray : IComparable<CharArray>, ISentenceKey<CharArray>
{
    char[] value;

    public CharArray(char[] value)
    {
        this.value = value;
    }

    //@Override
    public int CompareTo(CharArray? other)
    {
        int len1 = value.Length;
        int len2 = other.value.Length;
        int lim = Math.Min(len1, len2);
        char[] v1 = value;
        char[] v2 = other.value;

        int k = 0;
        while (k < lim)
        {
            char c1 = v1[k];
            char c2 = v2[k];
            if (c1 != c2)
            {
                return c1 - c2;
            }
            k++;
        }
        return len1 - len2;
    }

    //@Override
    public Double similarity(CharArray other)
    {
        int distance = EditDistance.compute(this.value, other.value) + 1;
        return 1.0 / distance;
    }
}
