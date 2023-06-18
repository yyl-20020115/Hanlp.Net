/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-05 PM8:19</create-date>
 *
 * <copyright file="ImmutableFeatureMap.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * @author hankcs
 */
public class ImmutableFeatureDatMap : FeatureMap
{
    DoubleArrayTrie<int> dat;

    public ImmutableFeatureDatMap(Dictionary<string, int> featureIdMap, TagSet tagSet)
    {
        base(tagSet);
        dat = new DoubleArrayTrie<int>();
        dat.build(featureIdMap);
    }

    //@Override
    public int idOf(string string)
    {
        return dat.exactMatchSearch(string);
    }

    //@Override
    public int size()
    {
        return dat.size();
    }

    //@Override
    public HashSet<KeyValuePair<string, int>> entrySet()
    {
        throw new InvalidOperationException("这份DAT实现不支持遍历");
    }
}
