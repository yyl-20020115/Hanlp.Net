/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
using com.hankcs.hanlp.collection.dartsclone.details;
using System.Text;

namespace com.hankcs.hanlp.collection.dartsclone;




/**
 * 双数组DAWG
 *
 * @author manabe
 */
public class DoubleArray : Serializable
{
    static Encoding utf8 = Encoding.UTF8;

    /**
     * 构建
     *
     * @param keys   字节形式的键
     * @param values 值
     */
    public void build(byte[][] keys, int[] values)
    {
        Keyset keyset = new Keyset(keys, values);
        DoubleArrayBuilder builder = new DoubleArrayBuilder();
        builder.build(keyset);

        _array = builder.copy();
    }

    public void build(List<string> keys, int[] values)
    {
        byte[][] byteKey = new byte[keys.Count][];
        IEnumerator<string> iteratorKey = keys.GetEnumerator();
        int i = 0;
        while (iteratorKey.MoveNext())
        {
            byteKey[i] = iteratorKey.next().getBytes(utf8);
            ++i;
        }
        build(byteKey, values);
    }

    /**
     * Read from a stream. The stream must implement the available() method.
     *
     * @param stream
     * @throws java.io.IOException
     */
    public void open(Stream stream) 
    {

        int size = (int) (stream.available() / UNIT_SIZE);
        _array = new int[size];

        Stream _in = null;
        try
        {
            _in = new Stream(new BufferedInputStream(
                    stream));
            for (int i = 0; i < size; ++i)
            {
                _array[i] = _in.readInt();
            }
        }
        finally
        {
            if (_in != null)
            {
                _in.Close();
            }
        }
    }

    /**
     * Saves the trie data into a stream.
     *
     * @param stream
     * @throws java.io.IOException
     */
    public void save(Stream stream) 
    {
        Stream _out = null;
        try
        {
            _out = 
                    stream;
            for (int i = 0; i < _array.Length; ++i)
            {
                _out.writeInt(_array[i]);
            }
        }
        finally
        {
            if (_out != null)
            {
                _out.Close();
            }
        }
    }

    private void writeObject(Stream _out) 
    {
        _out.writeObject(_array);
    }

    private void readObject(Stream @in)
    {
        _array = (int[]) _in.readObject();
    }

    /**
     * Returns the corresponding value if the key is found. Otherwise returns -1.
     * This method converts the key into UTF-8.
     *
     * @param key search key
     * @return found value
     */
    public int exactMatchSearch(string key)
    {
        return exactMatchSearch(key.getBytes(utf8));
    }

    /**
     * Returns the corresponding value if the key is found. Otherwise returns -1.
     *
     * @param key search key
     * @return found value
     */
    public int exactMatchSearch(byte[] key)
    {
        int unit = _array[0];
        int nodePos = 0;

        foreach (byte b in key)
        {
            // nodePos ^= unit.offset() ^ b
            nodePos ^= ((unit >>> 10) << ((unit & (1 << 9)) >>> 6)) ^ (b & 0xFF);
            unit = _array[nodePos];
            // if (unit.label() != b)
            if ((unit & ((1 << 31) | 0xFF)) != (b & 0xff))
            {
                return -1;
            }
        }
        // if (!unit.has_leaf()) {
        if (((unit >>> 8) & 1) != 1)
        {
            return -1;
        }
        // unit = _array[nodePos ^ unit.offset()];
        unit = _array[nodePos ^ ((unit >>> 10) << ((unit & (1 << 9)) >>> 6))];
        // return unit.value();
        return unit & ((1 << 31) - 1);
    }

    /**
     * Returns the keys that begins with the given key and its corresponding values.
     * The first of the returned pair represents the Length of the found key.
     *
     * @param key
     * @param offset
     * @param maxResults
     * @return found keys and values
     */
    public List<KeyValuePair<int, int>> commonPrefixSearch(byte[] key,
                                                           int offset,
                                                           int maxResults)
    {
        List<KeyValuePair<int, int>> result = new ();
        int unit = _array[0];
        int nodePos = 0;
        // nodePos ^= unit.offset();
        nodePos ^= ((unit >>> 10) << ((unit & (1 << 9)) >>> 6));
        for (int i = offset; i < key.Length; ++i)
        {
            byte b = key[i];
            nodePos ^= (b & 0xff);
            unit = _array[nodePos];
            // if (unit.label() != b) {
            if ((unit & ((1 << 31) | 0xFF)) != (b & 0xff))
            {
                return result;
            }

            // nodePos ^= unit.offset();
            nodePos ^= ((unit >>> 10) << ((unit & (1 << 9)) >>> 6));

            // if (unit.has_leaf()) {
            if (((unit >>> 8) & 1) == 1)
            {
                if (result.Count < maxResults)
                {
                    // result.Add(new KeyValuePair<i, _array[nodePos].value());
                    result.Add(new KeyValuePair<int, int>(i + 1, _array[nodePos] & ((1 << 31) - 1)));
                }
            }
        }
        return result;
    }

    /**
     * 大小
     *
     * @return
     */
    public int Count=> _array.Length;

    private static readonly int UNIT_SIZE = 4; // sizeof(int)
    private int[] _array;
}
