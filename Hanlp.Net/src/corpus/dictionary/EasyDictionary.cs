/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 3:11</create-date>
 *
 * <copyright file="EasyDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;

namespace com.hankcs.hanlp.corpus.dictionary;




/**
 * 一个通用的、满足特定格式的双数组词典
 *
 * @author hankcs
 */
public class EasyDictionary
{
    DoubleArrayTrie<Attribute> trie = new DoubleArrayTrie<Attribute>();

    public static EasyDictionary create(string path)
    {
        EasyDictionary dictionary = new EasyDictionary();
        if (dictionary.load(path))
        {
            return dictionary;
        }
        else
        {
            logger.warning("从" + path + "读取失败");
        }

        return null;
    }

    private bool load(string path)
    {
        logger.info("通用词典开始加载:" + path);
        var map = new Dictionary<string, Attribute>();
        BufferedReader br = null;
        try
        {
            br = new BufferedReader(new InputStreamReader(IOAdapter == null ? new FileInputStream(path) : IOAdapter.open(path), "UTF-8"));
            string line;
            while ((line = br.readLine()) != null)
            {
                string param[] = line.Split("\\s+");
                int natureCount = (param.length - 1) / 2;
                Attribute attribute = new Attribute(natureCount);
                for (int i = 0; i < natureCount; ++i)
                {
                    attribute.nature[i] = Nature.create(param[1 + 2 * i]);
                    attribute.frequency[i] = int.parseInt(param[2 + 2 * i]);
                    attribute.totalFrequency += attribute.frequency[i];
                }
                map.put(param[0], attribute);
            }
            logger.info("通用词典读入词条" + map.size());
            br.close();
        }
        catch (FileNotFoundException e)
        {
            logger.severe("通用词典" + path + "不存在！" + e);
            return false;
        }
        catch (IOException e)
        {
            logger.severe("通用词典" + path + "读取错误！" + e);
            return false;
        }

        logger.info("通用词典DAT构建结果:" + trie.build(map));
        logger.info("通用词典加载成功:" + trie.size() +"个词条" );
        return true;
    }

    public Attribute GetWordInfo(string key)
    {
        return trie.get(key);
    }

    public bool contains(string key)
    {
        return GetWordInfo(key) != null;
    }

    public BaseSearcher getSearcher(string text)
    {
        return new Searcher(text);
    }

    public class Searcher : BaseSearcher<Attribute>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        private List<KeyValuePair<string, Attribute>> entryList;

        protected Searcher(char[] c)
            :base(c)
        {
        }

        protected Searcher(string text)
            :base(text)
        {
            entryList = new ();
        }

        //@Override
        public KeyValuePair<string, Attribute> next()
        {
            // 保证首次调用找到一个词语
            while (entryList.size() == 0 && begin < c.length)
            {
                entryList = trie.commonPrefixSearchWithValue(c, begin);
                ++begin;
            }
            // 之后调用仅在缓存用完的时候调用一次
            if (entryList.size() == 0 && begin < c.length)
            {
                entryList = trie.commonPrefixSearchWithValue(c, begin);
                ++begin;
            }
            if (entryList.size() == 0)
            {
                return null;
            }
            KeyValuePair<string, Attribute> result = entryList.get(0);
            entryList.remove(0);
            offset = begin - 1;
            return result;
        }
    }

    /**
     * 通用词典中的词属性
     */
     public class Attribute
    {
        /**
         * 词性列表
         */
        public Nature[] nature;
        /**
         * 词性对应的词频
         */
        public int[] frequency;

        public int totalFrequency;

        public Attribute(int size)
        {
            nature = new Nature[size];
            frequency = new int[size];
        }

        public Attribute(Nature[] nature, int[] frequency)
        {
            this.nature = nature;
            this.frequency = frequency;
        }

        public Attribute(Nature nature, int frequency)
            :this(1)
        {
            this.nature[0] = nature;
            this.frequency[0] = frequency;
            totalFrequency = frequency;
        }

        /**
         * 使用单个词性，默认词频1000构造
         *
         * @param nature
         */
        public Attribute(Nature nature)
            :this(nature,1000)
        {
        }

        /**
         * 获取词性的词频
         *
         * @param nature 字符串词性
         * @return 词频
         * @deprecated 推荐使用Nature参数！
         */
        public int getNatureFrequency(string nature)
        {
            try
            {
                Nature pos = Nature.create(nature);
                return getNatureFrequency(pos);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        /**
         * 获取词性的词频
         *
         * @param nature 词性
         * @return 词频
         */
        public int getNatureFrequency( Nature nature)
        {
            int result = 0;
            int i = 0;
            foreach (Nature pos in this.nature)
            {
                if (nature == pos)
                {
                    return frequency[i];
                }
                ++i;
            }
            return result;
        }

        //@Override
        public string toString()
        {
            return "Attribute{" +
                    "nature=" + Arrays.toString(nature) +
                    ", frequency=" + Arrays.toString(frequency) +
                    '}';
        }
    }
}
