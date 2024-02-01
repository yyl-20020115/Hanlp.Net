using com.hankcs.hanlp.classification.tokenizers;
using com.hankcs.hanlp.collection.MDAG;
using com.hankcs.hanlp.corpus.io;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace com.hankcs.hanlp.collection.trie.datrie;




/**
 * 可变双数组trie树，重构自：https://github.com/fancyerii/DoubleArrayTrie
 */
public class MutableDoubleArrayTrieInteger : Serializable, IEnumerable<KeyValuePair<string, int>>, ICacheAble
{
    private static readonly long serialVersionUID = 5586394930559218802L;
    /**
     * 0x40000000
     */
    private static readonly int LEAF_BIT = 1073741824;
    private static readonly int[] EMPTY_WALK_STATE = { -1, -1 };
    CharacterMapping charMap;
    /**
     * 字符串的终止字符（会在传入的字符串末尾添加该字符）
     */
    private static readonly char UNUSED_CHAR = '\0';
    /**
     * 终止字符的codePoint，这个字符作为叶节点的标识
     */
    private static readonly int UNUSED_CHAR_VALUE = UNUSED_CHAR;
    private IntArrayList check;
    private IntArrayList _base;
    /**
     * 键值对数量
     */
    private int _size;

    public MutableDoubleArrayTrieInteger(Dictionary<string, int> stringIntegerMap)
        : this(stringIntegerMap.ToHashSet())
    {
        ;
    }

    public MutableDoubleArrayTrieInteger(HashSet<KeyValuePair<string, int>> entrySet)
        : this()
    {
        foreach (KeyValuePair<string, int> entry in entrySet)
        {
            Add(entry.Key, entry.Value);
        }
    }

    /**
     * 激活指数膨胀
     *
     * @param exponentialExpanding
     */
    public override void setExponentialExpanding(bool exponentialExpanding)
    {
        check.setExponentialExpanding(exponentialExpanding);
        base.setExponentialExpanding(exponentialExpanding);
    }

    /**
     * 指数膨胀的底数
     *
     * @param exponentialExpandFactor
     */
    public override void setExponentialExpandFactor(double exponentialExpandFactor)
    {
        check.setExponentialExpandFactor(exponentialExpandFactor);
        base.setExponentialExpandFactor(exponentialExpandFactor);
    }

    /**
     * 设置线性膨胀
     *
     * @param linearExpandFactor
     */
    public override void setLinearExpandFactor(int linearExpandFactor)
    {
        check.setLinearExpandFactor(linearExpandFactor);
        base.setLinearExpandFactor(linearExpandFactor);
    }

    public MutableDoubleArrayTrieInteger()
        : this(new Utf8CharacterMapping())
    {
        ;
    }

    public MutableDoubleArrayTrieInteger(CharacterMapping charMap)
    {
        this.charMap = charMap;
        Clear();
    }

    public void Clear()
    {
        this._base = new IntArrayList(this.charMap.getInitSize());
        this.check = new IntArrayList(this.charMap.getInitSize());

        this._base.Append(0);
        this.check.Append(0);

        this._base.Append(1);
        this.check.Append(0);
        expandArray(this.charMap.getInitSize());
    }

    public int getCheckArraySize()
    {
        return check.Count;
    }

    public int getFreeSize()
    {
        int count = 0;
        int chk = this.check.get(0);
        while (chk != 0)
        {
            count++;
            chk = this.check.get(-chk);
        }

        return count;
    }

    private bool isLeafValue(int value)
    {
        return (value > 0) && ((value & LEAF_BIT) != 0);
    }

    /**
     * 最高4位置1
     *
     * @param value
     * @return
     */
    private int setLeafValue(int value)
    {
        return value | LEAF_BIT;
    }

    /**
     * 最高4位置0
     *
     * @param value
     * @return
     */
    private int getLeafValue(int value)
    {
        return value ^ LEAF_BIT;
    }

    public int getBaseArraySize()
    {
        return this._base.Count;
    }

    private int getBase(int index)
    {
        return this._base.get(index);
    }

    private int getCheck(int index)
    {
        return this.check.get(index);
    }

