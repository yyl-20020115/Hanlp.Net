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
        : base(tagSet, true)
    {
        ;
        featureIdMap = new Dictionary<string, int>();
        addTransitionFeatures(tagSet);
    }

    private void addTransitionFeatures(TagSet tagSet)
    {
        for (int i = 0; i < tagSet.Count; i++)
        {
            idOf("BL=" + tagSet.stringOf(i));
        }
        idOf("BL=_BL_");
    }

    public MutableFeatureMap(TagSet tagSet, Dictionary<string, int> featureIdMap)
        : base(tagSet)
    {
       ;
        this.featureIdMap = featureIdMap;
        addTransitionFeatures(tagSet);
    }

    //@Override
    public override HashSet<KeyValuePair<string, int>> entrySet()
    {
        return featureIdMap.ToHashSet();
    }

    //@Override
    public override int idOf(string s)
    {
        if (!featureIdMap.TryGetValue(s,out var id))
        {
            featureIdMap.Add(s, id = featureIdMap.Count);
        }
        return id;
    }

    public int Count=> featureIdMap.Count;

    public HashSet<string> featureSet()
    {
        return featureIdMap.Keys.ToHashSet();
    }

    //@Override
    public virtual int[] allLabels()
    {
        return tagSet.allTags();
    }

    //@Override
    public virtual int bosTag()
    {
        return tagSet.Count;
    }
}