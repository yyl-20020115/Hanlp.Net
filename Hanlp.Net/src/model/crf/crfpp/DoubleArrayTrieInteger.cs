
using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;
/**
* DoubleArrayTrie: Java implementation of Darts (Double-ARray Trie System)
* <p>
* <p>
* Copyright(C) 2001-2007 Taku Kudo &lt;taku@chasen.org&gt;<br />
* Copyright(C) 2009 MURAWAKI Yugo &lt;murawaki@nlp.kuee.kyoto-u.ac.jp&gt;
* Copyright(C) 2012 KOMIYA Atsushi &lt;komiya.atsushi@gmail.com&gt;
* </p>
* <p>
* <p>
* The contents of this file may be used under the terms of either of the GNU
* Lesser General Public License Version 2.1 or later (the "LGPL"), or the BSD
* License (the "BSD").
* </p>
*/
namespace com.hankcs.hanlp.model.crf.crfpp;



/**
 * 储存{@code int}的{@code DoubleArrayTrie}，相当于{@code DoubleArrayTrie<int>}，但比后者省内存，所以保留两份代码
 */
public class DoubleArrayTrieInteger : Serializable
{

    private static int BUF_SIZE = 16384;
    private static int UNIT_SIZE = 8; // size of int + int
    private static readonly long serialVersionUID = -4908582458604586299L;

    public class Node
    {
        public int code;
        public int depth;
        public int left;
        public int right;
    }

    private int[] check;
    private int[] _base;

    private bool[] used;
    private int size;
    private int allocSize;
    private List<string> key;
    private int keySize;
    private int[] Length;
    private int[] value;
    private int progress;
    private int nextCheckPos;
    // bool no_delete_;
    int error_;

    private int resize(int newSize)
    {
        int[] base2 = new int[newSize];
        int[] check2 = new int[newSize];
        bool[] used2 = new bool[newSize];
        if (allocSize > 0)
        {
            Array.Copy(_base, 0, base2, 0, allocSize);
            Array.Copy(check, 0, check2, 0, allocSize);
            Array.Copy(used, 0, used2, 0, allocSize);
        }

        _base = base2;
        check = check2;
        used = used2;

        return allocSize = newSize;
    }

    private int fetch(Node parent, List<Node> siblings)
    {
        if (error_ < 0)
            return 0;

        int prev = 0;

        for (int i = parent.left; i < parent.right; i++)
        {
            if ((Length != null ? Length[i] : key[i].Length) < parent.depth)
                continue;

            string tmp = key[i];

            int cur = 0;
            if ((Length != null ? Length[i] : tmp.Length) != parent.depth)
                cur = (int) tmp[parent.depth] + 1;

            if (prev > cur)
            {
                error_ = -3;
                return 0;
            }

            if (cur != prev || siblings.Count == 0)
            {
                Node tmp_node = new Node();
                tmp_node.depth = parent.depth + 1;
                tmp_node.code = cur;
                tmp_node.left = i;
                if (siblings.Count != 0)
                    siblings[(^1)].right = i;

                siblings.Add(tmp_node);
            }

            prev = cur;
        }

        if (siblings.Count != 0)
            siblings[^1].right = parent.right;

        return siblings.Count;
    }

