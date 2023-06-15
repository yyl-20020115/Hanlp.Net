/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-12-08 下午1:15</create-date>
 *
 * <copyright file="DemoPinyinSegment.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.seg.Other;
using System.Collections.Specialized;

namespace com.hankcs.demo;



/**
 * HanLP中的数据结构和接口是灵活的，组合这些接口，可以自己创造新功能
 *
 * @author hankcs
 */
public class DemoPinyinToChinese
{
    public static void Main(String[] args)
    {
        StringDictionary dictionary = new StringDictionary("=");
        dictionary.load(HanLP.Config.PinyinDictionaryPath);
        var map = new Dictionary<String, HashSet<String>>();
        for (Map.Entry<String, String> entry : dictionary.entrySet())
        {
            String pinyins = entry.getValue().replaceAll("[\\d,]", "");
            Set<String> words = map.get(pinyins);
            if (words == null)
            {
                words = new TreeSet<String>();
                map.put(pinyins, words);
            }
            words.add(entry.getKey());
        }
        Set<String> words = new TreeSet<String>();
        words.add("绿色");
        words.add("滤色");
        map.put("lvse", words);

        // 1.5.2及以下版本
        AhoCorasickDoubleArrayTrie<HashSet<String>> trie = new AhoCorasickDoubleArrayTrie<HashSet<String>>();
        trie.build(map);
        Console.WriteLine(CommonAhoCorasickSegmentUtil.segment("renmenrenweiyalujiangbujianlvse", trie));

        // 1.5.3及以上版本
        CommonAhoCorasickDoubleArrayTrieSegment<HashSet<String>> segment = new CommonAhoCorasickDoubleArrayTrieSegment<Set<String>>(map);
        Console.WriteLine(segment.segment("renmenrenweiyalujiangbujianlvse"));

    }
}
