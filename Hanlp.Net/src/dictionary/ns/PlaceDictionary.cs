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
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.corpus.dictionary.item;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.dictionary.ns;




/**
 * 地名识别用的词典，实际上是对两个词典的包装
 *
 * @author hankcs
 */
public class PlaceDictionary
{
    /**
     * 地名词典
     */
    public static NSDictionary dictionary;
    /**
     * 转移矩阵词典
     */
    public static TransformMatrixDictionary<NS> transformMatrixDictionary;
    /**
     * AC算法用到的Trie树
     */
    public static AhoCorasickDoubleArrayTrie<string> trie;

    /**
     * 本词典专注的词的ID
     */
    static readonly int WORD_ID = CoreDictionary.getWordID(Predefine.TAG_PLACE);
    /**
     * 本词典专注的词的属性
     */
    static readonly CoreDictionary.Attribute ATTRIBUTE = CoreDictionary.get(WORD_ID);

    static PlaceDictionary()
    {
        long start = DateTime.Now.Microsecond;
        dictionary = new NSDictionary();
        if (dictionary.load(HanLP.Config.PlaceDictionaryPath))
            logger.info(HanLP.Config.PlaceDictionaryPath + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
        else
            throw new ArgumentException(HanLP.Config.PlaceDictionaryPath + "加载失败");
        transformMatrixDictionary = new TransformMatrixDictionary<NS>(NS.c);
        transformMatrixDictionary.load(HanLP.Config.PlaceDictionaryTrPath);
        trie = new AhoCorasickDoubleArrayTrie<string>();
        Dictionary<string, string> patternMap = new Dictionary<string, string>();
        patternMap.Add("CH", "CH");
        patternMap.Add("CDH", "CDH");
        patternMap.Add("CDEH", "CDEH");
        patternMap.Add("GH", "GH");
        trie.build(patternMap);
    }

    /**
     * 模式匹配
     *
     * @param nsList         确定的标注序列
     * @param vertexList     原始的未加角色标注的序列
     * @param wordNetOptimum 待优化的图
     * @param wordNetAll
     */
    public static void parsePattern(List<NS> nsList, List<Vertex> vertexList,  WordNet wordNetOptimum,  WordNet wordNetAll)
    {
//        ListIterator<Vertex> listIterator = vertexList.listIterator();
        var sbPattern = new StringBuilder(nsList.Count);
        foreach (NS ns in nsList)
        {
            sbPattern.Append(ns.ToString());
        }
        string pattern = sbPattern.ToString();
        Vertex[] wordArray = vertexList.ToArray();
        trie.parseText(pattern, new CT());
    }
    public class CT:
        AhoCorasickDoubleArrayTrie<string>.IHit<string>
    {
        //@Override
        public void hit(int begin, int end, string value)
        {
            StringBuilder sbName = new StringBuilder();
            for (int i = begin; i < end; ++i)
            {
                sbName.Append(wordArray[i].realWord);
            }
            string name = sbName.toString();
            // 对一些bad case做出调整
            if (isBadCase(name)) return;

            // 正式算它是一个名字
            if (HanLP.Config.DEBUG)
            {
                System._out.printf("识别出地名：%s %s\n", name, value);
            }
            int offset = 0;
            for (int i = 0; i < begin; ++i)
            {
                offset += wordArray[i].realWord.Length;
            }
            wordNetOptimum.insert(offset, new Vertex(Predefine.TAG_PLACE, name, ATTRIBUTE, WORD_ID), wordNetAll);
        }
    }
    /**
     * 因为任何算法都无法解决100%的问题，总是有一些bad case，这些bad case会以“盖公章 A 1”的形式加入词典中<BR>
     * 这个方法返回是否是bad case
     *
     * @param name
     * @return
     */
    static bool isBadCase(string name)
    {
        EnumItem<NS> nrEnumItem = dictionary.get(name);
        if (nrEnumItem == null) return false;
        return nrEnumItem.containsLabel(NS.Z);
    }
}
