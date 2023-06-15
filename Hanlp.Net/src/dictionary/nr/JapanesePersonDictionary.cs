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
namespace com.hankcs.hanlp.dictionary.nr;




/**
 * @author hankcs
 */
public class JapanesePersonDictionary
{
    static string path = HanLP.Config.JapanesePersonDictionaryPath;
    static DoubleArrayTrie<Character> trie;
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

    static
    {
        long start = DateTime.Now.Microsecond;
        if (!load())
        {
            throw new IllegalArgumentException("日本人名词典" + path + "加载失败");
        }

        logger.info("日本人名词典" + HanLP.Config.PinyinDictionaryPath + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    static bool load()
    {
        trie = new DoubleArrayTrie<Character>();
        if (loadDat()) return true;
        try
        {
            BufferedReader br = new BufferedReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            string line;
            TreeMap<string, Character> map = new TreeMap<string, Character>();
            while ((line = br.readLine()) != null)
            {
                string[] param = line.split(" ", 2);
                map.put(param[0], param[1].charAt(0));
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
    static bool saveDat(TreeMap<string, Character> map)
    {
        try
        {
            DataOutputStream _out = new DataOutputStream(new BufferedOutputStream(IOUtil.newOutputStream(path + Predefine.VALUE_EXT)));
            _out.writeInt(map.size());
            for (Character character : map.values())
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
        Character[] valueArray = new Character[size];
        for (int i = 0; i < valueArray.length; ++i)
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
    public static bool containsKey(string key)
    {
        return trie.containsKey(key);
    }

    /**
     * 包含key，且key至少长length
     * @param key
     * @param length
     * @return
     */
    public static bool containsKey(string key, int length)
    {
        if (!trie.containsKey(key)) return false;
        return key.length() >= length;
    }

    public static Character get(string key)
    {
        return trie.get(key);
    }

    public static DoubleArrayTrie<Character>.LongestSearcher getSearcher(char[] charArray)
    {
        return trie.getLongestSearcher(charArray, 0);
    }

    /**
     * 最长分词
     */
    public static class Searcher : BaseSearcher<Character>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        DoubleArrayTrie<Character> trie;

        protected Searcher(char[] c, DoubleArrayTrie<Character> trie)
        {
            super(c);
            this.trie = trie;
        }

        protected Searcher(string text, DoubleArrayTrie<Character> trie)
        {
            super(text);
            this.trie = trie;
        }

        //@Override
        public KeyValuePair<string, Character> next()
        {
            // 保证首次调用找到一个词语
            KeyValuePair<string, Character> result = null;
            while (begin < c.length)
            {
                LinkedList<KeyValuePair<string, Character>> entryList = trie.commonPrefixSearchWithValue(c, begin);
                if (entryList.size() == 0)
                {
                    ++begin;
                }
                else
                {
                    result = entryList.getLast();
                    offset = begin;
                    begin += result.getKey().length();
                    break;
                }
            }
            if (result == null)
            {
                return null;
            }
            return result;
        }
    }
}
