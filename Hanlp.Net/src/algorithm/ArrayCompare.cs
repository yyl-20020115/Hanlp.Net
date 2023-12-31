/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/17 14:15</create-date>
 *
 * <copyright file="ArrayCompare.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.algorithm;

/**
 * 比较两个数组
 * @author hankcs
 */
public class ArrayCompare
{
    /**
     * 比较数组A与B的大小关系
     * @param arrayA
     * @param arrayB
     * @return
     */
    public static int compare(long[] arrayA, long[] arrayB)
    {
        int len1 = arrayA.Length;
        int len2 = arrayB.Length;
        int lim = Math.Min(len1, len2);

        int k = 0;
        while (k < lim)
        {
            long c1 = arrayA[k];
            long c2 = arrayB[k];
            if (c1!=c2)
            {
                return c1.CompareTo(c2);
            }
            ++k;
        }
        return len1 - len2;
    }
}
