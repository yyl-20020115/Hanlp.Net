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
namespace com.hankcs.hanlp.dependency.nnparser;



/**
 * int 到 String 的双向map
 * @author hankcs
 */
public class Alphabet : ICacheAble
{
    ITrie<int> trie;
    String[] idToLabelMap;

    public Alphabet()
    {
        trie = new DoubleArrayTrie<int>();
    }

    /**
     * id转label
     * @param id
     * @return
     */
    public String labelOf(int id)
    {
        return idToLabelMap[id];
    }


    public int build(TreeMap<String, int> keyValueMap)
    {
        idToLabelMap = new String[keyValueMap.size()];
        for (Map.Entry<String, int> entry : keyValueMap.entrySet())
        {
            idToLabelMap[entry.getValue()] = entry.getKey();
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
    public int idOf(String label)
    {
        return trie.get(label);
    }

    /**
     * 字母表大小
     * @return
     */
    public int size()
    {
        return trie.size();
    }

    public void save(DataOutputStream _out)
    {
        _out.writeInt(idToLabelMap.length);
        for (String value : idToLabelMap)
        {
            TextUtility.writeString(value, _out);
        }
    }

    public bool load(ByteArray byteArray)
    {
        idToLabelMap = new String[byteArray.nextInt()];
        TreeMap<String, int> map = new TreeMap<String, int>();
        for (int i = 0; i < idToLabelMap.length; i++)
        {
            idToLabelMap[i] = byteArray.nextString();
            map.put(idToLabelMap[i], i);
        }

        return trie.build(map) == 0;
    }
}
