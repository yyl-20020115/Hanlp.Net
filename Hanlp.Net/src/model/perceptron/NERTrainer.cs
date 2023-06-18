/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-28 11:39</create-date>
 *
 * <copyright file="NERTrainer.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.dependency.nnparser;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron;


/**
 * @author hankcs
 */
public class NERTrainer : PerceptronTrainer
{
    /**
     * 支持任意自定义NER类型，例如：<br>
     * tagSet.nerLabels.Clear();<br>
     * tagSet.nerLabels.Add("nr");<br>
     * tagSet.nerLabels.Add("ns");<br>
     * tagSet.nerLabels.Add("nt");<br>
     */
    public NERTagSet tagSet;

    public NERTrainer(NERTagSet tagSet)
    {
        this.tagSet = tagSet;
    }

    public NERTrainer()
    {
        tagSet = new NERTagSet();
        tagSet.nerLabels.Add("nr");
        tagSet.nerLabels.Add("ns");
        tagSet.nerLabels.Add("nt");
    }

    /**
     * 重载此方法以支持任意自定义NER类型，例如：<br>
     * NERTagSet tagSet = new NERTagSet();<br>
     * tagSet.nerLabels.Add("nr");<br>
     * tagSet.nerLabels.Add("ns");<br>
     * tagSet.nerLabels.Add("nt");<br>
     * return tagSet;<br>
     * @return
     */
    //@Override
    protected override TagSet createTagSet()
    {
        return tagSet;
    }

    //@Override
    protected dependency.nnparser.Instance createInstance(Sentence sentence, FeatureMap featureMap)
    {
        return NERInstance.create(sentence, featureMap);
    }
}
