/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-05-28 AM9:44</create-date>
 *
 * <copyright file="DemoOccurrence.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.occurrence;

namespace com.hankcs.demo;



/**
 * 演示词共现统计
 *
 * @author hankcs
 */
public class DemoOccurrence
{ 
    public static void Main(String[] args)
    {
        Occurrence occurrence = new Occurrence();
        occurrence.addAll("在计算机音视频和图形图像技术等二维信息算法处理方面目前比较先进的视频处理算法");
        occurrence.compute();

        var uniGram = occurrence.getUniGram();
        foreach (var entry in uniGram) 
        {
            TermFrequency termFrequency = entry.Value; 
            Console.WriteLine(termFrequency);
        }

        var biGram = occurrence.getBiGram();
        foreach (var entry in biGram)
        {
            PairFrequency pairFrequency = entry.Value;
            if (pairFrequency.isRight())
                Console.WriteLine(pairFrequency);
        }

        var triGram = occurrence.getTriGram();
        foreach (var entry in triGram)
        {
            TriaFrequency triaFrequency = entry.Value;
            if (triaFrequency.isRight())
                Console.WriteLine(triaFrequency);
        }
    }
}
