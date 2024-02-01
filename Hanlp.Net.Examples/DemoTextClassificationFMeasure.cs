/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/16 PM12:04</create-date>
 *
 * <copyright file="Demo.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.classifiers;
using com.hankcs.hanlp.classification.corpus;
using com.hankcs.hanlp.classification.statistics.evaluations;
using com.hankcs.hanlp.classification.tokenizers;
using com.hankcs.hanlp.utility;

namespace com.hankcs.demo;





/**
 * 演示了分割训练集和测试集,进行更严谨的测试
 *
 * @author hankcs
 */
public class DemoTextClassificationFMeasure
{
    public static readonly string CORPUS_FOLDER = TestUtility.EnsureTestData("ChnSentiCorp情感分析酒店评论", "http://hanlp.linrunsoft.com/release/corpus/ChnSentiCorp.zip");

    public static void Main(String[] args) 
    {
        IDataSet trainingCorpus = new FileDataSet().                          // FileDataSet省内存，可加载大规模数据集
            SetTokenizer(new HanLPTokenizer()).                               // 支持不同的ITokenizer，详见源码中的文档
            Load(CORPUS_FOLDER, "UTF-8", 0.9);               // 前90%作为训练集
        IClassifier classifier = new NaiveBayesClassifier();
        classifier.Train(trainingCorpus);
        IDataSet testingCorpus = new MemoryDataSet(classifier.GetModel()).
            Load(CORPUS_FOLDER, "UTF-8", -0.1);        // 后10%作为测试集
        // 计算准确率
        FMeasure result = Evaluator.Evaluate(classifier, testingCorpus);
        Console.WriteLine(result);
        // 搜狗文本分类语料库上的准确率与速度（两种不同的ITokenizer）
        // ITokenizer         F1      速度
        // HanLPTokenizer   97.04%  20833.33 doc/s
        // BigramTokenizer  96.82%  3521.13 doc/s
    }
}