    private int insert(List<Node> siblings)
    {
        if (error_ < 0)
            return 0;

        int begin = 0;
        int pos = ((siblings[(0)].code + 1 > nextCheckPos) ? siblings[(0)].code + 1
            : nextCheckPos) - 1;
        int nonzero_num = 0;
        int first = 0;

        if (allocSize <= pos)
            resize(pos + 1);

        outer:
        while (true)
        {
            pos++;

            if (allocSize <= pos)
                resize(pos + 1);

            if (check[pos] != 0)
            {
                nonzero_num++;
                continue;
            }
            else if (first == 0)
            {
                nextCheckPos = pos;
                first = 1;
            }

            begin = pos - siblings[(0)].code;
            if (allocSize <= (begin + siblings[(siblings.Count - 1)].code))
            {
                // progress can be zero
                double l = (1.05 > 1.0 * keySize / (progress + 1)) ? 1.05 : 1.0
                    * keySize / (progress + 1);
                resize((int) (allocSize * l));
            }

            if (used[begin])
                continue;

            for (int i = 1; i < siblings.Count; i++)
                if (check[begin + siblings[(i)].code] != 0)
                    goto outer;

            break;
        }

        // -- Simple heuristics --
        // if the percentage of non-empty contents in check between the
        // index
        // 'next_check_pos' and 'check' is greater than some constant value
        // (e.g. 0.9),
        // new 'next_check_pos' index is written by 'check'.
        if (1.0 * nonzero_num / (pos - nextCheckPos + 1) >= 0.95)
            nextCheckPos = pos;

        used[begin] = true;
        size = (size > begin + siblings[^1].code + 1) ? size
            : begin + siblings[^1].code + 1;

        for (int i = 0; i < siblings.Count; i++)
            check[begin + siblings[(i)].code] = begin;

        for (int i = 0; i < siblings.Count; i++)
        {
            List<Node> new_siblings = new ();

            if (fetch(siblings[(i)], new_siblings) == 0)
            {
                base[begin + siblings[(i)].code] = (value != null) ? (-value[siblings
                    [(i)].left] - 1) : (-siblings[(i)].left - 1);

                if (value != null && (-value[siblings[(i)].left] - 1) >= 0)
                {
                    error_ = -2;
                    return 0;
                }

                progress++;
            }
            else
            {
                int h = insert(new_siblings);
                base[begin + siblings[(i)].code] = h;
            }
        }
        return begin;
    }

    public DoubleArrayTrieInteger()
    {
        check = null;
        _base = null;
        used = null;
        size = 0;
        allocSize = 0;
        // no_delete_ = false;
        error_ = 0;
    }

    // no deconstructor

    // set_result omitted
    // the search methods returns (the list of) the value(s) instead
    // of (the list of) the pair(s) of value(s) and Length(s)

    // set_array omitted
    // array omitted

    void Clear()
    {
        // if (! no_delete_)
        check = null;
        _base = null;
        used = null;
        allocSize = 0;
        size = 0;
        // no_delete_ = false;
    }

    public int getUnitSize()
    {
        return UNIT_SIZE;
    }

    public int getSize()
    {
        return size;
    }

    public int getTotalSize()
    {
        return size * UNIT_SIZE;
    }

    public int getNonzeroSize()
    {
        int result = 0;
        for (int i = 0; i < size; i++)
            if (check[i] != 0)
                result++;
        return result;
    }

    public int build(List<string> key)
    {
        return build(key, null, null, key.Count);
    }

    public int build(List<string> _key, int[] _length, int[] _value,
                     int _keySize)
    {
        if (_keySize > _key.Count || _key == null)
            return 0;

        key = _key;
        Length = _length;
        keySize = _keySize;
        value = _value;
        progress = 0;

        resize(65536 * 32);

        base[0] = 1;
        nextCheckPos = 0;

        Node root_node = new Node();
        root_node.left = 0;
        root_node.right = keySize;
        root_node.depth = 0;

        List<Node> siblings = new ();
        fetch(root_node, siblings);
        insert(siblings);

        used = null;
        key = null;

        return error_;
    }

    /*
     * recover original key list and value array from a DoubleArrayTrie object
     */
    public void recoverKeyValue()
    {
        key = new ();
        List<int> val1 = new ();
        Dictionary<int, List<int>> childIdxMap = new ();
        for (int i = 0; i < check.Length; i++)
        {
            if (check[i] <= 0) continue;
            if (!childIdxMap.ContainsKey(check[i]))
            {
                List<int> childList = new ();
                childIdxMap.Add(check[i], childList);
            }
            childIdxMap[(check[i])].Add(i);
        }
        Stack<int[]> s = new Stack<int[]>();
        s.Push(new int[]{1, -1});

        List<int> charBuf = new ();
        while (true)
        {
            int[] pair = s.Peek();
            //List<int> childList = childIdxMap.get(pair[0]);
            if (!childIdxMap.TryGetValue(pair[0],out var childList) || (childList.Count - 1) == pair[1])
            {
                s.Pop();
                if (s.Count==0)
                {
                    break;
                }
                else
                {
                    if (charBuf.Count>0)
                    {
                        charBuf.Remove(charBuf.Count - 1);
                    }
                    continue;
                }
            }
            else
            {
                pair[1]++;
            }
            int c = (int) childList[(pair[1])];
            int code = (c - 1 - pair[0]);
            if (base[c] > 0)
            {
                s.Push(new int[]{base[c], -1});
                charBuf.Add(code);
                continue;
            }
            else if (base[c] < 0)
            {
                if (check[c] == c)
                {
                    char[] chars = new char[charBuf.Count];
                    for (int i = 0; i < charBuf.Count; i++)
                    {
                        chars[i] = (char) (int) charBuf[i];
                    }
                    key.Add(new string(chars));
                    val1.Add(-base[c] - 1);
                }
                continue;
            }
        }
        if (val1.Count == 0)
        {
            value = new int[val1.Count];
            for (int i = 0; i < val1.Count; i++)
            {
                value[i] = val1[i];
            }
        }
    }