    private void setBase(int index, int value)
    {
        this._base.set(index, value);
    }

    private void setCheck(int index, int value)
    {
        this.check.set(index, value);
    }

    protected bool isEmpty(int index)
    {
        return getCheck(index) <= 0;
    }

    private int getNextFreeBase(int nextChar)
    {
        int index = -getCheck(0);
        while (index != 0)
        {
            if (index > nextChar + 1) // 因为ROOT的index从1开始，所以至少要大于1
            {
                return index - nextChar;
            }
            index = -getCheck(index);
        }
        int oldSize = getBaseArraySize();
        expandArray(oldSize + this._base.getLinearExpandFactor());
        return oldSize;
    }

    private void addFreeLink(int index)
    {
        this.check.set(index, this.check.get(-this._base.get(0)));
        this.check.set(-this._base.get(0), -index);
        this._base.set(index, this._base.get(0));
        this._base.set(0, -index);
    }

    /**
     * 将index从空闲循环链表中删除
     *
     * @param index
     */
    private void deleteFreeLink(int index)
    {
        this._base.set(-this.check.get(index), this._base.get(index));
        this.check.set(-this._base.get(index), this.check.get(index));
    }

    /**
     * 动态数组扩容
     *
     * @param maxSize 需要的容量
     */
    private void expandArray(int maxSize)
    {
        int curSize = getBaseArraySize();
        if (curSize > maxSize)
        {
            return;
        }
        if (maxSize >= LEAF_BIT)
        {
            throw new InvalidOperationException("Double Array Trie size exceeds absolute threshold");
        }
        for (int i = curSize; i <= maxSize; ++i)
        {
            this._base.Append(0);
            this.check.Append(0);
            addFreeLink(i);
        }
    }

    /**
     * 插入条目
     *
     * @param key       键
     * @param value     值
     * @param overwrite 是否覆盖
     * @return
     */
    public bool insert(string key, int value, bool overwrite)
    {
        if ((null == key) || key.Length == 0 || (key.IndexOf(UNUSED_CHAR) != -1))
        {
            return false;
        }
        if ((value < 0) || ((value & LEAF_BIT) != 0))
        {
            return false;
        }

        value = setLeafValue(value);

        int[] ids = this.charMap.toIdList(key + UNUSED_CHAR);

        int fromState = 1; // 根节点的index为1
        int toState = 1;
        int index = 0;
        while (index < ids.Length)
        {
            int c = ids[index];
            toState = getBase(fromState) + c; // to = base[from] + c
            expandArray(toState);

            if (isEmpty(toState))
            {
                deleteFreeLink(toState);

                setCheck(toState, fromState); // check[to] = from
                if (index == ids.Length - 1)  // Leaf
                {
                    ++this._size;
                    setBase(toState, value);  // base[to] = value
                }
                else
                {
                    int nextChar = ids[(index + 1)];
                    setBase(toState, getNextFreeBase(nextChar)); // base[to] = free_state - c
                }
            }
            else if (getCheck(toState) != fromState) // 冲突
            {
                solveConflict(fromState, c);
                continue;
            }
            fromState = toState;
            ++index;
        }
        if (overwrite)
        {
            setBase(toState, value);
        }
        return true;
    }

    /**
     * 寻找可以放下子节点集合的“连续”空闲区间
     *
     * @param children 子节点集合
     * @return base值
     */
    private int searchFreeBase(SortedSet<int> children)
    {
        int minChild = children.first();
        int maxChild = children.last();
        int current = 0;
        while (getCheck(current) != 0) // 循环链表回到了头，说明没有符合要求的“连续”区间
        {
            if (current > minChild + 1)
            {
                int _base = current - minChild;
                bool ok = true;
                for (IEnumerator<int> it = children.GetEnumerator(); it.MoveNext();) // 检查是否每个子节点的位置都空闲（“连续”区间）
                {
                    int to = _base + it.next();
                    if (to >= getBaseArraySize())
                    {
                        ok = false;
                        break;
                    }
                    if (!isEmpty(to))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    return _base;
                }
            }
            current = -getCheck(current); // 从链表中取出下一个空闲位置
        }
        int oldSize = getBaseArraySize(); // 没有足够长的“连续”空闲区间，所以在双数组尾部额外分配一块
        expandArray(oldSize + maxChild);
        return oldSize;
    }

