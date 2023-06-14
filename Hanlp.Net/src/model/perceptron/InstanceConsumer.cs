/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-15 下午7:34</create-date>
 *
 * <copyright file="InstanceConsumer.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron;



/**
 * 需要处理实例的消费者
 *
 * @author hankcs
 */
public abstract class InstanceConsumer
{

    protected abstract Instance createInstance(Sentence sentence, final FeatureMap featureMap);

    protected double[] evaluate(String developFile, String modelFile) 
    {
        return evaluate(developFile, new LinearModel(modelFile));
    }

    protected double[] evaluate(String developFile, final LinearModel model) 
    {
        final int[] stat = new int[2];
        IOUtility.loadInstance(developFile, new InstanceHandler()
        {
            //@Override
            public bool process(Sentence sentence)
            {
                Utility.normalize(sentence);
                Instance instance = createInstance(sentence, model.featureMap);
                IOUtility.evaluate(instance, model, stat);
                return false;
            }
        });

        return new double[]{stat[1] / (double) stat[0] * 100};
    }

    protected String normalize(String text)
    {
        return CharTable.convert(text);
    }
}