/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/6 11:11</create-date>
 *
 * <copyright file="DemoStopWordEx.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.stopword;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.demo;





/**
 * 演示如何去除停用词
 *
 * @author hankcs
 */
public class DemoStopWord
{
    public static void Main(String[] args)
    {
        String text = "小区居民有的反对喂养流浪猫，而有的居民却赞成喂养这些小宝贝";
        // 可以动态修改停用词词典
        CoreStopWordDictionary.add("居民");
        Console.WriteLine(NotionalTokenizer.segment(text));
        CoreStopWordDictionary.remove("居民");
        Console.WriteLine(NotionalTokenizer.segment(text));
        // 可以对任意分词器的结果执行过滤
        List<Term> termList = BasicTokenizer.segment(text);
        Console.WriteLine(termList);
        CoreStopWordDictionary.apply(termList);
        Console.WriteLine(termList);
        // 还可以自定义过滤逻辑
        CoreStopWordDictionary.FILTER = new FT(Nature.nz);
        Console.WriteLine(NotionalTokenizer.segment(text));
    }
    public class FT : Filter
    {
        private readonly Nature nz;

        public FT(Nature nz)
        {
            this.nz = nz;
        }
        public bool shouldInclude(Term term)
        {
            if (term.nature == nz)
            {
                return !CoreStopWordDictionary.contains(term.word);
            }
            return false;
        }
    }
}
