/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/12 21:39</create-date>
 *
 * <copyright file="JapanesePersonRecogniton.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.dictionary.nr;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.recognition.nr;




/**
 * 日本人名识别
 *
 * @author hankcs
 */
public class JapanesePersonRecognition
{
    /**
     * 执行识别
     *
     * @param segResult      粗分结果
     * @param wordNetOptimum 粗分结果对应的词图
     * @param wordNetAll     全词图
     */
    public static void recognition(List<Vertex> segResult, WordNet wordNetOptimum, WordNet wordNetAll)
    {
        StringBuilder sbName = new StringBuilder();
        int appendTimes = 0;
        char[] charArray = wordNetAll.charArray;
        DoubleArrayTrie<char>.LongestSearcher searcher = JapanesePersonDictionary.getSearcher(charArray);
        int activeLine = 1;
        int preOffset = 0;
        while (searcher.next())
        {
            char label = searcher.value;
            int offset = searcher.begin;
            string key = new string(charArray, offset, searcher.Length);
            if (preOffset != offset)
            {
                if (appendTimes > 1 && sbName.Length > 2) // 日本人名最短为3字
                {
                    insertName(sbName.ToString(), activeLine, wordNetOptimum, wordNetAll);
                }
                sbName.Length=0;
                appendTimes = 0;
            }
            if (appendTimes == 0)
            {
                if (label == JapanesePersonDictionary.X)
                {
                    sbName.Append(key);
                    ++appendTimes;
                    activeLine = offset + 1;
                }
            }
            else
            {
                if (label == JapanesePersonDictionary.M)
                {
                    sbName.Append(key);
                    ++appendTimes;
                }
                else
                {
                    if (appendTimes > 1 && sbName.Length > 2)
                    {
                        insertName(sbName.ToString(), activeLine, wordNetOptimum, wordNetAll);
                    }
                    sbName.Length=0;
                    appendTimes = 0;
                }
            }
            preOffset = offset + key.Length;
        }
        if (sbName.Length > 0)
        {
            if (appendTimes > 1)
            {
                insertName(sbName.ToString(), activeLine, wordNetOptimum, wordNetAll);
            }
        }
    }

    /**
     * 是否是bad case
     * @param name
     * @return
     */
    public static bool isBadCase(string name)
    {
        char label = JapanesePersonDictionary.get(name);
        if (label == null) return false;
        return label.Equals(JapanesePersonDictionary.A);
    }

    /**
     * 插入日本人名
     * @param name
     * @param activeLine
     * @param wordNetOptimum
     * @param wordNetAll
     */
    private static void insertName(string name, int activeLine, WordNet wordNetOptimum, WordNet wordNetAll)
    {
        if (isBadCase(name)) return;
        wordNetOptimum.insert(activeLine, new Vertex(Predefine.TAG_PEOPLE, name, new CoreDictionary.Attribute(Nature.nrj), WORD_ID), wordNetAll);
    }
}
