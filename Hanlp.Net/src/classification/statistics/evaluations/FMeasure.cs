/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/17 PM3:11</create-date>
 *
 * <copyright file="FMeasure.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.datrie;
using System.Text;

namespace com.hankcs.hanlp.classification.statistics.evaluations;


public class FMeasure : Serializable
{
    /**
     * 测试样本空间
     */
    public int size;
    /**
     * 平均准确率
     */
    public double average_accuracy;
    /**
     * 平均精确率
     */
    public double average_precision;
    /**
     * 平均召回率
     */
    public double average_recall;
    /**
     * 平均F1
     */
    public double average_f1;

    /**
     * 分类准确率
     */
    public double[] accuracy;
    /**
     * 分类精确率
     */
    public double[] precision;
    /**
     * 分类召回率
     */
    public double[] recall;
    /**
     * 分类F1
     */
    public double[] f1;
    /**
     * 分类名称
     */
    public string[] catalog;

    /**
     * 速度
     */
    public double speed;

    //@Override
    public override string ToString()
    {
        int l = -1;
        foreach (string c in catalog)
        {
            l = Math.Max(l, c.Length);
        }
         int w = 6;
         var sb = new StringBuilder(10000);

        Printf(sb, "%*s\t%*s\t%*s\t%*s\t%*s%n".Replace('*', (char)(w-(int)'0'), "P", "R", "F1", "A", ""));
        for (int i = 0; i < catalog.Length; i++)
        {
            Printf(sb, ("%*.2f\t%*.2f\t%*.2f\t%*.2f\t%"+l+"s%n").Replace('*', (char)(w-(int)'0')),
                   precision[i] * 100.0,
                   recall[i] * 100.0,
                   f1[i] * 100.0,
                   accuracy[i] * 100.0,
                   catalog[i]);
        }
        Printf(sb, ("%*.2f\t%*.2f\t%*.2f\t%*.2f\t%"+l+"s%n").Replace('*', (char)(w-(int)'0')),
               average_precision * 100.0,
               average_recall * 100.0,
               average_f1 * 100.0,
               average_accuracy * 100.0,
               "avg.");
        Printf(sb, "data size = %d, speed = %.2f doc/s\n", size, speed);
        return sb.ToString();
    }

    private static void Printf(StringBuilder sb, string Format, params Object[] args)
    {
        sb.Append(string.Format(Format, args));
    }
}
