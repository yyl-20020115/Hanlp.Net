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
namespace com.hankcs.hanlp.model.perceptron.feature;



/**
 * @author hankcs
 */
public abstract class FeatureMap : IStringIdMap, ICacheAble
{
    public abstract int size();

    public int[] allLabels()
    {
        return tagSet.allTags();
    }

    public int bosTag()
    {
        return tagSet.size();
    }

    public TagSet tagSet;
    /**
     * 是否允许新增特征
     */
    public bool mutable;

    public FeatureMap(TagSet tagSet)
    {
        this(tagSet, false);
    }

    public FeatureMap(TagSet tagSet, bool mutable)
    {
        this.tagSet = tagSet;
        this.mutable = mutable;
    }

    public abstract Set<Map.Entry<String, Integer>> entrySet();

    public FeatureMap(bool mutable)
    {
        this.mutable = mutable;
    }

    public FeatureMap()
    {
        this(false);
    }

    //@Override
    public void save(DataOutputStream out) 
    {
        tagSet.save(out);
        out.writeInt(size());
        for (Map.Entry<String, Integer> entry : entrySet())
        {
            out.writeUTF(entry.getKey());
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        loadTagSet(byteArray);
        int size = byteArray.nextInt();
        for (int i = 0; i < size; i++)
        {
            idOf(byteArray.nextUTF());
        }
        return true;
    }

    protected final void loadTagSet(ByteArray byteArray)
    {
        TaskType type = TaskType.values()[byteArray.nextInt()];
        switch (type)
        {
            case CWS:
                tagSet = new CWSTagSet();
                break;
            case POS:
                tagSet = new POSTagSet();
                break;
            case NER:
                tagSet = new NERTagSet();
                break;
            case CLASSIFICATION:
                tagSet = new TagSet(TaskType.CLASSIFICATION);
                break;
        }
        tagSet.load(byteArray);
    }
}