    /**
     * 解决冲突
     *
     * @param parent   父节点
     * @param newChild 子节点的char值
     */
    private void solveConflict(int parent, int newChild)
    {
        // 找出parent的所有子节点
        var children = new SortedSet<int>();
        children.Add(newChild);
        int charsetSize = this.charMap.getCharsetSize();
        for (int c = 0; c < charsetSize; ++c)
        {
            int next = getBase(parent) + c;
            if (next >= getBaseArraySize())
            {
                break;
            }
            if (getCheck(next) == parent)
            {
                children.Add(c);
            }
        }
        // 移动旧子节点到新的位置
        int newBase = searchFreeBase(children);
        children.Remove(newChild);
        foreach (int c in children)
        {
            int child = newBase + c;
            deleteFreeLink(child);

            setCheck(child, parent);
            int childBase = getBase(getBase(parent) + c);
            setBase(child, childBase);

            if (!isLeafValue(childBase))
            {
                for (int d = 0; d < charsetSize; ++d)
                {
                    int to = childBase + d;
                    if (to >= getBaseArraySize())
                    {
                        break;
                    }
                    if (getCheck(to) == getBase(parent) + c)
                    {
                        setCheck(to, child);
                    }
                }
            }
            addFreeLink(getBase(parent) + c);
        }
        // 更新新base值
        setBase(parent, newBase);
    }

    /**
     * 键值对个数
     *
     * @return
     */
    public int Count => this._size;

    public bool isEmpty => _size == 0;

    /**
     * 覆盖模式添加
     *
     * @param key
     * @param value
     * @return
     */
    public bool insert(string key, int value)
    {
        return insert(key, value, true);
    }

    /**
     * 非覆盖模式添加
     *
     * @param key
     * @param value
     * @return
     */
    public bool Add(string key, int value)
    {
        return insert(key, value, false);
    }

    /**
     * 非覆盖模式添加，值默认为当前集合大小
     *
     * @param key
     * @return
     */
    public bool Add(string key)
    {
        return Add(key, _size);
    }

    /**
     * 查询以prefix开头的所有键
     *
     * @param prefix
     * @return
     */
    public List<string> prefixMatch(string prefix)
    {
        int curState = 1;
        IntArrayList bytes = new IntArrayList(prefix.Length * 4);
        for (int i = 0; i < prefix.Length; i++)
        {
            int codePoint = prefix[i];
            if (curState < 1)
            {
                return new();
            }
            if ((curState != 1) && (isEmpty(curState)))
            {
                return new();
            }
            int[] ids = this.charMap.toIdList(codePoint);
            if (ids.Length == 0)
            {
                return new();
            }
            for (int j = 0; j < ids.Length; j++)
            {
                int c = ids[j];
                if ((getBase(curState) + c < getBaseArraySize())
                    && (getCheck(getBase(curState) + c) == curState))
                {
                    bytes.Append(c);
                    curState = getBase(curState) + c;
                }
                else
                {
                    return new();
                }
            }

        }
        List<string> result = new();
        recursiveAddSubTree(curState, result, bytes);

        return result;
    }

    private void recursiveAddSubTree(int curState, List<string> result, IntArrayList bytes)
    {
        if (getCheck(getBase(curState) + UNUSED_CHAR_VALUE) == curState)
        {
            byte[] array = new byte[bytes.Count];
            for (int i = 0; i < bytes.Count; i++)
            {
                array[i] = (byte)bytes[i];
            }
            result.Add(array, Utf8CharacterMapping.UTF_8);
        }
        int _base = getBase(curState);
        for (int c = 0; c < charMap.getCharsetSize(); c++)
        {
            if (c == UNUSED_CHAR_VALUE) continue;
            int check = getCheck(_base + c);
            if (_base + c < getBaseArraySize() && check == curState)
            {
                bytes.Append(c);
                recursiveAddSubTree(_base + c, result, bytes);
                bytes.removeLast();
            }
        }
    }

