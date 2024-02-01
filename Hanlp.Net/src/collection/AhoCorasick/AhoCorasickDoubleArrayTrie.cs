/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/22 21:13</create-date>
 *
 * <copyright file="AhoCorasickDoubleArrayTrie.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.MDAG;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.AhoCorasick;




/**
 * 基于双数组Trie树的AhoCorasick自动机
 *
 * @author hankcs
 */
public class AhoCorasickDoubleArrayTrie<V>
{
    /**
     * 双数组值check
     */
    protected int[] _check;
    /**
     * 双数组之base
     */
    protected int[] _base;
    /**
     * fail表
     */
    int[] fail;
    /**
     * 输出表
     */
    int[][] output;
    /**
     * 保存value
     */
    protected V[] v;

    /**
     * 每个key的长度
     */
    protected int[] l;

    /**
     * base 和 check 的大小
     */
    protected int _size;

    public AhoCorasickDoubleArrayTrie()
    {
    }

    /**
     * 由一个词典创建
     *
     * @param dictionary 词典
     */
    public AhoCorasickDoubleArrayTrie(Dictionary<string, V> dictionary)
    {
        Build(dictionary);
    }

    /**
     * 匹配母文本
     *
     * @param text 一些文本
     * @return 一个pair列表
     */
    public List<Hit<V>> ParseText(string text)
    {
        int position = 1;
        int currentState = 0;
        List<Hit<V>> collectedEmits = new ();
        for (int i = 0; i < text.Length; ++i)
        {
            currentState = GetState(currentState, text[i]);
            StoreEmits(position, currentState, collectedEmits);
            ++position;
        }

        return collectedEmits;
    }

    /**
     * 处理文本
     *
     * @param text      文本
     * @param processor 处理器
     */
    public void ParseText(string text, IHit<V> processor)
    {
        int position = 1;
        int currentState = 0;
        for (int i = 0; i < text.Length; ++i)
        {
            currentState = GetState(currentState, text[(i)]);
            int[] hitArray = output[currentState];
            if (hitArray != null)
            {
                foreach (int hit in hitArray)
                {
                    processor.Hit(position - l[hit], position, v[hit]);
                }
            }
            ++position;
        }
    }

    /**
     * 处理文本
     *
     * @param text
     * @param processor
     */
    public void ParseText(char[] text, IHit<V> processor)
    {
        int position = 1;
        int currentState = 0;
        foreach (char c in text)
        {
            currentState = GetState(currentState, c);
            int[] hitArray = output[currentState];
            if (hitArray != null)
            {
                foreach (int hit in hitArray)
                {
                    processor.Hit(position - l[hit], position, v[hit]);
                }
            }
            ++position;
        }
    }

    /**
     * 处理文本
     *
     * @param text
     * @param processor
     */
    public void ParseText(char[] text, IHitFull<V> processor)
    {
        int position = 1;
        int currentState = 0;
        foreach (char c in text)
        {
            currentState = GetState(currentState, c);
            int[] hitArray = output[currentState];
            if (hitArray != null)
            {
                foreach (int hit in hitArray)
                {
                    processor.Hit(position - l[hit], position, v[hit], hit);
                }
            }
            ++position;
        }
    }

    /**
     * 持久化
     *
     * @param _out 一个Stream
     * @throws Exception 可能的IO异常等
     */
    public void Save(Stream _out)
    {
        _out.writeInt(        Size);
        for (int i = 0; i <_size; i++)
        {
            _out.writeInt(_base[i]);
            _out.writeInt(check[i]);
            _out.writeInt(fail[i]);
            int[] output = this.output[i];
            if (output == null)
            {
                _out.writeInt(0);
            }
            else
            {
                _out.writeInt(output.Length);
                foreach (int o in output)
                {
                    _out.writeInt(o);
                }
            }
        }
        _out.writeInt(l.Length);
        foreach (int Length in l)
        {
            _out.writeInt(Length);
        }
    }

    /**
     * 持久化
     *
     * @param _out 一个Stream
     * @ 可能的IO异常
     */
    public void Save(Stream _out)
    {
        _out.writeObject(_base);
        _out.writeObject(_check);
        _out.writeObject(fail);
        _out.writeObject(output);
        _out.writeObject(l);
    }

