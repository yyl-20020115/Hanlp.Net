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
namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * 用MutableDoubleArrayTrie实现的ImmutableFeatureMap
 * @author hankcs
 */
public class ImmutableFeatureMDatMap : FeatureMap
{
    MutableDoubleArrayTrieInteger dat;

    public ImmutableFeatureMDatMap()
    {
        super();
        dat = new MutableDoubleArrayTrieInteger();
    }

    public ImmutableFeatureMDatMap(TagSet tagSet)
    {
        super(tagSet);
        dat = new MutableDoubleArrayTrieInteger();
    }

    public ImmutableFeatureMDatMap(MutableDoubleArrayTrieInteger dat, TagSet tagSet)
    {
        super(tagSet);
        this.dat = dat;
    }

    public ImmutableFeatureMDatMap(Map<String, Integer> featureIdMap, TagSet tagSet)
    {
        super(tagSet);
        dat = new MutableDoubleArrayTrieInteger(featureIdMap);
    }

    public ImmutableFeatureMDatMap(Set<Map.Entry<String, Integer>> featureIdSet, TagSet tagSet)
    {
        super(tagSet);
        dat = new MutableDoubleArrayTrieInteger();
        for (Map.Entry<String, Integer> entry : featureIdSet)
        {
            dat.put(entry.getKey(), entry.getValue());
        }
    }

    //@Override
    public int idOf(String string)
    {
        return dat.get(string);
    }

    //@Override
    public int size()
    {
        return dat.size();
    }

    //@Override
    public Set<Map.Entry<String, Integer>> entrySet()
    {
        return dat.entrySet();
    }

    //@Override
    public void save(DataOutputStream out) 
    {
        tagSet.save(out);
        dat.save(out);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        loadTagSet(byteArray);
        return dat.load(byteArray);
    }
}
