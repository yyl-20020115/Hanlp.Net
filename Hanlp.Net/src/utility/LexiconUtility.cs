/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/7/14 11:01</create-date>
 *
 * <copyright file="WordNatureUtil.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.utility;



/**
 * 跟词语与词性有关的工具类，可以全局动态修改HanLP内部词库
 *
 * @author hankcs
 */
public class LexiconUtility
{
    /**
     * 从HanLP的词库中提取某个单词的属性（包括核心词典和用户词典）
     *
     * @param word 单词
     * @return 包含词性与频次的信息
     */
    public static CoreDictionary.Attribute GetAttribute(string word)
    {
        CoreDictionary.Attribute attribute = CoreDictionary.get(word);
        if (attribute != null) return attribute;
        return CustomDictionary.get(word);
    }

    /**
     * 词库是否收录了词语（查询核心词典和用户词典）
     * @param word
     * @return
     */
    public static bool Contains(string word)
    {
        return GetAttribute(word) != null;
    }

    /**
     * 从HanLP的词库中提取某个单词的属性（包括核心词典和用户词典）
     *
     * @param term 单词
     * @return 包含词性与频次的信息
     */
    public static CoreDictionary.Attribute GetAttribute(Term term)
    {
        return GetAttribute(term.word);
    }

    /**
     * 获取某个单词的词频
     * @param word
     * @return
     */
    public static int GetFrequency(string word)
    {
        CoreDictionary.Attribute attribute = GetAttribute(word);
        if (attribute == null) return 0;
        return attribute.totalFrequency;
    }

    /**
     * 设置某个单词的属性
     * @param word
     * @param attribute
     * @return
     */
    public static bool SetAttribute(string word, CoreDictionary.Attribute attribute)
    {
        if (attribute == null) return false;

        if (CoreDictionary.trie.set(word, attribute)) return true;
        if (CustomDictionary.dat.set(word, attribute)) return true;
        if (CustomDictionary.trie == null)
        {
            CustomDictionary.Add(word);
        }
        CustomDictionary.trie.Add(word, attribute);
        return true;
    }

    /**
     * 设置某个单词的属性
     * @param word
     * @param natures
     * @return
     */
    public static bool SetAttribute(string word, params Nature[] natures)
    {
        if (natures == null) return false;

        CoreDictionary.Attribute attribute = new CoreDictionary.Attribute(natures, new int[natures.Length]);
        System.Array.Fill(attribute.frequency, 1);
        return SetAttribute(word, attribute);
    }

    /**
     * 设置某个单词的属性
     * @param word
     * @param natures
     * @return
     */
    public static bool SetAttribute(string word, params string[] natures)
    {
        if (natures == null) return false;

        Nature[] natureArray = new Nature[natures.Length];
        for (int i = 0; i < natureArray.Length; i++)
        {
            natureArray[i] = Nature.create(natures[i]);
        }

        return SetAttribute(word, natureArray);
    }


    /**
     * 设置某个单词的属性
     * @param word
     * @param natureWithFrequency
     * @return
     */
    public static bool SetAttribute(string word, string natureWithFrequency)
    {
        CoreDictionary.Attribute attribute = CoreDictionary.Attribute.create(natureWithFrequency);
        return SetAttribute(word, attribute);
    }

    /**
     * 将字符串词性转为Enum词性
     * @param name 词性名称
     * @param customNatureCollector 一个收集集合
     * @return 转换结果
     */
    public static Nature ConvertStringToNature(string name, HashSet<Nature> customNatureCollector)
    {
        Nature nature = Nature.fromString(name);
        if (nature == null)
        {
            nature = Nature.create(name);
            if (customNatureCollector != null) customNatureCollector.Add(nature);
        }
        return nature;
    }

    /**
     * 将字符串词性转为Enum词性
     * @param name 词性名称
     * @return 转换结果
     */
    public static Nature ConvertStringToNature(string name)
    {
        return ConvertStringToNature(name, null);
    }
}
