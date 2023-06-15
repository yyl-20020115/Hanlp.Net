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
        LinkedHashMap<K, V> output = new LinkedHashMap<K, V>(input.size());
        ArrayList<KeyValuePair<K, V>> entryList = new ArrayList<KeyValuePair<K, V>>(input.size());
        Collections.sort(entryList, new Comparator<KeyValuePair<K, V>>()
        {
            public int compare(KeyValuePair<K, V> o1, KeyValuePair<K, V> o2)
            {
                if (desc) return o2.getValue().compareTo(o1.getValue());
                return o1.getValue().compareTo(o2.getValue());
            }
        });
        for (KeyValuePair<K, V> entry : entryList)
        {
            output.put(entry.getKey(), entry.getValue());
        }

        return output;
    }

    public static <K, V : Comparable<V>> Dictionary<K, V> sortMapByValue(Dictionary<K, V> input)
    {
        return sortMapByValue(input, true);
    }

    public static string max(Dictionary<string, Double> scoreMap)
    {
        double max = Double.NEGATIVE_INFINITY;
        string best = null;
        for (KeyValuePair<string, Double> entry : scoreMap.entrySet())
        {
            Double score = entry.getValue();
            if (score > max)
            {
                max = score;
                best = entry.getKey();
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
        assert 0 <= rate && rate <= 1;
        string[][] output = new string[2][];
        output[0] = new string[(int) (src.length * rate)];
        output[1] = new string[src.length - output[0].length];
        System.arraycopy(src, 0, output[0], 0, output[0].length);
        System.arraycopy(src, output[0].length, output[1], 0, output[1].length);
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
        assert 0 <= rate && rate <= 1;
        Dictionary<string, string[]> output = new TreeMap<string, string[]>();
        for (KeyValuePair<string, string[]> entry : src.entrySet())
        {
            string[][] array = spiltArray(entry.getValue(), rate);
            output.put(entry.getKey(), array[0]);
            entry.setValue(array[1]);
        }

        return output;
    }
}
