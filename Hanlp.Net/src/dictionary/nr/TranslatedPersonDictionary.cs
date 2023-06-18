/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/12 14:45</create-date>
 *
 * <copyright file="TranslatedPersonDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.nr;




/**
 * 翻译人名词典，储存和识别翻译人名
 * @author hankcs
 */
public class TranslatedPersonDictionary
{
    static string path = HanLP.Config.TranslatedPersonDictionaryPath;
    static DoubleArrayTrie<Boolean> trie;

    static TranslatedPersonDictionary()
    {
        long start = DateTime.Now.Microsecond;
        if (!load())
        {
            throw new ArgumentException("音译人名词典" + path + "加载失败");
        }

        logger.info("音译人名词典" + path + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    static bool load()
    {
        trie = new DoubleArrayTrie<Boolean>();
        if (loadDat()) return true;
        try
        {
            BufferedReader br = new BufferedReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            string line;
            Dictionary<string, Boolean> map = new Dictionary<string, Boolean>();
            Dictionary<char, int> charFrequencyMap = new Dictionary<char, int>();
            while ((line = br.readLine()) != null)
            {
                map.put(line, true);
                // 音译人名常用字词典自动生成
                for (char c : line.ToCharArray())
                {
                    // 排除一些过于常用的字
                    if ("不赞".IndexOf(c) >= 0) continue;
                    int f = charFrequencyMap.get(c);
                    if (f == null) f = 0;
                    charFrequencyMap.put(c, f + 1);
                }
            }
            br.close();
            map.put(string.valueOf('·'), true);
//            map.put(string.valueOf('-'), true);
//            map.put(string.valueOf('—'), true);
            // 将常用字也加进去
            for (KeyValuePair<char, int> entry : charFrequencyMap.entrySet())
            {
                if (entry.getValue() < 10) continue;
                map.put(string.valueOf(entry.getKey()), true);
            }
            logger.info("音译人名词典" + path + "开始构建双数组……");
            trie.build(map);
            logger.info("音译人名词典" + path + "开始编译DAT文件……");
            logger.info("音译人名词典" + path + "编译结果：" + saveDat());
        }
        catch (Exception e)
        {
            logger.severe("自定义词典" + path + "读取错误！" + e);
            return false;
        }

        return true;
    }

    /**
     * 保存dat到磁盘
     * @return
     */
    static bool saveDat()
    {
        return trie.save(path + Predefine.TRIE_EXT);
    }

    static bool loadDat()
    {
        return trie.load(path + Predefine.TRIE_EXT);
    }

    /**
     * 是否包含key
     * @param key
     * @return
     */
    public static bool ContainsKey(string key)
    {
        return trie.ContainsKey(key);
    }

    /**
     * 时报包含key，且key至少长Length
     * @param key
     * @param Length
     * @return
     */
    public static bool ContainsKey(string key, int Length)
    {
        if (!trie.ContainsKey(key)) return false;
        return key.Length >= Length;
    }
}