    /**
     * 最长查询
     *
     * @param query
     * @param start
     * @return (最长长度，对应的值)
     */
    public int[] findLongest(string query, int start)
    {
        if ((query == null) || (start >= query.Length))
        {
            return new int[] { 0, -1 };
        }
        int state = 1;
        int maxLength = 0;
        int lastVal = -1;
        for (int i = start; i < query.Length; i++)
        {
            int[] res = transferValues(state, query[i]);
            if (res[0] == -1)
            {
                break;
            }
            state = res[0];
            if (res[1] != -1)
            {
                maxLength = i - start + 1;
                lastVal = res[1];
            }
        }
        return new int[] { maxLength, lastVal };
    }

    public int[] findWithSupplementary(string query, int start)
    {
        if ((query == null) || (start >= query.Length))
        {
            return new int[] { 0, -1 };
        }
        int curState = 1;
        int maxLength = 0;
        int lastVal = -1;
        int charCount = 1;
        for (int i = start; i < query.Length; i += charCount)
        {
            int codePoint = query.codePointAt(i);
            charCount = char.charCount(codePoint);
            int[] res = transferValues(curState, codePoint);
            if (res[0] == -1)
            {
                break;
            }
            curState = res[0];
            if (res[1] != -1)
            {
                maxLength = i - start + 1;
                lastVal = res[1];
            }
        }
        return new int[] { maxLength, lastVal };

    }

    public List<int[]> findAllWithSupplementary(string query, int start)
    {
        List<int[]> ret = new (5);
        if ((query == null) || (start >= query.Length))
        {
            return ret;
        }
        int curState = 1;
        int charCount = 1;
        for (int i = start; i < query.Length; i += charCount)
        {
            int codePoint = query.codePointAt(i);
            charCount = char.charCount(codePoint);
            int[] res = transferValues(curState, codePoint);
            if (res[0] == -1)
            {
                break;
            }
            curState = res[0];
            if (res[1] != -1)
            {
                ret.Add(new int[] { i - start + 1, res[1] });
            }
        }
        return ret;
    }

    /**
     * 查询与query的前缀重合的所有词语
     *
     * @param query
     * @param start
     * @return
     */
    public List<int[]> commonPrefixSearch(string query, int start)
    {
        List<int[]> ret = new (5);
        if ((query == null) || (start >= query.Length))
        {
            return ret;
        }
        int curState = 1;
        for (int i = start; i < query.Length; i++)
        {
            int[] res = transferValues(curState, query[i]);
            if (res[0] == -1)
            {
                break;
            }
            curState = res[0];
            if (res[1] != -1)
            {
                ret.Add(new int[] { i - start + 1, res[1] });
            }
        }
        return ret;
    }

    /**
     * 转移状态并输出值
     *
     * @param state
     * @param codePoint char
     * @return
     */
    public int[] transferValues(int state, int codePoint)
    {
        if (state < 1)
        {
            return EMPTY_WALK_STATE;
        }
        if ((state != 1) && (isEmpty(state)))
        {
            return EMPTY_WALK_STATE;
        }
        int[] ids = this.charMap.toIdList(codePoint);
        if (ids.Length == 0)
        {
            return EMPTY_WALK_STATE;
        }
        for (int i = 0; i < ids.Length; i++)
        {
            int c = ids[i];
            if ((getBase(state) + c < getBaseArraySize())
                && (getCheck(getBase(state) + c) == state))
            {
                state = getBase(state) + c;
            }
            else
            {
                return EMPTY_WALK_STATE;
            }
        }
        if (getCheck(getBase(state) + UNUSED_CHAR_VALUE) == state)
        {
            int value = getLeafValue(getBase(getBase(state)
                                                 + UNUSED_CHAR_VALUE));
            return new int[] { state, value };
        }
        return new int[] { state, -1 };
    }

    /**
     * 转移状态
     *
     * @param state
     * @param codePoint
     * @return
     */
    public int transfer(int state, int codePoint)
    {
        if (state < 1)
        {
            return -1;
        }
        if ((state != 1) && (isEmpty(state)))
        {
            return -1;
        }
        int[] ids = this.charMap.toIdList(codePoint);
        if (ids.Length == 0)
        {
            return -1;
        }
        return transfer(state, ids);
    }

