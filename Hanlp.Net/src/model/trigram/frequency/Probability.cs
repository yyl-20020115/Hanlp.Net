/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/6 19:57</create-date>
 *
 * <copyright file="Probability.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.model.trigram.frequency;



/**
 * 概率统计工具
 *
 * @author hankcs
 */
public class Probability : ICacheAble
{
    public BinTrie<int> d;
    int total;

    public Probability()
    {
        d = new BT();
    }
    public class BT:BinTrie<int>
    {
        //@Override
        public bool load(ByteArray byteArray, _ValueArray<int> valueArray)
        {
            BaseNode<int>[] nchild = new BaseNode<int>[child.Length - 1];    // 兼容旧模型
            Array.Copy(child, 0, nchild, 0, nchild.Length);
            child = nchild;
            return base.load(byteArray, valueArray);
        }
    }

    public bool exists(string key)
    {
        return d.ContainsKey(key);
    }

    public int getsum()
    {
        return total;
    }

    int get(string key)
    {
        return d.get(key);
    }

    public int get(params char[][] keyArray)
    {
        int f = get(convert(keyArray));
        if (f == null) return 0;
        return f;
    }

    public int get(params char[] key)
    {
        int f = d.get(key);
        if (f == null) return 0;
        return f;
    }

    public double freq(string key)
    {
        int f = get(key);
        if (f == null) f = 0;
        return f / (double) total;
    }

    public double freq(params char[][] keyArray)
    {
        return freq(convert(keyArray));
    }

    public double freq(params char[] keyArray)
    {
        int f = d.get(keyArray);
        if (f == null) f = 0;
        return f / (double) total;
    }

    public HashSet<string> samples()
    {
        return d.Keys();
    }

    void Add(string key, int value)
    {
        int f = get(key);
        if (f == null) f = 0;
        f += value;
        d.Add(key, f);
        total += value;
    }

    void Add(int value, params char[] key)
    {
        int f = d.get(key);
        if (f == null) f = 0;
        f += value;
        d.Add(key, f);
        total += value;
    }

    public void Add(int value, params char[][] keyArray)
    {
        Add(convert(keyArray), value);
    }

    public void Add(int value, ICollection<char[]> keyArray)
    {
        Add(convert(keyArray), value);
    }

    private string convert(ICollection<char[]> keyArray)
    {
        StringBuilder sbKey = new StringBuilder(keyArray.Count * 2);
        foreach (char[] key in keyArray)
        {
            sbKey.Append(key[0]);
            sbKey.Append(key[1]);
        }
        return sbKey.ToString();
    }

    static private string convert(params char[][] keyArray)
    {
        StringBuilder sbKey = new StringBuilder(keyArray.Length * 2);
        foreach (char[] key in keyArray)
        {
            sbKey.Append(key[0]);
            sbKey.Append(key[1]);
        }
        return sbKey.ToString();
    }

    //@Override
    public void save(Stream Out)
    {
        using var st = new BinaryWriter(Out);
        st.Write(total);
        int[] valueArray = d.getValueArray(new int[0]);
        st.Write(valueArray.Length);
        foreach (int v in valueArray)
        {
            st.Write(v);
        }
        d.save(Out);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        total = byteArray.Next();
        int size = byteArray.Next();
        int[] valueArray = new int[size];
        for (int i = 0; i < valueArray.Length; ++i)
        {
            valueArray[i] = byteArray.Next();
        }
        d.load(byteArray, valueArray);
        return true;
    }
}
