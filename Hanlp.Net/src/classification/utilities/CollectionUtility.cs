/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/6 PM5:01</create-date>
 *
 * <copyright file="ToolUtility.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.classification.utilities;


/**
 * @author hankcs
 */
public class CollectionUtility
{
    public static Dictionary<K, V> sortMapByValue<K, V>(Dictionary<K, V> input, bool desc)
    {
        var output = new Dictionary<K, V>(input.Count);
        var entryList = new List<KeyValuePair<K, V>>(input.Count);
        Collections.sort(entryList,new ST<K,V>());
        foreach (KeyValuePair<K, V> entry in entryList)
        {
            output.Add(entry.Key, entry.Value);
        }

        return output;
    }
    public class ST<K,V> : IComparer<KeyValuePair<K, V>>
    {
        public int Compare(KeyValuePair<K, V> o1, KeyValuePair<K, V> o2)
        {
            if (desc) return o2.getValue().compareTo(o1.getValue());
            return o1.getValue().compareTo(o2.getValue());
        }
    }
    public static Dictionary<K, V> sortMapByValue<K, V>(Dictionary<K, V> input)
    {
        return sortMapByValue(input, true);
    }

    public static string max(Dictionary<string, Double> scoreMap)
    {
        double max = double.NegativeInfinity;
        string best = null;
        foreach (KeyValuePair<string, Double> entry in scoreMap)
        {
            Double score = entry.Value;
            if (score > max)
            {
                max = score;
                best = entry.Key;
            }
        }

        return best;
    }

    /**
     * 分割数组为两个数组
     * @param src 原数组
     * @param rate 第一个数组所占的比例
     * @return 两个数组
     */
    public static string[][] spiltArray(string[] src, double rate)
    {
        //assert 0 <= rate && rate <= 1;
        string[][] output = new string[2][];
        output[0] = new string[(int)(src.Length * rate)];
        output[1] = new string[src.Length - output[0].Length];
        System.arraycopy(src, 0, output[0], 0, output[0].Length);
        System.arraycopy(src, output[0].Length, output[1], 0, output[1].Length);
        return output;
    }

    /**
     * 分割Map,其中旧map直接被改变
     * @param src
     * @param rate
     * @return
     */
    public static Dictionary<string, string[]> splitMap(Dictionary<string, string[]> src, double rate)
    {
        //assert 0 <= rate && rate <= 1;
        Dictionary<string, string[]> output = new ();
        foreach (KeyValuePair<string, string[]> entry in src)
        {
            string[][] array = spiltArray(entry.Value, rate);
            output.Add(entry.Key, array[0]);
            entry.setValue(array[1]);
        }

        return output;
    }
}
