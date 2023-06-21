/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM5:24</create-date>
 *
 * <copyright file="MutableFeatureMap.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * @author hankcs
 */
public class MutableFeatureMap : FeatureMap
{
    public Dictionary<string, int> featureIdMap;
    // Dictionary 5136
    // Bin 2712
    // DAT minutes
    // trie4j 3411

    public MutableFeatureMap(TagSet tagSet)
    {
        base(tagSet, true);
        featureIdMap = new Dictionary<string, int>();
        addTransitionFeatures(tagSet);
    }

    private void addTransitionFeatures(TagSet tagSet)
    {
        for (int i = 0; i < tagSet.size(); i++)
        {
            idOf("BL=" + tagSet.stringOf(i));
        }
        idOf("BL=_BL_");
    }

    public MutableFeatureMap(TagSet tagSet, Dictionary<string, int> featureIdMap)
    {
        base(tagSet);
        this.featureIdMap = featureIdMap;
        addTransitionFeatures(tagSet);
    }

    //@Override
    public HashSet<KeyValuePair<string, int>> entrySet()
    {
        return featureIdMap.entrySet();
    }

    //@Override
    public int idOf(string s)
    {
        int id = featureIdMap.get(s);
        if (id == null)
        {
            id = featureIdMap.size();
            featureIdMap.Add(s, id);
        }

        return id;
    }

    public int size()
    {
        return featureIdMap.size();
    }

    public HashSet<string> featureSet()
    {
        return featureIdMap.Keys;
    }

    //@Override
    public int[] allLabels()
    {
        return tagSet.allTags();
    }

    //@Override
    public int bosTag()
    {
        return tagSet.size();
    }
}