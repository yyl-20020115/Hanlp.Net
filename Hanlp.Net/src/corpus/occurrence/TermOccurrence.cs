/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/28 13:53</create-date>
 *
 * <copyright file="TermOccurence.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;

namespace com.hankcs.hanlp.corpus.occurrence;



/**
 * 词频统计
 * @author hankcs
 */
public class TermOccurrence
{
    /**
     * 词频统计用的储存结构
     */
    BinTrie<TermFrequency> trieSingle;
    int totalTerm;

    public TermOccurrence()
    {
        trieSingle = new BinTrie<TermFrequency>();
    }

    public void Add(string term)
    {
        TermFrequency value = trieSingle.get(term);
        if (value == null)
        {
            value = new TermFrequency(term);
            trieSingle.put(term, value);
        }
        else
        {
            value.increase();
        }
        ++totalTerm;
    }

    public void addAll(List<string> termList)
    {
        foreach (string s in termList)
        {
            Add(s);
        }
    }

    public HashSet<KeyValuePair<string, TermFrequency>> getEntrySet()
    {
        return trieSingle.entrySet();
    }
}
