/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-28 15:53</create-date>
 *
 * <copyright file="PerceptronNERTagger.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.model.perceptron.common;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 命名实体识别
 *
 * @author hankcs
 */
public class PerceptronNERecognizer : PerceptronTagger , NERecognizer
{
    readonly NERTagSet tagSet;

    public PerceptronNERecognizer(LinearModel nerModel)
        :base(nerModel)
    {
        if (nerModel.tagSet().type != TaskType.NER)
        {
            throw new ArgumentException(string.Format("错误的模型类型: 传入的不是命名实体识别模型，而是 %s 模型", nerModel.featureMap.tagSet.type));
        }
        this.tagSet = (NERTagSet) model.tagSet();
    }

    public PerceptronNERecognizer(string nerModelPath) 
        : this(new LinearModel(nerModelPath))
    {
    }

    /**
     * 加载配置文件指定的模型
     *
     * @
     */
    public PerceptronNERecognizer() 
       : this(HanLP.Config.PerceptronNERModelPath)
    {
    }

    public string[] recognize(string[] wordArray, string[] posArray)
    {
        NERInstance instance = new NERInstance(wordArray, posArray, model.featureMap);
        return recognize(instance);
    }

    public string[] recognize(NERInstance instance)
    {
        instance.tagArray = new int[instance.size()];
        model.viterbiDecode(instance);

        return instance.tags(tagSet);
    }

    //@Override
    public NERTagSet getNERTagSet()
    {
        return tagSet;
    }

    /**
     * 在线学习
     *
     * @param segmentedTaggedNERSentence 人民日报2014格式的句子
     * @return 是否学习成功（失败的原因是参数错误）
     */
    public bool learn(string segmentedTaggedNERSentence)
    {
        return learn(NERInstance.create(segmentedTaggedNERSentence, model.featureMap));
    }

    //@Override
    protected Instance createInstance(Sentence sentence, FeatureMap featureMap)
    {
        return NERInstance.create(sentence, featureMap);
    }
}
