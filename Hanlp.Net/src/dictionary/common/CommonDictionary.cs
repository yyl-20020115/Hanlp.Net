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
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.common;




/**
 * 通用的词典，对应固定格式的词典，但是标签可以泛型化
 *
 * @author hankcs
 */
public abstract class CommonDictionary<V>
{
    DoubleArrayTrie<V> trie;

    /**
     * 从字节中加载值数组
     *
     * @param byteArray
     * @return
     */
    protected abstract V[] loadValueArray(ByteArray byteArray);

    /**
     * 从txt路径加载
     *
     * @param path
     * @return
     */
    public bool load(string path)
    {
        trie = new DoubleArrayTrie<V>();
        long start = DateTime.Now.Microsecond;
        if (loadDat(ByteArray.createByteArray(path + Predefine.BIN_EXT)))
        {
            return true;
        }
        Dictionary<string, V> map = new Dictionary<string, V>();
        try
        {
            TextReader br = new StreamReader(IOUtil.newInputStream(path), "UTF-8");
            string line;
            while ((line = br.ReadLine()) != null)
            {
                string[] paramArray = line.Split("\\s");
                map.Add(paramArray[0], createValue(paramArray));
            }
            br.Close();
        }
        catch (Exception e)
        {
            logger.warning("读取" + path + "失败" + e);
            return false;
        }
        onLoaded(map);
        HashSet<KeyValuePair<string, V>> entrySet = map.ToHashSet();
        List<string> keyList = new (entrySet.Count);
        List<V> valueList = new (entrySet.Count);
        foreach (KeyValuePair<string, V> entry in entrySet)
        {
            keyList.Add(entry.Key);
            valueList.Add(entry.Value);
        }
        int resultCode = trie.build(keyList, valueList);
        if (resultCode != 0)
        {
            logger.warning("trie建立失败");
            return false;
        }
        logger.info(path + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
        saveDat(path + Predefine.BIN_EXT, valueList);
        return true;
    }

    /**
     * 从dat路径加载
     *
     * @param byteArray
     * @return
     */
    protected bool loadDat(ByteArray byteArray)
    {
        V[] valueArray = loadValueArray(byteArray);
        if (valueArray == null)
        {
            return false;
        }
        return trie.load(byteArray.getBytes(), byteArray.getOffset(), valueArray);
    }

    /**
     * 保存dat到路径
     *
     * @param path
     * @param valueArray
     * @return
     */
    protected bool saveDat(string path, List<V> valueArray)
    {
        try
        {
            Stream _out = IOUtil.newOutputStream(path);
            _out.writeInt(valueArray.Count);
            foreach (V item in valueArray)
            {
                saveValue(item, _out);
            }
            trie.save(_out);
            _out.Close();
        }
        catch (Exception e)
        {
            logger.warning("保存失败" + TextUtility.exceptionToString(e));
            return false;
        }
        return true;
    }

    /**
     * 保存单个值到流中
     *
     * @param value
     * @param _out
     * @
     */
    protected abstract void saveValue(V value, Stream _out) ;

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
     * 是否含有键
     *
     * @param key
     * @return
     */
    public bool Contains(string key)
    {
        return get(key) != null;
    }

    /**
     * 词典大小
     *
     * @return
     */
    public int Count=> trie.Count;

    /**
     * 从一行词典条目创建值
     *
     * @param params 第一个元素为键，请注意跳过
     * @return
     */
    protected abstract V createValue(string[] _params);

    /**
     * 文本词典加载完毕的回调函数
     *
     * @param map
     */
    protected void onLoaded(Dictionary<string, V> map)
    {
    }
}
