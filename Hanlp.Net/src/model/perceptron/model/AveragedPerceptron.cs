/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM4:45</create-date>
 *
 * <copyright file="AveragedPerceptron.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.feature;

namespace com.hankcs.hanlp.model.perceptron.model;



/**
 * 平均感知机算法学习的线性模型
 *
 * @author hankcs
 */
public class AveragedPerceptron : LinearModel
{
    public AveragedPerceptron(FeatureMap featureMap, float[] parameter)
        : base(featureMap, parameter)
    {
       ;
    }

    public AveragedPerceptron(FeatureMap featureMap)
        : base(featureMap)
    {
        ;
    }

    /**
     * 根据答案和预测更新参数
     *
     * @param goldIndex    预测正确的特征函数（非压缩形式）
     * @param predictIndex 命中的特征函数
     */
    public void update(int[] goldIndex, int[] predictIndex, double[] total, int[] timestamp, int current)
    {
        for (int i = 0; i < goldIndex.Length; ++i)
        {
            if (goldIndex[i] == predictIndex[i])
                continue;
            else
            {
                update(goldIndex[i], 1, total, timestamp, current);
                if (predictIndex[i] >= 0 && predictIndex[i] < parameter.Length)
                    update(predictIndex[i], -1, total, timestamp, current);
                else
                {
                    throw new IndexOutOfRangeException("更新参数时传入了非法的下标");
                }
            }
        }
    }

    /**
     * 根据答案和预测更新参数
     *
     * @param featureVector 特征向量
     * @param value         更新量
     * @param total         权值向量总和
     * @param timestamp     每个权值上次更新的时间戳
     * @param current       当前时间戳
     */
    public void update(ICollection<int> featureVector, float value, double[] total, int[] timestamp, int current)
    {
        foreach (int i in featureVector)
            update(i, value, total, timestamp, current);
    }

    /**
     * 根据答案和预测更新参数
     *
     * @param index     特征向量的下标
     * @param value     更新量
     * @param total     权值向量总和
     * @param timestamp 每个权值上次更新的时间戳
     * @param current   当前时间戳
     */
    private void update(int index, float value, double[] total, int[] timestamp, int current)
    {
        int passed = current - timestamp[index];
        total[index] += passed * parameter[index];
        parameter[index] += value;
        timestamp[index] = current;
    }

    public void average(double[] total, int[] timestamp, int current)
    {
        for (int i = 0; i < parameter.Length; i++)
        {
            parameter[i] = (float) ((total[i] + (current - timestamp[i]) * parameter[i]) / current);
        }
    }
}
