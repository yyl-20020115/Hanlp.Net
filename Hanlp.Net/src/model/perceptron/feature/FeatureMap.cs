/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM5:23</create-date>
 *
 * <copyright file="FeatureMap.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.common;
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * @author hankcs
 */
public abstract class FeatureMap : IStringIdMap, ICacheAble
{
    public abstract int Count { get; }

    public virtual int[] allLabels()
    {
        return tagSet.allTags();
    }

    public virtual int bosTag()
    {
        return tagSet.Count;
    }

    public TagSet tagSet;
    /**
     * 是否允许新增特征
     */
    public bool mutable;

    public FeatureMap(TagSet tagSet)
        : this(tagSet, false)
    {
    }

    public FeatureMap(TagSet tagSet, bool mutable)
    {
        this.tagSet = tagSet;
        this.mutable = mutable;
    }

    public abstract HashSet<KeyValuePair<string, int>> entrySet();

    public FeatureMap(bool mutable)
    {
        this.mutable = mutable;
    }

    public FeatureMap()
        : this(false)
    {
    }

    //@Override
    public virtual void save(Stream Out) 
    {
        tagSet.save(Out);
        Out.writeInt(size());
        foreach (KeyValuePair<string, int> entry in entrySet())
        {
            Out.writeUTF(entry.Key);
        }
    }

    //@Override
    public virtual bool load(ByteArray byteArray)
    {
        loadTagSet(byteArray);
        int size = byteArray.Next();
        for (int i = 0; i < size; i++)
        {
            idOf(byteArray.nextUTF());
        }
        return true;
    }

    protected virtual void loadTagSet(ByteArray byteArray)
    {
        TaskType type = TaskType.values()[byteArray.Next()];
        switch (type)
        {
            case TaskType.CWS:
                tagSet = new CWSTagSet();
                break;
            case TaskType.POS:
                tagSet = new POSTagSet();
                break;
            case TaskType.NER:
                tagSet = new NERTagSet();
                break;
            case TaskType.CLASSIFICATION:
                tagSet = new TagSet(TaskType.CLASSIFICATION);
                break;
        }
        tagSet.load(byteArray);
    }

    public virtual int idOf(string s)
    {
        throw new NotImplementedException();
    }
}