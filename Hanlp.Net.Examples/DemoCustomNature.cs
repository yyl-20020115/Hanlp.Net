/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/3/14 AM11:49</create-date>
 *
 * <copyright file="DemoCustomNature.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;
using com.hankcs.hanlp.utility;

namespace com.hankcs.demo;


/**
 * 演示自定义词性,以及往词典中插入自定义词性的词语
 *
 * @author hankcs
 */
public class DemoCustomNature
{
    public static void Main(String[] args)
    {
        // 对于系统中已有的词性,可以直接获取
        Nature pcNature = Nature.fromString("n");
        Console.WriteLine(pcNature);
        // 此时系统中没有"电脑品牌"这个词性
        pcNature = Nature.fromString("电脑品牌");
        Console.WriteLine(pcNature);
        // 我们可以动态添加一个
        pcNature = Nature.create("电脑品牌");
        Console.WriteLine(pcNature);
        // 可以将它赋予到某个词语
        LexiconUtility.setAttribute("苹果电脑", pcNature);
        // 或者
        LexiconUtility.setAttribute("苹果电脑", "电脑品牌 1000");
        // 它们将在分词结果中生效
        List<Term> termList = HanLP.segment("苹果电脑可以运行开源阿尔法狗代码吗");
        Console.WriteLine(termList);
        foreach (Term term in termList)
        {
            if (term.nature == pcNature)
                Console.WriteLine("找到了 [{0}] : {1}\n", pcNature, term.word);
        }
        // 还可以直接插入到用户词典
        CustomDictionary.insert("阿尔法狗", "科技名词 1024");
        StandardTokenizer.SEGMENT.enablePartOfSpeechTagging(true);  // 依然支持隐马词性标注
        termList = HanLP.segment("苹果电脑可以运行开源阿尔法狗代码吗");
        Console.WriteLine(termList);
        // 1.6.5之后Nature不再是枚举类型，无法switch。但终于不再涉及反射了，在各种JRE环境下都更稳定。
        foreach (Term term in termList)
        {
            if (term.nature == pcNature)
            {
                Console.WriteLine("找到了 [{0}] : {1}\n", "名词", term.word);
            }
        }
    }
}
