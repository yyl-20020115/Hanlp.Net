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
namespace com.hankcs.hanlp.collection.dartsclone;



/**
 * 双数组trie树map，更省内存，原本希望代替DoubleArrayTrie，后来发现效率不够
 * @author hankcs
 */
public class DartMap<V> : DoubleArray : Dictionary<string, V>, ITrie<V>
{
    V[] valueArray;

    public DartMap(List<string> keyList, V[] valueArray)
    {
        int[] indexArray = new int[valueArray.length];
        for (int i = 0; i < indexArray.length; ++i)
        {
            indexArray[i] = i;
        }
        this.valueArray = valueArray;
        build(keyList, indexArray);
    }

    public DartMap(TreeMap<string, V> map)
    {
        build(map);
    }

    public DartMap()
    {
    }

    //@Override
    public bool isEmpty()
    {
        return size() == 0;
    }

    //@Override
    public bool containsKey(Object key)
    {
        return containsKey(key.toString());
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
        return get(key.toString());
    }

    //@Override
    public int build(TreeMap<string, V> keyValueMap)
    {
        int size = keyValueMap.size();
        int[] indexArray = new int[size];
        valueArray = (V[]) keyValueMap.values().toArray();
        List<string> keyList = new ArrayList<string>(size);
        int i = 0;
        for (Entry<string, V> entry : keyValueMap.entrySet())
        {
            indexArray[i] = i;
            valueArray[i] = entry.getValue();
            keyList.add(entry.getKey());
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
    public ArrayList<Pair<string, V>> commonPrefixSearch(string key, int offset, int maxResults)
    {
        byte[] keyBytes = key.getBytes(utf8);
        List<Pair<int, int>> pairList = commonPrefixSearch(keyBytes, offset, maxResults);
        ArrayList<Pair<string, V>> resultList = new ArrayList<Pair<string, V>>(pairList.size());
        for (Pair<int, int> pair : pairList)
        {
            resultList.add(new Pair<string, V>(new string(keyBytes, 0, pair.first), valueArray[pair.second]));
        }
        return resultList;
    }

    public ArrayList<Pair<string, V>> commonPrefixSearch(string key)
    {
        return commonPrefixSearch(key, 0, int.MAX_VALUE);
    }

    //@Override
    public V put(string key, V value)
    {
        throw new UnsupportedOperationException("双数组不支持增量式插入");
    }

    //@Override
    public V remove(Object key)
    {
        throw new UnsupportedOperationException("双数组不支持删除");
    }

    //@Override
    public void putAll(Dictionary<? : string, ? : V> m)
    {
        throw new UnsupportedOperationException("双数组不支持增量式插入");
    }

    //@Override
    public void clear()
    {
        throw new UnsupportedOperationException("双数组不支持");
    }

    //@Override
    public Set<string> keySet()
    {
        throw new UnsupportedOperationException("双数组不支持");
    }

    //@Override
    public Collection<V> values()
    {
        return Arrays.asList(valueArray);
    }

    //@Override
    public Set<Entry<string, V>> entrySet()
    {
        throw new UnsupportedOperationException("双数组不支持");
    }
}
