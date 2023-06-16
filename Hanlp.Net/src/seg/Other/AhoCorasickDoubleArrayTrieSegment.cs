/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/7 18:48</create-date>
 *
 * <copyright file="AhoCorasickDoubleArrayTrieSegment.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.seg.Other;




/**
 * 使用AhoCorasickDoubleArrayTrie实现的最长分词器<br>
 * 需要用户调用setTrie()提供一个AhoCorasickDoubleArrayTrie
 *
 * @author hankcs
 */
public class AhoCorasickDoubleArrayTrieSegment : DictionaryBasedSegment
{
    AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute> trie;

    public AhoCorasickDoubleArrayTrieSegment() 
        : this(HanLP.Config.CoreDictionaryPath)
    {
        ;
    }

    public AhoCorasickDoubleArrayTrieSegment(Dictionary<string, CoreDictionary.Attribute> dictionary)
        : this(new AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>(dictionary))
    {
        ;
    }

    public AhoCorasickDoubleArrayTrieSegment(AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute> trie)
    {
        this.trie = trie;
        config.useCustomDictionary = false;
        config.speechTagging = false;
    }

    /**
     * 加载自己的词典，构造分词器
     * @param dictionaryPaths 任意数量个词典
     *
     * @ 加载过程中的IO异常
     */
    public AhoCorasickDoubleArrayTrieSegment(params string[] dictionaryPaths) 
    {
        this(new AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>(IOUtil.loadDictionary(dictionaryPaths)));
    }

    //@Override
    protected List<Term> segSentence(char[] sentence)
    {
        if (trie == null)
        {
            logger.warning("还未加载任何词典");
            return Collections.emptyList();
        }
         int[] wordNet = new int[sentence.Length];
        Arrays.fill(wordNet, 1);
         Nature[] natureArray = config.speechTagging ? new Nature[sentence.Length] : null;
        trie.parseText(sentence, new CT());
        LinkedList<Term> termList = new LinkedList<Term>();
        posTag(sentence, wordNet, natureArray);
        for (int i = 0; i < wordNet.Length; )
        {
            Term term = new Term(new string(sentence, i, wordNet[i]), config.speechTagging ? (natureArray[i] == null ? Nature.nz : natureArray[i]) : null);
            term.offset = i;
            termList.add(term);
            i += wordNet[i];
        }
        return termList;
    }
    public class CT: AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute>
    {
        //@Override
        public void hit(int begin, int end, CoreDictionary.Attribute value)
        {
            int Length = end - begin;
            if (Length > wordNet[begin])
            {
                wordNet[begin] = Length;
                if (config.speechTagging)
                {
                    natureArray[begin] = value.nature[0];
                }
            }
        }
    }
    //@Override
    public Segment enableCustomDictionary(bool enable)
    {
        throw new UnsupportedOperationException("AhoCorasickDoubleArrayTrieSegment暂时不支持用户词典。");
    }

    public AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute> getTrie()
    {
        return trie;
    }

    public void setTrie(AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute> trie)
    {
        this.trie = trie;
    }

    public AhoCorasickDoubleArrayTrieSegment loadDictionary(string... pathArray)
    {
        trie = new AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>();
        TreeMap<string, CoreDictionary.Attribute> map = null;
        try
        {
            map = IOUtil.loadDictionary(pathArray);
        }
        catch (IOException e)
        {
            logger.warning("加载词典失败\n" + TextUtility.exceptionToString(e));
            return this;
        }
        if (map != null && !map.isEmpty())
        {
            trie.build(map);
        }

        return this;
    }
}
