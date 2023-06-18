/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-05 PM11:07</create-date>
 *
 * <copyright file="StructuredPerceptron.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron.model;


/**
 * 结构化感知机算法学习的线性模型
 *
 * @author hankcs
 */
public class StructuredPerceptron : LinearModel
{
    public StructuredPerceptron(FeatureMap featureMap, float[] parameter)
    {
        super(featureMap, parameter);
    }

    public StructuredPerceptron(FeatureMap featureMap)
    {
        super(featureMap);
    }

    /**
     * 根据答案和预测更新参数
     *
     * @param goldIndex    答案的特征函数（非压缩形式）
     * @param predictIndex 预测的特征函数（非压缩形式）
     */
    public void update(int[] goldIndex, int[] predictIndex)
    {
        for (int i = 0; i < goldIndex.Length; ++i)
        {
            if (goldIndex[i] == predictIndex[i])
                continue;
            else // 预测与答案不一致
            {
                parameter[goldIndex[i]]++; // 奖励正确的特征函数（将它的权值加一）
                if (predictIndex[i] >= 0 && predictIndex[i] < parameter.Length)
                    parameter[predictIndex[i]]--; // 惩罚招致错误的特征函数（将它的权值减一）
                else
                {
                    throw new ArgumentException("更新参数时传入了非法的下标");
                }
            }
        }
    }

    /**
     * 在线学习
     *
     * @param instance 样本
     */
    public void update(Instance instance)
    {
        int[] guessLabel = new int[instance.Length];
        viterbiDecode(instance, guessLabel);
        TagSet tagSet = featureMap.tagSet;
        for (int i = 0; i < instance.Length; i++)
        {
            int[] featureVector = instance.getFeatureAt(i);
            int[] goldFeature = new int[featureVector.Length]; // 根据答案应当被激活的特征
            int[] predFeature = new int[featureVector.Length]; // 实际预测时激活的特征
            for (int j = 0; j < featureVector.Length - 1; j++)
            {
                goldFeature[j] = featureVector[j] * tagSet.size() + instance.tagArray[i];
                predFeature[j] = featureVector[j] * tagSet.size() + guessLabel[i];
            }
            goldFeature[featureVector.Length - 1] = (i == 0 ? tagSet.bosId() : instance.tagArray[i - 1]) * tagSet.size() + instance.tagArray[i];
            predFeature[featureVector.Length - 1] = (i == 0 ? tagSet.bosId() : guessLabel[i - 1]) * tagSet.size() + guessLabel[i];
            update(goldFeature, predFeature);
        }
    }
}