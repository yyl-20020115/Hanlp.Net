/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-18 11:11 PM</create-date>
 *
 * <copyright file="DemoTextClustering.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.mining.cluster;
using com.hankcs.hanlp.utility;

namespace com.hankcs.demo;



/**
 * @author hankcs
 */
public class DemoTextClusteringFMeasure
{
    public static readonly string CORPUS_FOLDER = TestUtility.ensureTestData("搜狗文本分类语料库迷你版", "http://hanlp.linrunsoft.com/release/corpus/sogou-text-classification-corpus-mini.zip");

    public static void Main(String[] args)
    {
        foreach (String algorithm in new String[]{"kmeans", "repeated bisection"})
        {
            Console.Write("{0} F1={1}\n", algorithm, ClusterAnalyzer<object>.evaluate(CORPUS_FOLDER, algorithm) * 100);
        }
    }
}
