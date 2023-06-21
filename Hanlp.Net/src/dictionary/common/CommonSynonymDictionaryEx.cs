/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/13 13:13</create-date>
 *
 * <copyright file="CommonSynonymDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.synonym;
using System.Text;

namespace com.hankcs.hanlp.dictionary.common;


/**
 * 一个没有指定资源位置的通用同义词词典
 *
 * @author hankcs
 */
public class CommonSynonymDictionaryEx
{
    DoubleArrayTrie<long[]> trie;

    private CommonSynonymDictionaryEx()
    {
    }

    public static CommonSynonymDictionaryEx create(Stream inputStream)
    {
        CommonSynonymDictionaryEx dictionary = new CommonSynonymDictionaryEx();
        if (dictionary.load(inputStream))
        {
            return dictionary;
        }

//        TreeSet<float> set = new TreeSet<float>();

        return null;
    }

    public bool load(Stream inputStream)
    {
        trie = new DoubleArrayTrie<long[]>();
        Dictionary<string, HashSet<long>> treeMap = new Dictionary<string, HashSet<long>>();
        string line = null;
        try
        {
            TextReader bw = new TextReader(new InputStreamReader(inputStream, "UTF-8"));
            while ((line = bw.ReadLine()) != null)
            {
                string[] args = line.Split(" ");
                List<Synonym> synonymList = Synonym.create(args);
                foreach (Synonym synonym in synonymList)
                {
                    HashSet<long> idSet = treeMap.get(synonym.realWord);
                    if (idSet == null)
                    {
                        idSet = new ();
                        treeMap.Add(synonym.realWord, idSet);
                    }
                    idSet.Add(synonym.id);
                }
            }
            bw.Close();
            List<string> keyList = new (treeMap.size());
            foreach (string key in treeMap.keySet())
            {
                keyList.Add(key);
            }
            List<long[]> valueList = new (treeMap.size());
            foreach (HashSet<long> idSet in treeMap.values())
            {
                valueList.Add(idSet.ToArray());
            }
            int resultCode = trie.build(keyList, valueList);
            if (resultCode != 0)
            {
                logger.warning("构建" + inputStream + "失败，错误码" + resultCode);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.warning("读取" + inputStream + "失败，可能由行" + line + "造成" + e);
            return false;
        }
        return true;
    }

    public long[] get(string key)
    {
        return trie.get(key);
    }

    /**
     * 语义距离
     * @param a
     * @param b
     * @return
     */
    public long distance(string a, string b)
    {
        long[] itemA = get(a);
        if (itemA == null) return long.MaxValue / 3;
        long[] itemB = get(b);
        if (itemB == null) return long.MaxValue / 3;

        return ArrayDistance.computeAverageDistance(itemA, itemB);
    }

    /**
     * 词典中的一个条目
     */
    public  class SynonymItem : Synonym
    {
        /**
         * 条目的value，是key的同义词近义词列表
         */
        public Dictionary<string, Synonym> synonymMap;

        public SynonymItem(Synonym entry, Dictionary<string, Synonym> synonymMap)
            : base(entry.realWord, entry.id, entry.type)
        {
            ;
            this.synonymMap = synonymMap;
        }

        //@Override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(' ');
            sb.Append(synonymMap);
            return sb.ToString();
        }
    }
}
