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
        d = new BinTrie<int>(){
            //@Override
            public bool load(ByteArray byteArray, _ValueArray valueArray)
            {
                BaseNode<int>[] nchild = new BaseNode[child.Length - 1];    // 兼容旧模型
                System.arraycopy(child, 0, nchild, 0, nchild.Length);
                child = nchild;
                return base.load(byteArray, valueArray);
            }
        };
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

    public int get(char[]... keyArray)
    {
        int f = get(convert(keyArray));
        if (f == null) return 0;
        return f;
    }

    public int get(char... key)
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

    public double freq(char[]... keyArray)
    {
        return freq(convert(keyArray));
    }

    public double freq(char... keyArray)
    {
        int f = d.get(keyArray);
        if (f == null) f = 0;
        return f / (double) total;
    }

    public HashSet<string> samples()
    {
        return d.keySet();
    }

    void Add(string key, int value)
    {
        int f = get(key);
        if (f == null) f = 0;
        f += value;
        d.put(key, f);
        total += value;
    }

    void Add(int value, char... key)
    {
        int f = d.get(key);
        if (f == null) f = 0;
        f += value;
        d.put(key, f);
        total += value;
    }

    public void Add(int value, char[]... keyArray)
    {
        Add(convert(keyArray), value);
    }

    public void Add(int value, Collection<char[]> keyArray)
    {
        Add(convert(keyArray), value);
    }

    private string convert(Collection<char[]> keyArray)
    {
        StringBuilder sbKey = new StringBuilder(keyArray.size() * 2);
        for (char[] key : keyArray)
        {
            sbKey.Append(key[0]);
            sbKey.Append(key[1]);
        }
        return sbKey.ToString();
    }

    static private string convert(char[]... keyArray)
    {
        StringBuilder sbKey = new StringBuilder(keyArray.Length * 2);
        for (char[] key : keyArray)
        {
            sbKey.Append(key[0]);
            sbKey.Append(key[1]);
        }
        return sbKey.ToString();
    }

    //@Override
    public void save(DataOutputStream _out)
    {
        _out.writeInt(total);
        int[] valueArray = d.getValueArray(new int[0]);
        _out.writeInt(valueArray.Length);
        for (int v : valueArray)
        {
            _out.writeInt(v);
        }
        d.save(_out);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        total = byteArray.nextInt();
        int size = byteArray.nextInt();
        int[] valueArray = new int[size];
        for (int i = 0; i < valueArray.Length; ++i)
        {
            valueArray[i] = byteArray.nextInt();
        }
        d.load(byteArray, valueArray);
        return true;
    }
}
