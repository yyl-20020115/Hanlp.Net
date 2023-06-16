/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/5/3 11:34</create-date>
 *
 * <copyright file="BinTrie.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;
using Microsoft.Extensions.Logging;
using System.Text;

namespace com.hankcs.hanlp.collection.trie.bintrie;




/**
 * 首字直接分配内存，之后二分动态数组的Trie树，能够平衡时间和空间
 *
 * @author hankcs
 */
public class BinTrie<V> : BaseNode<V> , ITrie<V>//, Externalizable
{
    private int _size;

    public BinTrie()
    {
        child = new BaseNode[65535 + 1];    // (int)char.MAX_VALUE
        _size = 0;
        status = Status.NOT_WORD_1;
    }

    public BinTrie(Dictionary<string, V> map)
        :this()
    {
        foreach (KeyValuePair<string, V> entry in map.entrySet())
        {
            put(entry.getKey(), entry.getValue());
        }
    }

    /**
     * 插入一个词
     *
     * @param key
     * @param value
     */
    public void put(string key, V value)
    {
        if (key.Length == 0) return;  // 安全起见
        BaseNode branch = this;
        char[] chars = key.ToCharArray();
        for (int i = 0; i < chars.Length - 1; ++i)
        {
            // 除了最后一个字外，都是继续
            branch.addChild(new Node(chars[i], Status.NOT_WORD_1, null));
            branch = branch.getChild(chars[i]);
        }
        // 最后一个字加入时属性为end
        if (branch.addChild(new Node<V>(chars[chars.Length - 1], Status.WORD_END_3, value)))
        {
            ++size; // 维护size
        }
    }

    public void put(char[] key, V value)
    {
        BaseNode branch = this;
        for (int i = 0; i < key.Length - 1; ++i)
        {
            // 除了最后一个字外，都是继续
            branch.addChild(new Node(key[i], Status.NOT_WORD_1, null));
            branch = branch.getChild(key[i]);
        }
        // 最后一个字加入时属性为end
        if (branch.addChild(new Node<V>(key[key.Length - 1], Status.WORD_END_3, value)))
        {
            ++size; // 维护size
        }
    }

    /**
     * 设置键值对，当键不存在的时候会自动插入
     * @param key
     * @param value
     */
    public void set(string key, V value)
    {
        put(key.ToCharArray(), value);
    }

    /**
     * 删除一个词
     *
     * @param key
     */
    public void remove(string key)
    {
        BaseNode branch = this;
        char[] chars = key.ToCharArray();
        for (int i = 0; i < chars.Length - 1; ++i)
        {
            if (branch == null) return;
            branch = branch.getChild(chars[i]);
        }
        if (branch == null) return;
        // 最后一个字设为undefined
        if (branch.addChild(new Node(chars[chars.Length - 1], Status.UNDEFINED_0, value)))
        {
            --size;
        }
    }

    public bool containsKey(string key)
    {
        BaseNode branch = this;
        char[] chars = key.ToCharArray();
        for (char aChar : chars)
        {
            if (branch == null) return false;
            branch = branch.getChild(aChar);
        }

        return branch != null && (branch.status == Status.WORD_END_3 || branch.status == Status.WORD_MIDDLE_2);
    }

    public V get(string key)
    {
        BaseNode branch = this;
        char[] chars = key.ToCharArray();
        for (char aChar : chars)
        {
            if (branch == null) return null;
            branch = branch.getChild(aChar);
        }

        if (branch == null) return null;
        // 下面这句可以保证只有成词的节点被返回
        if (!(branch.status == Status.WORD_END_3 || branch.status == Status.WORD_MIDDLE_2)) return null;
        return (V) branch.getValue();
    }

    public V get(char[] key)
    {
        BaseNode branch = this;
        for (char aChar : key)
        {
            if (branch == null) return null;
            branch = branch.getChild(aChar);
        }

        if (branch == null) return null;
        // 下面这句可以保证只有成词的节点被返回
        if (!(branch.status == Status.WORD_END_3 || branch.status == Status.WORD_MIDDLE_2)) return null;
        return (V) branch.getValue();
    }

    //@Override
    public V[] getValueArray(V[] a)
    {
        if (a.Length < size)
            a = (V[]) java.lang.reflect.Array.newInstance(
                    a.getClass().getComponentType(), size);
        int i = 0;
        for (KeyValuePair<string, V> entry : entrySet())
        {
            a[i++] = entry.getValue();
        }
        return a;
    }

    /**
     * 获取键值对集合
     *
     * @return
     */
    public HashSet<KeyValuePair<string, V>> entrySet()
    {
        HashSet<KeyValuePair<string, V>> entrySet = new TreeSet<KeyValuePair<string, V>>();
        StringBuilder sb = new StringBuilder();
        for (BaseNode node : child)
        {
            if (node == null) continue;
            node.walk(new StringBuilder(sb.toString()), entrySet);
        }
        return entrySet;
    }

