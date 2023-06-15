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
using com.hankcs.hanlp;
using com.hankcs.hanlp.dictionary.py;

namespace com.hankcs.demo;



/**
 * 汉字转拼音
 * @author hankcs
 */
public class DemoPinyin
{
    public static void Main(String[] args)
    {
        String text = "重载不是重任！";
        List<Pinyin> pinyinList = HanLP.convertToPinyinList(text);
        Console.Write("原文,");
        foreach (char c in text.ToCharArray())
        {
            Console.Write("{0},", c);
        }
        Console.WriteLine();

        Console.Write("拼音（数字音调）,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin);
        }
        Console.WriteLine();

        Console.Write("拼音（符号音调）,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin.getPinyinWithToneMark());
        }
        Console.WriteLine();

        Console.Write("拼音（无音调）,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin.getPinyinWithoutTone());
        }
        Console.WriteLine();

        Console.Write("声调,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin.getTone());
        }
        Console.WriteLine();

        Console.Write("声母,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin.getShengmu());
        }
        Console.WriteLine();

        Console.Write("韵母,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin.getYunmu());
        }
        Console.WriteLine();

        Console.Write("输入法头,");
        foreach (Pinyin pinyin in pinyinList)
        {
            Console.Write("{0},", pinyin.getHead());
        }
        Console.WriteLine();

        // 拼音转换可选保留无拼音的原字符
        Console.WriteLine(HanLP.convertToPinyinString("截至2012年，", " ", true));
        Console.WriteLine(HanLP.convertToPinyinString("截至2012年，", " ", false));
    }
}
