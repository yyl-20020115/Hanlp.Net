/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/17 PM3:10</create-date>
 *
 * <copyright file="Evaluator.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.classifiers;
using com.hankcs.hanlp.classification.corpus;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.classification.statistics.evaluations;



/**
 * 分类器性能评测
 * @author hankcs
 */
public class Evaluator
{
    private Evaluator()
    {
    }

    public static FMeasure Evaluate(IClassifier classifier, IDataSet testingDataSet)
    {
        int c = classifier.GetModel().catalog.Length;
        double[] TP_FP = new double[c]; // 判定为某个类别的数量
        double[] TP_FN = new double[c]; // 某个类别的样本数量
        double[] TP = new double[c];    // 判定为某个类别且判断正确的数量
        double time = DateTime.Now.Microsecond;
        foreach (Document document in testingDataSet)
        {
            int _out = classifier.Label(document);
            int key = document.category;
            ++TP_FP[_out];
            ++TP_FN[key];
            if (key == _out)
            {
                ++TP[_out];
            }
        }
        time = DateTime.Now.Microsecond - time;

        FMeasure result = Calculate(c, testingDataSet.Count, TP, TP_FP, TP_FN);
        result.catalog = testingDataSet.Catalog.ToArray();
        result.speed = result.size / (time / 1000.0);

        return result;
    }

    public static FMeasure Evaluate(IClassifier classifier, Dictionary<string, string[]> testingDataSet)
    {
        return Evaluate(classifier, new MemoryDataSet(classifier.GetModel()).Add(testingDataSet));
    }

    /**
     *
     * @param c 类目数量
     * @param size 样本数量
     * @param TP 判定为某个类别且判断正确的数量
     * @param TP_FP 判定为某个类别的数量
     * @param TP_FN 某个类别的样本数量
     * @return
     */
    private static FMeasure Calculate(int c, int size, double[] TP, double[] TP_FP, double[] TP_FN)
    {
        double[] precision = new double[c];
        double[] recall = new double[c];
        double[] f1 = new double[c];
        double[] accuracy = new double[c];
        FMeasure result = new FMeasure();
        result.size = size;

        for (int i = 0; i < c; i++)
        {
            double TN = result.size - TP_FP[i] - (TP_FN[i] - TP[i]);
            accuracy[i] = (TP[i] + TN) / result.size;
            if (TP[i] != 0)
            {
                precision[i] = TP[i] / TP_FP[i];
                recall[i] = TP[i] / TP_FN[i];
                result.average_accuracy += TP[i];
            }
            else
            {
                precision[i] = 0;
                recall[i] = 0;
            }
            f1[i] = 2 * precision[i] * recall[i] / (precision[i] + recall[i]);
        }
        result.average_precision = MathUtility.average(precision);
        result.average_recall = MathUtility.average(recall);
        result.average_f1 = 2 * result.average_precision * result.average_recall
                / (result.average_precision + result.average_recall);
        result.average_accuracy /= (double) result.size;
        result.accuracy = accuracy;
        result.precision = precision;
        result.recall = recall;
        result.f1 = f1;
        return result;
    }
}
