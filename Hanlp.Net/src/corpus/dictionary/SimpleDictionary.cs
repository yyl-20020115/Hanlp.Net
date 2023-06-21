/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 22:30</create-date>
 *
 * <copyright file="CommonDictioanry.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;

namespace com.hankcs.hanlp.corpus.dictionary;




/**
 * 可以调整大小的词典
 *
 * @author hankcs
 */
public abstract class SimpleDictionary<V>
{
    BinTrie<V> trie = new BinTrie<V>();

    public bool load(string path)
    {
        try
        {
            TextReader br = new TextReader(new InputStreamReader(IOAdapter == null ? new FileStream(path) : IOAdapter.open(path), "UTF-8"));
            string line;
            while ((line = br.ReadLine()) != null)
            {
                KeyValuePair<string, V> entry = onGenerateEntry(line);
                if (entry == null) continue;
                trie.Add(entry.Key, entry.Value);
            }
            br.Close();
        }
        catch (Exception e)
        {
            logger.warning("读取" + path + "失败" + e);
            return false;
        }
        return true;
    }

    /**
     * 查询一个单词
     *
     * @param key
     * @return 单词对应的条目
     */
    public V get(string key)
    {
        return trie.get(key);
    }

    /**
     * 由参数构造一个词条
     *
     * @param line
     * @return
     */
    protected abstract KeyValuePair<string, V> onGenerateEntry(string line);

    /**
     * 以我为主词典，合并一个副词典，我有的词条不会被副词典覆盖
     * @param other 副词典
     */
    public void combine(SimpleDictionary<V> other)
    {
        if (other.trie == null)
        {
            logger.warning("有个词典还没加载");
            return;
        }
        foreach (KeyValuePair<string, V> entry in other.trie.entrySet())
        {
            if (trie.ContainsKey(entry.Key)) continue;
            trie.Add(entry.Key, entry.Value);
        }
    }
    /**
     * 获取键值对集合
     * @return
     */
    public HashSet<KeyValuePair<string, V>> entrySet()
    {
        return trie.entrySet();
    }

    /**
     * 键集合
     * @return
     */
    public HashSet<string> keySet()
    {
        TreeSet<string> keySet = new TreeSet<string>();

        foreach (KeyValuePair<string, V> entry in entrySet())
        {
            keySet.Add(entry.Key);
        }

        return keySet;
    }

    /**
     * 过滤部分词条
     * @param filter 过滤器
     * @return 删除了多少条
     */
    public int Remove(Filter filter)
    {
        int size = trie.size();
        foreach (KeyValuePair<string, V> entry in entrySet())
        {
            if (filter.Remove(entry))
            {
                trie.Remove(entry.Key);
            }
        }

        return size - trie.size();
    }

    public interface Filter<V>
    {
        bool Remove(KeyValuePair<string, V> entry);
    }
    /**
     * 向中加入单词
     * @param key
     * @param value
     */
    public void Add(string key, V value)
    {
        trie.Add(key, value);
    }

    public int size()
    {
        return trie.size();
    }
}
