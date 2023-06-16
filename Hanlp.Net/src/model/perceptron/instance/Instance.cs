/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM5:16</create-date>
 *
 * <copyright file="SentenceInstance.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.tagset;
using System.Text;

namespace com.hankcs.hanlp.model.perceptron.instance;



/**
 * @author hankcs
 */
public class Instance
{
    public int[][] featureMatrix;
    public int[] tagArray;

    protected Instance()
    {
    }

    protected static int[] toFeatureArray(List<int> featureVector)
    {
        int[] featureArray = new int[featureVector.Count + 1];   // 最后一列留给转移特征
        int index = -1;
        foreach (int feature in featureVector)
        {
            featureArray[++index] = feature;
        }

        return featureArray;
    }

    public int[] getFeatureAt(int position)
    {
        return featureMatrix[position];
    }

    public int Length()
    {
        return tagArray.Length;
    }

    protected static void addFeature(string rawFeature, List<int> featureVector, FeatureMap featureMap)
    {
        int id = featureMap.idOf(rawFeature.ToString());
        if (id != -1)
        {
            featureVector.Add(id);
        }
    }

    /**
     * 添加特征，同时清空缓存
     *
     * @param rawFeature
     * @param featureVector
     * @param featureMap
     */
    protected static void addFeatureThenClear(StringBuilder rawFeature, List<int> featureVector, FeatureMap featureMap)
    {
        int id = featureMap.idOf(rawFeature.ToString());
        if (id != -1)
        {
            featureVector.Add(id);
        }
        rawFeature.Length = 0;
    }

    /**
     * 根据标注集还原字符形式的标签
     *
     * @param tagSet
     * @return
     */
    public string[] tags(TagSet tagSet)
    {
        //assert tagArray != null;

        string[] tags = new string[tagArray.Length];
        for (int i = 0; i < tags.Length; i++)
        {
            tags[i] = tagSet.stringOf(tagArray[i]);
        }

        return tags;
    }

    /**
     * 实例大小（有多少个要预测的元素）
     *
     * @return
     */
    public int size()
    {
        return featureMatrix.Length;
    }
}