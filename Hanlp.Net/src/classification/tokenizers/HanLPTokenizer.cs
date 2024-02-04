/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/13 PM8:02</create-date>
 *
 * <copyright file="HanLPTokenizer.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.classification.tokenizers;



/**
 * @author hankcs
 */
public class HanLPTokenizer : ITokenizer
{
    public string[] Segment(string text)
    {
        char[] charArray = text.ToCharArray();
        List<Term> termList = NotionalTokenizer.segment(charArray);
        IEnumerator<Term> listIterator = termList.GetEnumerator();
        List<Term> result = new List<Term>();
        while (listIterator.MoveNext())
        {
            Term term = listIterator.Current;
            if (term.word.IndexOf('\u0000') >= 0)
            {
                //TODO:
                //listIterator.Remove();
            }
            else
            {
                result.Add(term);
            }
        }
        string[] termArray = new string[result.Count];
        int p = -1;
        foreach (Term term in result)
        {
            termArray[++p] = term.word;
        }
        return termArray;
    }
}
