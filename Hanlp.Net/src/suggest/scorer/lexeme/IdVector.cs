/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/17 14:01</create-date>
 *
 * <copyright file="IdVector.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.suggest.scorer.lexeme;



/**
 * 一个同义词有多个id，多个同义词用这个封装做key
 *
 * @author hankcs
 */
public class IdVector : IComparable<IdVector>, ISentenceKey<IdVector>
{
    public List<long[]> idArrayList;

    public IdVector(string sentence)
        :this(CoreSynonymDictionaryEx.convert(IndexTokenizer.segment(sentence), false))
    {
    }

    public IdVector(List<long[]> idArrayList)
    {
        this.idArrayList = idArrayList;
    }

    //@Override
    public virtual int CompareTo(IdVector? o)
    {
        int len1 = idArrayList.Count;
        int len2 = o.idArrayList.Count;
        int lim = Math.Min(len1, len2);
        IEnumerator<long[]> iterator1 = idArrayList.GetEnumerator();
        IEnumerator<long[]> iterator2 = o.idArrayList.GetEnumerator();

        int k = 0;
        while (k < lim)
        {
            long[] c1 = iterator1.next();
            long[] c2 = iterator2.next();
            if (ArrayDistance.ComputeMinimumDistance(c1, c2) != 0)
            {
                return ArrayCompare.Compare(c1, c2);
            }
            ++k;
        }
        return len1 - len2;
    }

    //@Override
    public double similarity(IdVector other)
    {
        double score = 0.0;
        foreach (long[] a in idArrayList)
        {
            foreach (long[] b in other.idArrayList)
            {
                long distance = ArrayDistance.ComputeAverageDistance(a, b);
                score += 1.0 / (0.1 + distance);
            }
        }

        return score / other.idArrayList.Count;
    }
}
