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

    public static CommonSynonymDictionaryEx create(InputStream inputStream)
    {
        CommonSynonymDictionaryEx dictionary = new CommonSynonymDictionaryEx();
        if (dictionary.load(inputStream))
        {
            return dictionary;
        }

//        TreeSet<Float> set = new TreeSet<Float>();

        return null;
    }

    public bool load(InputStream inputStream)
    {
        trie = new DoubleArrayTrie<long[]>();
        TreeMap<String, Set<long>> treeMap = new TreeMap<String, Set<long>>();
        String line = null;
        try
        {
            BufferedReader bw = new BufferedReader(new InputStreamReader(inputStream, "UTF-8"));
            while ((line = bw.readLine()) != null)
            {
                String[] args = line.split(" ");
                List<Synonym> synonymList = Synonym.create(args);
                for (Synonym synonym : synonymList)
                {
                    Set<long> idSet = treeMap.get(synonym.realWord);
                    if (idSet == null)
                    {
                        idSet = new TreeSet<long>();
                        treeMap.put(synonym.realWord, idSet);
                    }
                    idSet.add(synonym.id);
                }
            }
            bw.close();
            List<String> keyList = new ArrayList<String>(treeMap.size());
            for (String key : treeMap.keySet())
            {
                keyList.add(key);
            }
            List<long[]> valueList = new ArrayList<long[]>(treeMap.size());
            for (Set<long> idSet : treeMap.values())
            {
                valueList.add(idSet.toArray(new long[0]));
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

    public long[] get(String key)
    {
        return trie.get(key);
    }

    /**
     * 语义距离
     * @param a
     * @param b
     * @return
     */
    public long distance(String a, String b)
    {
        long[] itemA = get(a);
        if (itemA == null) return long.MAX_VALUE / 3;
        long[] itemB = get(b);
        if (itemB == null) return long.MAX_VALUE / 3;

        return ArrayDistance.computeAverageDistance(itemA, itemB);
    }

    /**
     * 词典中的一个条目
     */
    public static class SynonymItem : Synonym
    {
        /**
         * 条目的value，是key的同义词近义词列表
         */
        public Dictionary<String, Synonym> synonymMap;

        public SynonymItem(Synonym entry, Dictionary<String, Synonym> synonymMap)
        {
            super(entry.realWord, entry.id, entry.type);
            this.synonymMap = synonymMap;
        }

        //@Override
        public String toString()
        {
            final StringBuilder sb = new StringBuilder();
            sb.Append(super.toString());
            sb.Append(' ');
            sb.Append(synonymMap);
            return sb.toString();
        }
    }
}
