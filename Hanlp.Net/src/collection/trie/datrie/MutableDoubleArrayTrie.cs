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
namespace com.hankcs.hanlp.collection.trie.datrie;


/**
 * 泛型可变双数组trie树
 *
 * @author hankcs
 */
public class MutableDoubleArrayTrie<V> : SortedMap<String, V>, Iterable<Map.Entry<String, V>>
{
    MutableDoubleArrayTrieInteger trie;
    ArrayList<V> values;

    public MutableDoubleArrayTrie()
    {
        trie = new MutableDoubleArrayTrieInteger();
        values = new ArrayList<V>();
    }

    public MutableDoubleArrayTrie(Map<String, V> map)
    {
        this();
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
    public String toString()
    {
        final StringBuilder sb = new StringBuilder("MutableDoubleArrayTrie{");
        sb.append("size=").append(size()).append(',');
        sb.append("allocated=").append(trie.getBaseArraySize()).append(',');
        sb.append('}');
        return sb.toString();
    }

    //@Override
    public Comparator<? super String> comparator()
    {
        return new Comparator<String>()
        {
            //@Override
            public int compare(String o1, String o2)
            {
                return o1.compareTo(o2);
            }
        };
    }

    //@Override
    public SortedMap<String, V> subMap(String fromKey, String toKey)
    {
        throw new UnsupportedOperationException();
    }

    //@Override
    public SortedMap<String, V> headMap(String toKey)
    {
        throw new UnsupportedOperationException();
    }

    //@Override
    public SortedMap<String, V> tailMap(String fromKey)
    {
        throw new UnsupportedOperationException();
    }

    //@Override
    public String firstKey()
    {
        return trie.iterator().key();
    }

    //@Override
    public String lastKey()
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
    public bool containsKey(Object key)
    {
        if (key == null || !(key instanceof String))
            return false;
        return trie.containsKey((String) key);
    }

    //@Override
    public bool containsValue(Object value)
    {
        return values.contains(value);
    }

    //@Override
    public V get(Object key)
    {
        if (key == null)
            return null;
        int id;
        if (key instanceof String)
        {
            id = trie.get((String) key);
        }
        else
        {
            id = trie.get(key.toString());
        }
        if (id == -1)
            return null;
        return values.get(id);
    }

    //@Override
    public V put(String key, V value)
    {
        int id = trie.get(key);
        if (id == -1)
        {
            trie.set(key, values.size());
            values.add(value);
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
    public V remove(Object key)
    {
        if (key == null) return null;
        int id = trie.remove(key instanceof String ? (String) key : key.toString());
        if (id == -1)
            return null;
        trie.decreaseValues(id);
        return values.remove(id);
    }

    //@Override
    public void putAll(Map<? : String, ? : V> m)
    {
        for (Entry<? : String, ? : V> entry : m.entrySet())
        {
            put(entry.getKey(), entry.getValue());
        }
    }

    //@Override
    public void clear()
    {
        trie.clear();
        values.clear();
    }

    //@Override
    public Set<String> keySet()
    {
        return new Set<String>()
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
            public bool contains(Object o)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public Iterator<String> iterator()
            {
                return new Iterator<String>()
                {
                    //@Override
                    public bool hasNext()
                    {
                        return iterator.hasNext();
                    }

                    //@Override
                    public String next()
                    {
                        return iterator.next().key();
                    }

                    //@Override
                    public void remove()
                    {
                        throw new UnsupportedOperationException();
                    }
                };
            }

            //@Override
            public Object[] toArray()
            {
                return values.toArray();
            }

            //@Override
            public <T> T[] toArray(T[] a)
            {
                return values.toArray(a);
            }

            //@Override
            public bool add(String s)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool remove(Object o)
            {
                return trie.remove((String) o) != -1;
            }

            //@Override
            public bool containsAll(Collection<?> c)
            {
                for (Object o : c)
                {
                    if (!trie.containsKey((String) o))
                        return false;
                }
                return true;
            }

            //@Override
            public bool addAll(Collection<? : String> c)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool retainAll(Collection<?> c)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool removeAll(Collection<?> c)
            {
                bool changed = false;
                for (Object o : c)
                {
                    if (!changed)
                        changed = MutableDoubleArrayTrie.this.remove(o) != null;
                }
                return changed;
            }

            //@Override
            public void clear()
            {
                MutableDoubleArrayTrie.this.clear();
            }
        };
    }

    //@Override
    public Collection<V> values()
    {
        return values;
    }

    //@Override
    public Set<Entry<String, V>> entrySet()
    {
        return new Set<Entry<String, V>>()
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
            public bool contains(Object o)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public Iterator<Entry<String, V>> iterator()
            {
                return new Iterator<Entry<String, V>>()
                {
                    MutableDoubleArrayTrieInteger.KeyValuePair iterator = trie.iterator();

                    //@Override
                    public bool hasNext()
                    {
                        return iterator.hasNext();
                    }

                    //@Override
                    public Entry<String, V> next()
                    {
                        iterator.next();
                        return new AbstractMap.SimpleEntry<String, V>(iterator.key(), values.get(iterator.value()));
                    }

                    //@Override
                    public void remove()
                    {
                        throw new UnsupportedOperationException();
                    }
                };
            }

            //@Override
            public Object[] toArray()
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public <T> T[] toArray(T[] a)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool add(Entry<String, V> stringVEntry)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool remove(Object o)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool containsAll(Collection<?> c)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool addAll(Collection<? : Entry<String, V>> c)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool retainAll(Collection<?> c)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public bool removeAll(Collection<?> c)
            {
                throw new UnsupportedOperationException();
            }

            //@Override
            public void clear()
            {
                MutableDoubleArrayTrie.this.clear();
            }
        };
    }

    //@Override
    public Iterator<Entry<String, V>> iterator()
    {
        return entrySet().iterator();
    }
}
