/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/5 19:35</create-date>
 *
 * <copyright file="CharArray.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.collection.sequence;


/**
 * (SimpleString)字符串，因为string内部的char[]无法访问，而许多任务经常操作char[]，所以封装了这个结构。
 *
 * @author hankcs
 */
public class SString : IComparable<SString>
{
    public char[] value;
    /**
     * 开始位置，包含
     */
    public int b;
    /**
     * 结束位置，不包含
     */
    public int e;

    /**
     * 建立一个字符串
     *
     * @param value
     * @param b
     * @param e
     */
    public SString(char[] value, int b, int e)
    {
        this.value = value;
        this.b = b;
        this.e = e;
    }

    public SString(string s)
    {
        value = s.ToCharArray();
        b = 0;
        e = s.Length;
    }

    //@Override
    public override bool Equals(Object? anObject)
    {
        if (this == anObject)
        {
            return true;
        }
        if (anObject is SString)
        {
            SString anotherString = (SString) anObject;
            int n = value.Length;
            if (n == anotherString.value.Length)
            {
                char[] v1 = value;
                char[] v2 = anotherString.value;
                int i = 0;
                while (n-- != 0)
                {
                    if (v1[i] != v2[i])
                        return false;
                    i++;
                }
                return true;
            }
        }
        return false;
    }

    //@Override
    public int Length()
    {
        return e - b;
    }

    //@Override
    public char charAt(int index)
    {
        return value[b + index];
    }

    //@Override
    public SString subSequence(int start, int end)
    {
        return new SString(value, b + start, b + end);
    }

    //@Override
    public override string ToString()
    {
        return new string(value, b, e - b);
    }

    //@Override
    public int CompareTo(SString? anotherString)
    {
        int len1 = value.Length;
        int len2 = anotherString.value.Length;
        int lim = Math.Min(len1, len2);
        char []v1 = value;
        char []v2 = anotherString.value;

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

    public char[] ToCharArray()
    {
        return Arrays.copyOfRange(value, b, e);
    }

    public static SString valueOf(char word)
    {
        SString s = new SString(new char[]{word}, 0, 1);

        return s;
    }

    public SString Add(SString other)
    {
        char[] value = new char[Length + other.Length];
        Array.Copy(this.value, b, value, 0, Length);
        Array.Copy(other.value, other.b, value, Length, other.Length);
        b = 0;
        e = Length + other.Length;
        this.value = value;

        return this;
    }
}
