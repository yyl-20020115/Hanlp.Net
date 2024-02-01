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
using com.hankcs.hanlp.utility;
using Document = com.hankcs.hanlp.classification.corpus.Document;

namespace com.hankcs.hanlp.classification.classifiers;




/**
 * @author hankcs
 */
public abstract class AbstractClassifier : IClassifier
{
    //@Override
    public IClassifier EnableProbability(bool enable) => this;

    /**
     * 是否计算概率
     */
    public bool configProbabilityEnabled = true;
    public abstract AbstractModel GetModel();

    /**
     * 使用一个训练出来的分类器来预测分类
     *
     * @param text
     * @return
     * @
     * @throws InvalidOperationException
     */
    //@Override
    public string Classify(string text) 
    {
        Dictionary<string, Double> scoreMap = Predict(text);

        return CollectionUtility.Max(scoreMap);
    }

    //@Override
    public string Classify(Document document) 
    {
        Dictionary<string, Double> scoreMap = Predict(document);

        return CollectionUtility.Max(scoreMap);
    }

    //@Override
    public void Train(string folderPath, string charsetName) 
    {
        IDataSet dataSet = new MemoryDataSet();
        dataSet.Load(folderPath, charsetName);
        train(dataSet);
    }

    //@Override
    public void Train(Dictionary<string, string[]> trainingDataSet) 
    {
        IDataSet dataSet = new MemoryDataSet();
        logger.start("正在构造训练数据集...");
        int total = trainingDataSet.Count;
        int cur = 0;
        foreach (KeyValuePair<string, string[]> entry in trainingDataSet)
        {
            string category = entry.Key;
            logger._out("[%s]...", category);
            foreach (string doc in entry.Value)
            {
                dataSet.Add(category, doc);
            }
            ++cur;
            logger._out("%.2f%%...", MathUtility.percentage(cur, total));
        }
        logger.finish(" 加载完毕\n");
        Train(dataSet);
    }

    //@Override
    public void Train(string folderPath) 
    {
        Train(folderPath, "UTF-8");
    }

    //@Override
    public Dictionary<string, Double> Predict(Document document)
    {
        var model = GetModel();
        if (model == null)
        {
            throw new InvalidOperationException("未训练模型！无法执行预测！");
        }
        if (document == null)
        {
            throw new InvalidOperationException("参数 text == null");
        }

        double[] probs = Categorize(document);
        Dictionary<string, Double> scoreMap = new ();
        for (int i = 0; i < probs.Length; i++)
        {
            scoreMap.Add(model.catalog[i], probs[i]);
        }
        return scoreMap;
    }

    //@Override
    public int Label(Document document) 
    {
        AbstractModel model = GetModel();
        if (model == null)
        {
            throw new InvalidOperationException("未训练模型！无法执行预测！");
        }
        if (document == null)
        {
            throw new InvalidOperationException("参数 text == null");
        }

        double[] probs = Categorize(document);
        double max = double.NegativeInfinity;
        int best = -1;
        for (int i = 0; i < probs.Length; i++)
        {
            if (probs[i] > max)
            {
                max = probs[i];
                best = i;
            }
        }
        return best;
    }

    public abstract Dictionary<string, double> Predict(string text);
    public abstract double[] Categorize(Document document);
    public abstract void Train(IDataSet dataSet);
}