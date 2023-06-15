/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-18 下午8:57</create-date>
 *
 * <copyright file="ImmutableFeatureMDatMap.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * 用MutableDoubleArrayTrie实现的ImmutableFeatureMap
 * @author hankcs
 */
public class ImmutableFeatureMDatMap : FeatureMap
{
    MutableDoubleArrayTrieInteger dat;

    public ImmutableFeatureMDatMap()
        :base()
    {
        dat = new MutableDoubleArrayTrieInteger();
    }

    public ImmutableFeatureMDatMap(TagSet tagSet)
        :base(tagSet)
    {
        dat = new MutableDoubleArrayTrieInteger();
    }

    public ImmutableFeatureMDatMap(MutableDoubleArrayTrieInteger dat, TagSet tagSet)
        :base(tagSet)
    {
        this.dat = dat;
    }

    public ImmutableFeatureMDatMap(Dictionary<String, int> featureIdMap, TagSet tagSet)
        : base(tagSet)
    {
        dat = new MutableDoubleArrayTrieInteger(featureIdMap);
    }

    public ImmutableFeatureMDatMap(Set<Map.Entry<String, int>> featureIdSet, TagSet tagSet)
        : base(tagSet)
    {
        dat = new MutableDoubleArrayTrieInteger();
        for (Map.Entry<String, int> entry : featureIdSet)
        {
            dat.put(entry.getKey(), entry.getValue());
        }
    }

    //@Override
    public int idOf(String s)
    {
        return dat.get(s);
    }

    //@Override
    public int size()
    {
        return dat.size();
    }

    //@Override
    public Set<Map.Entry<String, int>> entrySet()
    {
        return dat.entrySet();
    }

    //@Override
    public void save(DataOutputStream _out) 
    {
        tagSet.save(_out);
        dat.save(_out);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        loadTagSet(byteArray);
        return dat.load(byteArray);
    }
}
