/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/12 20:17</create-date>
 *
 * <copyright file="JapanesePersonDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.nr;




/**
 * @author hankcs
 */
public class JapanesePersonDictionary
{
    static string path = HanLP.Config.JapanesePersonDictionaryPath;
    static DoubleArrayTrie<char> trie;
    /**
     * 姓
     */
    public static readonly char X = 'x';
    /**
     * 名
     */
    public static readonly char M = 'm';
    /**
     * bad case
     */
    public static readonly char A = 'A';

    static JapanesePersonDictionary()
    {
        long start = DateTime.Now.Microsecond;
        if (!load())
        {
            throw new ArgumentException("日本人名词典" + path + "加载失败");
        }

        logger.info("日本人名词典" + HanLP.Config.PinyinDictionaryPath + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    static bool load()
    {
        trie = new DoubleArrayTrie<char>();
        if (loadDat()) return true;
        try
        {
            BufferedReader br = new BufferedReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            string line;
            Dictionary<string, char> map = new Dictionary<string, char>();
            while ((line = br.readLine()) != null)
            {
                string[] param = line.Split(" ", 2);
                map.Add(param[0], param[1][0]);
            }
            br.close();
            logger.info("日本人名词典" + path + "开始构建双数组……");
            trie.build(map);
            logger.info("日本人名词典" + path + "开始编译DAT文件……");
            logger.info("日本人名词典" + path + "编译结果：" + saveDat(map));
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
     * @param map
     * @return
     */
    static bool saveDat(Dictionary<string, char> map)
    {
        try
        {
            DataOutputStream _out = new DataOutputStream(new BufferedOutputStream(IOUtil.newOutputStream(path + Predefine.VALUE_EXT)));
            _out.writeInt(map.size());
            foreach (char character in map.values())
            {
                _out.writeChar(character);
            }
            _out.close();
        }
        catch (Exception e)
        {
            logger.warning("保存值" + path + Predefine.VALUE_EXT + "失败" + e);
            return false;
        }
        return trie.save(path + Predefine.TRIE_EXT);
    }

    static bool loadDat()
    {
        ByteArray byteArray = ByteArray.createByteArray(path + Predefine.VALUE_EXT);
        if (byteArray == null) return false;
        int size = byteArray.nextInt();
        char[] valueArray = new char[size];
        for (int i = 0; i < valueArray.Length; ++i)
        {
            valueArray[i] = byteArray.nextChar();
        }
        return trie.load(path + Predefine.TRIE_EXT, valueArray);
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
     * 包含key，且key至少长Length
     * @param key
     * @param Length
     * @return
     */
    public static bool ContainsKey(string key, int Length)
    {
        if (!trie.ContainsKey(key)) return false;
        return key.Length >= Length;
    }

    public static char get(string key)
    {
        return trie.get(key);
    }

    public static DoubleArrayTrie<char>.LongestSearcher getSearcher(char[] charArray)
    {
        return trie.getLongestSearcher(charArray, 0);
    }

    /**
     * 最长分词
     */
    public class Searcher : BaseSearcher<char>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        DoubleArrayTrie<char> trie;

        protected Searcher(char[] c, DoubleArrayTrie<char> trie)
            :base(c)
        {
            this.trie = trie;
        }

        protected Searcher(string text, DoubleArrayTrie<char> trie)
            : base(text.ToCharArray())
        {
            this.trie = trie;
        }

        //@Override
        public override KeyValuePair<string, char> next()
        {
            // 保证首次调用找到一个词语
            KeyValuePair<string, char> result = new();
            while (begin < c.Length)
            {
                var entryList = trie.commonPrefixSearchWithValue(c, begin);
                if (entryList.Count == 0)
                {
                    ++begin;
                }
                else
                {
                    result = entryList[^1];
                    offset = begin;
                    begin += result.Key.Length;
                    break;
                }
            }

            return result;
        }
    }
}
