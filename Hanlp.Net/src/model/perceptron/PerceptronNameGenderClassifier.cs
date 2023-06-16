/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-21 9:08 AM</create-date>
 *
 * <copyright file="PerceptronNameGenderClassifier.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.model;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 基于感知机的人名性别分类器，预测人名的性别
 *
 * @author hankcs
 */
public class PerceptronNameGenderClassifier : PerceptronClassifier
{
    public PerceptronNameGenderClassifier()
    {
    }

    public PerceptronNameGenderClassifier(LinearModel model)
        :base(model)
    {
    }

    public PerceptronNameGenderClassifier(string modelPath) 
        : base(modelPath) 
    {
    }

    //@Override
    protected List<int> extractFeature(string text, FeatureMap featureMap)
    {
        List<int> featureList = new ();
        string givenName = extractGivenName(text);
        // 特征模板1：g[0]
        addFeature("1" + givenName[..1], featureMap, featureList);
        // 特征模板2：g[1]
        addFeature("2" + givenName[1..], featureMap, featureList);
        // 特征模板3：g
//        addFeature("3" + givenName, featureMap, featureList);
        // 偏置特征（代表标签的先验分布，当样本不均衡时有用，但此处的男女预测无用）
//        addFeature("b", featureMap, featureList);
        return featureList;
    }

    /**
     * 去掉姓氏，截取中国人名中的名字
     *
     * @param name 姓名
     * @return 名
     */
    public static string extractGivenName(string name)
    {
        if (name.Length <= 2)
            return "_" + name.Substring(name.Length - 1);
        else
            return name.Substring(name.Length - 2);

    }
}
