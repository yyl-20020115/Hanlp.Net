/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/17 9:47</create-date>
 *
 * <copyright file="BinSearch.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.algorithm;

using System.Collections.Generic;

/**
 * 求两个集合中最相近的两个数
 *
 * @author hankcs
 */
public class ArrayDistance
{
    public static long ComputeMinimumDistance(HashSet<long> setA, HashSet<long> setB)
    {
        long[] arrayA = setA.ToArray();
        long[] arrayB = setB.ToArray();
       return ComputeMinimumDistance(arrayA, arrayB);
    }

    public static long ComputeMinimumDistance(long[] arrayA, long[] arrayB)
    {
        int aIndex = 0;
        int bIndex = 0;
        long min = Math.Abs(arrayA[0] - arrayB[0]);
        while (true)
        {
            if (arrayA[aIndex] > arrayB[bIndex])
            {
                bIndex++;
            }
            else
            {
                aIndex++;
            }
            if (aIndex >= arrayA.Length || bIndex >= arrayB.Length)
            {
                break;
            }
            if (Math.Abs(arrayA[aIndex] - arrayB[bIndex]) < min)
            {
                min = Math.Abs(arrayA[aIndex] - arrayB[bIndex]);
            }
        }

        return min;
    }

    public static long ComputeAverageDistance(long[] arrayA, long[] arrayB)
    {
        long totalA = 0L;
        long totalB = 0L;
        foreach (long a in arrayA) totalA += a;
        foreach (long b in arrayB) totalB += b;

        return Math.Abs(totalA / arrayA.Length - totalB / arrayB.Length);
    }
}
