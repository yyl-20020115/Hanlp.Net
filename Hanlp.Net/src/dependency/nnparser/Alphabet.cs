/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/1 22:35</create-date>
 *
 * <copyright file="Alphabet.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dependency.nnparser;



/**
 * int 到 string 的双向map
 * @author hankcs
 */
public class Alphabet : ICacheAble
{
    ITrie<int> trie;
    string[] idToLabelMap;

    public Alphabet()
    {
        trie = new DoubleArrayTrie<int>();
    }

    /**
     * id转label
     * @param id
     * @return
     */
    public string labelOf(int id)
    {
        return idToLabelMap[id];
    }


    public int build(Dictionary<string, int> keyValueMap)
    {
        idToLabelMap = new string[keyValueMap.Count];
        foreach (KeyValuePair<string, int> entry in keyValueMap)
        {
            idToLabelMap[entry.Value] = entry.Key;
        }
        return trie.build(keyValueMap);
    }

    /**
     * label转id
     * @param label
     * @return
     */
    public int idOf(char[] label)
    {
        return trie.get(label);
    }

    /**
     * label转id
     * @param label
     * @return
     */
    public int idOf(string label)
    {
        return trie.get(label);
    }

    /**
     * 字母表大小
     * @return
     */
    public int size()
    {
        return trie.Count;
    }

    public void save(Stream _out)
    {
        using var writer = new BinaryWriter(_out);
        writer.Write(idToLabelMap.Length);
        foreach (string value in idToLabelMap)
        {
            TextUtility.writeString(value, _out);
        }

    }

    public bool load(ByteArray byteArray)
    {
        idToLabelMap = new string[byteArray.Next()];
        var map = new Dictionary<string, int>();
        for (int i = 0; i < idToLabelMap.Length; i++)
        {
            idToLabelMap[i] = byteArray.nextString();
            map.Add(idToLabelMap[i], i);
        }

        return trie.build(map) == 0;
    }
}