    /**
     * 载入
     *
     * @param in    一个Stream
     * @param value 值（持久化的时候并没有持久化值，现在需要额外提供）
     * @
     * @throws ClassNotFoundException
     */
    public void Load(Stream _in, V[] value)
    {
        _base = (int[]) _in.readObject();
        _check = (int[]) _in.readObject();
        fail = (int[]) _in.readObject();
        output = (int[][]) _in.readObject();
        l = (int[]) _in.readObject();
        v = value;
    }

    /**
     * 载入
     *
     * @param byteArray 一个字节数组
     * @param value     值数组
     * @return 成功与否
     */
    public bool Load(ByteArray byteArray, V[] value)
    {
        if (byteArray == null) return false;
        _size = byteArray.Next();
        _base = new int[_size + 65535];   // 多留一些，防止越界
        _check = new int[_size + 65535];
        fail = new int[_size + 65535];
        output = new int[_size + 65535][];
        int Length;
        for (int i = 0; i < _size; ++i)
        {
            _base[i] = byteArray.Next();
            _check[i] = byteArray.Next();
            fail[i] = byteArray.Next();
            Length = byteArray.Next();
            if (Length == 0) continue;
            output[i] = new int[Length];
            for (int j = 0; j < output[i].Length; ++j)
            {
                output[i][j] = byteArray.Next();
            }
        }
        Length = byteArray.Next();
        l = new int[Length];
        for (int i = 0; i < l.Length; ++i)
        {
            l[i] = byteArray.Next();
        }
        v = value;
        return true;
    }

    /**
     * 获取值
     *
     * @param key 键
     * @return
     */
    public V Get(string key)
    {
        int index = ExactMatchSearch(key);
        if (index >= 0)
        {
            return v[index];
        }

        return default;
    }

    /**
     * 更新某个键对应的值
     *
     * @param key   键
     * @param value 值
     * @return 是否成功（失败的原因是没有这个键）
     */
    public bool Set(string key, V value)
    {
        int index = ExactMatchSearch(key);
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
    public V Get(int index)
    {
        return v[index];
    }

    /**
     * 命中一个模式串的处理方法
     */
    public interface IHit<V>
    {
        /**
         * 命中一个模式串
         *
         * @param begin 模式串在母文本中的起始位置
         * @param end   模式串在母文本中的终止位置
         * @param value 模式串对应的值
         */
        void Hit(int begin, int end, V value);
    }

    public interface IHitFull<V>
    {
        /**
         * 命中一个模式串
         *
         * @param begin 模式串在母文本中的起始位置
         * @param end   模式串在母文本中的终止位置
         * @param value 模式串对应的值
         * @param index 模式串对应的值的下标
         */
        void Hit(int begin, int end, V value, int index);
    }

    /**
     * 一个命中结果
     *
     * @param <V>
     */
    public class Hit<V>
    {
        /**
         * 模式串在母文本中的起始位置
         */
        public readonly int begin;
        /**
         * 模式串在母文本中的终止位置
         */
        public readonly int end;
        /**
         * 模式串对应的值
         */
        public readonly V value;

        public Hit(int begin, int end, V value)
        {
            this.begin = begin;
            this.end = end;
            this.value = value;
        }

        //@Override
        public override string ToString()
        {
            return string.Format("[%d:%d]=%s", begin, end, value);
        }
    }

    /**
     * 转移状态，支持failure转移
     *
     * @param currentState
     * @param character
     * @return
     */
    private int GetState(int currentState, char character)
    {
        int newCurrentState = TransitionWithRoot(currentState, character);  // 先按success跳转
        while (newCurrentState == -1) // 跳转失败的话，按failure跳转
        {
            currentState = fail[currentState];
            newCurrentState = TransitionWithRoot(currentState, character);
        }
        return newCurrentState;
    }

    /**
     * 保存输出
     *
     * @param position
     * @param currentState
     * @param collectedEmits
     */
    private void StoreEmits(int position, int currentState, List<Hit<V>> collectedEmits)
    {
        int[] hitArray = output[currentState];
        if (hitArray != null)
        {
            foreach (int hit in hitArray)
            {
                collectedEmits.Add(new Hit<V>(position - l[hit], position, v[hit]));
            }
        }
    }

    /**
     * 转移状态
     *
     * @param current
     * @param c
     * @return
     */
    protected int Transition(int current, char c)
    {
        int b = current;
        int p;

        p = b + c + 1;
        if (b == _check[p])
            b = _base[p];
        else
            return -1;

        p = b;
        return p;
    }

    /**
     * c转移，如果是根节点则返回自己
     *
     * @param nodePos
     * @param c
     * @return
     */
    protected int TransitionWithRoot(int nodePos, char c)
    {
        int b = _base[nodePos];
        int p;

        p = b + c + 1;
        if (b != _check[p])
        {
            if (nodePos == 0) return 0;
            return -1;
        }

        return p;
    }


    /**
     * 由一个排序好的map创建
     */
    public void Build(Dictionary<string, V> map)
    {
        new Builder().Build(map);
    }

    /**
     * 获取直接相连的子节点
     *
     * @param parent   父节点
     * @param siblings （子）兄弟节点
     * @return 兄弟节点个数
     */
    private int Fetch(State parent, List<KeyValuePair<int, State>> siblings)
    {
        if (parent.IsAcceptable)
        {
            State fakeNode = new State(-(parent.Depth + 1));  // 此节点是parent的子节点，同时具备parent的输出
            fakeNode.AddEmit(parent.LargestValueId);
            siblings.Add(new AbstractMap<int,State>.SimpleEntry<int, State>(0, fakeNode));
        }
        foreach (KeyValuePair<char, State> entry in parent.Success)
        {
            siblings.Add(new AbstractMap<int,State>.SimpleEntry<int, State>(entry.Key + 1, entry.Value));
        }
        return siblings.Count;
    }

    /**
     * 精确匹配
     *
     * @param key 键
     * @return 值的下标
     */
    public int ExactMatchSearch(string key)
    {
        return ExactMatchSearch(key, 0, 0, 0);
    }

    /**
     * 精确匹配
     *
     * @param key
     * @param pos
     * @param len
     * @param nodePos
     * @return
     */
    private int ExactMatchSearch(string key, int pos, int len, int nodePos)
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
            if (b == _check[p])
                b = _base[p];
            else
                return result;
        }

