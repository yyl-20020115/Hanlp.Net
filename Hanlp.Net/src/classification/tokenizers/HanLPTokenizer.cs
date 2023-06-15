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
namespace com.hankcs.hanlp.classification.tokenizers;



/**
 * @author hankcs
 */
public class HanLPTokenizer : ITokenizer
{
    public string[] segment(string text)
    {
        char[] charArray = text.ToCharArray();
        List<Term> termList = NotionalTokenizer.segment(charArray);
        ListIterator<Term> listIterator = termList.listIterator();
        while (listIterator.hasNext())
        {
            Term term = listIterator.next();
            if (term.word.indexOf('\u0000') >= 0)
            {
                listIterator.remove();
            }
        }
        string[] termArray = new string[termList.size()];
        int p = -1;
        for (Term term : termList)
        {
            termArray[++p] = term.word;
        }
        return termArray;
    }
}
