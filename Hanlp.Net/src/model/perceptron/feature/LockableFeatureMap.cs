/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-21 9:04 AM</create-date>
 *
 * <copyright file="LockableFeatureMap.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron.feature;


/**
 * 可切换锁定状态的特征id映射
 *
 * @author hankcs
 */
public class LockableFeatureMap : ImmutableFeatureMDatMap
{
    public LockableFeatureMap(TagSet tagSet)
        :base(tagSet)
    {
    }

    //@Override
    public int idOf(string s)
    {
        int id = base.idOf(s); // 查询id
        if (id == -1 && mutable) // 如果不存在该key且处于可写状态
        {
            id = dat.Count;
            dat.Add(s, id); // 则为key分配新id
        }
        return id;
    }
}
