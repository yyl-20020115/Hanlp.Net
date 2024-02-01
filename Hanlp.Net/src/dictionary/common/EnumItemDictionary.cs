/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-14 下午8:32</create-date>
 *
 * <copyright file="EnumItemDictionary.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dictionary.item;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.dictionary.common;



/**
 * 枚举条目的通用词典（类似C++的模板，然而Java并没有模板机制，所以有几个方法需要子类啰嗦一下）
 *
 * @author hankcs
 */
public abstract class EnumItemDictionary<E> : CommonDictionary<EnumItem<E>>
{
    //@Override
    protected override EnumItem<E> createValue(string[] _params)
    {
        KeyValuePair<string, KeyValuePair<string, int>[]> args = EnumItem.create(_params);
        EnumItem<E> nrEnumItem = new EnumItem<E>();
        foreach (KeyValuePair<string, int> e in args.Value)
        {
            nrEnumItem.labelMap.Add(valueOf(e.Key), e.Value);
        }
        return nrEnumItem;
    }

    /**
     * 代理E.valueOf
     *
     * @param name
     * @return
     */
    protected abstract E valueOf(string name);

    /**
     * 代理E.values
     *
     * @return
     */
    protected abstract E[] values();

    /**
     * 代理new EnumItem<E>
     *
     * @return
     */
    protected abstract EnumItem<E> newItem();

    //@Override
    protected override EnumItem<E>[] loadValueArray(ByteArray byteArray)
    {
        if (byteArray == null)
        {
            return null;
        }
        E[] nrArray = values();
        int size = byteArray.Next();
        EnumItem<E>[] valueArray = new EnumItem<E>[size];
        for (int i = 0; i < size; ++i)
        {
            int currentSize = byteArray.Next();
            EnumItem<E> item = newItem();
            for (int j = 0; j < currentSize; ++j)
            {
                E nr = nrArray[byteArray.Next()];
                int frequency = byteArray.Next();
                item.labelMap.Add(nr, frequency);
            }
            valueArray[i] = item;
        }
        return valueArray;
    }

    //@Override
    protected void saveValue(EnumItem<E> item, Stream _out) 
    {
        _out.writeInt(item.labelMap.Count);
        foreach (KeyValuePair<E, int> entry in item.labelMap)
        {
            _out.writeInt(entry.Key.Ordinal);
            _out.writeInt(entry.Value);
        }
    }
}
