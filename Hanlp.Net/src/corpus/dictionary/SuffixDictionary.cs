/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/17 17:24</create-date>
 *
 * <copyright file="SuffixDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;
using System.Text;

namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * 后缀树词典
 * @author hankcs
 */
public class SuffixDictionary
{
    BinTrie<int> trie;

    public SuffixDictionary()
    {
        trie = new BinTrie<int>();
    }

    /**
     * 添加一个词语
     * @param word
     */
    public void Add(string word)
    {
        word = reverse(word);
        trie.Add(word, word.Length);
    }

    public void AddRange(string total)
    {
        for (int i = 0; i < total.Length; ++i)
        {
            Add(string.valueOf(total[(i)]));
        }
    }

    public void AddRange(string[] total)
    {
        for (string single : total)
        {
            Add(single);
        }
    }

    /**
     * 查找是否有该后缀
     * @param suffix
     * @return
     */
    public int get(string suffix)
    {
        suffix = reverse(suffix);
        int Length = trie.get(suffix);
        if (Length == null) return 0;

        return Length;
    }

    /**
     * 词语是否以该词典中的某个单词结尾
     * @param word
     * @return
     */
    public bool EndsWith(string word)
    {
        word = reverse(word);
        return trie.commonPrefixSearchWithValue(word).Count > 0;
    }

    /**
     * 获取最长的后缀
     * @param word
     * @return
     */
    public int getLongestSuffixLength(string word)
    {
        word = reverse(word);
        LinkedList<KeyValuePair<string, int>> suffixList = trie.commonPrefixSearchWithValue(word);
        if (suffixList.Count == 0) return 0;
        return suffixList.getLast().Value;
    }

    private static string reverse(string word)
    {
        return new StringBuilder(word).reverse().ToString();
    }

    /**
     * 键值对
     * @return
     */
    public HashSet<KeyValuePair<string, int>> entrySet()
    {
        HashSet<KeyValuePair<string, int>> treeSet = new ();
        for (KeyValuePair<string, int> entry : trie.entrySet())
        {
            treeSet.Add(new AbstractMap.SimpleEntry<string, int>(reverse(entry.Key), entry.Value));
        }

        return treeSet;
    }
}
