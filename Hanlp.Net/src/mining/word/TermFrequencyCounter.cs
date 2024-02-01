/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-07-31 9:16 PM</create-date>
 *
 * <copyright file="TermFrequencyCounter.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.corpus.occurrence;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.summary;
using System.Collections.ObjectModel;

namespace com.hankcs.hanlp.mining.word;



/**
 * 词频统计工具
 *
 * @author hankcs
 */
public class TermFrequencyCounter : KeywordExtractor , ICollection<TermFrequency>
{
    bool filterStopWord;
    Dictionary<string, TermFrequency> termFrequencyMap;

    /**
     * 构造
     *
     * @param filterStopWord 是否过滤停用词
     * @param segment        分词器
     */
    public TermFrequencyCounter(Segment segment, bool filterStopWord)
    {
        this.filterStopWord = filterStopWord;
        this.defaultSegment = segment;
        termFrequencyMap = new ();
    }

    public TermFrequencyCounter()
        : this(HanLP.newSegment(), true)
    {
       ;
    }

    public void Add(string document)
    {
        if (document == null || document.isEmpty()) return;
        List<Term> termList = defaultSegment.seg(document);
        Add(termList);
    }

    public void Add(List<Term> termList)
    {
        if (filterStopWord)
        {
            filter(termList);
        }
        foreach (Term term in termList)
        {
            string word = term.word;
            TermFrequency frequency = termFrequencyMap.get(word);
            if (frequency == null)
            {
                frequency = new TermFrequency(word);
                termFrequencyMap.Add(word, frequency);
            }
            else
            {
                frequency.increase();
            }
        }
    }

    /**
     * 取前N个高频词
     *
     * @param N
     * @return
     */
    public ICollection<TermFrequency> top(int N)
    {
        MaxHeap<TermFrequency> heap = new MaxHeap<TermFrequency>(N, new CMP());
        heap.AddRange(termFrequencyMap.values());
        return heap.ToList();
    }

    public class CMP : IComparer<TermFrequency>
    {
        //@Override
        public int Compare(TermFrequency o1, TermFrequency o2)
        {
            return o1.compareTo(o2);
        }
    }
    /**
     * 所有词汇的频次
     *
     * @return
     */
    public ICollection<TermFrequency> all()
    {
        return termFrequencyMap.Values;
    }

    //@Override
    public int Count=> termFrequencyMap.Count;

    //@Override
    public bool isEmpty()
    {
        return termFrequencyMap.Count == 0;
    }

    //@Override
    public bool Contains(Object o)
    {
        if (o is string s)
            return termFrequencyMap.ContainsKey(s);
        else if (o is TermFrequency t)
            return termFrequencyMap.ContainsValue(t);
        return false;
    }

    //@Override
    public IEnumerator<TermFrequency> GetEnumerator()
    {
        return termFrequencyMap.Values.GetEnumerator();
    }

    //@Override
    public Object[] ToArray()
    {
        return termFrequencyMap.Values.ToArray();
    }

    //@Override
    public  T[] ToArray<T>(T[] a)
    {
        return termFrequencyMap.Values.ToArray();
    }

    //@Override
    public bool Add(TermFrequency termFrequency)
    {
        TermFrequency tf = termFrequencyMap.get(termFrequency.getTerm());
        if (tf == null)
        {
            termFrequencyMap.Add(termFrequency.Key, termFrequency);
            return true;
        }
        tf.increase(termFrequency.getFrequency());
        return false;
    }

    //@Override
    public bool Remove(Object o)
    {
        return termFrequencyMap.Remove(o) != null;
    }

    //@Override
    public bool containsAll(Collection c)
    {
        foreach (Object o in c)
        {
            if (!Contains(o))
                return false;
        }
        return true;
    }

    //@Override
    public bool AddRange(Collection<TermFrequency> c)
    {
        foreach (TermFrequency termFrequency in c)
        {
            Add(termFrequency);
        }
        return !c.isEmpty();
    }

    //@Override
    public bool removeAll(Collection c)
    {
        foreach (Object o in c)
        {
            if (!Remove(o))
                return false;
        }
        return true;
    }

    //@Override
    public bool retainAll(Collection c)
    {
        return termFrequencyMap.Values.retainAll(c);
    }

    //@Override
    public void Clear()
    {
        termFrequencyMap.Clear();
    }

    /**
     * 提取关键词（非线程安全）
     *
     * @param termList
     * @param size
     * @return
     */
    //@Override
    public List<string> getKeywords(List<Term> termList, int size)
    {
        Clear();
        Add(termList);
        Collection<TermFrequency> topN = top(size);
        List<string> r = new (topN.Count);
        foreach (TermFrequency termFrequency in topN)
        {
            r.Add(termFrequency.getTerm());
        }
        return r;
    }

    /**
     * 提取关键词（线程安全）
     *
     * @param document 文档内容
     * @param size     希望提取几个关键词
     * @return 一个列表
     */
    public static List<string> getKeywordList(string document, int size)
    {
        return new TermFrequencyCounter().getKeywords(document, size);
    }

    //@Override
    public override string ToString()
    {
        int max = 100;
        return top(Math.Min(max, Count)).ToString();
    }
}