    /**
     * 键集合
     * @return
     */
    public HashSet<string> keySet()
    {
        var keySet = new HashSet<string>();
        for (KeyValuePair<string, V> entry : entrySet())
        {
            keySet.add(entry.getKey());
        }

        return keySet;
    }

    /**
     * 前缀查询
     *
     * @param key 查询串
     * @return 键值对
     */
    public HashSet<KeyValuePair<string, V>> prefixSearch(string key)
    {
        HashSet<KeyValuePair<string, V>> entrySet = new HashSet<KeyValuePair<string, V>>();
        StringBuilder sb = new StringBuilder(key.substring(0, key.Length - 1));
        BaseNode branch = this;
        char[] chars = key.ToCharArray();
        for (char aChar : chars)
        {
            if (branch == null) return entrySet;
            branch = branch.getChild(aChar);
        }

        if (branch == null) return entrySet;
        branch.walk(sb, entrySet);
        return entrySet;
    }

    /**
     * 前缀查询，包含值
     *
     * @param key 键
     * @return 键值对列表
     */
    public LinkedList<KeyValuePair<string, V>> commonPrefixSearchWithValue(string key)
    {
        char[] chars = key.ToCharArray();
        return commonPrefixSearchWithValue(chars, 0);
    }

    /**
     * 前缀查询，通过字符数组来表示字符串可以优化运行速度
     *
     * @param chars 字符串的字符数组
     * @param begin 开始的下标
     * @return
     */
    public LinkedList<KeyValuePair<string, V>> commonPrefixSearchWithValue(char[] chars, int begin)
    {
        LinkedList<KeyValuePair<string, V>> result = new LinkedList<KeyValuePair<string, V>>();
        StringBuilder sb = new StringBuilder();
        BaseNode branch = this;
        for (int i = begin; i < chars.Length; ++i)
        {
            char aChar = chars[i];
            branch = branch.getChild(aChar);
            if (branch == null || branch.status == Status.UNDEFINED_0) return result;
            sb.Append(aChar);
            if (branch.status == Status.WORD_MIDDLE_2 || branch.status == Status.WORD_END_3)
            {
                result.add(new AbstractMap.SimpleEntry<string, V>(sb.toString(), (V) branch.value));
            }
        }

        return result;
    }

    //@Override
    protected bool addChild(BaseNode node)
    {
        bool add = false;
        char c = node.getChar();
        BaseNode target = getChild(c);
        if (target == null)
        {
            child[c] = node;
            add = true;
        }
        else
        {
            switch (node.status)
            {
                case UNDEFINED_0:
                    if (target.status != Status.NOT_WORD_1)
                    {
                        target.status = Status.NOT_WORD_1;
                        add = true;
                    }
                    break;
                case NOT_WORD_1:
                    if (target.status == Status.WORD_END_3)
                    {
                        target.status = Status.WORD_MIDDLE_2;
                    }
                    break;
                case WORD_END_3:
                    if (target.status == Status.NOT_WORD_1)
                    {
                        target.status = Status.WORD_MIDDLE_2;
                    }
                    if (target.getValue() == null)
                    {
                        add = true;
                    }
                    target.setValue(node.getValue());
                    break;
            }
        }
        return add;
    }

    public int size()
    {
        return size;
    }

    //@Override
    protected char getChar()
    {
        return 0;   // 根节点没有char
    }

    //@Override
    public BaseNode getChild(char c)
    {
        return child[c];
    }

    public bool save(string path)
    {
        try
        {
            DataOutputStream _out = new DataOutputStream(IOUtil.newOutputStream(path));
            for (BaseNode node : child)
            {
                if (node == null)
                {
                    _out.writeInt(0);
                }
                else
                {
                    _out.writeInt(1);
                    node.walkToSave(_out);
                }
            }
            _out.close();
        }
        catch (Exception e)
        {
            logger.warning("保存到" + path + "失败" + TextUtility.exceptionToString(e));
            return false;
        }

        return true;
    }

    //@Override
    public int build(TreeMap<string, V> keyValueMap)
    {
        for (KeyValuePair<string, V> entry : keyValueMap.entrySet())
        {
            put(entry.getKey(), entry.getValue());
        }
        return 0;
    }

    /**
     * 保存到二进制输出流
     *
     * @param _out
     * @return
     */
    public bool save(DataOutputStream _out)
    {
        try
        {
            for (BaseNode node : child)
            {
                if (node == null)
                {
                    _out.writeInt(0);
                }
                else
                {
                    _out.writeInt(1);
                    node.walkToSave(_out);
                }
            }
        }
        catch (Exception e)
        {
            Logger.warning("保存到" + _out + "失败" + TextUtility.exceptionToString(e));
            return false;
        }

        return true;
    }

