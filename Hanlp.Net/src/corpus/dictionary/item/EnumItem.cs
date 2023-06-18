/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/10 15:10</create-date>
 *
 * <copyright file="EnumItem.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.MDAG;
using System.Text;

namespace com.hankcs.hanlp.corpus.dictionary.item;


/**
 * 对标签-频次的封装
 * @author hankcs
 */
public class EnumItem<E>
{
    public Dictionary<E, int> labelMap;

    public EnumItem()
    {
        labelMap = new ();
    }

    /**
     * 创建只有一个标签的条目
     * @param label
     * @param frequency
     */
    public EnumItem(E label, int frequency)
    {
        this();
        labelMap.put(label, frequency);
    }

    /**
     * 创建一个条目，其标签频次都是1，各标签由参数指定
     * @param labels
     */
    public EnumItem(params E[] labels)
    {
        this();
        for (E label : labels)
        {
            labelMap.put(label, 1);
        }
    }

    public void addLabel(E label)
    {
        int frequency = labelMap.get(label);
        if (frequency == null)
        {
            frequency = 1;
        }
        else
        {
            ++frequency;
        }

        labelMap.put(label, frequency);
    }

    public void addLabel(E label, int frequency)
    {
        int innerFrequency = labelMap.get(label);
        if (innerFrequency == null)
        {
            innerFrequency = frequency;
        }
        else
        {
            innerFrequency += frequency;
        }

        labelMap.put(label, innerFrequency);
    }

    public bool containsLabel(E label)
    {
        return labelMap.ContainsKey(label);
    }

    public int getFrequency(E label)
    {
        int frequency = labelMap.get(label);
        if (frequency == null) return 0;
        return frequency;
    }

    //@Override
    public override string ToString()
    {
         StringBuilder sb = new StringBuilder();
        var entries = new List<KeyValuePair<E, int>>(labelMap.entrySet());
        Collections.sort(entries, new ST<int>());
        foreach (KeyValuePair<E, int> entry in entries)
        {
            sb.Append(entry.getKey());
            sb.Append(' ');
            sb.Append(entry.getValue());
            sb.Append(' ');
        }
        return sb.ToString();
    }
    public class ST<E> : IComparer<KeyValuePair<E, int>>
    {
        //@Override
        public int Compare(KeyValuePair<E, int> o1, KeyValuePair<E, int> o2)
        {
            return -o1.Value.compareTo(o2.Value);
        }
    }
    public static KeyValuePair<string, KeyValuePair<string, int>[]> create(string param)
    {
        if (param == null) return null;
        string[] array = param.Split(" ");
        return create(array);
    }

    
    public static KeyValuePair<string, KeyValuePair<string, int>[]> create(string[] param)
    {
        if (param.Length % 2 == 0) return null;
        int natureCount = (param.Length - 1) / 2;
        KeyValuePair<string, int>[] entries = (KeyValuePair<string, int>[]) Array.newInstance(KeyValuePair.c, natureCount);
        for (int i = 0; i < natureCount; ++i)
        {
            entries[i] = new AbstractMap.SimpleEntry<string, int>(param[1 + 2 * i], int.parseInt(param[2 + 2 * i]));
        }
        return new AbstractMap.SimpleEntry<string, KeyValuePair<string, int>[]>(param[0], entries);
    }
}
