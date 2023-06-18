/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-17 下午1:48</create-date>
 *
 * <copyright file="MutableDoubleArrayTrie.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace com.hankcs.hanlp.collection.trie.datrie;


/**
 * 泛型可变双数组trie树
 *
 * @author hankcs
 */
public class MutableDoubleArrayTrie<V> : SortedDictionary<string, V>, IEnumerable<KeyValuePair<string, V>>
{
    MutableDoubleArrayTrieInteger trie;
    List<V> values;

    public MutableDoubleArrayTrie()
    {
        trie = new MutableDoubleArrayTrieInteger();
        values = new();
    }

    public MutableDoubleArrayTrie(Dictionary<string, V> map)
        : this()
    {
        putAll(map);
    }

    /**
     * 去掉多余的buffer
     */
    public void loseWeight()
    {
        trie.loseWeight();
    }

    //@Override
    public override string ToString()
    {
        var sb = new StringBuilder("MutableDoubleArrayTrie{");
        sb.Append("size=").Append(size()).Append(',');
        sb.Append("allocated=").Append(trie.getBaseArraySize()).Append(',');
        sb.Append('}');
        return sb.ToString();
    }

    //@Override
    public IComparer<string> comparator()
    {
        return new CT();
    }
    public class CT : IComparer<string>
    {
        //@Override
        public int Compare(string o1, string o2)
        {
            return o1.CompareTo(o2);
        }
    }

    //@Override
    public SortedDictionary<string, V> subMap(string fromKey, string toKey)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public SortedDictionary<string, V> headMap(string toKey)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public SortedDictionary<string, V> tailMap(string fromKey)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public string firstKey()
    {
        return trie.iterator().key();
    }

    //@Override
    public string lastKey()
    {
        MutableDoubleArrayTrieInteger.KeyValuePair iterator = trie.iterator();
        while (iterator.hasNext())
        {
            iterator.next();
        }
        return iterator.key();
    }

    //@Override
    public int size()
    {
        return trie.size();
    }

    //@Override
    public bool isEmpty()
    {
        return trie.isEmpty();
    }

    //@Override
    public bool ContainsKey(Object key)
    {
        if (key == null || !(key is string))
            return false;
        return trie.ContainsKey((string)key);
    }

    //@Override
    public bool containsValue(Object value)
    {
        return values.Contains(value);
    }

    //@Override
    public V get(Object key)
    {
        if (key == null)
            return null;
        int id;
        if (key is string)
        {
            id = trie.get((string)key);
        }
        else
        {
            id = trie.get(key.ToString());
        }
        if (id == -1)
            return null;
        return values.get(id);
    }

    //@Override
    public V put(string key, V value)
    {
        int id = trie.get(key);
        if (id == -1)
        {
            trie.set(key, values.size());
            values.Add(value);
            return null;
        }
        else
        {
            V v = values.get(id);
            values.set(id, value);
            return v;
        }
    }

    //@Override
    public V Remove(Object key)
    {
        if (key == null) return null;
        int id = trie.Remove(key is string ? (string)key : key.ToString());
        if (id == -1)
            return null;
        trie.decreaseValues(id);
        return values.Remove(id);
    }

    //@Override
    public void putAll(Dictionary<string, V> m)
    {
        for (var entry : m.entrySet())
        {
            put(entry.getKey(), entry.getValue());
        }
    }

    //@Override
    public void Clear()
    {
        trie.Clear();
        values.Clear();
    }

    //@Override
    public HashSet<string> keySet()
    {
        return new HashSet<string>();
    }

    public class HS : HashSet<string>
    {
        MutableDoubleArrayTrieInteger.KeyValuePair iterator = trie.iterator();

        //@Override
        public int size()
        {
            return trie.size();
        }

        //@Override
        public bool isEmpty()
        {
            return trie.isEmpty();
        }

        //@Override
        public bool Contains(Object o)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public IEnumerator<string> iterator()
        {
            return new IEnumerator<string>()
            {
                    //@Override
                    public bool hasNext()
            {
                return iterator.hasNext();
            }

            //@Override
            public string next()
            {
                return iterator.next().key();
            }

            //@Override
            public void Remove()
            {
                throw new InvalidOperationException();
            }
        };
    }

    //@Override
    public Object[] ToArray()
    {
        return values.ToArray();
    }

    //@Override
    public T[] ToArray<T>(T[] a)
    {
        return values.ToArray(a);
    }

    //@Override
    public bool Add(string s)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public bool Remove(Object o)
    {
        return trie.Remove((string)o) != -1;
    }

    //@Override
    public bool containsAll(Collection c)
    {
        for (Object o : c)
        {
            if (!trie.ContainsKey((string)o))
                return false;
        }
        return true;
    }

    //@Override
    public bool addAll(Collection<string> c)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public bool retainAll(Collection c)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public bool removeAll(Collection c)
    {
        bool changed = false;
        for (Object o : c)
        {
            if (!changed)
                changed = MutableDoubleArrayTrie.Remove(o) != null;
        }
        return changed;
    }

    //@Override
    public void Clear()
    {
        MutableDoubleArrayTrie.this.Clear();
    }
}

//@Override
public Collection<V> values()
{
    return values;
}

//@Override
public HashSet<Entry<string, V>> entrySet()
{
    return new HashSet<Entry<string, V>>()
    {
            //@Override
            public int size()
    {
        return trie.size();
    }

    //@Override
    public bool isEmpty()
    {
        return trie.isEmpty();
    }

    //@Override
    public bool Contains(Object o)
    {
        throw new InvalidOperationException();
    }

    //@Override
    public Iterator<Entry<string, V>> iterator()
    {
        return new Iterator<Entry<string, V>>()
                {
                    MutableDoubleArrayTrieInteger.KeyValuePair iterator = trie.iterator();

        //@Override
        public bool hasNext()
        {
            return iterator.hasNext();
        }

        //@Override
        public Entry<string, V> next()
        {
            iterator.next();
            return new AbstractMap.SimpleEntry<string, V>(iterator.key(), values.get(iterator.value()));
        }

        //@Override
        public void Remove()
        {
            throw new InvalidOperationException();
        }
    };
}

//@Override
public Object[] ToArray()
{
    throw new InvalidOperationException();
}

//@Override
public <T> T[] ToArray(T[] a)
{
    throw new InvalidOperationException();
}

//@Override
public bool Add(Entry<string, V> stringVEntry)
{
    throw new InvalidOperationException();
}

//@Override
public bool Remove(Object o)
{
    throw new InvalidOperationException();
}

//@Override
public bool containsAll(Collection c)
{
    throw new InvalidOperationException();
}

//@Override
public bool addAll(Collection<Entry<string, V>> c)
{
    throw new InvalidOperationException();
}

//@Override
public bool retainAll(Collection c)
{
    throw new InvalidOperationException();
}

//@Override
public bool removeAll(Collection c)
{
    throw new InvalidOperationException();
}

//@Override
public void Clear()
{
    MutableDoubleArrayTrie.s.Clear();
}
        };
    }

    //@Override
    public Iterator<Entry<string, V>> iterator()
{
    return entrySet().iterator();
}
}