        p = b;
        int n = _base[p];
        if (b == _check[p] && n < 0)
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
    private int ExactMatchSearch(char[] keyChars, int pos, int len, int nodePos)
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

//    public void dfs(IWalker walker)
//    {
//        dfs(rootState, "", walker);
//    }
//
//    private void dfs(State currentState, string path, IWalker walker)
//    {
//        walker.meet(path, currentState);
//        for (char _transition in currentState.getTransitions())
//        {
//            State targetState = currentState.nextState(_transition);
//            dfs(targetState, path + _transition, walker);
//        }
//    }
//
//
//    public static interface IWalker
//    {
//        /**
//         * 遇到了一个节点
//         *
//         * @param path
//         * @param state
//         */
//        void meet(string path, State state);
//    }
//

//    public void debug()
//    {
//        Console.WriteLine("base:");
//        for (int i = 0; i < base.Length; i++)
//        {
//            if (base[i] < 0)
//            {
//                Console.WriteLine(i + " : " + -base[i]);
//            }
//        }
//
//        Console.WriteLine("output:");
//        for (int i = 0; i < output.Length; i++)
//        {
//            if (output[i] != null)
//            {
//                Console.WriteLine(i + " : " + String.Join(',',output[i]));
//            }
//        }
//
//        Console.WriteLine("fail:");
//        for (int i = 0; i < fail.Length; i++)
//        {
//            if (fail[i] != 0)
//            {
//                Console.WriteLine(i + " : " + fail[i]);
//            }
//        }
//
//        Console.WriteLine(this);
//    }

//    //@Override
//    public override string ToString()
//    {
//        string infoIndex = "i    = ";
//        string infoChar = "char = ";
//        string infoBase = "base = ";
//        string infoCheck = "check= ";
//        for (int i = 0; i < Math.Min(base.Length, 200); ++i)
//        {
//            if (base[i] != 0 || check[i] != 0)
//            {
//                infoChar += "    " + (i == check[i] ? " ×" : (char) (i - check[i] - 1));
//                infoIndex += " " + string.Format("%5d", i);
//                infoBase += " " + string.Format("%5d", base[i]);
//                infoCheck += " " + string.Format("%5d", check[i]);
//            }
//        }
//        return "DoubleArrayTrie：" +
//                "\n" + infoChar +
//                "\n" + infoIndex +
//                "\n" + infoBase +
//                "\n" + infoCheck + "\n" +
////                "check=" + String.Join(',',check) +
////                ", base=" + String.Join(',',base) +
////                ", used=" + String.Join(',',used) +
//                "size=" + size +
//                ", allocSize=" + allocSize +
//                ", keySize=" + keySize +
////                ", Length=" + String.Join(',',Length) +
////                ", value=" + String.Join(',',value) +
//                ", progress=" + progress +
//                ", nextCheckPos=" + nextCheckPos
//                ;
//    }

