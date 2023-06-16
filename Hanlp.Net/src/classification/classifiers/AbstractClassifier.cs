/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016/1/29 18:00</create-date>
 *
 * <copyright file="AbstractClassifier.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.corpus;
using com.hankcs.hanlp.classification.models;
using com.hankcs.hanlp.classification.utilities;
using com.hankcs.hanlp.corpus.document;
using Document = com.hankcs.hanlp.classification.corpus.Document;

namespace com.hankcs.hanlp.classification.classifiers;




/**
 * @author hankcs
 */
public abstract class AbstractClassifier : IClassifier
{
    //@Override
    public IClassifier enableProbability(bool enable)
    {
        return this;
    }

    /**
     * 是否计算概率
     */
    bool configProbabilityEnabled = true;

    /**
     * 使用一个训练出来的分类器来预测分类
     *
     * @param text
     * @return
     * @
     * @throws IllegalStateException
     */
    //@Override
    public string classify(string text) 
    {
        Dictionary<string, Double> scoreMap = predict(text);

        return CollectionUtility.max(scoreMap);
    }

    //@Override
    public string classify(Document document) 
    {
        Dictionary<string, Double> scoreMap = predict(document);

        return CollectionUtility.max(scoreMap);
    }

    //@Override
    public void train(string folderPath, string charsetName) 
    {
        IDataSet dataSet = new MemoryDataSet();
        dataSet.load(folderPath, charsetName);
        train(dataSet);
    }

    //@Override
    public void train(Dictionary<string, string[]> trainingDataSet) 
    {
        IDataSet dataSet = new MemoryDataSet();
        logger.start("正在构造训练数据集...");
        int total = trainingDataSet.size();
        int cur = 0;
        for (KeyValuePair<string, string[]> entry : trainingDataSet.entrySet())
        {
            string category = entry.getKey();
            logger._out("[%s]...", category);
            for (string doc : entry.getValue())
            {
                dataSet.add(category, doc);
            }
            ++cur;
            logger._out("%.2f%%...", MathUtility.percentage(cur, total));
        }
        logger.finish(" 加载完毕\n");
        train(dataSet);
    }

    //@Override
    public void train(string folderPath) 
    {
        train(folderPath, "UTF-8");
    }

    //@Override
    public Dictionary<string, Double> predict(Document document)
    {
        var model = getModel();
        if (model == null)
        {
            throw new IllegalStateException("未训练模型！无法执行预测！");
        }
        if (document == null)
        {
            throw new IllegalArgumentException("参数 text == null");
        }

        double[] probs = categorize(document);
        Dictionary<string, Double> scoreMap = new ();
        for (int i = 0; i < probs.length; i++)
        {
            scoreMap.put(model.catalog[i], probs[i]);
        }
        return scoreMap;
    }

    //@Override
    public int label(Document document) 
    {
        AbstractModel model = getModel();
        if (model == null)
        {
            throw new IllegalStateException("未训练模型！无法执行预测！");
        }
        if (document == null)
        {
            throw new IllegalArgumentException("参数 text == null");
        }

        double[] probs = categorize(document);
        double max = Double.NEGATIVE_INFINITY;
        int best = -1;
        for (int i = 0; i < probs.length; i++)
        {
            if (probs[i] > max)
            {
                max = probs[i];
                best = i;
            }
        }
        return best;
    }
}