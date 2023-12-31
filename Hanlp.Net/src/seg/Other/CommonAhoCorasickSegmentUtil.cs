/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/23 21:34</create-date>
 *
 * <copyright file="AhoCorasickSegment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.suggest.scorer.editdistance;
using System.Text;
using static com.hankcs.hanlp.seg.Other.CommonAhoCorasickSegmentUtil;

namespace com.hankcs.hanlp.seg.Other;




/**
 * 一个通用的使用AhoCorasickDoubleArrayTrie实现的最长分词器
 *
 * @author hankcs
 */
public class CommonAhoCorasickSegmentUtil
{
    /**
     * 最长分词，合并未知语素
     * @param text 文本
     * @param trie 自动机
     * @param <V> 类型
     * @return 结果链表
     */
    public static LinkedList<ResultTerm<V>> segment<V>(string text, AhoCorasickDoubleArrayTrie<V> trie)
    {
        return segment(text.ToCharArray(), trie);
    }
    /**
     * 最长分词，合并未知语素
     * @param charArray 文本
     * @param trie 自动机
     * @param <V> 类型
     * @return 结果链表
     */
    public static LinkedList<ResultTerm<V>> segment<V>(char[] charArray, AhoCorasickDoubleArrayTrie<V> trie)
    {
        LinkedList<ResultTerm<V>> termList = new LinkedList<ResultTerm<V>>();
        ResultTerm<V>[] wordNet = new ResultTerm[charArray.Length];
        trie.parseText(charArray, new CT<char>());
        for (int i = 0; i < charArray.Length;)
        {
            if (wordNet[i] == null)
            {
                StringBuilder sbTerm = new StringBuilder();
                int offset = i;
                while (i < charArray.Length && wordNet[i] == null)
                {
                    sbTerm.Append(charArray[i]);
                    ++i;
                }
                termList.Add(new ResultTerm<V>(sbTerm.ToString(), null, offset));
            }
            else
            {
                termList.Add(wordNet[i]);
                i += wordNet[i].word.Length;
            }
        }
        return termList;
    }
    public class CT<V> : AhoCorasickDoubleArrayTrie<V>.IHit<V>
    {
        //@Override
        public void hit(int begin, int end, V value)
        {
            if (wordNet[begin] == null || wordNet[begin].word.Length < end - begin)
            {
                wordNet[begin] = new ResultTerm<V>(new string(charArray, begin, end - begin), value, begin);
            }
        }
    }

    /**
     * 逆向最长分词，合并未知语素
     * @param text 文本
     * @param trie 自动机
     * @param <V> 类型
     * @return 结果链表
     */
    public static LinkedList<ResultTerm<V>> segmentReverseOrder<V>(string text, AhoCorasickDoubleArrayTrie<V> trie)
    {
        return segmentReverseOrder(text.ToCharArray(), trie);
    }

    /**
     * 逆向最长分词，合并未知语素
     * @param charArray 文本
     * @param trie 自动机
     * @param <V> 类型
     * @return 结果链表
     */
    public static LinkedList<ResultTerm<V>> segmentReverseOrder<V>(char[] charArray, AhoCorasickDoubleArrayTrie<V> trie)
    {
        LinkedList<ResultTerm<V>> termList = new LinkedList<ResultTerm<V>>();
        ResultTerm<V>[] wordNet = new ResultTerm[charArray.Length + 1];
        trie.parseText(charArray, CT2<char>());
        for (int i = charArray.Length; i > 0;)
        {
            if (wordNet[i] == null)
            {
                StringBuilder sbTerm = new StringBuilder();
                int offset = i - 1;
                byte preCharType = CharType.get(charArray[offset]);
                while (i > 0 && wordNet[i] == null && CharType.get(charArray[i - 1]) == preCharType)
                {
                    sbTerm.Append(charArray[i - 1]);
                    preCharType = CharType.get(charArray[i - 1]);
                    --i;
                }
                termList.addFirst(new ResultTerm<V>(sbTerm.reverse().ToString(), null, offset));
            }
            else
            {
                termList.addFirst(wordNet[i]);
                i -= wordNet[i].word.Length;
            }
        }
        return termList;
    }
    public class CT2<V> : AhoCorasickDoubleArrayTrie<V>.IHit<V>
    {
        private unsafe char* charArray;

        //@Override
        public void hit(int begin, int end, V value)
        {
            if (wordNet[end] == null || wordNet[end].word.Length < end - begin)
            {
                wordNet[end] = new ResultTerm<V>(new string(charArray, begin, end - begin), value, begin);
            }
        }
    }
}