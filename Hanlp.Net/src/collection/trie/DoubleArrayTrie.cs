
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.collection.MDAG;
using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;
/**
* DoubleArrayTrie: Java implementation of Darts (Double-ARray Trie System)
* <p/>
* <p>
* Copyright(C) 2001-2007 Taku Kudo &lt;taku@chasen.org&gt;<br />
* Copyright(C) 2009 MURAWAKI Yugo &lt;murawaki@nlp.kuee.kyoto-u.ac.jp&gt;
* Copyright(C) 2012 KOMIYA Atsushi &lt;komiya.atsushi@gmail.com&gt;
* </p>
* <p/>
* <p>
* The contents of this file may be used under the terms of either of the GNU
* Lesser General Public License Version 2.1 or later (the "LGPL"), or the BSD
* License (the "BSD").
* </p>
*/
namespace com.hankcs.hanlp.collection.trie;



/**
 * 双数组Trie树
 */
public class DoubleArrayTrie<V> : ITrie<V>
{
    private  static int BUF_SIZE = 16384;
    private  static int UNIT_SIZE = 8; // size of int + int

    public class Node
    {
        public int code;
        public int depth;
        public int left;
        public int right;

        //@Override
        public override string ToString()
        {
            return "Node{" +
                    "code=" + code +
                    ", depth=" + depth +
                    ", left=" + left +
                    ", right=" + right +
                    '}';
        }
    }

    ;

    protected int[] check;
    protected int[] _base;

    private BitSet used;
    /**
     * base 和 check 的大小
     */
    protected int _size;
    private int allocSize;
    private List<string> key;
    private int keySize;
    private int[] Length;
    private int[] value;
    protected V[] v;
    private int progress;
    private int nextCheckPos;
    // bool no_delete_;
    int error_;

    // int (*progressfunc_) (size_t, size_t);

    // inline _resize expanded

    /**
     * 拓展数组
     *
     * @param newSize
     * @return
     */
    private int resize(int newSize)
    {
        int[] base2 = new int[newSize];
        int[] check2 = new int[newSize];
        if (allocSize > 0)
        {
            Array.Copy(_base, 0, base2, 0, allocSize);
            Array.Copy(check, 0, check2, 0, allocSize);
        }

        _base = base2;
        check = check2;

        return allocSize = newSize;
    }

    /**
     * 获取直接相连的子节点
     *
     * @param parent   父节点
     * @param siblings （子）兄弟节点
     * @return 兄弟节点个数
     */
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
                cur = (int) tmp[(parent.depth)] + 1;

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
                    siblings.get(siblings.Count - 1).right = i;

