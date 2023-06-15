/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-20 下午12:19</create-date>
 *
 * <copyright file="SegmentTestCase.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.seg;



/**
 * @author hankcs
 */
[TestClass]
public class SegmentTestCase : TestCase
{
    
    public static void assertNoNature(List<Term> termList, Nature nature)
    {
        foreach (Term term in termList)
        {
            Assert.AreNotSame(nature, term.nature);
        }
    }

    
    public static void assertSegmentationHas(List<Term> termList, String part)
    {
        StringBuilder sbSentence = new StringBuilder();
        for (Term term : termList)
        {
            sbSentence.append(term.word);
        }
        assertFalse(sbSentence.ToString().Contains(part));
    }

}
