/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/13 20:30</create-date>
 *
 * <copyright file="EditDistance.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dictionary.common;

namespace com.hankcs.hanlp.algorithm;



/**
 * 基于语义距离的编辑距离实现
 * @author hankcs
 */
public class EditDistance
{
    public static long Compute(List<CommonSynonymDictionary.SynonymItem> synonymItemListA, List<CommonSynonymDictionary.SynonymItem> synonymItemListB)
    {
        long[] arrayA = new long[synonymItemListA.Count];
        long[] arrayB = new long[synonymItemListB.Count];
        int i = 0;
        foreach(CommonSynonymDictionary.SynonymItem item in synonymItemListA)
        {
            arrayA[i++] = item.entry.id;
        }
        i = 0;
        foreach (CommonSynonymDictionary.SynonymItem item in synonymItemListB)
        {
            arrayB[i++] = item.entry.id;
        }
        return Compute(arrayA, arrayB);
    }

    public static long Compute(long[] arrayA, long[] arrayB)
    {
        int m = arrayA.Length;
        int n = arrayB.Length;
        if (m == 0 || n == 0) return long.MaxValue / 3;

        long[,] d = new long[m + 1,n + 1];
        for (int j = 0; j <= n; ++j)
        {
            d[0, j] = j;
        }
        for (int i = 0; i <= m; ++i)
        {
            d[i, 0] = i;
        }

        for (int i = 1; i <= m; ++i)
        {
            long ci = arrayA[i - 1];
            for (int j = 1; j <= n; ++j)
            {
                long cj = arrayB[j - 1];
                if (ci == cj)
                {
                    d[i, j] = d[i - 1, j - 1];
                }
//                else if (i > 1 && j > 1 && ci == arrayA[j - 2] && cj == arrayB[i - 2])
//                {
//                    // 交错相等
//                    d[i][j] = 1 + Math.Min(d[i - 2][j - 2], Math.Min(d[i][j - 1], d[i - 1][j]));
//                }
                else
                {
                    // 等号右边的分别代表 将ci改成cj                                    错串加cj         错串删ci
                    d[i, j] = Math.Min(d[i - 1, j - 1] + Math.Abs(ci - cj), Math.Min(d[i, j - 1] + cj, d[i - 1, j] + ci));
                }
            }
        }

        return d[m, n];
    }

    public static int Compute(int[] arrayA, int[] arrayB)
    {
        int m = arrayA.Length;
        int n = arrayB.Length;
        if (m == 0 || n == 0) return int.MaxValue / 3;

        var d = new int[m + 1,n + 1];
        for (int j = 0; j <= n; ++j)
        {
            d[0,j] = j;
        }
        for (int i = 0; i <= m; ++i)
        {
            d[i,0] = i;
        }

        for (int i = 1; i <= m; ++i)
        {
            int ci = arrayA[i - 1];
            for (int j = 1; j <= n; ++j)
            {
                int cj = arrayB[j - 1];
                if (ci == cj)
                {
                    d[i,j] = d[i - 1,j - 1];
                }
//                else if (i > 1 && j > 1 && ci == arrayA[j - 2] && cj == arrayB[i - 2])
//                {
//                    // 交错相等
//                    d[i][j] = 1 + Math.Min(d[i - 2][j - 2], Math.Min(d[i][j - 1], d[i - 1][j]));
//                }
                else
                {
                    // 等号右边的分别代表 将ci改成cj                                    错串加cj         错串删ci
                    d[i,j] = Math.Min(d[i - 1,j - 1] + Math.Abs(ci - cj), Math.Min(d[i,j - 1] + cj, d[i - 1,j] + ci));
                }
            }
        }

        return d[m,n];
    }

    /**
     * 编辑距离
     *
     * @param a 串A，其实它们两个调换位置还是一样的
     * @param b 串B
     * @return 它们之间的距离
     */
    public static int Compute(string a, string b)
    {
        return Ed(a, b);
    }

    /**
     * 编辑距离
     *
     * @param wrongWord 串A，其实它们两个调换位置还是一样的
     * @param rightWord 串B
     * @return 它们之间的距离
     */
    public static int Ed(string wrongWord, string rightWord)
    {
        int m = wrongWord.Length;
        int n = rightWord.Length;

        int[,] d = new int[m + 1,n + 1];
        for (int j = 0; j <= n; ++j)
        {
            d[0,j] = j;
        }
        for (int i = 0; i <= m; ++i)
        {
            d[i,0] = i;
        }

        for (int i = 1; i <= m; ++i)
        {
            char ci = wrongWord[(i - 1)];
            for (int j = 1; j <= n; ++j)
            {
                char cj = rightWord[(j - 1)];
                if (ci == cj)
                {
                    d[i,j] = d[i - 1,j - 1];
                }
                else if (i > 1 && j > 1 && ci == rightWord[(j - 2)] && cj == wrongWord[(i - 2)])
                {
                    // 交错相等
                    d[i,j] = 1 + Math.Min(d[i - 2,j - 2], Math.Min(d[i,j - 1], d[i - 1,j]));
                }
                else
                {
                    // 等号右边的分别代表 将ci改成cj                   错串加cj         错串删ci
                    d[i,j] = Math.Min(d[i - 1,j - 1] + 1, Math.Min(d[i,j - 1] + 1, d[i - 1,j] + 1));
                }
            }
        }

        return d[m,n];
    }

    /**
     * 编辑距离
     *
     * @param wrongWord 串A，其实它们两个调换位置还是一样的
     * @param rightWord 串B
     * @return 它们之间的距离
     */
    public static int Compute(char[] wrongWord, char[] rightWord)
    {
        int m = wrongWord.Length;
        int n = rightWord.Length;

        int[,] d = new int[m + 1, n + 1];
        for (int j = 0; j <= n; ++j)
        {
            d[0, j] = j;
        }
        for (int i = 0; i <= m; ++i)
        {
            d[i, 0] = i;
        }

        for (int i = 1; i <= m; ++i)
        {
            char ci = wrongWord[i - 1];
            for (int j = 1; j <= n; ++j)
            {
                char cj = rightWord[j - 1];
                if (ci == cj)
                {
                    d[i,j] = d[i - 1,j - 1];
                }
                else if (i > 1 && j > 1 && ci == rightWord[j - 2] && cj == wrongWord[i - 2])
                {
                    // 交错相等
                    d[i,j] = 1 + Math.Min(d[i - 2,j - 2], Math.Min(d[i,j - 1], d[i - 1,j]));
                }
                else
                {
                    // 等号右边的分别代表 将ci改成cj                   错串加cj         错串删ci
                    d[i,j] = Math.Min(d[i - 1,j - 1] + 1, Math.Min(d[i,j - 1] + 1, d[i - 1,j] + 1));
                }
            }
        }

        return d[m,n];
    }
}
