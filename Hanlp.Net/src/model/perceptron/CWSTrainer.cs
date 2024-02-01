/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM4:48</create-date>
 *
 * <copyright file="PerceptronSegmentTrainer.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.dependency.nnparser;
using com.hankcs.hanlp.mining.word2vec;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 感知机分词器训练工具
 *
 * @author hankcs
 */
public class CWSTrainer : PerceptronTrainer
{
    //@Override
    protected override TagSet createTagSet()
    {
        return new CWSTagSet();
    }

    //@Override
    protected override Instance createInstance(Sentence sentence, FeatureMap mutableFeatureMap)
    {
        List<Word> wordList = sentence.toSimpleWordList();
        string[] termArray = Utility.toWordArray(wordList);
        Instance instance = new CWSInstance(termArray, mutableFeatureMap);
        return instance;
    }

    //@Override
    public override double[] evaluate(string developFile, LinearModel model) 
    {
        PerceptronSegmenter segmenter = new PerceptronSegmenter(model);
        double[] prf = Utility.prf(Utility.evaluateCWS(developFile, segmenter));
        return prf;
    }

}
