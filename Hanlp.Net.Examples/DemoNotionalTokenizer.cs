/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2015/4/6 23:20</create-date>
 *
 * <copyright file="DemoStopword.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.demo;



/**
 * 演示自动去除停用词、自动断句的分词器
 * @author hankcs
 */
public class DemoNotionalTokenizer
{
    public static void Main(String[] args)
    {
        String text = "小区居民有的反对喂养流浪猫，而有的居民却赞成喂养这些小宝贝";
        // 自动去除停用词
        Console.WriteLine(NotionalTokenizer.segment(text));    // 停用词典位于data/dictionary/stopwords.txt，可以自行修改
        // 自动断句+去除停用词
        for (List<Term> sentence : NotionalTokenizer.seg2sentence(text))
        {
            Console.WriteLine(sentence);
        }
    }
}