    /**
     * 从磁盘加载二分数组树
     *
     * @param path  路径
     * @param value 额外提供的值数组，按照值的字典序。（之所以要求提供它，是因为泛型的保存不归树管理）
     * @return 是否成功
     */
    public bool load(string path, V[] value)
    {
        byte[] bytes = IOUtil.readBytes(path);
        if (bytes == null) return false;
        _ValueArray valueArray = new _ValueArray(value);
        ByteArray byteArray = new ByteArray(bytes);
        for (int i = 0; i < child.Length; ++i)
        {
            int flag = byteArray.nextInt();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(byteArray, valueArray);
            }
        }
        size = value.Length;

        return true;
    }

    /**
     * 只加载值，此时相当于一个set
     *
     * @param path
     * @return
     */
    public bool load(string path)
    {
        byte[] bytes = IOUtil.readBytes(path);
        if (bytes == null) return false;
        _ValueArray valueArray = new _EmptyValueArray();
        ByteArray byteArray = new ByteArray(bytes);
        for (int i = 0; i < child.Length; ++i)
        {
            int flag = byteArray.nextInt();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(byteArray, valueArray);
            }
        }
        size = -1;  // 不知道有多少

        return true;
    }

    public bool load(ByteArray byteArray, _ValueArray valueArray)
    {
        for (int i = 0; i < child.Length; ++i)
        {
            int flag = byteArray.nextInt();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(byteArray, valueArray);
            }
        }
        size = valueArray.value.Length;

        return true;
    }

    public bool load(ByteArray byteArray, V[] value)
    {
        return load(byteArray, newValueArray().setValue(value));
    }

    public _ValueArray newValueArray()
    {
        return new _ValueArray();
    }

    //@Override
    public void writeExternal(ObjectOutput _out) 
    {
        _out.writeInt(size);
        for (BaseNode node : child)
        {
            if (node == null)
            {
                _out.writeInt(0);
            }
            else
            {
                _out.writeInt(1);
                node.walkToSave(_out);
            }
        }
    }

    //@Override
    public void readExternal(ObjectInput _in) 
    {
        size = _in.readInt();
        for (int i = 0; i < child.Length; ++i)
        {
            int flag = _in.readInt();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(_in);

}
        }
    }

    /**
     * 最长匹配
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void parseLongestText(string text, AhoCorasickDoubleArrayTrie<string>.IHit<V> processor)
    {
        int Length = text.Length;
        for (int i = 0; i < Length; ++i)
        {
            BaseNode<V> state = transition(text.charAt(i));
            if (state != null)
            {
                int to = i + 1;
                int end = to;
                V value = state.getValue();
                for (; to < Length; ++to)
                {
                    state = state.transition(text.charAt(to));
                    if (state == null) break;
                    if (state.getValue() != null)
                    {
                        value = state.getValue();
                        end = to + 1;
                    }
                }
                if (value != null)
                {
                    processor.hit(i, end, value);
                    i = end - 1;
                }
            }
        }
    }

    /**
     * 最长匹配
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void parseLongestText(char[] text, AhoCorasickDoubleArrayTrie<string>.IHit<V> processor)
    {
        int Length = text.Length;
        for (int i = 0; i < Length; ++i)
        {
            BaseNode<V> state = transition(text[i]);
            if (state != null)
            {
                int to = i + 1;
                int end = to;
                V value = state.getValue();
                for (; to < Length; ++to)
                {
                    state = state.transition(text[to]);
                    if (state == null) break;
                    if (state.getValue() != null)
                    {
                        value = state.getValue();
                        end = to + 1;
                    }
                }
                if (value != null)
                {
                    processor.hit(i, end, value);
                    i = end - 1;
                }
            }
        }
    }

    /**
     * 匹配文本
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void parseText(string text, AhoCorasickDoubleArrayTrie<string>.IHit<V> processor)
    {
        int Length = text.Length;
        int begin = 0;
        BaseNode<V> state = this;

        for (int i = begin; i < Length; ++i)
        {
            state = state.transition(text.charAt(i));
            if (state != null)
            {
                V value = state.getValue();
                if (value != null)
                {
                    processor.hit(begin, i + 1, value);
                }
            }
            else
            {
                i = begin;
                ++begin;
                state = this;
            }
        }
    }

    /**
     * 匹配文本
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void parseText(char[] text, AhoCorasickDoubleArrayTrie<string>.IHit<V> processor)
    {
        int Length = text.Length;
        int begin = 0;
        BaseNode<V> state = this;

        for (int i = begin; i < Length; ++i)
        {
            state = state.transition(text[i]);
            if (state != null)
            {
                V value = state.getValue();
                if (value != null)
                {
                    processor.hit(begin, i + 1, value);
                }
            }
            else
            {
                i = begin;
                ++begin;
                state = this;
            }
        }
    }
}