    public void open(string fileName) 
    {
        string file = (fileName);
        size = (int) file.Length / UNIT_SIZE;
        check = new int[size];
        _base = new int[size];

        Stream @is = null;
        try
        {
            @is =  new FileStream(file);
            for (int i = 0; i < size; i++)
            {
                _base[i] = @is.readInt();
                check[i] = @is.readInt();
            }
        }
        finally
        {
            if (@is != null)
                @is.Close();
        }
    }

    public void save(string fileName) 
    {
        Stream Out = null;
        try
        {
            Out = new 
                IOUtil.newOutputStream(fileName);
            for (int i = 0; i < size; i++)
            {
                Out.writeInt(base[i]);
                Out.writeInt(check[i]);
            }
            Out.Close();
        }
        finally
        {
            if (Out != null)
                Out.Close();
        }
    }

    public int exactMatchSearch(string key)
    {
        return exactMatchSearch(key, 0, 0, 0);
    }

    public int exactMatchSearch(string key, int pos, int len, int nodePos)
    {
        if (len <= 0)
            len = key.Length;
        if (nodePos <= 0)
            nodePos = 0;

        int result = -1;

        char[] keyChars = key.ToCharArray();

        int b = _base[nodePos];
        int p;

        for (int i = pos; i < len; i++)
        {
            p = b + (int) (keyChars[i]) + 1;
            if (b == check[p])
                b = _base[p];
            else
                return result;
        }

        p = b;
        int n = _base[p];
        if (b == check[p] && n < 0)
        {
            result = -n - 1;
        }
        return result;
    }

    public List<int> commonPrefixSearch(string key)
    {
        return commonPrefixSearch(key, 0, 0, 0);
    }

    public List<int> commonPrefixSearch(string key, int pos, int len,
                                            int nodePos)
    {
        if (len <= 0)
            len = key.Length;
        if (nodePos <= 0)
            nodePos = 0;

        List<int> result = new ();

        char[] keyChars = key.ToCharArray();

        int b = base[nodePos];
        int n;
        int p;

        for (int i = pos; i < len; i++)
        {
            p = b;
            n = _base[p];

            if (b == check[p] && n < 0)
            {
                result.Add(-n - 1);
            }

            p = b + (int) (keyChars[i]) + 1;
            if (b == check[p])
                b = base[p];
            else
                return result;
        }

        p = b;
        n = _base[p];

        if (b == check[p] && n < 0)
        {
            result.Add(-n - 1);
        }

        return result;
    }

    // debug
    public void dump()
    {
        for (int i = 0; i < size; i++)
        {
            Console.Error.WriteLine("i: " + i + " [" + base[i] + ", " + check[i]
                                   + "]");
        }
    }

    public List<string> Key()
    {
        return key;
    }

    public int[] Value()
    {
        return value;
    }

    public void setValue(int[] value)
    {
        this.value = value;
    }

    public void setKey(List<string> key)
    {
        this.key = key;
    }

    public int[] getCheck()
    {
        return check;
    }

    public void setCheck(int[] check)
    {
        this.check = check;
    }

    public int[] getBase()
    {
        return _base;
    }

    public void setBase(int[] _base)
    {
        this._base = _base;
    }

    public int[] getLength()
    {
        return Length;
    }

    public void setLength(int[] Length)
    {
        this.Length = Length;
    }

    public void setSize(int size)
    {
        this.size = size;
    }
}
