/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/22 18:17</create-date>
 *
 * <copyright file="DartMap.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.dartsclone;



/**
 * 双数组trie树map，更省内存，原本希望代替DoubleArrayTrie，后来发现效率不够
 * @author hankcs
 */
public class DartMap<V> : DoubleArray,  IDictionary<string, V>, ITrie<V>
{
    V[] valueArray;

    public DartMap(List<string> keyList, V[] valueArray)
    {
        int[] indexArray = new int[valueArray.Length];
        for (int i = 0; i < indexArray.Length; ++i)
        {
            indexArray[i] = i;
        }
        this.valueArray = valueArray;
        build(keyList, indexArray);
    }

    public DartMap(Dictionary<string, V> map)
    {
        build(map);
    }

    public DartMap()
    {
    }

    //@Override
    public bool isEmpty()
    {
        return this.valueArray.Length == 0;
    }

    //@Override
    public bool containsKey(Object key)
    {
        return containsKey(key.ToString());
    }

    /**
     * 是否包含key
     *
     * @param key
     * @return
     */
    public bool containsKey(string key)
    {
        return exactMatchSearch(key) != -1;
    }

    //@Override
    public bool containsValue(Object value)
    {
        return false;
    }

    //@Override
    public V get(Object key)
    {
        return get(key.ToString());
    }

    //@Override
    public int build(Dictionary<string, V> keyValueMap)
    {
        int size = keyValueMap.Count;
        int[] indexArray = new int[size];
        valueArray = (V[]) keyValueMap.Values.ToArray();
        List<string> keyList = new (size);
        int i = 0;
        foreach (var entry in keyValueMap)
        {
            indexArray[i] = i;
            valueArray[i] = entry.Value;
            keyList.Add(entry.Key);
            ++i;
        }
        build(keyList, indexArray);
        return 0;
    }

    //@Override
    public bool save(DataOutputStream _out)
    {
        return false;
    }

    //@Override
    public bool load(ByteArray byteArray, V[] value)
    {
        return false;
    }

    //@Override
    public V get(char[] key)
    {
        return get(new string(key));
    }

    public V get(string key)
    {
        int id = exactMatchSearch(key);
        if (id == -1) return null;
        return valueArray[id];
    }

    //@Override
    public V[] getValueArray(V[] a)
    {
        return valueArray;
    }

    /**
     * 前缀查询
     * @param key
     * @param offset
     * @param maxResults
     * @return
     */
    public List<Pair<string, V>> commonPrefixSearch(string key, int offset, int maxResults)
    {
        byte[] keyBytes = key.GetBytes(utf8);
        List<Pair<int, int>> pairList = commonPrefixSearch(keyBytes, offset, maxResults);
        List<Pair<string, V>> resultList = new List<Pair<string, V>>(pairList.Count);
        foreach (Pair<int, int> pair in pairList)
        {
            resultList.Add(new Pair<string, V>(new string(keyBytes, 0, pair.first), valueArray[pair.second]));
        }
        return resultList;
    }

    public List<Pair<string, V>> commonPrefixSearch(string key)
    {
        return commonPrefixSearch(key, 0, int.MaxValue);
    }

    //@Override
    public V put(string key, V value)
    {
        throw new InvalidOperationException("双数组不支持增量式插入");
    }

    //@Override
    public V Remove(Object key)
    {
        throw new InvalidOperationException("双数组不支持删除");
    }

    //@Override
    public void putAll(Dictionary<string, V> m)
    {
        throw new InvalidOperationException("双数组不支持增量式插入");
    }

    //@Override
    public void clear()
    {
        throw new InvalidOperationException("双数组不支持");
    }

    //@Override
    public ISet<string> keySet()
    {
        throw new InvalidOperationException("双数组不支持");
    }

    //@Override
    public ICollection<V> values()
    {
        return valueArray.ToList();
    }

    //@Override
    public ISet<KeyValuePair<string, V>> entrySet()
    {
        throw new InvalidOperationException("双数组不支持");
    }
}
