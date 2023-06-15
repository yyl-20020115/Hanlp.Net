/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-05 PM7:56</create-date>
 *
 * <copyright file="AveragedPerceptronSegment.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dependency.nnparser;
using com.hankcs.hanlp.model.perceptron.common;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.tokenizer.lexical;
using System.Text;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 中文分词
 *
 * @author hankcs
 */
public class PerceptronSegmenter : PerceptronTagger , Segmenter
{
    private readonly CWSTagSet CWSTagSet;

    public PerceptronSegmenter(LinearModel cwsModel)
        :base(cwsModel)
    {
        //super(cwsModel);
        if (cwsModel.featureMap.tagSet.type != TaskType.CWS)
        {
            throw new IllegalArgumentException(String.format("错误的模型类型: 传入的不是分词模型，而是 %s 模型", cwsModel.featureMap.tagSet.type));
        }
        CWSTagSet = (CWSTagSet) cwsModel.featureMap.tagSet;
    }

    public PerceptronSegmenter(String cwsModelFile)
        : this(new LinearModel(cwsModelFile))
    {
    }

    /**
     * 加载配置文件指定的模型
     * @
     */
    public PerceptronSegmenter() 
        : this(HanLP.Config.PerceptronCWSModelPath)
    {
    }

    public void segment(String text, List<String> output)
    {
        String normalized = normalize(text);
        segment(text, normalized, output);
    }

    public void segment(String text, String normalized, List<String> output)
    {
        if (text.isEmpty()) return;
        Instance instance = new CWSInstance(normalized, model.featureMap);
        segment(text, instance, output);
    }

    public void segment(String text, Instance instance, List<String> output)
    {
        int[] tagArray = instance.tagArray;
        model.viterbiDecode(instance, tagArray);

        StringBuilder result = new StringBuilder();
        result.Append(text[(0)]);

        for (int i = 1; i < tagArray.Length; i++)
        {
            if (tagArray[i] == CWSTagSet.B || tagArray[i] == CWSTagSet.S)
            {
                output.add(result.ToString());
                result.setLength(0);
            }
            result.Append(text.charAt(i));
        }
        if (result.Length != 0)
        {
            output.Add(result.ToString());
        }
    }

    public List<String> segment(String sentence)
    {
        List<String> result = new ();
        segment(sentence, result);
        return result;
    }

    /**
     * 在线学习
     *
     * @param segmentedSentence 分好词的句子，空格或tab分割，不含词性
     * @return 是否学习成功（失败的原因是参数错误）
     */
    public bool learn(String segmentedSentence)
    {
        return learn(segmentedSentence.Split("\\s+"));
    }

    /**
     * 在线学习
     *
     * @param words 分好词的句子
     * @return 是否学习成功（失败的原因是参数错误）
     */
    public bool learn(params String[] words)
    {
//        for (int i = 0; i < words.length; i++) // 防止传入带词性的词语
//        {
//            int index = words[i].indexOf('/');
//            if (index > 0)
//            {
//                words[i] = words[i].substring(0, index);
//            }
//        }
        return learn(new CWSInstance(words, model.featureMap));
    }

    //@Override
    protected Instance createInstance(Sentence sentence, FeatureMap featureMap)
    {
        return CWSInstance.create(sentence, featureMap);
    }

    //@Override
    public double[] evaluate(String corpora) 
    {
        // 这里用CWS的F1
        double[] prf = Utility.prf(Utility.evaluateCWS(corpora, this));
        return prf;
    }
}