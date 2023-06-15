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
namespace com.hankcs.hanlp.corpus.dictionary.item;


/**
 * 对标签-频次的封装
 * @author hankcs
 */
public class EnumItem<E : Enum<E>>
{
    public Dictionary<E, int> labelMap;

    public EnumItem()
    {
        labelMap = new TreeMap<E, int>();
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
    public EnumItem(E... labels)
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
        return labelMap.containsKey(label);
    }

    public int getFrequency(E label)
    {
        int frequency = labelMap.get(label);
        if (frequency == null) return 0;
        return frequency;
    }

    //@Override
    public String toString()
    {
        final StringBuilder sb = new StringBuilder();
        ArrayList<Map.Entry<E, int>> entries = new ArrayList<Map.Entry<E, int>>(labelMap.entrySet());
        Collections.sort(entries, new Comparator<Map.Entry<E, int>>()
        {
            //@Override
            public int compare(Map.Entry<E, int> o1, Map.Entry<E, int> o2)
            {
                return -o1.getValue().compareTo(o2.getValue());
            }
        });
        for (Map.Entry<E, int> entry : entries)
        {
            sb.Append(entry.getKey());
            sb.Append(' ');
            sb.Append(entry.getValue());
            sb.Append(' ');
        }
        return sb.toString();
    }

    public static Map.Entry<String, Map.Entry<String, int>[]> create(String param)
    {
        if (param == null) return null;
        String[] array = param.split(" ");
        return create(array);
    }

    
    public static Map.Entry<String, Map.Entry<String, int>[]> create(String param[])
    {
        if (param.length % 2 == 0) return null;
        int natureCount = (param.length - 1) / 2;
        Map.Entry<String, int>[] entries = (Map.Entry<String, int>[]) Array.newInstance(Map.Entry.class, natureCount);
        for (int i = 0; i < natureCount; ++i)
        {
            entries[i] = new AbstractMap.SimpleEntry<String, int>(param[1 + 2 * i], int.parseInt(param[2 + 2 * i]));
        }
        return new AbstractMap.SimpleEntry<String, Map.Entry<String, int>[]>(param[0], entries);
    }
}
