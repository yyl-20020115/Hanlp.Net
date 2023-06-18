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
        tfMap = new HashMap<Object, Dictionary<string, Double>>();
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
        List<string> r = new List<string>(entryList.size());
        foreach (KeyValuePair<string, Double> entry in entryList)
        {
            r.Add(entry.getKey());
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
        tfMap.put(id, tf);
        idf = null;
    }

    private static List<string> convert(List<Term> termList)
    {
        List<string> words = new List<string>(termList.size());
        foreach (Term term in termList)
        {
            words.Add(term.word);
        }
        return words;
    }

    public void Add(List<Term> termList)
    {
        Add(tfMap.size(), termList);
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
        int id = tfMap.size();
        Add(id, text);
        return id;
    }

    public Dictionary<Object, Dictionary<string, Double>> compute()
    {
        idf = TfIdf.idfFromTfs(tfMap.values());
        tfidfMap = new HashMap<Object, Dictionary<string, Double>>(idf.size());
        for (KeyValuePair<Object, Dictionary<string, Double>> entry : tfMap.entrySet())
        {
            Dictionary<string, Double> tfidf = TfIdf.tfIdf(entry.getValue(), idf);
            tfidfMap.put(entry.getKey(), tfidf);
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
        heap.addAll(tfidfs.entrySet());
        return heap.toList();
    }
    public class CT: IComparer<KeyValuePair<string, Double>>()
    {
        //@Override
        public int Compare(KeyValuePair<string, Double> o1, KeyValuePair<string, Double> o2)
        {
            return o1.getValue().compareTo(o2.getValue());
        }
    }
    public HashSet<Object> documents()
    {
        return tfMap.keySet();
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
        foreach (Dictionary<string, Double> d in tfMap.values())
        {
            foreach (KeyValuePair<string, Double> tf in d.entrySet())
            {
                Double f = result.get(tf.getKey());
                if (f == null)
                {
                    result.put(tf.getKey(), tf.getValue());
                }
                else
                {
                    result.put(tf.getKey(), f + tf.getValue());
                }
            }
        }

        return result;
    }

    private static List<KeyValuePair<string, Double>> sort(Dictionary<string, Double> map)
    {
        List<KeyValuePair<string, Double>> list = new (map.entrySet());
        Collections.sort(list, new CN());

        return list;
    }
    public class CN: IComparer<KeyValuePair<string, Double>>
    {
        //@Override
        public int Compare(KeyValuePair<string, Double> o1, KeyValuePair<string, Double> o2)
        {
            return o2.getValue().compareTo(o1.getValue());
        }
    }
    private static List<KeyValuePair<string, int>> doubleToInteger(List<KeyValuePair<string, Double>> list)
    {
        List<KeyValuePair<string, int>> result = new ArrayList<KeyValuePair<string, int>>(list.size());
        foreach (KeyValuePair<string, Double> entry in list)
        {
            result.Add(new AbstractMap.SimpleEntry<string, int>(entry.getKey(), entry.getValue().intValue()));
        }

        return result;
    }
}
