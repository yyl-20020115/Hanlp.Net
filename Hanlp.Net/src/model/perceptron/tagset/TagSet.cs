/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-26 下午4:40</create-date>
 *
 * <copyright file="Tag.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.common;

namespace com.hankcs.hanlp.model.perceptron.tagset;



/**
 * @author hankcs
 */
public class TagSet : IIdStringMap, IStringIdMap, IEnumerable<KeyValuePair<string, int>>, ICacheAble
{
    private Dictionary<string, int> stringIdMap;
    private List<string> idStringMap;
    private int[] _allTags;
    public TaskType type;

    public TagSet(TaskType type)
    {
        stringIdMap = new();
        idStringMap = new();
        this.type = type;
    }

    public int Add(string tag)
    {
        //        assertUnlock();
        int id = stringIdMap.get(tag);
        if (id == null)
        {
            id = stringIdMap.Count;
            stringIdMap.Add(tag, id);
            idStringMap.Add(tag);
        }

        return id;
    }

    public int Count=> stringIdMap.Count;

    public int sizeIncludingBos()
    {
        return Count + 1;
    }

    public int bosId()
    {
        return Count;
    }

    public void _lock()
    {
        //        assertUnlock();
        allTags = new int[Count];
        for (int i = 0; i < Count; i++)
        {
            allTags[i] = i;
        }
    }

    //    private void assertUnlock()
    //    {
    //        if (allTags != null)
    //        {
    //            throw new InvalidOperationException("标注集已锁定，无法修改");
    //        }
    //    }

    //@Override
    public string stringOf(int id)
    {
        return idStringMap[(id)];
    }

    //@Override
    public int idOf(string s)
    {
        int id = stringIdMap[(s)];
        if (id == null) id = -1;
        return id;
    }

    //@Override
    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    {
        return stringIdMap.entrySet().GetEnumerator();
    }

    /**
     * 获取所有标签及其下标
     *
     * @return
     */
    public int[] allTags()
    {
        return _allTags;
    }

    public void save(Stream _out)
    {
        _out.writeInt(type.ordinal());
        _out.writeInt(Count);
        foreach (string tag in idStringMap)
        {
            _out.writeUTF(tag);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        idStringMap.Clear();
        stringIdMap.Clear();
        int size = byteArray.Next();
        for (int i = 0; i < size; i++)
        {
            string tag = byteArray.nextUTF();
            idStringMap.Add(tag);
            stringIdMap.Add(tag, i);
        }
        _lock () ;
        return true;
    }

    public void load(Stream _in)
    {
        idStringMap.Clear();
        stringIdMap.Clear();
        int size = _in.readInt();
        for (int i = 0; i < size; i++)
        {
            string tag = _in.readUTF();
            idStringMap.Add(tag);
            stringIdMap.Add(tag, i);
        }
        _lock () ;
    }

    public ICollection<string> tags()
    {
        return idStringMap;
    }
}
