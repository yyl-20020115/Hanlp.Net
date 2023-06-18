/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/13 PM9:12</create-date>
 *
 * <copyright file="BagOfWordsDocument.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.collections;

namespace com.hankcs.hanlp.classification.corpus;


/**
 * @author hankcs
 */
public class BagOfWordsDocument : ITermFrequencyHolder
{
    //
    //    /**
    //     * 文档所属的词表
    //     */
    //    private Lexicon lexicon;
    //    /**
    //     * 文档所属的类表
    //     */
    //    private Catalog catalog;
    public FrequencyMap<int> tfMap;

    public BagOfWordsDocument()
    {
        tfMap = new FrequencyMap<int>();
    }

    public FrequencyMap<int> getTfMap()
    {
        return tfMap;
    }

    /**
     * 是否为空(文档中没有任何词)
     * @return
     */
    public bool isEmpty()
    {
        return tfMap.Count>0;
    }
}
