/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/2 12:41</create-date>
 *
 * <copyright file="PinyinDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dictionary.py;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.py;




/**
 * @author hankcs
 */
public class PinyinDictionary
{
    static AhoCorasickDoubleArrayTrie<Pinyin[]> trie = new AhoCorasickDoubleArrayTrie<Pinyin[]>();
    public static readonly Pinyin[] pinyins = Integer2PinyinConverter.pinyins;

    static PinyinDictionary()
    {
        long start = DateTime.Now.Microsecond;
        if (!load(HanLP.Config.PinyinDictionaryPath))
        {
            throw new ArgumentException("拼音词典" + HanLP.Config.PinyinDictionaryPath + "加载失败");
        }

        logger.info("拼音词典" + HanLP.Config.PinyinDictionaryPath + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
    }

    /**
     * 读取词典
     * @param path
     * @return
     */
    static bool load(string path)
    {
        if (loadDat(path)) return true;
        // 从文本中载入并且尝试生成dat
        StringDictionary dictionary = new StringDictionary("=");
        if (!dictionary.load(path)) return false;
        var map = new Dictionary<string, Pinyin[]>();
        foreach (KeyValuePair<string, string> entry in dictionary)
        {
            string[] args = entry.getValue().Split(",");
            Pinyin[] pinyinValue = new Pinyin[args.Length];
            for (int i = 0; i < pinyinValue.Length; ++i)
            {
                try
                {
                    Pinyin pinyin = Pinyin.valueOf(args[i]);
                    pinyinValue[i] = pinyin;
                }
                catch (Exception e)
                {
                    logger.severe("读取拼音词典" + path + "失败，问题出在【" + entry + "】，异常是" + e);
                    return false;
                }
            }
            map.Add(entry.Key, pinyinValue);
        }
        trie.build(map);
        logger.info("正在缓存双数组" + path);
        saveDat(path, trie, map.ToHashSet());
        return true;
    }

    static bool loadDat(string path)
    {
        ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
        if (byteArray == null) return false;
        int size = byteArray.nextInt();
        Pinyin[][] valueArray = new Pinyin[size][];
        for (int i = 0; i < valueArray.Length; ++i)
        {
            int Length = byteArray.nextInt();
            valueArray[i] = new Pinyin[Length];
            for (int j = 0; j < Length; ++j)
            {
                valueArray[i][j] = pinyins[byteArray.nextInt()];
            }
        }
        if (!trie.load(byteArray, valueArray)) return false;
        return true;
    }

    static bool saveDat(string path, AhoCorasickDoubleArrayTrie<Pinyin[]> trie, HashSet<KeyValuePair<string, Pinyin[]>> entrySet)
    {
        try
        {
            var _out = new FileStream(IOUtil.newOutputStream(path + Predefine.BIN_EXT)));
            _out.writeInt(entrySet.size());
            foreach (KeyValuePair<string, Pinyin[]> entry in entrySet)
            {
                Pinyin[] value = entry.getValue();
                _out.writeInt(value.Length);
                foreach (Pinyin pinyin in value)
                {
                    _out.writeInt(pinyin.ordinal());
                }
            }
            trie.save(_out);
            _out.Close();
        }
        catch (Exception e)
        {
            logger.warning("缓存值dat" + path + "失败");
            return false;
        }

        return true;
    }

    public static Pinyin[] get(string key)
    {
        return trie.get(key);
    }

    /**
     * 转为拼音
     * @param text
     * @return List形式的拼音，对应每一个字（所谓字，指的是任意字符）
     */
    public static List<Pinyin> convertToPinyin(string text)
    {
        return segLongest(text.ToCharArray(), trie);
    }

    public static List<Pinyin> convertToPinyin(string text, bool remainNone)
    {
        return segLongest(text.ToCharArray(), trie, remainNone);
    }

    /**
     * 转为拼音
     * @param text
     * @return 数组形式的拼音
     */
    public static Pinyin[] convertToPinyinArray(string text)
    {
        return convertToPinyin(text).toArray(new Pinyin[0]);
    }

    public static BaseSearcher getSearcher(char[] charArray, DoubleArrayTrie<Pinyin[]> trie)
    {
        return new Searcher(charArray, trie);
    }

    /**
     * 用最长分词算法匹配拼音
     * @param charArray
     * @param trie
     * @return
     */
    protected static List<Pinyin> segLongest(char[] charArray, AhoCorasickDoubleArrayTrie<Pinyin[]> trie)
    {
        return segLongest(charArray, trie, true);
    }

    protected static List<Pinyin> segLongest(char[] charArray, AhoCorasickDoubleArrayTrie<Pinyin[]> trie, bool remainNone)
    {
        Pinyin[][] wordNet = new Pinyin[charArray.Length][];
        trie.parseText(charArray, new CT());
        List<Pinyin> pinyinList = new (charArray.Length);
        for (int offset = 0; offset < wordNet.Length; )
        {
            if (wordNet[offset] == null)
            {
                if (remainNone)
                {
                    pinyinList.Add(Pinyin.none5);
                }
                ++offset;
                continue;
            }
            foreach (Pinyin pinyin in wordNet[offset])
            {
                pinyinList.Add(pinyin);
            }
            offset += wordNet[offset].Length;
        }
        return pinyinList;
    }
    public class CT: AhoCorasickDoubleArrayTrie<Pinyin[]>.IHit<Pinyin[]>
    {
        //@Override
        public void hit(int begin, int end, Pinyin[] value)
        {
            int Length = end - begin;
            if (wordNet[begin] == null || Length > wordNet[begin].Length)
            {
                wordNet[begin] = Length == 1 ? new Pinyin[] { value[0] } : value;
            }
        }
    }
    public class Searcher : BaseSearcher<Pinyin[]>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        DoubleArrayTrie<Pinyin[]> trie;

        protected Searcher(char[] c, DoubleArrayTrie<Pinyin[]> trie)
            :base(c)
        {
            this.trie = trie;
        }

        protected Searcher(string text, DoubleArrayTrie<Pinyin[]> trie)
            :base(text)
        {
            this.trie = trie;
        }

        //@Override
        public KeyValuePair<string, Pinyin[]> next()
        {
            // 保证首次调用找到一个词语
            KeyValuePair<string, Pinyin[]> result = null;
            while (begin < c.Length)
            {
                List<KeyValuePair<string, Pinyin[]>> entryList = trie.commonPrefixSearchWithValue(c, begin);
                if (entryList.size() == 0)
                {
                    ++begin;
                }
                else
                {
                    result = entryList.getLast();
                    offset = begin;
                    begin += result.Key.Length;
                    break;
                }
            }
            return result;
        }
    }
}
