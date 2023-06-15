/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-02 13:42</create-date>
 *
 * <copyright file="AbstractWordVectorModel.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.mining.word2vec;

namespace com.hankcs.hanlp.mining.word2vec;



/**
 * 抽象的向量模型，将抽象的对象映射为向量
 *
 * @author hankcs
 */
public abstract class AbstractVectorModel<K>
{
    Dictionary<K, Vector> storage;

    public AbstractVectorModel(Dictionary<K, Vector> storage)
    {
        this.storage = storage;
    }

    public AbstractVectorModel()
    {
        storage = new ();
    }

    /**
     * 获取一个键的向量（键不会被预处理）
     *
     * @param key 键
     * @return 向量
     */
    public Vector vector(K key)
    {
        Vector vector = storage.get(key);
        if (vector == null) return null;
        return vector;
    }

    /**
     * 余弦相似度
     *
     * @param what 一个词
     * @param with 另一个词
     * @return 余弦相似度
     */
    public float similarity(K what, K with)
    {
        Vector vectorWhat = storage.get(what);
        if (vectorWhat == null)
        {
            return -1f;
        }
        Vector vectorWith = storage.get(with);
        if (vectorWith == null)
        {
            return -1f;
        }
        return vectorWhat.cosineForUnitVector(vectorWith);
    }

    /**
     * 查询与key最相似的元素
     *
     * @param key  键
     * @param size topN个
     * @return 键值对列表, 键是相似词语, 值是相似度, 按相似度降序排列
     */
    public List<KeyValuePair<K, float>> nearest(K key, int size)
    {
        Vector vector = storage.get(key);
        if (vector == null)
        {
            return Collections.emptyList();
        }
        return nearest(key, vector, size);
    }

    /**
     * 查询与key最相似的元素
     *
     * @param key    键 结果将排除该键
     * @param vector 向量
     * @param size   topN个
     * @return 键值对列表, 键是相似词语, 值是相似度, 按相似度降序排列
     */
    private List<KeyValuePair<K, float>> nearest(K key, Vector vector, int size)
    {
        MaxHeap<KeyValuePair<K, Float>> maxHeap = new MaxHeap<KeyValuePair<K, Float>>(size, new Comparator<KeyValuePair<K, Float>>()
        {
            //@Override
            public int compare(KeyValuePair<K, Float> o1, KeyValuePair<K, Float> o2)
            {
                return o1.getValue().compareTo(o2.getValue());
            }
        });

        for (KeyValuePair<K, Vector> entry : storage.entrySet())
        {
            if (entry.getKey().equals(key))
            {
                continue;
            }
            maxHeap.add(new AbstractMap.SimpleEntry<K, Float>(entry.getKey(), entry.getValue().cosineForUnitVector(vector)));
        }
        return maxHeap.toList();
    }

    /**
     * 获取与向量最相似的词语
     *
     * @param vector 向量
     * @param size   topN个
     * @return 键值对列表, 键是相似词语, 值是相似度, 按相似度降序排列
     */
    public List<KeyValuePair<K, float>> nearest(Vector vector, int size)
    {
        MaxHeap<KeyValuePair<K, Float>> maxHeap = new MaxHeap<KeyValuePair<K, Float>>(size, new Comparator<KeyValuePair<K, Float>>()
        {
            //@Override
            public int compare(KeyValuePair<K, Float> o1, KeyValuePair<K, Float> o2)
            {
                return o1.getValue().compareTo(o2.getValue());
            }
        });

        for (KeyValuePair<K, Vector> entry : storage.entrySet())
        {
            maxHeap.add(new AbstractMap.SimpleEntry<K, Float>(entry.getKey(), entry.getValue().cosineForUnitVector(vector)));
        }
        return maxHeap.toList();
    }

    /**
     * 获取与向量最相似的词语（默认10个）
     *
     * @param vector 向量
     * @return 键值对列表, 键是相似词语, 值是相似度, 按相似度降序排列
     */
    public List<KeyValuePair<K, float>> nearest(Vector vector)
    {
        return nearest(vector, 10);
    }

    /**
     * 查询与词语最相似的词语
     *
     * @param key 词语
     * @return 键值对列表, 键是相似词语, 值是相似度, 按相似度降序排列
     */
    public List<KeyValuePair<K, float>> nearest(K key)
    {
        return nearest(key, 10);
    }

    /**
     * 执行查询最相似的对象（子类通过query方法决定如何解析query，然后通过此方法执行查询）
     *
     * @param query 查询语句（或者说一个对象的内容）
     * @param size  需要返回前多少个对象
     * @return
     */
    List<KeyValuePair<K, Float>> queryNearest(string query, int size)
    {
        if (query == null || query.length() == 0)
        {
            return Collections.emptyList();
        }
        try
        {
            return nearest(query(query), size);
        }
        catch (Exception e)
        {
            return Collections.emptyList();
        }
    }

    /**
     * 查询抽象文本对应的向量。此方法应当保证返回单位向量。
     *
     * @param query
     * @return
     */
    public abstract Vector query(string query);

    /**
     * 模型中的词向量总数（词表大小）
     *
     * @return
     */
    public int size()
    {
        return storage.size();
    }

    /**
     * 模型中的词向量维度
     *
     * @return
     */
    public int dimension()
    {
        if (storage == null || storage.isEmpty())
        {
            return 0;
        }
        return storage.values().iterator().next().size();
    }

    /**
     * 删除元素
     *
     * @param key
     * @return
     */
    public Vector remove(K key)
    {
        return storage.remove(key);
    }
}
