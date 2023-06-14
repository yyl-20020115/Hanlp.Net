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
namespace com.hankcs.hanlp.suggest.scorer.lexeme;



/**
 * 一个同义词有多个id，多个同义词用这个封装做key
 *
 * @author hankcs
 */
public class IdVector : Comparable<IdVector>, ISentenceKey<IdVector>
{
    public List<long[]> idArrayList;

    public IdVector(String sentence)
    {
        this(CoreSynonymDictionaryEx.convert(IndexTokenizer.segment(sentence), false));
    }

    public IdVector(List<long[]> idArrayList)
    {
        this.idArrayList = idArrayList;
    }

    //@Override
    public int compareTo(IdVector o)
    {
        int len1 = idArrayList.size();
        int len2 = o.idArrayList.size();
        int lim = Math.min(len1, len2);
        Iterator<long[]> iterator1 = idArrayList.iterator();
        Iterator<long[]> iterator2 = o.idArrayList.iterator();

        int k = 0;
        while (k < lim)
        {
            long[] c1 = iterator1.next();
            long[] c2 = iterator2.next();
            if (ArrayDistance.computeMinimumDistance(c1, c2) != 0)
            {
                return ArrayCompare.compare(c1, c2);
            }
            ++k;
        }
        return len1 - len2;
    }

    //@Override
    public Double similarity(IdVector other)
    {
        Double score = 0.0;
        for (long[] a : idArrayList)
        {
            for (long[] b : other.idArrayList)
            {
                long distance = ArrayDistance.computeAverageDistance(a, b);
                score += 1.0 / (0.1 + distance);
            }
        }

        return score / other.idArrayList.size();
    }
}
