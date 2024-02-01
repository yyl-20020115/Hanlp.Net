/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/05/2014/5/19 21:06</create-date>
 *
 * <copyright file="Graph.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.seg.NShort.Path;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.seg.common;




/**
 * @author hankcs
 */
public class WordNet
{
    /**
     * 节点，每一行都是前缀词，跟图的表示方式不同
     */
    private List<Vertex>[] vertexes;

    /**
     * 共有多少个节点
     */
    int _size;

    /**
     * 原始句子
     *
     * @deprecated 应当使用数组，这样比较快
     */
    public string sentence;

    /**
     * 原始句子对应的数组
     */
    public char[] charArray;

    /**
     * 为一个句子生成空白词网
     *
     * @param sentence 句子
     */
    public WordNet(string sentence)
        : this(sentence.ToCharArray())
    {
        ;
    }

    public WordNet(char[] charArray)
    {
        this.charArray = charArray;
        vertexes = new List<Vertex> [charArray.Length + 2];
        for (int i = 0; i < vertexes.Length; ++i)
        {
            vertexes[i] = new ();
        }
        vertexes[0].Add(Vertex.newB());
        vertexes[vertexes.Length - 1].Add(Vertex.newE());
        _size = 2;
    }

    public WordNet(char[] charArray, List<Vertex> vertexList)
    {
        this.charArray = charArray;
        vertexes = new List<Vertex>[charArray.Length + 2];
        for (int i = 0; i < vertexes.Length; ++i)
        {
            vertexes[i] = new ();
        }
        int i2 = 0;
        foreach (Vertex vertex in vertexList)
        {
            vertexes[i2].Add(vertex);
            ++_size;
            i2 += vertex.realWord.Length;
        }
    }

    /**
     * 添加顶点
     *
     * @param line   行号
     * @param vertex 顶点
     */
    public void Add(int line, Vertex vertex)
    {
        foreach (Vertex oldVertex in vertexes[line])
        {
            // 保证唯一性
            if (oldVertex.realWord.Length == vertex.realWord.Length) return;
        }
        vertexes[line].Add(vertex);
        ++_size;
    }

    /**
     * 强行添加，替换已有的顶点
     *
     * @param line
     * @param vertex
     */
    public void push(int line, Vertex vertex)
    {
        IEnumerator<Vertex> iterator = vertexes[line].GetEnumerator();
        while (iterator.MoveNext())
        {
            if (iterator.next().realWord.Length == vertex.realWord.Length)
            {
                iterator.Remove();
                --_size;
                break;
            }
        }
        vertexes[line].Add(vertex);
        ++_size;
    }

    /**
     * 添加顶点，同时检查此顶点是否悬孤，如果悬孤则自动补全
     *
     * @param line
     * @param vertex
     * @param wordNetAll 这是一个完全的词图
     */
    public void insert(int line, Vertex vertex, WordNet wordNetAll)
    {
        foreach (Vertex oldVertex in vertexes[line])
        {
            // 保证唯一性
            if (oldVertex.realWord.Length == vertex.realWord.Length) return;
        }
        vertexes[line].Add(vertex);
        ++_size;
        // 保证这个词语前面直连
        int start = Math.Max(0, line - 5); // 效率起见，只扫描前4行
        for (int l = line - 1; l > start; --l)
        {
            var all = wordNetAll.get(l);
            if (all.Count <= vertexes[l].Count)
                continue;
            foreach (Vertex pre in all)
            {
                if (pre.Length() + l == line)
                {
                    vertexes[l].Add(pre);
                    ++_size;
                }
            }
        }
        // 保证这个词语后面直连
        int l = line + vertex.realWord.Length;
        LinkedList<Vertex> targetLine = wordNetAll.get(l);
        if (vertexes[l].Count == 0 && targetLine.Count != 0) // 有时候vertexes里面的词语已经经过用户词典合并，造成数量更少
        {
            _size += targetLine.Count;
            vertexes[l] = targetLine;
        }
    }

    /**
     * 全自动添加顶点
     *
     * @param vertexList
     */
    public void AddRange(List<Vertex> vertexList)
    {
        int i = 0;
        foreach (Vertex vertex in vertexList)
        {
            Add(i, vertex);
            i += vertex.realWord.Length;
        }
    }

    /**
     * 获取某一行的所有节点
     *
     * @param line 行号
     * @return 一个数组
     */
    public LinkedList<Vertex> get(int line)
    {
        return vertexes[line];
    }

    /**
     * 获取某一行的逆序迭代器
     *
     * @param line 行号
     * @return 逆序迭代器
     */
    public IEnumerator<Vertex> descendingIterator(int line)
    {
        return vertexes[line].descendingIterator();
    }

    /**
     * 获取某一行的第一个节点
     *
     * @param line
     * @return
     */
    public Vertex getFirst(int line)
    {
        var iterator = vertexes[line].GetEnumerator();
        if (iterator.MoveNext()) return iterator.next();

        return null;
    }