    /**
     * 转移状态
     *
     * @param state
     * @param ids
     * @return
     */
    private int transfer(int state, int[] ids)
    {
        foreach (int c in ids)
        {
            if ((getBase(state) + c < getBaseArraySize())
                && (getCheck(getBase(state) + c) == state))
            {
                state = getBase(state) + c;
            }
            else
            {
                return -1;
            }
        }
        return state;
    }

    public int stateValue(int state)
    {
        int leaf = getBase(state) + UNUSED_CHAR_VALUE;
        if (getCheck(leaf) == state)
        {
            return getLeafValue(getBase(leaf));
        }
        return -1;
    }

    /**
     * 去掉多余的buffer
     */
    public void loseWeight()
    {
        base.loseWeight();
        check.loseWeight();
    }

    /**
     * 将值大于等于from的统一递减1<br>
     *
     * @param from
     */
    public void decreaseValues(int from)
    {
        for (int state = 1; state < getBaseArraySize(); ++state)
        {
            int leaf = getBase(state) + UNUSED_CHAR_VALUE;
            if (1 < leaf && leaf < getCheckArraySize() && getCheck(leaf) == state)
            {
                int value = getLeafValue(getBase(leaf));
                if (value >= from)
                {
                    setBase(leaf, setLeafValue(--value));
                }
            }
        }
    }

    /**
     * 精确查询
     *
     * @param key
     * @param start
     * @return -1表示不存在
     */
    public int get(string key, int start)
    {
        //assert key != null;
        //assert 0 <= start && start <= key.Length;
        int state = 1;
        int[] ids = charMap.toIdList(key.Substring(start));
        state = transfer(state, ids);
        if (state < 0)
        {
            return -1;
        }
        return stateValue(state);
    }

    /**
     * 精确查询
     *
     * @param key
     * @return -1表示不存在
     */
    public int get(string key)
    {
        return get(key, 0);
    }

    /**
     * 设置键值 （同Add）
     *
     * @param key
     * @param value
     * @return 是否设置成功（失败的原因是键值不合法）
     */
    public bool set(string key, int value)
    {
        return insert(key, value, true);
    }

    /**
     * 设置键值 （同set）
     *
     * @param key
     * @param value
     * @return 是否设置成功（失败的原因是键值不合法）
     */
    public bool add(string key, int value)
    {
        return insert(key, value, true);
    }

    /**
     * 删除键
     *
     * @param key
     * @return 值
     */
    public int Remove(string key)
    {
        return delete(key);
    }

    /**
     * 删除键
     *
     * @param key
     * @return 值
     */
    public int delete(string key)
    {
        if (key == null)
        {
            return -1;
        }
        int curState = 1;
        int[] ids = this.charMap.toIdList(key);

        int[] path = new int[ids.Length + 1];
        int i = 0;
        for (; i < ids.Length; i++)
        {
            int c = ids[i];
            if ((getBase(curState) + c >= getBaseArraySize())
                || (getCheck(getBase(curState) + c) != curState))
            {
                break;
            }
            curState = getBase(curState) + c;
            path[i] = curState;
        }
        int ret = -1;
        if (i == ids.Length)
        {
            if (getCheck(getBase(curState) + UNUSED_CHAR_VALUE) == curState)
            {
                --this._size;
                ret = getLeafValue(getBase(getBase(curState) + UNUSED_CHAR_VALUE));
                path[(path.Length - 1)] = (getBase(curState) + UNUSED_CHAR_VALUE);
                for (int j = path.Length - 1; j >= 0; --j)
                {
                    bool isLeaf = true;
                    int state = path[j];
                    for (int k = 0; k < this.charMap.getCharsetSize(); k++)
                    {
                        if (isLeafValue(getBase(state)))
                        {
                            break;
                        }
                        if ((getBase(state) + k < getBaseArraySize())
                            && (getCheck(getBase(state) + k) == state))
                        {
                            isLeaf = false;
                            break;
                        }
                    }
                    if (!isLeaf)
                    {
                        break;
                    }
                    addFreeLink(state);
                }
            }
        }
        return ret;
    }

