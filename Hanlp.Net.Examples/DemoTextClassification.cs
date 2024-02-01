/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/20 AM11:46</create-date>
 *
 * <copyright file="DemoAtFirstSight.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.classifiers;
using com.hankcs.hanlp.classification.models;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.demo;




/**
 * 第一个demo,演示文本分类最基本的调用方式
 *
 * @author hankcs
 */
public class DemoTextClassification
{
    /**
     * 搜狗文本分类语料库5个类目，每个类目下1000篇文章，共计5000篇文章
     */
    public static readonly string CORPUS_FOLDER = TestUtility.EnsureTestData("搜狗文本分类语料库迷你版", "http://hanlp.linrunsoft.com/release/corpus/sogou-text-classification-corpus-mini.zip");
    /**
     * 模型保存路径
     */
    public static readonly string MODEL_PATH = "data/test/classification-model.ser";


    public static void Main(String[] args) 
    {
        IClassifier classifier = new NaiveBayesClassifier(trainOrLoadModel());
        predict(classifier, "C罗压梅西内马尔蝉联金球奖 2017=C罗年");
        predict(classifier, "英国造航母耗时8年仍未服役 被中国速度远远甩在身后");
        predict(classifier, "研究生考录模式亟待进一步专业化");
        predict(classifier, "如果真想用食物解压,建议可以食用燕麦");
        predict(classifier, "通用及其部分竞争对手目前正在考虑解决库存问题");
    }

    private static void predict(IClassifier classifier, String text)
    {
        Console.Write("《{0}》 属于分类 【{1}】\n", text, classifier.Classify(text));
    }

    private static NaiveBayesModel trainOrLoadModel() 
    {
        NaiveBayesModel model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        if (model != null) return model;

        var corpusFolder = (CORPUS_FOLDER);
        if (!Directory.Exists(corpusFolder))
        {
            Console.Error.WriteLine("没有文本分类语料，请阅读IClassifier.train(java.lang.String)中定义的语料格式与语料下载：" +
                                   "https://github.com/hankcs/HanLP/wiki/%E6%96%87%E6%9C%AC%E5%88%86%E7%B1%BB%E4%B8%8E%E6%83%85%E6%84%9F%E5%88%86%E6%9E%90");
            Environment.Exit(1);
        }

        IClassifier classifier = new NaiveBayesClassifier(); // 创建分类器，更高级的功能请参考IClassifier的接口定义
        classifier.Train(CORPUS_FOLDER);                     // 训练后的模型支持持久化，下次就不必训练了
        model = (NaiveBayesModel) classifier.GetModel();
        IOUtil.saveObjectTo(model, MODEL_PATH);
        return model;
    }
}