    /**
     * 一个顺序输出变量名与变量值的调试类
     */
    private class DebugArray
    {
        Dictionary<string, string> nameValueMap = new ();

        public void Add(string name, int value)
        {
            string valueInMap = nameValueMap.get(name);
            if (valueInMap == null)
            {
                valueInMap = "";
            }

            valueInMap += " " + string.Format("%5d", value);

            nameValueMap.Add(name, valueInMap);
        }

        //@Override
        public override string ToString()
        {
            string text = "";
            foreach (KeyValuePair<string, string> entry in nameValueMap)
            {
                string name = entry.Key;
                string value = entry.Value;
                text += string.Format("%-5s", name) + "= " + value + '\n';
            }

            return text;
        }

        public void Println()
        {
            Console.Write(this);
        }
    }

    /**
     * 大小，即包含多少个模式串
     *
     * @return
     */
    public int Size => v == null ? 0 : v.Length;

    /**
     * 构建工具
     */
    private class Builder
    {
        /**
         * 根节点，仅仅用于构建过程
         */
        private State rootState = new State();
        /**
         * 是否占用，仅仅用于构建
         */
        private bool[] used;
        /**
         * 已分配在内存中的大小
         */
        private int allocSize;
        /**
         * 一个控制增长速度的变量
         */
        private int progress;
        /**
         * 下一个插入的位置将从此开始搜索
         */
        private int nextCheckPos;
        /**
         * 键值对的大小
         */
        private int keySize;

        /**
         * 由一个排序好的map创建
         */
        
        public void Build(Dictionary<string, V> map)
        {
            // 把值保存下来
            v = (V[]) map.Values.ToArray();
            l = new int[v.Length];
            HashSet<string> keySet = map.Keys.ToHashSet();
            // 构建二分trie树
            AddAllKeyword(keySet);
            // 在二分trie树的基础上构建双数组trie树
            buildDoubleArrayTrie(keySet);
            used = null;
            // 构建failure表并且合并output表
            ConstructFailureStates();
            rootState = null;
            LoseWeight();
        }

        /**
         * 添加一个键
         *
         * @param keyword 键
         * @param index   值的下标
         */
        private void AddKeyword(string keyword, int index)
        {
            State currentState = this.rootState;
            foreach (char character in keyword.ToCharArray())
            {
                currentState = currentState.AddState(character);
            }
            currentState.AddEmit(index);
            l[index] = keyword.Length;
        }

        /**
         * 一系列键
         *
         * @param keywordSet
         */
        private void AddAllKeyword(ICollection<string> keywordSet)
        {
            int i = 0;
            foreach (string keyword in keywordSet)
            {
                AddKeyword(keyword, i++);
            }
        }

        /**
         * 建立failure表
         */
        private void ConstructFailureStates()
        {
            fail = new int[_size + 1];
            fail[1] = _base[0];
            output = new int[_size + 1][];
            Queue<State> queue = new ();

            // 第一步，将深度为1的节点的failure设为根节点
            foreach (State depthOneState in this.rootState.States)
            {
                depthOneState.SetFailure(this.rootState, fail);
                queue.Enqueue(depthOneState);
                ConstructOutput(depthOneState);
            }

            // 第二步，为深度 > 1 的节点建立failure表，这是一个bfs
            while (queue.Count>0)
            {
                State currentState = queue.Dequeue();

                foreach (char transition in currentState.Transitions)
                {
                    State targetState = currentState.NextState(transition);
                    queue.Enqueue(targetState);

                    State traceFailureState = currentState.Failure;
                    while (traceFailureState.NextState(transition) == null)
                    {
                        traceFailureState = traceFailureState.Failure;
                    }
                    State newFailureState = traceFailureState.NextState(transition);
                    targetState.SetFailure(newFailureState, fail);
                    targetState.AddEmit(newFailureState.Emit());
                    ConstructOutput(targetState);
                }
            }
        }

