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
using com.hankcs.hanlp.collection.MDAG;
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
        child = new BaseNode<V>[65535 + 1];    // (int)char.MaxValue
        _size = 0;
        status = Status.NOT_WORD_1;
    }

    public BinTrie(Dictionary<string, V> map)
        :this()
    {
        foreach (KeyValuePair<string, V> entry in map)
        {
            Add(entry.Key, entry.Value);
        }
    }

    /**
     * 插入一个词
     *
     * @param key
     * @param value
     */
    public void Add(string key, V value)
    {
        if (key.Length == 0) return;  // 安全起见
        BaseNode<V> branch = this;
        char[] chars = key.ToCharArray();
        for (int i = 0; i < chars.Length - 1; ++i)
        {
            // 除了最后一个字外，都是继续
            branch.addChild(new Node<V>(chars[i], Status.NOT_WORD_1, default));
            branch = branch.getChild(chars[i]);
        }
        // 最后一个字加入时属性为end
        if (branch.addChild(new Node<V>(chars[chars.Length - 1], Status.WORD_END_3, value)))
        {
            ++this._size; // 维护size
        }
    }

    public void Add(char[] key, V value)
    {
        BaseNode<V> branch = this;
        for (int i = 0; i < key.Length - 1; ++i)
        {
            // 除了最后一个字外，都是继续
            branch.addChild(new Node<V>(key[i], Status.NOT_WORD_1, default));
            branch = branch.getChild(key[i]);
        }
        // 最后一个字加入时属性为end
        if (branch.addChild(new Node<V>(key[key.Length - 1], Status.WORD_END_3, value)))
        {
            ++this._size; // 维护size
        }
    }

    /**
     * 设置键值对，当键不存在的时候会自动插入
     * @param key
     * @param value
     */
    public void set(string key, V value)
    {
        Add(key.ToCharArray(), value);
    }

    /**
     * 删除一个词
     *
     * @param key
     */
    public void Remove(string key)
    {
        BaseNode<V> branch = this;
        char[] chars = key.ToCharArray();
        for (int i = 0; i < chars.Length - 1; ++i)
        {
            if (branch == null) return;
            branch = branch.getChild(chars[i]);
        }
        if (branch == null) return;
        // 最后一个字设为undefined
        if (branch.addChild(new Node<V>(chars[^1], Status.UNDEFINED_0, value)))
        {
            --this._size;
        }
    }

    public bool ContainsKey(string key)
    {
        BaseNode<V> branch = this;
        char[] chars = key.ToCharArray();
        foreach (char aChar in chars)
        {
            if (branch == null) return false;
            branch = branch.getChild(aChar);
        }

        return branch != null && (branch.status == Status.WORD_END_3 || branch.status == Status.WORD_MIDDLE_2);
    }

    public V get(string key)
    {
        BaseNode<V> branch = this;
        char[] chars = key.ToCharArray();
        foreach (char aChar in chars)
        {
            if (branch == null) return default;
            branch = branch.getChild(aChar);
        }

        if (branch == null) return default;
        // 下面这句可以保证只有成词的节点被返回
        if (!(branch.status == Status.WORD_END_3 || branch.status == Status.WORD_MIDDLE_2)) return null;
        return (V) branch.Value();
    }

    public V get(char[] key)
    {
        BaseNode<V> branch = this;
        foreach (char aChar in key)
        {
            if (branch == null) return default;
            branch = branch.getChild(aChar);
        }

        if (branch == null) return default;
        // 下面这句可以保证只有成词的节点被返回
        if (!(branch.status == Status.WORD_END_3 || branch.status == Status.WORD_MIDDLE_2)) return null;
        return (V) branch.Value();
    }

    //@Override
    public V[] getValueArray(V[] a)
    {
        if (a.Length < _size)
            a = (V[]) java.lang.reflect.Array.newInstance(
                    a.getClass().getComponentType(), size);
        int i = 0;
        foreach (KeyValuePair<string, V> entry in this)
        {
            a[i++] = entry.Value;
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
        HashSet<KeyValuePair<string, V>> entrySet = new ();
        StringBuilder sb = new StringBuilder();
        foreach (BaseNode<V> node in child)
        {
            if (node == null) continue;
            node.walk(new StringBuilder(sb.ToString()), entrySet);
        }
        return entrySet;
    }

    /**
     * 键集合
     * @return
     */
    public HashSet<string> Keys()
    {
        var keySet = new HashSet<string>();
        foreach (KeyValuePair<string, V> entry in entrySet())
        {
            keySet.Add(entry.Key);
        }

        return keySet;
    }

    /**
     * 前缀查询
     *
     * @param key 查询串
     * @return 键值对
     */
    public HashSet<TrieEntry> prefixSearch(string key)
    {
        HashSet<TrieEntry> entrySet = new ();
        StringBuilder sb = new StringBuilder(key[0 .. (key.Length - 1)]);
        BaseNode<V> branch = this;
        char[] chars = key.ToCharArray();
        foreach (char aChar in chars)
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
    public List<KeyValuePair<string, V>> commonPrefixSearchWithValue(char[] chars, int begin)
    {
        List<KeyValuePair<string, V>> result = new ();
        StringBuilder sb = new StringBuilder();
        BaseNode<V> branch = this;
        for (int i = begin; i < chars.Length; ++i)
        {
            char aChar = chars[i];
            branch = branch.getChild(aChar);
            if (branch == null || branch.status == Status.UNDEFINED_0) return result;
            sb.Append(aChar);
            if (branch.status == Status.WORD_MIDDLE_2 || branch.status == Status.WORD_END_3)
            {
                result.Add(new AbstractMap.SimpleEntry<string, V>(sb.ToString(), (V) branch.Value()));
            }
        }

        return result;
    }

    //@Override
    public override bool addChild(BaseNode<V> node)
    {
        bool Add = false;
        char c = node.getChar();
        BaseNode<V> target = getChild(c);
        if (target == null)
        {
            child[c] = node;
            Add = true;
        }
        else
        {
            switch (node.status)
            {
                case UNDEFINED_0:
                    if (target.status != Status.NOT_WORD_1)
                    {
                        target.status = Status.NOT_WORD_1;
                        Add = true;
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
                    if (target.Value == null)
                    {
                        Add = true;
                    }
                    target.setValue(node.Value);
                    break;
            }
        }
        return Add;
    }

    public int size()
    {
        return this._size;
    }

    //@Override
    protected char getChar()
    {
        return '\0';   // 根节点没有char
    }

    //@Override
    public override BaseNode<V> getChild(char c)
    {
        return child[c];
    }

    public bool save(string path)
    {
        try
        {
            Stream _out = new Stream(IOUtil.newOutputStream(path));
            foreach (BaseNode<V> node in child)
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
            _out.Close();
        }
        catch (Exception e)
        {
            logger.warning("保存到" + path + "失败" + TextUtility.exceptionToString(e));
            return false;
        }

        return true;
    }

    //@Override
    public int build(Dictionary<string, V> keyValueMap)
    {
        foreach (KeyValuePair<string, V> entry in keyValueMap)
        {
            Add(entry.Key, entry.Value);
        }
        return 0;
    }

    /**
     * 保存到二进制输出流
     *
     * @param _out
     * @return
     */
    public bool save(Stream _out)
    {
        try
        {
            foreach (BaseNode<V> node in child)
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
        _ValueArray<V> valueArray = new _ValueArray<V>(value);
        ByteArray byteArray = new ByteArray(bytes);
        for (int i = 0; i < child.Length; ++i)
        {
            int flag = byteArray.Next();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(byteArray, valueArray);
            }
        }
        _size = value.Length;

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
            int flag = byteArray.Next();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(byteArray, valueArray);
            }
        }
        this._size = -1;  // 不知道有多少

        return true;
    }

    public bool load(ByteArray byteArray, _ValueArray valueArray)
    {
        for (int i = 0; i < child.Length; ++i)
        {
            int flag = byteArray.Next();
            if (flag == 1)
            {
                child[i] = new Node<V>();
                child[i].walkToLoad(byteArray, valueArray);
            }
        }
        this._size = valueArray.value.Length;

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
        foreach (BaseNode<V> node in child)
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
        this._size = _in.readInt();
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
            BaseNode<V> state = transition(text[i]);
            if (state != null)
            {
                int to = i + 1;
                int end = to;
                V value = state.Value();
                for (; to < Length; ++to)
                {
                    state = state.transition(text[to]);
                    if (state == null) break;
                    if (state.Value != null)
                    {
                        value = state.Value();
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
                V value = state.Value();
                for (; to < Length; ++to)
                {
                    state = state.transition(text[to]);
                    if (state == null) break;
                    if (state.Value != null)
                    {
                        value = state.Value();
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
            state = state.transition(text[i]);
            if (state != null)
            {
                V value = state.Value();
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
                V value = state.Value();
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