    /**
     * 获取空闲的数组元素个数
     *
     * @return
     */
    public int getEmptySize()
    {
        int size = 0;
        for (int i = 0; i < getBaseArraySize(); i++)
        {
            if (isEmpty(i))
            {
                ++size;
            }
        }
        return size;
    }

    /**
     * 可以设置的最大值
     *
     * @return
     */
    public int getMaximumValue()
    {
        return LEAF_BIT - 1;
    }

    public HashSet<KeyValuePair<string, int>> entrySet()
    {
        return new ST();
    }
    public class ST : HashSet<KeyValuePair<string, int>>
    {
        //@Override
        public int Count() => MutableDoubleArrayTrieInteger.Count;

        //@Override
        public bool isEmpty()
        {
            return MutableDoubleArrayTrieInteger.isEmpty;
        }

        //@Override
        public bool Contains(Object o)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return new SI();
        }
        public class SI : IEnumerator<KeyValuePair<string, int>>
        {
            KeyValuePair iterator = MutableDoubleArrayTrieInteger.GetEnumerator();

            //@Override
            public bool MoveNext()
            {
                return iterator.MoveNext();
            }

            //@Override
            public void Remove()
            {
                throw new InvalidOperationException();
            }

            //@Override
            public KeyValuePair<string, int> next()
            {
                iterator.next();
                return new AbstractMap.SimpleEntry<string, int>(iterator.Key, iterator.Value);
            }
        }
        //@Override
        public Object[] ToArray()
        {
            var entries = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<string, int> entry in this)
            {
                entries.Add(entry);
            }
            return entries.ToArray();
        }

        //@Override
        public T[] ToArray<T>(T[] a)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public bool Add(KeyValuePair<string, int> stringIntegerEntry)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public bool Remove(Object o)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public bool containsAll(ICollection c)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public bool AddRange(ICollection<KeyValuePair<string, int>> c)
        {
            throw new InvalidOperationException();
        }

        //@Override
        public bool retainAll(ICollection c)
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
            throw new InvalidOperationException();
        }
    }


    //@Override
    public KeyValuePair GetEnumerator()
    {
        return new KeyValuePair();
    }

    public bool ContainsKey(string key)
    {
        return get(key) != -1;
    }

    public HashSet<string> Keys()
    {
        return new HS();
    }

    public class HS : HashSet<string>
    {
        //@Override
        public int Count() => MutableDoubleArrayTrieInteger.Count();

        //@Override
        public bool isEmpty()
        {
            return MutableDoubleArrayTrieInteger.isEmpty;
        }

        //@Override
        public bool Contains(Object o)
        {
            return MutableDoubleArrayTrieInteger.ContainsKey((string)o);
        }

        //@Override
        public IEnumerator<string> GetEnumerator()
        {
            return new SX();
        }

        public class SX : IEnumerator<string>
        {
            KeyValuePair iterator = MutableDoubleArrayTrieInteger.GetEnumerator();

            //@Override
            public void Remove()
            {
                throw new InvalidOperationException();
            }

            //@Override
            public bool MoveNext()
            {
                return iterator.MoveNext();
            }

            //@Override
            public string next()
            {
                return iterator.next().Key();
            }
            //@Override
            public Object[] ToArray()
            {
                throw new InvalidOperationException();
            }

            //@Override
            public T[] ToArray<T>(T[] a)
            {
                throw new InvalidOperationException();
            }

            //@Override
            public bool Add(string s)
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
            public bool AddRange(Collection<string> c)
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
                throw new InvalidOperationException();
            }
        }
    }


    //@Override
    public void save(Stream _out)
    {
        if (!(charMap is Utf8CharacterMapping))
        {
            logger.warning("将来需要在构造的时候传入 " + charMap.getClass());
        }
        _out.writeInt(size);
        _base.save(_out);
        check.save(_out);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        _size = byteArray.Next();
        if (!base.load(byteArray)) return false;
        if (!check.load(byteArray)) return false;
        return true;
    }

    private void writeObject(Stream _out)
    {
        _out.Write(_size);
        _out.Write(_base);
        _out.Write(check);
    }

    private void readObject(Stream _in)
    {
        _size = _in.readInt();
        _base = (IntArrayList)_in.readObject();
        check = (IntArrayList)_in.readObject();
        charMap = new Utf8CharacterMapping();
    }

    IEnumerator<KeyValuePair<string, int>> IEnumerable<KeyValuePair<string, int>>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    //    /**
    //     * 遍历时无法删除
    //     *
    //     * @return
    //     */
    //    public DATIterator GetEnumerator()
    //    {
    //        return new KeyValuePair();
    //    }

    public class KeyValuePair : IEnumerable<KeyValuePair>
    {
        /**
         * 储存(index, charPoint)
         */
        private IntArrayList path;
        /**
         * 当前所处的键值的索引
         */
        private int index;
        private int value = -1;
        private string key = null;
        private int currentBase;

        public KeyValuePair()
        {
            path = new IntArrayList(20);
            path.Append(1); // ROOT
            int from = 1;
            int b = base.get(from);
            if (_size > 0)
            {
                while (true)
                {
                    for (int i = 0; i < charMap.getCharsetSize(); i++)
                    {
                        int c = check.get(b + i);
                        if (c == from)
                        {
                            path.Append(i);
                            from = b + i;
                            path.Append(from);
                            b = base.get(from);
                            i = 0;
                            if (getCheck(b + UNUSED_CHAR_VALUE) == from)
                            {
                                value = getLeafValue(getBase(b + UNUSED_CHAR_VALUE));
                                int[] ids = new int[path.Count / 2];
                                for (int k = 0, j = 1; j < path.Count; k++, j += 2)
                                {
                                    ids[k] = path.get(j);
                                }
                                key = charMap.ToString(ids);
                                path.Append(UNUSED_CHAR_VALUE);
                                currentBase = b;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public string Key()
        {
            return key;
        }

        public int Value()
        {
            return value;
        }

        public int setValue(int v)
        {
            int value = getLeafValue(v);
            setBase(currentBase + UNUSED_CHAR_VALUE, value);
            this.value = v;
            return v;
        }

        //@Override
        public bool MoveNext()
        {
            return index < _size;
        }

        //@Override
        public KeyValuePair next()
        {
            if (index >= _size)
            {
                throw new NoSuchElementException();
            }
            else if (index == 0)
            {
            }
            else
            {
                while (path.Count > 0)
                {
                    int charPoint = path.pop();
                    int _base = path.getLast();
                    int n = getNext(_base, charPoint);
                    if (n != -1) break;
                    path.removeLast();
                }
            }

            ++index;
            return this;
        }

        //@Override
        public void Remove()
        {
            throw new InvalidOperationException();
        }

        /**
         * 遍历下一个终止路径
         *
         * @param parent    父节点
         * @param charPoint 子节点的char
         * @return
         */
        private int getNext(int parent, int charPoint)
        {
            int startChar = charPoint + 1;
            int baseParent = getBase(parent);
            int from = parent;

            for (int i = startChar; i < charMap.getCharsetSize(); i++)
            {
                int to = baseParent + i;
                if (check.Count > to && check.get(to) == from)
                {
                    path.Append(i);
                    from = to;
                    path.Append(from);
                    baseParent = base.get(from);
                    if (getCheck(baseParent + UNUSED_CHAR_VALUE) == from)
                    {
                        value = getLeafValue(getBase(baseParent + UNUSED_CHAR_VALUE));
                        int[] ids = new int[path.Count / 2];
                        for (int k = 0, j = 1; j < path.Count; ++k, j += 2)
                        {
                            ids[k] = path.get(j);
                        }
                        key = charMap.ToString(ids);
                        path.Append(UNUSED_CHAR_VALUE);
                        currentBase = baseParent;
                        return from;
                    }
                    else
                    {
                        return getNext(from, 0);
                    }
                }
            }
            return -1;
        }

        //@Override
        public override string ToString()
        {
            return key + "=" + value;
        }
    }

}
