/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 23:07</create-date>
 *
 * <copyright file="BaseChineseDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.dictionary.ts;




/**
 * @author hankcs
 */
public class BaseChineseDictionary
{
    static void combineChain(Dictionary<string, string> s2t, Dictionary<string, string> t2x)
    {
        foreach (KeyValuePair<string, string> entry in s2t.entrySet())
        {
            string x = t2x.get(entry.getValue());
            if (x != null)
            {
                entry.setValue(x);
            }
        }
        foreach (KeyValuePair<string, string> entry in t2x.entrySet())
        {
            string s = CharTable.convert(entry.getKey());
            if (!s2t.ContainsKey(s))
            {
                s2t.put(s, entry.getValue());
            }
        }
    }

    static void combineReverseChain(Dictionary<string, string> t2s, Dictionary<string, string> tw2t, bool convert)
    {
        foreach (KeyValuePair<string, string> entry in tw2t.entrySet())
        {
            string tw = entry.getKey();
            string s = t2s.get(entry.getValue());
            if (s == null)
                s = convert ? CharTable.convert(entry.getValue()) : entry.getValue();
            t2s.put(tw, s);
        }
    }

    /**
     * 读取词典
     * @param storage 储存空间
     * @param reverse 是否翻转键值对
     * @param pathArray 路径
     * @return 是否加载成功
     */
    static bool load(Dictionary<string, string> storage, bool reverse, params string[] pathArray)
    {
        StringDictionary dictionary = new StringDictionary("=");
        foreach (string path in pathArray)
        {
            if (!dictionary.load(path)) return false;
        }
        if (reverse) dictionary = dictionary.reverse();
        HashSet<KeyValuePair<string, string>> entrySet = dictionary.entrySet();
        foreach (KeyValuePair<string, string> entry in entrySet)
        {
            storage.put(entry.getKey(), entry.getValue());
        }

        return true;
    }
    /**
     * 将path的内容载入trie中
     * @param path
     * @param trie
     * @return
     */
    static bool load(string path, AhoCorasickDoubleArrayTrie<string> trie)
    {
        return load(path, trie, false);
    }

    /**
     * 读取词典
     * @param path
     * @param trie
     * @param reverse 是否将其翻转
     * @return
     */
    static bool load(string path, AhoCorasickDoubleArrayTrie<string> trie, bool reverse)
    {
        string datPath = path;
        if (reverse)
        {
            datPath += Predefine.REVERSE_EXT;
        }
        if (loadDat(datPath, trie)) return true;
        // 从文本中载入并且尝试生成dat
        var map = new Dictionary<string, string>();
        if (!load(map, reverse, path)) return false;
        logger.info("正在构建AhoCorasickDoubleArrayTrie，来源：" + path);
        trie.build(map);
        logger.info("正在缓存双数组" + datPath);
        saveDat(datPath, trie, map.entrySet());
        return true;
    }

    static bool loadDat(string path, AhoCorasickDoubleArrayTrie<string> trie)
    {
        ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
        if (byteArray == null) return false;
        int size = byteArray.nextInt();
        string[] valueArray = new string[size];
        for (int i = 0; i < valueArray.Length; ++i)
        {
            valueArray[i] = byteArray.nextString();
        }
        trie.load(byteArray, valueArray);
        return true;
    }

    static bool saveDat(string path, AhoCorasickDoubleArrayTrie<string> trie, HashSet<KeyValuePair<string, string>> entrySet)
    {
        if (trie.size() != entrySet.size())
        {
            logger.warning("键值对不匹配");
            return false;
        }
        try
        {
            DataOutputStream _out = new DataOutputStream(new BufferedOutputStream(IOUtil.newOutputStream(path + Predefine.BIN_EXT)));
            _out.writeInt(entrySet.size());
            foreach (KeyValuePair<string, string> entry in entrySet)
            {
                char[] charArray = entry.getValue().ToCharArray();
                _out.writeInt(charArray.Length);
                foreach (char c in charArray)
                {
                    _out.writeChar(c);
                }
            }
            trie.save(_out);
            _out.close();
        }
        catch (Exception e)
        {
            logger.warning("缓存值dat" + path + "失败");
            return false;
        }

        return true;
    }

    public static BaseSearcher getSearcher(char[] charArray, DoubleArrayTrie<string> trie)
    {
        return new Searcher(charArray, trie);
    }

    protected static string segLongest(char[] charArray, DoubleArrayTrie<string> trie)
    {
        StringBuilder sb = new StringBuilder(charArray.Length);
        var searcher = getSearcher(charArray, trie);
        KeyValuePair<string, string> entry;
        int p = 0;  // 当前处理到什么位置
        int offset;
        while ((entry = searcher.next()) != null)
        {
            offset = searcher.getOffset();
            // 补足没查到的词
            while (p < offset)
            {
                sb.Append(charArray[p]);
                ++p;
            }
            sb.Append(entry.getValue());
            p = offset + entry.getKey().Length;
        }
        // 补足没查到的词
        while (p < charArray.Length)
        {
            sb.Append(charArray[p]);
            ++p;
        }
        return sb.ToString();
    }

    protected static string segLongest(char[] charArray, AhoCorasickDoubleArrayTrie<string> trie)
    {
         string[] wordNet = new string[charArray.Length];
         int[] lengthNet = new int[charArray.Length];
        trie.parseText(charArray, new ST());
        var sb = new StringBuilder(charArray.Length);
        for (int offset = 0; offset < wordNet.Length; )
        {
            if (wordNet[offset] == null)
            {
                sb.Append(charArray[offset]);
                ++offset;
                continue;
            }
            sb.Append(wordNet[offset]);
            offset += lengthNet[offset];
        }
        return sb.ToString();
    }
    public class ST: AhoCorasickDoubleArrayTrie<string>.IHit<string>
    {
        //@Override
        public void hit(int begin, int end, string value)
        {
            int Length = end - begin;
            if (Length > lengthNet[begin])
            {
                wordNet[begin] = value;
                lengthNet[begin] = Length;
            }
        }
    }
    /**
     * 最长分词
     */
    public class Searcher : BaseSearcher<string>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        DoubleArrayTrie<string> trie;

        protected Searcher(char[] c, DoubleArrayTrie<string> trie)
        :base(c)
        {
            this.trie = trie;
        }

        protected Searcher(string text, DoubleArrayTrie<string> trie)
        : base(c)
        {
            this.trie = trie;
        }

        //@Override
        public KeyValuePair<string, string> next()
        {
            // 保证首次调用找到一个词语
            KeyValuePair<string, string> result;
            while (begin < c.Length)
            {
                LinkedList<KeyValuePair<string, string>> entryList = trie.commonPrefixSearchWithValue(c, begin);
                if (entryList.size() == 0)
                {
                    ++begin;
                }
                else
                {
                    result = entryList.getLast();
                    offset = begin;
                    begin += result.getKey().Length;
                    break;
                }
            }
            return result;
        }
    }
}
