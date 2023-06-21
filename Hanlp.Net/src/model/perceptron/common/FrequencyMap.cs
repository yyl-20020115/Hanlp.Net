/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM7:41</create-date>
 *
 * <copyright file="FrequencyMap.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron.common;


/**
 * @author hankcs
 */
public class FrequencyMap : Dictionary<string, int>
{
    public int totalFrequency;

    public int Add(string word)
    {
        ++totalFrequency;
        int frequency = get(word);
        if (frequency == null)
        {
            Add(word, 1);
            return 1;
        }
        else
        {
            Add(word, ++frequency);
            return frequency;
        }
    }
}
