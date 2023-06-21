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
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.seg.Other;



/**
 * 使用DoubleArrayTrie实现的最长分词器
 *
 * @author hankcs
 */
public class DoubleArrayTrieSegment : DictionaryBasedSegment
{
    /**
     * 分词用到的trie树，可以直接赋值为自己的trie树（赋值操作不保证线程安全）
     */
    public DoubleArrayTrie<CoreDictionary.Attribute> trie;

    /**
     * 使用核心词库的trie树构造分词器
     */
    public DoubleArrayTrieSegment()
        : this(CoreDictionary.trie)
    {
        ;
    }

    /**
     * 根据自己的trie树构造分词器
     *
     * @param trie
     */
    public DoubleArrayTrieSegment(DoubleArrayTrie<CoreDictionary.Attribute> trie)
        :base()
    {
        this.trie = trie;
        config.useCustomDictionary = false;
    }

    /**
     * 加载自己的词典，构造分词器
     * @param dictionaryPaths 任意数量个词典
     *
     * @ 加载过程中的IO异常
     */
    public DoubleArrayTrieSegment(params string[] dictionaryPaths)
        : this(new DoubleArrayTrie<CoreDictionary.Attribute>(
            IOUtil.loadDictionary(dictionaryPaths)))
    {
        ;
    }

    //@Override
    protected List<Term> segSentence(char[] sentence)
    {
        char[] charArray = sentence;
         int[] wordNet = new int[charArray.Length];
        Array.Fill(wordNet, 1);
         Nature[] natureArray = config.speechTagging ? new Nature[charArray.Length] : null;
        matchLongest(sentence, wordNet, natureArray, trie);
        if (config.useCustomDictionary)
        {
            matchLongest(sentence, wordNet, natureArray, CustomDictionary.dat);
            if (CustomDictionary.trie != null)
            {
                CustomDictionary.trie.parseLongestText(charArray, new CT());
            }
        }
        LinkedList<Term> termList = new ();
        posTag(charArray, wordNet, natureArray);
        for (int i = 0; i < wordNet.Length; )
        {
            Term term = new Term(new string(charArray, i, wordNet[i]), 
                config.speechTagging ? (natureArray[i] == null ? Nature.nz : natureArray[i]) : null);
            term.offset = i;
            termList.Add(term);
            i += wordNet[i];
        }
        return termList;
    }
    public class CT: AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute>
    {
        //@Override
        public void hit(int begin, int end, CoreDictionary.Attribute value)
        {
            int Length = end - begin;
            if (Length > wordNet[begin])
            {
                wordNet[begin] = Length;
                if (config.speechTagging)
                {
                    natureArray[begin] = value.nature[0];
                }
            }
        }
    }
    private void matchLongest(char[] sentence, int[] wordNet, Nature[] natureArray, DoubleArrayTrie<CoreDictionary.Attribute> trie)
    {
        DoubleArrayTrie<CoreDictionary.Attribute>.LongestSearcher searcher = trie.getLongestSearcher(sentence, 0);
        while (searcher.next())
        {
            wordNet[searcher.begin] = searcher.Length;
            if (config.speechTagging)
            {
                natureArray[searcher.begin] = searcher.value.nature[0];
            }
        }
    }
}
