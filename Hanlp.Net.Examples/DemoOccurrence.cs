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

        Set<Map.Entry<String, TermFrequency>> uniGram = occurrence.getUniGram();
        for (Map.Entry<String, TermFrequency> entry : uniGram)
        {
            TermFrequency termFrequency = entry.getValue();
            Console.WriteLine(termFrequency);
        }

        Set<Map.Entry<String, PairFrequency>> biGram = occurrence.getBiGram();
        for (Map.Entry<String, PairFrequency> entry : biGram)
        {
            PairFrequency pairFrequency = entry.getValue();
            if (pairFrequency.isRight())
                Console.WriteLine(pairFrequency);
        }

        Set<Map.Entry<String, TriaFrequency>> triGram = occurrence.getTriGram();
        for (Map.Entry<String, TriaFrequency> entry : triGram)
        {
            TriaFrequency triaFrequency = entry.getValue();
            if (triaFrequency.isRight())
                Console.WriteLine(triaFrequency);
        }
    }
}
