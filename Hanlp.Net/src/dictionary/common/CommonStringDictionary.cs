/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/18 17:16</create-date>
 *
 * <copyright file="CommonStringDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dictionary.common;





/**
 * 最简单的词典，每一行只有一个词，没别的
 * @author hankcs
 */
public class CommonStringDictionary
{
    BinTrie<Byte> trie;
    public bool load(string path)
    {
        trie = new BinTrie<Byte>();
        if (loadDat(path + Predefine.TRIE_EXT)) return true;
        string line = null;
        try
        {
            BufferedReader bw = new BufferedReader(new InputStreamReader(IOUtil.newInputStream(path)));
            while ((line = bw.readLine()) != null)
            {
                trie.put(line, null);
            }
            bw.close();
        }
        catch (Exception e)
        {
            logger.warning("加载" + path + "失败，" + e);
        }
        if (!trie.save(path + Predefine.TRIE_EXT)) logger.warning("缓存" + path + Predefine.TRIE_EXT + "失败");
        return true;
    }

    bool loadDat(string path)
    {
        return trie.load(path);
    }

    public HashSet<string> keySet()
    {
        HashSet<string> keySet = new LinkedHashSet<string>();
        for (KeyValuePair<string, Byte> entry : trie.entrySet())
        {
            keySet.Add(entry.getKey());
        }

        return keySet;
    }
}
