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
public class FrequencyMap : TreeMap<String, int>
{
    public int totalFrequency;

    public int add(String word)
    {
        ++totalFrequency;
        int frequency = get(word);
        if (frequency == null)
        {
            put(word, 1);
            return 1;
        }
        else
        {
            put(word, ++frequency);
            return frequency;
        }
    }
}
