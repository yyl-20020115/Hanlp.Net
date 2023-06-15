/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-18 下午10:18</create-date>
 *
 * <copyright file="PerceptronTagger.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.dependency.nnparser;
using com.hankcs.hanlp.model.perceptron.model;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 抽象的感知机标注器
 *
 * @author hankcs
 */
public abstract class PerceptronTagger : InstanceConsumer
{
    /**
     * 用StructurePerceptron实现在线学习
     */
    protected StructuredPerceptron model;

    public PerceptronTagger(LinearModel model)
    {
        //assert model != null;
        this.model = model is StructuredPerceptron ? (StructuredPerceptron) model : new StructuredPerceptron(model.featureMap, model.parameter);
    }

    public PerceptronTagger(StructuredPerceptron model)
    {
        //assert model != null;
        this.model = model;
    }

    public LinearModel getModel()
    {
        return model;
    }

    /**
     * 在线学习
     *
     * @param instance
     * @return
     */
    public bool learn(Instance instance)
    {
        if (instance == null) return false;
        model.update(instance);
        return true;
    }

    /**
     * 在线学习
     *
     * @param sentence
     * @return
     */
    public bool learn(Sentence sentence)
    {
        return learn(createInstance(sentence, model.featureMap));
    }

    /**
     * 性能测试
     *
     * @param corpora 数据集
     * @return 默认返回accuracy，有些子类可能返回P,R,F1
     * @
     */
    public double[] evaluate(String corpora) 
    {
        return evaluate(corpora, this.getModel());
    }
}