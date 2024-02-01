/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-05 PM8:39</create-date>
 *
 * <copyright file="ImmutableFeatureHashMap.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * @author hankcs
 */
public class ImmutableFeatureMap : FeatureMap
{
    public Dictionary<string, int> featureIdMap;

    public ImmutableFeatureMap(Dictionary<string, int> featureIdMap, TagSet tagSet)
        : base(tagSet)
    {
        ;
        this.featureIdMap = featureIdMap;
    }

    public ImmutableFeatureMap(HashSet<KeyValuePair<string, int>> entrySet, TagSet tagSet)
        : base(tagSet)
    {
        ;
        this.featureIdMap = new ();
        foreach (KeyValuePair<string, int> entry in entrySet)
        {
            featureIdMap.Add(entry.Key, entry.Value);
        }
    }

    //@Override
    public override int idOf(string s)
    {
        return !featureIdMap.TryGetValue(s,out var id) ? -1 : id;
    }

    //@Override
    public override int Count()
    {
        return featureIdMap.Count;
    }

    //@Override
    public HashSet<KeyValuePair<string, int>> entrySet()
    {
        return featureIdMap.ToHashSet();
    }
}