/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/10 14:47</create-date>
 *
 * <copyright file="PersonDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.corpus.dictionary.item;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.nr;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.dictionary.nr;




/**
 * 人名识别用的词典，实际上是对两个词典的包装
 *
 * @author hankcs
 */
public class PersonDictionary
{
    /**
     * 人名词典
     */
    public static NRDictionary dictionary;
    /**
     * 转移矩阵词典
     */
    public static TransformMatrixDictionary<NR> transformMatrixDictionary;
    /**
     * AC算法用到的Trie树
     */
    public static AhoCorasickDoubleArrayTrie<NRPattern> trie;

    public static readonly CoreDictionary.Attribute ATTRIBUTE = new CoreDictionary.Attribute(Nature.nr, 100);

    static PersonDictionary()
    {
        long start = DateTime.Now.Microsecond;
        dictionary = new NRDictionary();
        if (!dictionary.load(HanLP.Config.PersonDictionaryPath))
        {
            throw new ArgumentException("人名词典加载失败：" + HanLP.Config.PersonDictionaryPath);
        }
        transformMatrixDictionary = new TransformMatrixDictionary<NR>(typeof(NR));
        transformMatrixDictionary.load(HanLP.Config.PersonDictionaryTrPath);
        trie = new AhoCorasickDoubleArrayTrie<NRPattern>();
        var map = new Dictionary<string, NRPattern>();
        foreach (NRPattern pattern in NRPattern.values())
        {
            map.Add(pattern.ToString(), pattern);
        }
        trie.build(map);
        logger.info(HanLP.Config.PersonDictionaryPath + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    /**
     * 模式匹配
     *
     * @param nrList         确定的标注序列
     * @param vertexList     原始的未加角色标注的序列
     * @param wordNetOptimum 待优化的图
     * @param wordNetAll     全词图
     */
    public static void parsePattern(List<NR> nrList, List<Vertex> vertexList,  WordNet wordNetOptimum,  WordNet wordNetAll)
    {
        // 拆分UV
        ListIterator<Vertex> listIterator = vertexList.GetEnumerator();
        StringBuilder sbPattern = new StringBuilder(nrList.size());
        NR preNR = NR.A;
        bool backUp = false;
        int index = 0;
        foreach (NR nr in nrList)
        {
            ++index;
            Vertex current = listIterator.next();
//            logger.trace("{}/{}", current.realWord, nr);
            switch (nr)
            {
                case U:
                    if (!backUp)
                    {
                        vertexList = new (vertexList);
                        listIterator = vertexList.listIterator(index);
                        backUp = true;
                    }
                    sbPattern.Append(NR.K.ToString());
                    sbPattern.Append(NR.B.ToString());
                    preNR = B;
                    listIterator.previous();
                    string nowK = current.realWord.substring(0, current.realWord.Length - 1);
                    string nowB = current.realWord.substring(current.realWord.Length - 1);
                    listIterator.set(new Vertex(nowK));
                    listIterator.next();
                    listIterator.Add(new Vertex(nowB));
                    continue;
                case V:
                    if (!backUp)
                    {
                        vertexList = new (vertexList);
                        listIterator = vertexList.listIterator(index);
                        backUp = true;
                    }
                    if (preNR == B)
                    {
                        sbPattern.Append(NR.E.ToString());  //BE
                    }
                    else
                    {
                        sbPattern.Append(NR.D.ToString());  //CD
                    }
                    sbPattern.Append(NR.L.ToString());
                    // 对串也做一些修改
                    listIterator.previous();
                    string EorD = current.realWord.substring(0, 1);
                    string L = current.realWord.substring(1, current.realWord.Length);
                    listIterator.set(new Vertex(EorD));
                    listIterator.next();
                    listIterator.Add(new Vertex(L));
                    continue;
                default:
                    sbPattern.Append(nr.ToString());
                    break;
            }
            preNR = nr;
        }
        string pattern = sbPattern.ToString();
//        logger.trace("模式串：{}", pattern);
//        logger.trace("对应串：{}", vertexList);
//        if (pattern.Length != vertexList.size())
//        {
//            logger.warn("人名识别模式串有bug", pattern, vertexList);
//            return;
//        }
         Vertex[] wordArray = vertexList.ToArray();
         int[] offsetArray = new int[wordArray.Length];
        offsetArray[0] = 0;
        for (int i = 1; i < wordArray.Length; ++i)
        {
            offsetArray[i] = offsetArray[i - 1] + wordArray[i - 1].realWord.Length;
        }
        trie.parseText(pattern, new CT());
    }
    public class CT : AhoCorasickDoubleArrayTrie<NRPattern>.IHit<NRPattern>
    {
        //@Override
        public void hit(int begin, int end, NRPattern value)
        {
            //            logger.trace("匹配到：{}", keyword);
            StringBuilder sbName = new StringBuilder();
            for (int i = begin; i < end; ++i)
            {
                sbName.Append(wordArray[i].realWord);
            }
            string name = sbName.ToString();
            //            logger.trace("识别出：{}", name);
            // 对一些bad case做出调整
            switch (value)
            {
                case BCD:
                    if (name[0] == name.charAt(2)) return; // 姓和最后一个名不可能相等的
                                                                  //                        string cd = name.substring(1);
                                                                  //                        if (CoreDictionary.Contains(cd))
                                                                  //                        {
                                                                  //                            EnumItem<NR> item = PersonDictionary.dictionary.get(cd);
                                                                  //                            if (item == null || !item.containsLabel(Z)) return; // 三字名字但是后两个字不在词典中，有很大可能性是误命中
                                                                  //                        }
                    break;
            }
            if (isBadCase(name)) return;

            // 正式算它是一个名字
            if (HanLP.Config.DEBUG)
            {
                Console.WriteLine("识别出人名：%s %s\n", name, value);
            }
            int offset = offsetArray[begin];
            wordNetOptimum.insert(offset, new Vertex(Predefine.TAG_PEOPLE, name, ATTRIBUTE, WORD_ID), wordNetAll);
        }
    }
    /**
     * 因为任何算法都无法解决100%的问题，总是有一些bad case，这些bad case会以“盖公章 A 1”的形式加入词典中<BR>
     * 这个方法返回人名是否是bad case
     *
     * @param name
     * @return
     */
    static bool isBadCase(string name)
    {
        EnumItem<NR> nrEnumItem = dictionary.get(name);
        if (nrEnumItem == null) return false;
        return nrEnumItem.containsLabel(NR.A);
    }
}
