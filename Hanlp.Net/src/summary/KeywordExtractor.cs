/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/18 18:37</create-date>
 *
 * <copyright file="KeywordExtractor.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dictionary.stopword;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.summary;



/**
 * 提取关键词的基类
 *
 * @author hankcs
 */
public abstract class KeywordExtractor
{
    /**
     * 默认分词器
     */
    protected Segment defaultSegment;

    public KeywordExtractor(Segment defaultSegment)
    {
        this.defaultSegment = defaultSegment;
    }

    public KeywordExtractor()
        : this(StandardTokenizer.SEGMENT)
    {
        ;
    }

    /**
     * 是否应当将这个term纳入计算，词性属于名词、动词、副词、形容词
     *
     * @param term
     * @return 是否应当
     */
    protected bool ShouldInclude(Term term)
    {
        // 除掉停用词
        return CoreStopWordDictionary.shouldInclude(term);
    }

    /**
     * 设置关键词提取器使用的分词器
     *
     * @param segment 任何开启了词性标注的分词器
     * @return 自己
     */
    public KeywordExtractor SetSegment(Segment segment)
    {
        defaultSegment = segment;
        return this;
    }

    public Segment GetSegment()
    {
        return defaultSegment;
    }

    /**
     * 提取关键词
     *
     * @param document 关键词
     * @param size     需要几个关键词
     * @return
     */
    public List<string> GetKeywords(string document, int size)
    {
        return GetKeywords(defaultSegment.seg(document), size);
    }

    /**
     * 提取关键词（top 10）
     *
     * @param document 文章
     * @return
     */
    public List<string> GetKeywords(string document)
    {
        return GetKeywords(defaultSegment.seg(document), 10);
    }

    protected void filter(List<Term> termList)
    {
        IEnumerator<Term> listIterator = termList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            if (listIterator.MoveNext() && !ShouldInclude(listIterator.Current))
            {
                //TODO:?
                listIterator.Remove();
            }
        }
    }

    abstract public List<string> GetKeywords(List<Term> termList, int size);
}
