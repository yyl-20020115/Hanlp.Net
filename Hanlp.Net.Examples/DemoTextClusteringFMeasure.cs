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
namespace com.hankcs.demo;



/**
 * @author hankcs
 */
public class DemoTextClusteringFMeasure
{
    public static void Main(String[] args)
    {
        foreach (String algorithm in new String[]{"kmeans", "repeated bisection"})
        {
            Console.Write("%s F1=%.2f\n", algorithm, ClusterAnalyzer.evaluate(CORPUS_FOLDER, algorithm) * 100);
        }
    }
}
