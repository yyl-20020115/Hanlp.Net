/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/5/2 20:22</create-date>
 *
 * <copyright file="INode.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.MDAG;
using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.collection.trie.bintrie;



/**
 * 节点，统一Trie树根和其他节点的基类
 *
 * @param <V> 值
 * @author He Han
 */
public abstract class BaseNode<V> : IComparable<BaseNode<V>>
{
    /**
     * 状态数组，方便读取的时候用
     */
    public static readonly Status[] ARRAY_STATUS = Status.values();
    /**
     * 子节点
     */
    protected BaseNode<V>[] child;
    /**
     * 节点状态
     */
    public Status status;
    /**
     * 节点代表的字符
     */
    protected char c;
    /**
     * 节点代表的值
     */
    protected V value;

    public BaseNode<V> transition(string path, int begin)
    {
        BaseNode<V> cur = this;
        for (int i = begin; i < path.Length; ++i)
        {
            cur = cur.getChild(path[(i)]);
            if (cur == null || cur.status == Status.UNDEFINED_0) return null;
        }
        return cur;
    }

    public BaseNode<V> transition(char[] path, int begin)
    {
        BaseNode<V> cur = this;
        for (int i = begin; i < path.Length; ++i)
        {
            cur = cur.getChild(path[i]);
            if (cur == null || cur.status == Status.UNDEFINED_0) return null;
        }
        return cur;
    }

    /**
     * 转移状态
     * @param path
     * @return
     */
    public BaseNode<V> transition(char path)
    {
        BaseNode<V> cur = this;
        cur = cur.getChild(path);
        if (cur == null || cur.status == Status.UNDEFINED_0) return null;
        return cur;
    }

    /**
     * 添加子节点
     *
     * @return true-新增了节点 false-修改了现有节点
     */
    public abstract bool addChild(BaseNode<V> node);

    /**
     * 是否含有子节点
     *
     * @param c 子节点的char
     * @return 是否含有
     */
    public bool hasChild(char c)
    {
        return getChild(c) != null;
    }

    public char getChar()
    {
        return c;
    }

    /**
     * 获取子节点
     *
     * @param c 子节点的char
     * @return 子节点
     */
    public abstract BaseNode<V> getChild(char c);

    /**
     * 获取节点对应的值
     *
     * @return 值
     */
    public V Value()
    {
        return value;
    }

    /**
     * 设置节点对应的值
     *
     * @param value 值
     */
    public void setValue(V value)
    {
        this.value = value;
    }

    //@Override
    public int CompareTo(BaseNode<V> other)
    {
        return CompareTo(other.getChar());
    }

    /**
     * 重载，与字符的比较
     * @param other
     * @return
     */
    public int CompareTo(char other)
    {
        if (this.c > other)
        {
            return 1;
        }
        if (this.c < other)
        {
            return -1;
        }
        return 0;
    }

    /**
     * 获取节点的成词状态
     * @return
     */
    public Status getStatus()
    {
        return status;
    }

    public void walk(StringBuilder sb, HashSet<TrieEntry> entrySet)
    {
        sb.Append(c);
        if (status == Status.WORD_MIDDLE_2 || status == Status.WORD_END_3)
        {
            entrySet.Add(new TrieEntry(sb.ToString(), value));
        }
        if (child == null) return;
        foreach (BaseNode<V> node in child)
        {
            if (node == null) continue;
            node.walk(new StringBuilder(sb.ToString()), entrySet);
        }
    }

    public void walkToSave(Stream _out) 
    {
        _out.writeChar(c);
        _out.writeInt(status.Ordinal);
        int childSize = 0;
        if (child != null) childSize = child.Length;
        _out.writeInt(childSize);
        if (child == null) return;
        foreach (BaseNode<V> node in child)
        {
            node.walkToSave(_out);
        }
    }

    protected void walkToSave(ObjectOutput _out) 
    {
        _out.writeChar(c);
        _out.writeInt(status.Ordinal);
        if (status == Status.WORD_END_3 || status == Status.WORD_MIDDLE_2)
        {
            _out.writeObject(value);
        }
        int childSize = 0;
        if (child != null) childSize = child.Length;
        _out.writeInt(childSize);
        if (child == null) return;
        foreach (BaseNode<V> node in child)
        {
            node.walkToSave(_out);
        }
    }

    public void walkToLoad(ByteArray byteArray, _ValueArray<V> valueArray)
    {
        c = byteArray.nextChar();
        status = ARRAY_STATUS[byteArray.Next()];
        if (status == Status.WORD_END_3 || status == Status.WORD_MIDDLE_2)
        {
            value = valueArray.nextValue();
        }
        int childSize = byteArray.Next();
        child = new BaseNode<V>[childSize];
        for (int i = 0; i < childSize; ++i)
        {
            child[i] = new Node<V>();
            child[i].walkToLoad(byteArray, valueArray);
        }
    }

    protected void walkToLoad(ObjectInput byteArray) 
    {
        c = byteArray.readChar();
        status = ARRAY_STATUS[byteArray.readInt()];
        if (status == Status.WORD_END_3 || status == Status.WORD_MIDDLE_2)
        {
            value = (V) byteArray.readObject();
        }
        int childSize = byteArray.readInt();
        child = new BaseNode<V>[childSize];
        for (int i = 0; i < childSize; ++i)
        {
            child[i] = new Node<V>();
            child[i].walkToLoad(byteArray);
        }
    }

    public enum Status
    {
        /**
         * 未指定，用于删除词条
         */
        UNDEFINED_0,
        /**
         * 不是词语的结尾
         */
        NOT_WORD_1,
        /**
         * 是个词语的结尾，并且还可以继续
         */
        WORD_MIDDLE_2,
        /**
         * 是个词语的结尾，并且没有继续
         */
        WORD_END_3,
    }

    public class TrieEntry : AbstractMap<string,V>.SimpleEntry<string, V> , IComparable<TrieEntry>
    {
        public TrieEntry(string key, V value)
            : base(key, value)
        {
           ;
        }
        //@Override
        public int CompareTo(TrieEntry o)
        {
            return Key.CompareTo(o.Key);
        }
    }

    //@Override
    public override string ToString() 
        => child == null
            ? "BaseNode{" +
                     "status=" + status +
                     ", c=" + c +
                     ", value=" + value +
                    '}'
            : "BaseNode{" +
                "child=" + child.Length +
                ", status=" + status +
                ", c=" + c +
                ", value=" + value +
                '}';
}
