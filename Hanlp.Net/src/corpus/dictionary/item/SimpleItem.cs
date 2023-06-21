/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/10 15:00</create-date>
 *
 * <copyright file="SimpleItem.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.corpus.dictionary.item;


/**
 * @author hankcs
 */
public class SimpleItem
{
    /**
     * 该条目的标签
     */
    public Dictionary<string, int> labelMap;

    public SimpleItem()
    {
        labelMap = new();
    }

    public void addLabel(string label)
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

        labelMap.Add(label, frequency);
    }

    /**
     * 添加一个标签和频次
     * @param label
     * @param frequency
     */
    public void addLabel(string label, int frequency)
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

        labelMap.Add(label, innerFrequency);
    }

    /**
     * 删除一个标签
     * @param label 标签
     */
    public void removeLabel(string label)
    {
        labelMap.Remove(label);
    }

    public bool containsLabel(string label)
    {
        return labelMap.ContainsKey(label);
    }

    public int getFrequency(string label)
    {
        int frequency = labelMap.get(label);
        if (frequency == null) return 0;
        return frequency;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        var entries = new List<KeyValuePair<string, int>>(labelMap.entrySet());
        Collections.sort(entries, new CT());
        foreach (KeyValuePair<string, int> entry in entries)
        {
            sb.Append(entry.Key);
            sb.Append(' ');
            sb.Append(entry.Value);
            sb.Append(' ');
        }
        return sb.ToString();
    }
    public class CT : IComparer<KeyValuePair<string, int>>
    {
        //@Override
        public int Compare(KeyValuePair<string, int> o1, KeyValuePair<string, int> o2)
        {
            return -o1.Value.compareTo(o2.Value);
        }
    }

    public static SimpleItem create(string param)
    {
        if (param == null) return null;
        string[] array = param.Split(" ");
        return create(array);
    }

    public static SimpleItem create(string[] param)
    {
        if (param.Length % 2 == 1) return null;
        SimpleItem item = new SimpleItem();
        int natureCount = (param.Length) / 2;
        for (int i = 0; i < natureCount; ++i)
        {
            item.labelMap.Add(param[2 * i], int.parseInt(param[1 + 2 * i]));
        }
        return item;
    }

    /**
     * 合并两个条目，两者的标签map会合并
     * @param other
     */
    public void combine(SimpleItem other)
    {
        foreach (KeyValuePair<string, int> entry in other.labelMap.entrySet())
        {
            addLabel(entry.Key, entry.Value);
        }
    }

    /**
     * 获取全部频次
     * @return
     */
    public int getTotalFrequency()
    {
        int frequency = 0;
        foreach (int f in labelMap.Values)
        {
            frequency += f;
        }
        return frequency;
    }

    public string getMostLikelyLabel()
    {
        return labelMap.entrySet().iterator().next().Key;
    }
}