        /**
         * 建立output表
         */
        private void ConstructOutput(State targetState)
        {
            var emit = targetState.Emit();
            if (emit == null || emit.Count == 0) return;
            int[] output = new int[emit.Count];
            IEnumerator<int> it = emit.GetEnumerator();
            for (int i = 0; i < output.Length; ++i)
            {
                output[i] = it.next();
            }
            AhoCorasickDoubleArrayTrie.s.output[targetState.Index] = output;
        }

        private void buildDoubleArrayTrie(HashSet<string> keySet)
        {
            progress = 0;
            keySize = keySet.Count;
            Resize(65536 * 32); // 32个双字节

            _base[0] = 1;
            nextCheckPos = 0;

            State root_node = this.rootState;

            List<KeyValuePair<int, State>> siblings = new (root_node.Success.Count);
            Fetch(root_node, siblings);
            Insert(siblings);
        }

        /**
         * 拓展数组
         *
         * @param newSize
         * @return
         */
        private int Resize(int newSize)
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

        /**
         * 插入节点
         *
         * @param siblings 等待插入的兄弟节点
         * @return 插入位置
         */
        private int Insert(List<KeyValuePair<int, State>> siblings)
        {
            int begin = 0;
            int pos = Math.Max(siblings[(0)].Key + 1, nextCheckPos) - 1;
            int nonzero_num = 0;
            int first = 0;

            if (allocSize <= pos)
                Resize(pos + 1);

            outer:
            // 此循环体的目标是找出满足base[begin + a1...an]  == 0的n个空闲空间,a1...an是siblings中的n个节点
            while (true)
            {
                pos++;

                if (allocSize <= pos)
                    Resize(pos + 1);

                if (_check[pos] != 0)
                {
                    nonzero_num++;
                    continue;
                }
                else if (first == 0)
                {
                    nextCheckPos = pos;
                    first = 1;
                }

                begin = pos - siblings[(0)].Key; // 当前位置离第一个兄弟节点的距离
                if (allocSize <= (begin + siblings[(siblings.Count - 1)].Key))
                {
                    // progress can be zero // 防止progress产生除零错误
                    double l = (1.05 > 1.0 * keySize / (progress + 1)) ? 1.05 : 1.0 * keySize / (progress + 1);
                    Resize((int) (allocSize * l));
                }

                if (used[begin])
                    continue;

                for (int i = 1; i < siblings.Count; i++)
                    if (_check[begin + siblings[(i)].Key] != 0)
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
            used[begin] = true;

            _size = (_size > begin + siblings.get(siblings.Count - 1).Key + 1) ? Size : begin + siblings.get(siblings.Count - 1).Key + 1;

            foreach (KeyValuePair<int, State> sibling in siblings)
            {
                check[begin + sibling.Key] = begin;
            }

            foreach (KeyValuePair<int, State> sibling in siblings)
            {
                List<KeyValuePair<int, State>> new_siblings = new (sibling.Value.Success.entrySet().Count + 1);

                if (Fetch(sibling.Value, new_siblings) == 0)  // 一个词的终止且不为其他词的前缀，其实就是叶子节点
                {
                    _base[begin + sibling.Key] = (-sibling.Value.LargestValueId - 1);
                    progress++;
                }
                else
                {
                    int h = Insert(new_siblings);   // dfs
                    _base[begin + sibling.Key] = h;
                }
                sibling.Value.                Index = begin + sibling.Key;
            }
            return begin;
        }

        /**
         * 释放空闲的内存
         */
        private void LoseWeight()
        {
            int[] _base = new int[Size + 65535];
            Array.Copy(_base, 0, nbase, 0,             Size);
            _base = nbase;

            int[] ncheck = new int[Size + 65535];
            Array.Copy(check, 0, ncheck, 0,             Size);
            check = ncheck;
        }
    }
}
