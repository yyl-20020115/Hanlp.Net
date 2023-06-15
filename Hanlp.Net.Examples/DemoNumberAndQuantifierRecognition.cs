/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/6 18:44</create-date>
 *
 * <copyright file="DemoNumberAndQuantifier.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.demo;


/**
 * 演示数词和数量词识别
 *
 * @author hankcs
 */
public class DemoNumberAndQuantifierRecognition
{
    public static void Main(String[] args)
    {
        StandardTokenizer.SEGMENT.enableNumberQuantifierRecognize(true);
        String[] testCase = new String[]
                {
                        "十九元套餐包括什么",
                        "九千九百九十九朵玫瑰",
                        "壹佰块都不给我",
                        "９０１２３４５６７８只蚂蚁",
                        "牛奶三〇〇克*2",
                        "ChinaJoy“扫黄”细则露胸超2厘米罚款",
                };
        foreach (String sentence in testCase)
        {
            Console.WriteLine(StandardTokenizer.segment(sentence));
        }
    }
}
