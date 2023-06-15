/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-27 下午5:06</create-date>
 *
 * <copyright file="PerceptronPOSTagger.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.model.perceptron.common;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 词性标注器
 *
 * @author hankcs
 */
public class PerceptronPOSTagger : PerceptronTagger , POSTagger
{
    public PerceptronPOSTagger(LinearModel model)
        :base(model)
    {
        if (model.featureMap.tagSet.type != TaskType.POS)
        {
            throw new IllegalArgumentException(string.format("错误的模型类型: 传入的不是词性标注模型，而是 %s 模型", model.featureMap.tagSet.type));
        }
    }

    public PerceptronPOSTagger(string modelPath) 
        : this(new LinearModel(modelPath))
    {
    }

    /**
     * 加载配置文件指定的模型
     * @
     */
    public PerceptronPOSTagger()
        : this(HanLP.Config.PerceptronPOSModelPath)
    {
    }

    /**
     * 标注
     *
     * @param words
     * @return
     */
    //@Override
    public string[] tag(params string[] words)
    {
        POSInstance instance = new POSInstance(words, model.featureMap);
        return tag(instance);
    }

    public string[] tag(POSInstance instance)
    {
        instance.tagArray = new int[instance.featureMatrix.Length];

        model.viterbiDecode(instance, instance.tagArray);
        return instance.tags(model.tagSet());
    }

    /**
     * 标注
     *
     * @param wordList
     * @return
     */
    //@Override
    public string[] tag(List<string> wordList)
    {
        string[] termArray = new string[wordList.size()];
        wordList.toArray(termArray);
        return tag(termArray);
    }

    /**
     * 在线学习
     *
     * @param segmentedTaggedSentence 人民日报2014格式的句子
     * @return 是否学习成功（失败的原因是参数错误）
     */
    public bool learn(string segmentedTaggedSentence)
    {
        return learn(POSInstance.create(segmentedTaggedSentence, model.featureMap));
    }

    /**
     * 在线学习
     *
     * @param wordTags [单词]/[词性]数组
     * @return 是否学习成功（失败的原因是参数错误）
     */
    public bool learn(params string[] wordTags)
    {
        string[] words = new string[wordTags.length];
        string[] tags = new string[wordTags.length];
        for (int i = 0; i < wordTags.length; i++)
        {
            string[] wordTag = wordTags[i].Split("//");
            words[i] = wordTag[0];
            tags[i] = wordTag[1];
        }
        return learn(new POSInstance(words, tags, model.featureMap));
    }

    //@Override
    protected Instance createInstance(Sentence sentence, FeatureMap featureMap)
    {
        return POSInstance.create(sentence, featureMap);
    }
}
