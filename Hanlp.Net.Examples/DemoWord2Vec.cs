/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-02 12:09</create-date>
 *
 * <copyright file="Demo.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.mining.word2vec;
using com.hankcs.hanlp.utility;

namespace com.hankcs.demo;



/**
 * 演示词向量的训练与应用
 *
 * @author hankcs
 */
public class DemoWord2Vec
{
    private static readonly string TRAIN_FILE_NAME = TestUtility.ensureTestData("搜狗文本分类语料库已分词.txt", "http://hanlp.linrunsoft.com/release/corpus/sogou-mini-segmented.zip");
    private static readonly string MODEL_FILE_NAME = "data/test/word2vec.txt";

    public static void Main(String[] args) 
    {
        WordVectorModel wordVectorModel = trainOrLoadModel();
        printNearest("中国", wordVectorModel);
        printNearest("美丽", wordVectorModel);
        printNearest("购买", wordVectorModel);

        // 文档向量
        DocVectorModel docVectorModel = new DocVectorModel(wordVectorModel);
        String[] documents = new String[]{
            "山东苹果丰收",
            "农民在江苏种水稻",
            "奥运会女排夺冠",
            "世界锦标赛胜出",
            "中国足球失败",
        };

        Console.WriteLine(docVectorModel.similarity(documents[0], documents[1]));
        Console.WriteLine(docVectorModel.similarity(documents[0], documents[4]));

        for (int i = 0; i < documents.Length; i++)
        {
            docVectorModel.addDocument(i, documents[i]);
        }

        printNearestDocument("体育", documents, docVectorModel);
        printNearestDocument("农业", documents, docVectorModel);
        printNearestDocument("我要看比赛", documents, docVectorModel);
        printNearestDocument("要不做饭吧", documents, docVectorModel);
    }

    static void printNearest(String word, WordVectorModel model)
    {
        Console.Write("\n                                                Word     Cosine\n------------------------------------------------------------------------\n");
        foreach (var entry in model.nearest(word))
        {
            Console.Write("%50s\t\t%f\n", entry.getKey(), entry.getValue());
        }
    }

    static void printNearestDocument(String document, String[] documents, DocVectorModel model)
    {
        printHeader(document);
        foreach (var entry in model.nearest(document))
        {
            Console.Write("%50s\t\t%f\n", documents[entry.getKey()], entry.getValue());
        }
    }

    private static void printHeader(String query)
    {
        Console.Write("\n%50s          Cosine\n------------------------------------------------------------------------\n", query);
    }

    static WordVectorModel trainOrLoadModel() 
    {
        if (!IOUtil.isFileExisted(MODEL_FILE_NAME))
        {
            if (!IOUtil.isFileExisted(TRAIN_FILE_NAME))
            {
                Console.WriteLine("语料不存在，请阅读文档了解语料获取与格式：https://github.com/hankcs/HanLP/wiki/word2vec");
                Environment.Exit(1);
            }
            Word2VecTrainer trainerBuilder = new Word2VecTrainer();
            return trainerBuilder.train(TRAIN_FILE_NAME, MODEL_FILE_NAME);
        }

        return loadModel();
    }

    static WordVectorModel loadModel() 
    {
        return new WordVectorModel(MODEL_FILE_NAME);
    }
}