                siblings.Add(tmp_node);
            }

            prev = cur;
        }

        if (siblings.Count != 0)
            siblings[(^1)].right = parent.right;

        return siblings.Count;
    }

    /**
     * 插入节点
     *
     * @param siblings 等待插入的兄弟节点
     * @return 插入位置
     */
    private int insert(List<Node> siblings)
    {
        if (error_ < 0)
            return 0;

        int begin = 0;
        int pos = Math.Max(siblings[(0)].code + 1, nextCheckPos) - 1;
        int nonzero_num = 0;
        int first = 0;

        if (allocSize <= pos)
            resize(pos + 1);

        outer:
        // 此循环体的目标是找出满足base[begin + a1...an]  == 0的n个空闲空间,a1...an是siblings中的n个节点
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

            begin = pos - siblings.get(0).code; // 当前位置离第一个兄弟节点的距离
            if (allocSize <= (begin + siblings.get(siblings.Count - 1).code))
            {
                resize(begin + siblings.get(siblings.Count - 1).code + char.MaxValue);
            }

            //if (used[begin])
             //   continue;
            if(used.get(begin)){
            	continue;
            }

            for (int i = 1; i < siblings.Count; i++)
                if (check[begin + siblings[i].code] != 0)
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
            nextCheckPos = pos; // 从位置 next_check_pos 开始到 pos 间，如果已占用的空间在95%以上，下次插入节点时，直接从 pos 位置处开始查找

        //used[begin] = true;
        used.set(begin);
        
        _size = (_size > begin + siblings.get(siblings.Count - 1).code + 1) ? size
                : begin + siblings.get(siblings.Count - 1).code + 1;

        for (int i = 0; i < siblings.Count; i++)
        {
            check[begin + siblings[i].code] = begin;
//            Console.WriteLine(this);
        }

        for (int i = 0; i < siblings.Count; i++)
        {
            List<Node> new_siblings = new ();

            if (fetch(siblings[i], new_siblings) == 0)  // 一个词的终止且不为其他词的前缀
            {
                _base[begin + siblings[i].code] = (value != null) ? (-value[siblings
                        [i].left] - 1) : (-siblings[i].left - 1);
//                Console.WriteLine(this);

                if (value != null && (-value[siblings[i].left] - 1) >= 0)
                {
                    error_ = -2;
                    return 0;
                }

                progress++;
                // if (progress_func_) (*progress_func_) (progress,
                // keySize);
            }
            else
            {
                int h = insert(new_siblings);   // dfs
                base[begin + siblings[i].code] = h;
//                Console.WriteLine(this);
            }
        }
        return begin;
    }

    public DoubleArrayTrie()
    {
        check = null;
        _base = null;
        used = new BitSet();
        _size = 0;
        allocSize = 0;
        // no_delete_ = false;
        error_ = 0;
    }

    /**
     * 从Dictionary构造
     * @param buildFrom
     */
    public DoubleArrayTrie(Dictionary<string, V> buildFrom)
        :this()
    {
        if (build(buildFrom) != 0)
        {
            throw new ArgumentException("构造失败");
        }
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
        _size = 0;
        // no_delete_ = false;
    }

    public int getUnitSize()
    {
        return UNIT_SIZE;
    }

    public int getSize()
    {
        return _size;
    }

    public int getTotalSize()
    {
        return _size * UNIT_SIZE;
    }

    public int getNonzeroSize()
    {
        int result = 0;
        for (int i = 0; i < check.Length; ++i)
            if (check[i] != 0)
                ++result;
        return result;
    }

    public int build(List<string> key, List<V> value)
    {
        //assert key.Count == value.Count : "键的个数与值的个数不一样！";
        //assert key.Count > 0 : "键值个数为0！";
        v = (V[]) value.ToArray();
        return build(key, null, null, key.Count);
    }

    public int build(List<string> key, V[] value)
    {
        //assert key.Count == value.Length : "键的个数与值的个数不一样！";
        //assert key.Count > 0 : "键值个数为0！";
        v = value;
        return build(key, null, null, key.Count);
    }

    /**
     * 构建DAT
     *
     * @param entrySet 注意此entrySet一定要是字典序的！否则会失败
     * @return
     */
    public int build(HashSet<KeyValuePair<string, V>> entrySet)
    {
        List<string> keyList = new (entrySet.Count);
        List<V> valueList = new (entrySet.Count);
        foreach (KeyValuePair<string, V> entry in entrySet)
        {
            keyList.Add(entry.Key);
            valueList.Add(entry.Value);
        }

        return build(keyList, valueList);
    }

    /**
     * 方便地构造一个双数组trie树
     *
     * @param keyValueMap 升序键值对map
     * @return 构造结果
     */
    public int build(Dictionary<string, V> keyValueMap)
    {
        //assert keyValueMap != null;
        var entrySet = keyValueMap.ToHashSet();
        return build(entrySet);
    }

    /**
     * 唯一的构建方法
     *
     * @param _key     值set，必须字典序
     * @param _length  对应每个key的长度，留空动态获取
     * @param _value   每个key对应的值，留空使用key的下标作为值
     * @param _keySize key的长度，应该设为_key.Count
     * @return 是否出错
     */
    public int build(List<string> _key, int[] _length, int[] _value,
                     int _keySize)
    {
        if (_key == null || _keySize > _key.Count)
            return 0;

        // progress_func_ = progress_func;
        key = _key;
        Length = _length;
        keySize = _keySize;
        value = _value;
        progress = 0;

        resize(65536 * 32); // 32个双字节

        _base[0] = 1;
        nextCheckPos = 0;

        Node root_node = new Node();
        root_node.left = 0;
        root_node.right = keySize;
        root_node.depth = 0;

        List<Node> siblings = new ();
        fetch(root_node, siblings);
        insert(siblings);
        shrink();

        // size += (1 << 8 * 2) + 1; // ???
        // if (size >= allocSize) resize (size);

        used = null;
        key = null;
        Length = null;

        return error_;
    }

    public void open(string fileName) 
    {
        var file = (fileName);
        _size = (int) file.Length / UNIT_SIZE;
        check = new int[size];
        _base = new int[size];

        Stream _is = null;
        try
        {
            _is = IOUtil.newInputStream(fileName);//, BUF_SIZE;
            for (int i = 0; i < size; i++)
            {
                _base[i] = _is.readInt();
                check[i] = _is.readInt();
            }
        }
        finally
        {
            if (_is != null)
                _is.Close();
        }
    }

    public bool save(string fileName)
    {
        Stream Out;
        try
        {
            Out = new Stream(new BufferedOutputStream(IOUtil.newOutputStream(fileName)));
            Out.writeInt(size);
            for (int i = 0; i < size; i++)
            {
                Out.writeInt(_base[i]);
                Out.writeInt(check[i]);
            }
            Out.Close();
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    /**
     * 将base和check保存下来
     *
     * @param Out
     * @return
     */
    public bool save(Stream Out)
    {
        try
        {
            Out.writeInt(size);
            for (int i = 0; i < _size; i++)
            {
                Out.writeInt(_base[i]);
                Out.writeInt(check[i]);
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    public void save(Stream Out) 
    {
        Out.writeObject(_base);
        Out.writeObject(check);
    }

    /**
     * 从磁盘加载，需要额外提供值
     *
     * @param path
     * @param value
     * @return
     */
    public bool load(string path, List<V> value)
    {
        if (!loadBaseAndCheck(path)) return false;
        v = (V[]) value.ToArray();
        return true;
    }

    /**
     * 从磁盘加载，需要额外提供值
     *
     * @param path
     * @param value
     * @return
     */
    public bool load(string path, V[] value)
    {
        if (!(IOAdapter == null ? loadBaseAndCheckByFileChannel(path) :
        load(ByteArrayStream.createByteArrayStream(path), value)
        )) return false;
        v = value;
        return true;
    }

    public bool load(ByteArray byteArray, V[] value)
    {
        if (byteArray == null) return false;
        _size = byteArray.Next();
        _base = new int[_size + 65535];   // 多留一些，防止越界
        check = new int[_size + 65535];
        for (int i = 0; i < _size; i++)
        {
            _base[i] = byteArray.Next();
            check[i] = byteArray.Next();
        }
        v = value;
        used = null;    // 无用的对象,释放掉
        return true;
    }

    /**
     * 从字节数组加载（发现在MacOS上，此方法比ByteArray更快）
     * @param bytes
     * @param offset
     * @param value
     * @return
     */
    public bool load(byte[] bytes, int offset, V[] value)
    {
        if (bytes == null) return false;
        _size = ByteUtil.bytesHighFirstToInt(bytes, offset);
        offset += 4;
        _base = new int[_size + 65535];   // 多留一些，防止越界
        check = new int[_size + 65535];
        for (int i = 0; i < _size; i++)
        {
            _base[i] = ByteUtil.bytesHighFirstToInt(bytes, offset);
            offset += 4;
            check[i] = ByteUtil.bytesHighFirstToInt(bytes, offset);
            offset += 4;
        }
        v = value;
        used = null;    // 无用的对象,释放掉
        return true;
    }

    /**
     * 载入双数组，但是不提供值，此时本trie相当于一个set
     *
     * @param path
     * @return
     */
    public bool load(string path)
    {
        return loadBaseAndCheckByFileChannel(path);
    }

    /**
     * 从磁盘加载双数组
     *
     * @param path
     * @return
     */
    private bool loadBaseAndCheck(string path)
    {
        try
        {
            Stream _in = new Stream(
                new BufferedInputStream(IOAdapter == null ? new FileStream(path) :
                    IOAdapter.open(path)
            ));
            _size = _in.readInt();
            _base = new int[_size + 65535];   // 多留一些，防止越界
            check = new int[_size + 65535];
            for (int i = 0; i < _size; i++)
            {
                _base[i] = _in.readInt();
                check[i] = _in.readInt();
            }
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    private bool loadBaseAndCheckByFileChannel(string path)
    {
        try
        {
            FileStream fis = new FileStream(path);
            // 1.从FileStream对象获取文件通道FileChannel
            FileChannel channel = fis.getChannel();
            int fileSize = (int) channel.Count;

            // 2.从通道读取文件内容
            ByteBuffer byteBuffer = ByteBuffer.allocate(fileSize);

            // channel.read(ByteBuffer) 方法就类似于 inputstream.read(byte)
            // 每次read都将读取 allocate 个字节到ByteBuffer
            channel.read(byteBuffer);
            // 注意先调用flip方法反转Buffer,再从Buffer读取数据
            byteBuffer.flip();
            // 有几种方式可以操作ByteBuffer
            // 可以将当前Buffer包含的字节数组全部读取出来
            byte[] bytes = byteBuffer.array();
            byteBuffer.Clear();
            // 关闭通道和文件流
            channel.Close();
            fis.Close();

            int index = 0;
            _size = ByteUtil.bytesHighFirstToInt(bytes, index);
            index += 4;
            _base = new int[size + 65535];   // 多留一些，防止越界
            check = new int[size + 65535];
            for (int i = 0; i < size; i++)
            {
                _base[i] = ByteUtil.bytesHighFirstToInt(bytes, index);
                index += 4;
                check[i] = ByteUtil.bytesHighFirstToInt(bytes, index);
                index += 4;
            }
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    /**
     * 将自己序列化到
     *
     * @param path
     * @return
     */
    public bool serializeTo(string path)
    {
        Stream Out = null;
        try
        {
            Out = (IOUtil.newOutputStream(path));
            Out.writeObject(this);
        }
        catch (Exception e)
        {
//            //e.printStackTrace();
            return false;
        }
        return true;
    }

    public static  DoubleArrayTrie<T> unSerialize<T>(string path)
    {
        Stream _in;
        try
        {
            _in = new Stream(IOAdapter == null ? new FileStream(path) : IOAdapter.open(path));
            return (DoubleArrayTrie<T>) @in.readObject();
        }
        catch (Exception e)
        {
//            //e.printStackTrace();
            return null;
        }
    }

    /**
     * 精确匹配
     *
     * @param key 键
     * @return 值
     */
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

        int b = _base[nodePos];
        int p;

        for (int i = pos; i < len; i++)
        {
            p = b + (int) (key[i]) + 1;
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

    /**
     * 精确查询
     *
     * @param keyChars 键的char数组
     * @param pos      char数组的起始位置
     * @param len      键的长度
     * @param nodePos  开始查找的位置（本参数允许从非根节点查询）
     * @return 查到的节点代表的value ID，负数表示不存在
     */
    public int exactMatchSearch(char[] keyChars, int pos, int len, int nodePos)
    {
        int result = -1;

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

    /**
     * 前缀查询
     *
     * @param key     查询字串
     * @param pos     字串的开始位置
     * @param len     字串长度
     * @param nodePos base中的开始位置
     * @return 一个含有所有下标的list
     */
    public List<int> commonPrefixSearch(string key, int pos, int len, int nodePos)
    {
        if (len <= 0)
            len = key.Length;
        if (nodePos <= 0)
            nodePos = 0;

        List<int> result = new ();

        char[] keyChars = key.ToCharArray();

        int b = _base[nodePos];
        int n;
        int p;

        for (int i = pos; i < len; i++)
        {
            p = b + (int) (keyChars[i]) + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
            if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                b = _base[p];
            else
                return result;
            p = b;
            n = _base[p];
            if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
            {
                result.Add(-n - 1);
            }
        }

        return result;
    }

    /**
     * 前缀查询，包含值
     *
     * @param key 键
     * @return 键值对列表
     * @deprecated 最好用优化版的
     */
    public List<KeyValuePair<string, V>> commonPrefixSearchWithValue(string key)
    {
        int len = key.Length;
        List<KeyValuePair<string, V>> result = new List<KeyValuePair<string, V>>();
        char[] keyChars = key.ToCharArray();
        int b = _base[0];
        int n;
        int p;

        for (int i = 0; i < len; ++i)
        {
            p = b;
            n = _base[p];
            if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
            {
                result.Add(new AbstractMap.SimpleEntry<string, V>(new string(keyChars, 0, i), v[-n - 1]));
            }

            p = b + (int) (keyChars[i]) + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
            // 下面这句可能产生下标越界，不如改为if (p < size && b == check[p])，或者多分配一些内存
            if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                b = _base[p];
            else
                return result;
        }

        p = b;
        n = _base[p];

        if (b == check[p] && n < 0)
        {
            result.Add(new AbstractMap.SimpleEntry<string, V>(key, v[-n - 1]));
        }

        return result;
    }

    /**
     * 优化的前缀查询，可以复用字符数组
     *
     * @param keyChars
     * @param begin
     * @return
     */
    public List<KeyValuePair<string, V>> commonPrefixSearchWithValue(char[] keyChars, int begin)
    {
        int len = keyChars.Length;
        List<KeyValuePair<string, V>> result = new List<KeyValuePair<string, V>>();
        int b = _base[0];
        int n;
        int p;

        for (int i = begin; i < len; ++i)
        {
            p = b;
            n = _base[p];
            if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
            {
                result.Add(new AbstractMap.SimpleEntry<string, V>(new string(keyChars, begin, i - begin), v[-n - 1]));
            }

            p = b + (int) (keyChars[i]) + 1;    // 状态转移 p = base[char[i-1]] + char[i] + 1
            // 下面这句可能产生下标越界，不如改为if (p < size && b == check[p])，或者多分配一些内存
            if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                b = _base[p];
            else
                return result;
        }

        p = b;
        n = _base[p];

        if (b == check[p] && n < 0)
        {
            result.Add(new AbstractMap.SimpleEntry<string, V>(new string(keyChars, begin, len - begin), v[-n - 1]));
        }

        return result;
    }

    //@Override
    public override string ToString()
    {
//        string infoIndex    = "i    = ";
//        string infoChar     = "char = ";
//        string infoBase     = "base = ";
//        string infoCheck    = "check= ";
//        for (int i = 0; i < base.Length; ++i)
//        {
//            if (base[i] != 0 || check[i] != 0)
//            {
//                infoChar  += "    " + (i == check[i] ? " ×" : (char)(i - check[i] - 1));
//                infoIndex += " " + string.Format("%5d", i);
//                infoBase  += " " +  string.Format("%5d", base[i]);
//                infoCheck += " " + string.Format("%5d", check[i]);
//            }
//        }
        return "DoubleArrayTrie{" +
//                "\n" + infoChar +
//                "\n" + infoIndex +
//                "\n" + infoBase +
//                "\n" + infoCheck + "\n" +
//                "check=" + String.Join(',',check) +
//                ", base=" + String.Join(',',base) +
//                ", used=" + String.Join(',',used) +
                "size=" + size +
                ", allocSize=" + allocSize +
                ", key=" + key +
                ", keySize=" + keySize +
//                ", Length=" + String.Join(',',Length) +
//                ", value=" + String.Join(',',value) +
                ", progress=" + progress +
                ", nextCheckPos=" + nextCheckPos +
                ", error_=" + error_ +
                '}';
    }

    /**
     * 树叶子节点个数
     *
     * @return
     */
    public int Count => v.Length;

    /**
     * 获取check数组引用，不要修改check
     *
     * @return
     */
    public int[] getCheck()
    {
        return check;
    }

    /**
     * 获取base数组引用，不要修改base
     *
     * @return
     */
    public int[] getBase()
    {
        return _base;
    }

    /**
     * 获取index对应的值
     *
     * @param index
     * @return
     */
    public V getValueAt(int index)
    {
        return v[index];
    }

    /**
     * 精确查询
     *
     * @param key 键
     * @return 值
     */
    public V get(string key)
    {
        int index = exactMatchSearch(key);
        if (index >= 0)
        {
            return getValueAt(index);
        }

        return default;
    }

    public V get(char[] key)
    {
        int index = exactMatchSearch(key, 0, key.Length, 0);
        if (index >= 0)
        {
            return getValueAt(index);
        }

        return default;
    }

    public V[] getValueArray(V[] a)
    {
        // I hate this but just have to
        int size = v.Length;
        if (a.Length < size)
            a = (V[]) java.lang.reflect.Array.newInstance(
                    a.getClass().getComponentType(), size);
        Array.Copy(v, 0, a, 0, size);
        return a;
    }

    public bool ContainsKey(string key)
    {
        return exactMatchSearch(key) >= 0;
    }

    /**
     * 沿着路径转移状态
     *
     * @param path
     * @return
     */
    protected int transition(string path)
    {
        return transition(path.ToCharArray());
    }

    /**
     * 沿着节点转移状态
     *
     * @param path
     * @return
     */
    protected int transition(char[] path)
    {
        int b = _base[0];
        int p;

        for (int i = 0; i < path.Length; ++i)
        {
            p = b + (int) (path[i]) + 1;
            if (b == check[p])
                b = _base[p];
            else
                return -1;
        }

        p = b;
        return p;
    }

    /**
     * 沿着路径转移状态
     *
     * @param path 路径
     * @param from 起点（根起点为base[0]=1）
     * @return 转移后的状态（双数组下标）
     */
    public int transition(string path, int from)
    {
        int b = from;
        int p;

        for (int i = 0; i < path.Length; ++i)
        {
            p = b + (int) (path[i]) + 1;
            if (b == check[p])
                b = _base[p];
            else
                return -1;
        }

        p = b;
        return p;
    }

    /**
     * 转移状态
     * @param c
     * @param from
     * @return
     */
    public int transition(char c, int from)
    {
        int b = from;
        int p;

        p = b + (int) (c) + 1;
        if (b == check[p])
            b = _base[p];
        else
            return -1;

        return b;
    }

    /**
     * 检查状态是否对应输出
     *
     * @param state 双数组下标
     * @return 对应的值，null表示不输出
     */
    public V output(int state)
    {
        if (state < 0) return default;
        int n = _base[state];
        if (state == check[state] && n < 0)
        {
            return v[-n - 1];
        }
        return default;
    }

    /**
     * 一个搜索工具（注意，当调用next()返回false后不应该继续调用next()，除非reset状态）
     */
    public class Searcher
    {
        /**
         * key的起点
         */
        public int begin;
        /**
         * key的长度
         */
        public int Length;
        /**
         * key的字典序坐标
         */
        public int index;
        /**
         * key对应的value
         */
        public V value;
        /**
         * 传入的字符数组
         */
        private char[] charArray;
        /**
         * 上一个node位置
         */
        private int last;
        /**
         * 上一个字符的下标
         */
        private int i;
        /**
         * charArray的长度，效率起见，开个变量
         */
        private int arrayLength;

        /**
         * 构造一个双数组搜索工具
         *
         * @param offset    搜索的起始位置
         * @param charArray 搜索的目标字符数组
         */
        public Searcher(int offset, char[] charArray)
        {
            this.charArray = charArray;
            i = offset;
            last = _base[0];
            arrayLength = charArray.Length;
            // A trick，如果文本长度为0的话，调用next()时，会带来越界的问题。
            // 所以我要在第一次调用next()的时候触发begin == arrayLength进而返回false。
            // 当然也可以改成begin >= arrayLength，不过我觉得操作符>=的效率低于==
            if (arrayLength == 0) begin = -1;
            else begin = offset;
        }

        /**
         * 取出下一个命中输出
         *
         * @return 是否命中，当返回false表示搜索结束，否则使用公开的成员读取命中的详细信息
         */
        public bool next()
        {
            int b = last;
            int n;
            int p;

            for (; ; ++i)
            {
                if (i == arrayLength)               // 指针到头了，将起点往前挪一个，重新开始，状态归零
                {
                    ++begin;
                    if (begin == arrayLength) break;
                    i = begin;
                    b = _base[0];
                }
                p = b + (int) (charArray[i]) + 1;   // 状态转移 p = base[char[i-1]] + char[i] + 1
                if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = _base[p];                    // 转移成功
                else
                {
                    i = begin;                      // 转移失败，也将起点往前挪一个，重新开始，状态归零
                    ++begin;
                    if (begin == arrayLength) break;
                    b = _base[0];
                    continue;
                }
                p = b;
                n = _base[p];
                if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    Length = i - begin + 1;
                    index = -n - 1;
                    value = v[index];
                    last = b;
                    ++i;
                    return true;
                }
            }

            return false;
        }
    }

    public Searcher getSearcher(string text)
    {
        return getSearcher(text, 0);
    }

    public Searcher getSearcher(string text, int offset)
    {
        return new Searcher(offset, text.ToCharArray());
    }

    public Searcher getSearcher(char[] text, int offset)
    {
        return new Searcher(offset, text);
    }

    /**
     * 一个最长搜索工具（注意，当调用next()返回false后不应该继续调用next()，除非reset状态）
     */
    public class LongestSearcher
    {
        /**
         * key的起点
         */
        public int begin;
        /**
         * key的长度
         */
        public int Length;
        /**
         * key的字典序坐标
         */
        public int index;
        /**
         * key对应的value
         */
        public V value;
        /**
         * 传入的字符数组
         */
        private char[] charArray;
        /**
         * 上一个字符的下标
         */
        private int i;
        /**
         * charArray的长度，效率起见，开个变量
         */
        private int arrayLength;

        /**
         * 构造一个双数组搜索工具
         *
         * @param offset    搜索的起始位置
         * @param charArray 搜索的目标字符数组
         */
        public LongestSearcher(int offset, char[] charArray)
        {
            this.charArray = charArray;
            i = offset;
            arrayLength = charArray.Length;
            begin = offset;
        }

        /**
         * 取出下一个命中输出
         *
         * @return 是否命中，当返回false表示搜索结束，否则使用公开的成员读取命中的详细信息
         */
        public bool next()
        {
            value = default;
            begin = i;
            int b = _base[0];
            int n;
            int p;

            for (; ; ++i)
            {
                if (i >= arrayLength)               // 指针到头了，将起点往前挪一个，重新开始，状态归零
                {
                    return value != null;
                }
                p = b + (int) (charArray[i]) + 1;   // 状态转移 p = base[char[i-1]] + char[i] + 1
                if (b == check[p])                  // base[char[i-1]] == check[base[char[i-1]] + char[i] + 1]
                    b = _base[p];                    // 转移成功
                else
                {
                    if (begin == arrayLength) break;
                    if (value != null)
                    {
                        i = begin + Length;         // 输出最长词后，从该词语的下一个位置恢复扫描
                        return true;
                    }

                    i = begin;                      // 转移失败，也将起点往前挪一个，重新开始，状态归零
                    ++begin;
                    b = _base[0];
                }
                p = b;
                n = _base[p];
                if (b == check[p] && n < 0)         // base[p] == check[p] && base[p] < 0 查到一个词
                {
                    Length = i - begin + 1;
                    index = -n - 1;
                    value = v[index];
                }
            }

            return false;
        }
    }

    /**
     * 全切分
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void parseText(string text, AhoCorasickDoubleArrayTrie<V>.IHit<V> processor)
    {
        Searcher searcher = getSearcher(text, 0);
        while (searcher.next())
        {
            processor.Hit(searcher.begin, searcher.begin + searcher.Length, searcher.value);
        }
    }

    public LongestSearcher getLongestSearcher(string text, int offset)
    {
        return getLongestSearcher(text.ToCharArray(), offset);
    }

    public LongestSearcher getLongestSearcher(char[] text, int offset)
    {
        return new LongestSearcher(offset, text);
    }

    /**
     * 最长匹配
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void parseLongestText(string text, AhoCorasickDoubleArrayTrie<V>.IHit<V> processor)
    {
        LongestSearcher searcher = getLongestSearcher(text, 0);
        while (searcher.next())
        {
            processor.Hit(searcher.begin, searcher.begin + searcher.Length, searcher.value);
        }
    }

    /**
     * 转移状态
     *
     * @param current
     * @param c
     * @return
     */
    protected int transition(int current, char c)
    {
        int b = _base[current];
        int p;

        p = b + c + 1;
        if (b == check[p])
            b = _base[p];
        else
            return -1;

        p = b;
        return p;
    }

    /**
     * 更新某个键对应的值
     *
     * @param key   键
     * @param value 值
     * @return 是否成功（失败的原因是没有这个键）
     */
    public bool set(string key, V value)
    {
        int index = exactMatchSearch(key);
        if (index >= 0)
        {
            v[index] = value;
            return true;
        }

        return false;
    }

    /**
     * 从值数组中提取下标为index的值<br>
     * 注意为了效率，此处不进行参数校验
     *
     * @param index 下标
     * @return 值
     */
    public V get(int index)
    {
        return v[index];
    }

    /**
     * 释放空闲的内存
     */
    private void shrink()
    {
//        if (HanLP.Config.DEBUG)
//        {
//            Console.Error.WriteLine("释放内存 %d bytes\n", base.Length - size - 65535);
//        }
        int[] nbase = new int[_size + 65535];
        Array.Copy(_base, 0, nbase, 0, size);
        _base = nbase;

        int[] ncheck = new int[_size + 65535];
        Array.Copy(check, 0, ncheck, 0, size);
        check = ncheck;
    }


    /**
     * 打印统计信息
     */
//    public void report()
//    {
//        Console.WriteLine("size: " + size);
//        int nonZeroIndex = 0;
//        for (int i = 0; i < base.Length; i++)
//        {
//            if (base[i] != 0) nonZeroIndex = i;
//        }
//        Console.WriteLine("BaseUsed: " + nonZeroIndex);
//        nonZeroIndex = 0;
//        for (int i = 0; i < check.Length; i++)
//        {
//            if (check[i] != 0) nonZeroIndex = i;
//        }
//        Console.WriteLine("CheckUsed: " + nonZeroIndex);
//    }
}