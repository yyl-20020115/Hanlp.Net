/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/16 17:47</create-date>
 *
 * <copyright file="DemoConfigSegment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.demo;


/**
 * 演示动态设置预置分词器，这里的设置是全局的
 * @author hankcs
 */
public class DemoTokenizerConfig
{
    public static void Main(String[] args)
    {
        String text = "泽田依子是上外日本文化经济学院的外教";
        Console.WriteLine(StandardTokenizer.segment(text));
        StandardTokenizer.SEGMENT.enableAllNamedEntityRecognize(true);
        Console.WriteLine(StandardTokenizer.segment(text));
    }
}