    /**
     * 获取某一行长度为Length的节点
     *
     * @param line
     * @param Length
     * @return
     */
    public Vertex get(int line, int Length)
    {
        foreach (Vertex vertex in vertexes[line])
        {
            if (vertex.realWord.Length == Length)
            {
                return vertex;
            }
        }

        return null;
    }

    /**
     * 添加顶点，由原子分词顶点添加
     *
     * @param line
     * @param atomSegment
     */
    public void Add(int line, List<AtomNode> atomSegment)
    {
        // 将原子部分存入m_segGraph
        int offset = 0;
        foreach (AtomNode atomNode in atomSegment)//Init the cost array
        {
            string sWord = atomNode.sWord;//init the word
            Nature nature = Nature.n;
            int id = -1;
            switch (atomNode.nPOS)
            {
                case CharType.CT_CHINESE:
                    break;
                case CharType.CT_NUM:
                case CharType.CT_INDEX:
                case CharType.CT_CNUM:
                    nature = Nature.m;
                    sWord = Predefine.TAG_NUMBER;
                    id = CoreDictionary.M_WORD_ID;
                    break;
                case CharType.CT_DELIMITER:
                case CharType.CT_OTHER:
                    nature = Nature.w;
                    break;
                case CharType.CT_SINGLE://12021-2129-3121
                    nature = Nature.nx;
                    sWord = Predefine.TAG_CLUSTER;
                    id = CoreDictionary.X_WORD_ID;
                    break;
                default:
                    break;
            }
            // 这些通用符的量级都在10万左右
            Add(line + offset, new Vertex(sWord, atomNode.sWord, new CoreDictionary.Attribute(nature, 10000), id));
            offset += atomNode.sWord.Length;
        }
    }

    public int Count=> _size;

    /**
     * 获取顶点数组
     *
     * @return Vertex[] 按行优先列次之的顺序构造的顶点数组
     */
    private Vertex[] getVertexesLineFirst()
    {
        Vertex[] vertexes = new Vertex[_size];
        int i = 0;
        foreach (List<Vertex> vertexList in this.vertexes)
        {
            foreach (Vertex v in vertexList)
            {
                v.index = i;    // 设置id
                vertexes[i++] = v;
            }
        }

        return vertexes;
    }

    /**
     * 词网转词图
     *
     * @return 词图
     */
    public Graph toGraph()
    {
        Graph graph = new Graph(getVertexesLineFirst());

        for (int row = 0; row < vertexes.Length - 1; ++row)
        {
            List<Vertex> vertexListFrom = vertexes[row];
            foreach (Vertex from in vertexListFrom)
            {
                //assert from.realWord.Length > 0 : "空节点会导致死循环！";
                int toIndex = row + from.realWord.Length;
                foreach (Vertex to in vertexes[toIndex])
                {
                    graph.connect(from.index, to.index, MathUtility.calculateWeight(from, to));
                }
            }
        }
        return graph;
    }

    //@Override
    public override string ToString()
    {
//        return "Graph{" +
//                "vertexes=" + Arrays.ToString(vertexes) +
//                '}';
        StringBuilder sb = new StringBuilder();
        int line = 0;
        foreach (List<Vertex> vertexList in vertexes)
        {
            sb.Append(string.valueOf(line++) + ':' + vertexList.ToString()).Append("\n");
        }
        return sb.ToString();
    }

    /**
     * 将连续的ns节点合并为一个
     */
    public void mergeContinuousNsIntoOne()
    {
        for (int row = 0; row < vertexes.Length - 1; ++row)
        {
            List<Vertex> vertexListFrom = vertexes[row];
            var listIteratorFrom = vertexListFrom.GetEnumerator();
            while (listIteratorFrom.MoveNext())
            {
                Vertex from = listIteratorFrom.next();
                if (from.getNature() == Nature.ns)
                {
                    int toIndex = row + from.realWord.Length;
                    var listIteratorTo = vertexes[toIndex].GetEnumerator();
                    while (listIteratorTo.MoveNext())
                    {
                        Vertex to = listIteratorTo.next();
                        if (to.getNature() == Nature.ns)
                        {
                            // 我们不能直接改，因为很多条线路在公用指针
//                            from.realWord += to.realWord;
                            logger.info("合并【" + from.realWord + "】和【" + to.realWord + "】");
                            listIteratorFrom.set(Vertex.newAddressInstance(from.realWord + to.realWord));
//                            listIteratorTo.Remove();
                            break;
                        }
                    }
                }
            }
        }
    }

    /**
     * 清空词图
     */
    public void Clear()
    {
        foreach (List<Vertex> vertexList in vertexes)
        {
            vertexList.Clear();
        }
        _size = 0;
    }

    /**
     * 清理from属性
     */
    public void clean()
    {
        foreach (List<Vertex> vertexList in vertexes)
        {
            foreach (Vertex vertex in vertexList)
            {
                vertex.from = null;
            }
        }
    }

    /**
     * 获取内部顶点表格，谨慎操作！
     *
     * @return
     */
    public List<Vertex>[] getVertexes()
    {
        return vertexes;
    }
}
