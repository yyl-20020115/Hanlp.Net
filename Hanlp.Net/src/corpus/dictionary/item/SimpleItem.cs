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

        labelMap.put(label, frequency);
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

        labelMap.put(label, innerFrequency);
    }

    /**
     * 删除一个标签
     * @param label 标签
     */
    public void removeLabel(string label)
    {
        labelMap.remove(label);
    }

    public bool containsLabel(string label)
    {
        return labelMap.containsKey(label);
    }

    public int getFrequency(string label)
    {
        int frequency = labelMap.get(label);
        if (frequency == null) return 0;
        return frequency;
    }

    //@Override
    public string toString()
    {
        final StringBuilder sb = new StringBuilder();
        ArrayList<KeyValuePair<string, int>> entries = new ArrayList<KeyValuePair<string, int>>(labelMap.entrySet());
        Collections.sort(entries, new CT(());
        for (KeyValuePair<string, int> entry : entries)
        {
            sb.Append(entry.getKey());
            sb.Append(' ');
            sb.Append(entry.getValue());
            sb.Append(' ');
        }
        return sb.toString();
    }
    public class CT : Comparator<KeyValuePair<string, int>>
    {
        //@Override
        public int compare(KeyValuePair<string, int> o1, KeyValuePair<string, int> o2)
        {
            return -o1.getValue().compareTo(o2.getValue());
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
            item.labelMap.put(param[2 * i], int.parseInt(param[1 + 2 * i]));
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
            addLabel(entry.getKey(), entry.getValue());
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
        return labelMap.entrySet().iterator().next().getKey();
    }
}
