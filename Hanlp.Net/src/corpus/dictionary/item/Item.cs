/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 0:06</create-date>
 *
 * <copyright file="Item.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.corpus.dictionary.item;


/**
 * 词典中的一个条目，比如“希望 v 7685 vn 616”
 * @author hankcs
 */
public class Item : SimpleItem
{
    /**
     * 该条目的索引，比如“啊”
     */
    public string key;

    public Item(string key, string label)
        :this(key)
    {
        labelMap.put(label, 1);
    }

    public Item(string key)
        :base()
    {
        this.key = key;
    }

    //@Override
    public string ToString()
    {
        var sb = new StringBuilder(key);
        ArrayList<KeyValuePair<string, int>> entries = new ArrayList<KeyValuePair<string, int>>(labelMap.entrySet());
        Collections.sort(entries, );
        foreach (KeyValuePair<string, int> entry in entries)
        {
            sb.Append(' ');             // 现阶段词典分隔符统一使用空格
            sb.Append(entry.getKey());
            sb.Append(' ');
            sb.Append(entry.getValue());
        }
        return sb.ToString();
    }

    public class CT : Comparator<KeyValuePair<string, int>>
    {
        //@Override
        public int compare(KeyValuePair<string, int> o1, KeyValuePair<string, int> o2)
        {
            return -o1.getValue().compareTo(o2.getValue());
        }
    }

    /**
     * 获取首个label
     * @return
     */
    public string firstLabel()
    {
        return labelMap.keySet().iterator().next();
    }

    /**
     *
     * @param param 类似 “希望 v 7685 vn 616” 的字串
     * @return
     */
    public static Item create(string param)
    {
        if (param == null) return null;
        string mark = "\\s";    // 分隔符，历史格式用空格，但是现在觉得用制表符比较好
        if (param.indexOf('\t') > 0) mark = "\t";
        string[] array = param.Split(mark);
        return create(array);
    }

    public static Item create(string[] param)
    {
        if (param.Length % 2 == 0) return null;
        Item item = new Item(param[0]);
        int natureCount = (param.Length - 1) / 2;
        for (int i = 0; i < natureCount; ++i)
        {
            item.labelMap.Add(param[1 + 2 * i], int.parseInt(param[2 + 2 * i]));
        }
        return item;
    }
}
