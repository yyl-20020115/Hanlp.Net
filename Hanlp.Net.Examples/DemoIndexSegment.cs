/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/7 19:02</create-date>
 *
 * <copyright file="DemoSegment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.demo;



/**
 * 索引分词
 * @author hankcs
 */
public class DemoIndexSegment
{
    public static void Main(String[] args)
    {
        List<Term> termList = IndexTokenizer.segment("主副食品");
        foreach (Term term in termList)
        {
            Console.WriteLine(term + " [" + term.offset + ":" + (term.offset + term.word.Length) + "]");
        }

        Console.WriteLine("\n最细颗粒度切分：");
        IndexTokenizer.SEGMENT.enableIndexMode(1);
        termList = IndexTokenizer.segment("主副食品");
        foreach (Term term in termList)
        {
            Console.WriteLine(term + " [" + term.offset + ":" + (term.offset + term.word.Length) + "]");
        }
    }
}
