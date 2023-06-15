/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/24 23:20</create-date>
 *
 * <copyright file="DemoHighSpeedSegment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.demo;


/**
 * 演示极速分词，基于DoubleArrayTrie实现的词典正向最长分词，适用于“高吞吐量”“精度一般”的场合
 * @author hankcs
 */
public class DemoHighSpeedSegment
{
    public static void Main(String[] args)
    {
        String text = "江西鄱阳湖干枯，中国最大淡水湖变成大草原";
        HanLP.Config.ShowTermNature = false;
        Console.WriteLine(SpeedTokenizer.segment(text));
        long start = DateTime.Now.Microsecond;
        int pressure = 1000000;
        for (int i = 0; i < pressure; ++i)
        {
            SpeedTokenizer.segment(text);
        }
        double costTime = (DateTime.Now.Microsecond - start) / (double)1000;
        Console.WriteLine("SpeedTokenizer分词速度：{0}字每秒\n", text.Length * pressure / costTime);
    }
}
