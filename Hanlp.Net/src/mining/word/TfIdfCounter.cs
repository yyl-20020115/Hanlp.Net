/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-12 PM4:22</create-date>
 *
 * <copyright file="TfIdfKeyword.java" company="码农场">
 * Copyright (c) 2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.collection.MDAG;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.summary;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.mining.word;



/**
 * TF-IDF统计工具兼关键词提取工具
 *
 * @author hankcs
 */
public class TfIdfCounter : KeywordExtractor
{
    private bool filterStopWord;
    private Dictionary<Object, Dictionary<string, Double>> tfMap;
    private Dictionary<Object, Dictionary<string, Double>> tfidfMap;
    private Dictionary<string, Double> idf;

    public TfIdfCounter()
        : this(true)
    {
        ;
    }

    public TfIdfCounter(bool filterStopWord)
        : this(StandardTokenizer.SEGMENT, filterStopWord)
    {
        ;
    }

    public TfIdfCounter(Segment defaultSegment, bool filterStopWord)
        : base(defaultSegment)
    {
        ;
        this.filterStopWord = filterStopWord;
        tfMap = new ();
    }

    public TfIdfCounter(Segment defaultSegment)
        : this(defaultSegment, true)
    {
        ;
    }

    //@Override
    public List<string> getKeywords(List<Term> termList, int size)
    {
        List<KeyValuePair<string, Double>> entryList = getKeywordsWithTfIdf(termList, size);
        List<string> r = new List<string>(entryList.Count);
        foreach (KeyValuePair<string, Double> entry in entryList)
        {
            r.Add(entry.Key);
        }

        return r;
    }

    public List<KeyValuePair<string, Double>> getKeywordsWithTfIdf(string document, int size)
    {
        return getKeywordsWithTfIdf(preprocess(document), size);
    }


    public List<KeyValuePair<string, Double>> getKeywordsWithTfIdf(List<Term> termList, int size)
    {
        if (idf == null)
            compute();

        Dictionary<string, Double> tfIdf = TfIdf.tfIdf(TfIdf.tf(convert(termList)), idf);
        return topN(tfIdf, size);
    }

    public void Add(Object id, List<Term> termList)
    {
        List<string> words = convert(termList);
        Dictionary<string, Double> tf = TfIdf.tf(words);
        tfMap.Add(id, tf);
        idf = null;
    }

    private static List<string> convert(List<Term> termList)
    {
        List<string> words = new List<string>(termList.Count);
        foreach (Term term in termList)
        {
            words.Add(term.word);
        }
        return words;
    }

    public void Add(List<Term> termList)
    {
        Add(tfMap.Count, termList);
    }

    /**
     * 添加文档
     *
     * @param id   文档id
     * @param text 文档内容
     */
    public void Add(Object id, string text)
    {
        List<Term> termList = preprocess(text);
        Add(id, termList);
    }

    private List<Term> preprocess(string text)
    {
        List<Term> termList = defaultSegment.seg(text);
        if (filterStopWord)
        {
            filter(termList);
        }
        return termList;
    }

    /**
     * 添加文档，自动分配id
     *
     * @param text
     */
    public int Add(string text)
    {
        int id = tfMap.Count;
        Add(id, text);
        return id;
    }

    public Dictionary<Object, Dictionary<string, Double>> compute()
    {
        idf = TfIdf.idfFromTfs(tfMap.Values);
        tfidfMap = new (idf.Count);
        foreach (KeyValuePair<Object, Dictionary<string, Double>> entry in tfMap)
        {
            Dictionary<string, Double> tfidf = TfIdf.tfIdf(entry.Value, idf);
            tfidfMap.Add(entry.Key, tfidf);
        }
        return tfidfMap;
    }

    public List<KeyValuePair<string, Double>> getKeywordsOf(Object id)
    {
        return getKeywordsOf(id, 10);
    }


    public List<KeyValuePair<string, Double>> getKeywordsOf(Object id, int size)
    {
        Dictionary<string, Double> tfidfs = tfidfMap.get(id);
        if (tfidfs == null) return null;

        return topN(tfidfs, size);
    }

    private List<KeyValuePair<string, Double>> topN(Dictionary<string, Double> tfidfs, int size)
    {
        MaxHeap<KeyValuePair<string, Double>> heap = new MaxHeap<KeyValuePair<string, Double>>(size, new CT());
        heap.AddRange(tfidfs);
        return heap.ToList();
    }
    public class CT: IComparer<KeyValuePair<string, Double>>
    {
        //@Override
        public int Compare(KeyValuePair<string, Double> o1, KeyValuePair<string, Double> o2)
        {
            return o1.Value.CompareTo(o2.Value);
        }
    }
    public HashSet<Object> documents()
    {
        return tfMap.Keys;
    }

    public Dictionary<Object, Dictionary<string, Double>> getTfMap()
    {
        return tfMap;
    }

    public List<KeyValuePair<string, Double>> sortedAllTf()
    {
        return sort(allTf());
    }

    public List<KeyValuePair<string, int>> sortedAllTfInt()
    {
        return doubleToInteger(sortedAllTf());
    }

    public Dictionary<string, Double> allTf()
    {
        Dictionary<string, Double> result = new ();
        foreach (Dictionary<string, Double> d in tfMap.Values)
        {
            foreach (KeyValuePair<string, Double> tf in d.entrySet())
            {
                Double f = result.get(tf.Key);
                if (f == null)
                {
                    result.Add(tf.Key, tf.Value);
                }
                else
                {
                    result.Add(tf.Key, f + tf.Value);
                }
            }
        }

        return result;
    }

    private static List<KeyValuePair<string, Double>> sort(Dictionary<string, Double> map)
    {
        List<KeyValuePair<string, Double>> list = new (map.ToHashSet());
        Collections.sort(list, new CN());

        return list;
    }
    public class CN: IComparer<KeyValuePair<string, Double>>
    {
        //@Override
        public int Compare(KeyValuePair<string, Double> o1, KeyValuePair<string, Double> o2)
        {
            return o2.Value.compareTo(o1.Value);
        }
    }
    private static List<KeyValuePair<string, int>> doubleToInteger(List<KeyValuePair<string, Double>> list)
    {
        List<KeyValuePair<string, int>> result = new (list.Count);
        foreach (KeyValuePair<string, Double> entry in list)
        {
            result.Add(new AbstractMap.SimpleEntry<string, int>(entry.Key, entry.Value.intValue()));
        }

        return result;
    }
}
