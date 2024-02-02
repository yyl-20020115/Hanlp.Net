/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/23 20:07</create-date>
 *
 * <copyright file="CoreDictionaryACDAT.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.dictionary;




/**
 * 使用DoubleArrayTrie实现的核心词典
 * @author hankcs
 */
public class CoreDictionary
{
    public static DoubleArrayTrie<Attribute> trie = new DoubleArrayTrie<Attribute>();
    public readonly static string path = HanLP.Config.CoreDictionaryPath;
    public static readonly int totalFrequency = 221894;

    // 自动加载词典
    static CoreDictionary()
    {
        long start = DateTime.Now.Microsecond;
        if (!load(path))
        {
            throw new ArgumentException("核心词典" + path + "加载失败");
        }
        else
        {
            logger.info(path + "加载成功，" + trie.Count + "个词条，耗时" + (DateTime.Now.Microsecond - start) + "ms");
        }
    }

    // 一些特殊的WORD_ID
    public static readonly int NR_WORD_ID = getWordID(Predefine.TAG_PEOPLE);
    public static readonly int NS_WORD_ID = getWordID(Predefine.TAG_PLACE);
    public static readonly int NT_WORD_ID = getWordID(Predefine.TAG_GROUP);
    public static readonly int T_WORD_ID = getWordID(Predefine.TAG_TIME);
    public static readonly int X_WORD_ID = getWordID(Predefine.TAG_CLUSTER);
    public static readonly int M_WORD_ID = getWordID(Predefine.TAG_NUMBER);
    public static readonly int NX_WORD_ID = getWordID(Predefine.TAG_PROPER);

    private static bool load(string path)
    {
        logger.info("核心词典开始加载:" + path);
        if (loadDat(path)) return true;
        Dictionary<string, CoreDictionary.Attribute> map = new Dictionary<string, Attribute>();
        TextReader br = null;
        try
        {
            br = new StreamReader(IOUtil.newInputStream(path), "UTF-8");
            string line;
            int MAX_FREQUENCY = 0;
            long start = DateTime.Now.Microsecond;
            while ((line = br.ReadLine()) != null)
            {
                string[] param = line.Split("\\s");
                int natureCount = (param.Length - 1) / 2;
                CoreDictionary.Attribute attribute = new CoreDictionary.Attribute(natureCount);
                for (int i = 0; i < natureCount; ++i)
                {
                    attribute.nature[i] = Nature.create(param[1 + 2 * i]);
                    attribute.frequency[i] = int.parseInt(param[2 + 2 * i]);
                    attribute.totalFrequency += attribute.frequency[i];
                }
                map.Add(param[0], attribute);
                MAX_FREQUENCY += attribute.totalFrequency;
            }
            logger.info("核心词典读入词条" + map.Count + " 全部频次" + MAX_FREQUENCY + "，耗时" + (DateTime.Now.Microsecond - start) + "ms");
            br.Close();
            trie.build(map);
            logger.info("核心词典加载成功:" + trie.Count + "个词条，下面将写入缓存……");
            try
            {
                Stream Out = new Stream(new BufferedOutputStream(IOUtil.newOutputStream(path + Predefine.BIN_EXT)));
                ICollection<CoreDictionary.Attribute> attributeList = map.values();
                Out.writeInt(attributeList.Count);
                foreach (CoreDictionary.Attribute attribute in attributeList)
                {
                    Out.writeInt(attribute.totalFrequency);
                    Out.writeInt(attribute.nature.Length);
                    for (int i = 0; i < attribute.nature.Length; ++i)
                    {
                        Out.writeInt(attribute.nature[i].Ordinal);
                        Out.writeInt(attribute.frequency[i]);
                    }
                }
                trie.save(Out);
                Out.Close();
            }
            catch (Exception e)
            {
                logger.warning("保存失败" + e);
                return false;
            }
        }
        catch (FileNotFoundException e)
        {
            logger.warning("核心词典" + path + "不存在！" + e);
            return false;
        }
        catch (IOException e)
        {
            logger.warning("核心词典" + path + "读取错误！" + e);
            return false;
        }

        return true;
    }

    /**
     * 从磁盘加载双数组
     *
     * @param path
     * @return
     */
    static bool loadDat(string path)
    {
        try
        {
            ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
            if (byteArray == null) return false;
            int size = byteArray.Next();
            CoreDictionary.Attribute[] attributes = new CoreDictionary.Attribute[size];
            Nature[] natureIndexArray = Nature.values();
            for (int i = 0; i < size; ++i)
            {
                // 第一个是全部频次，第二个是词性个数
                int currentTotalFrequency = byteArray.Next();
                int Length = byteArray.Next();
                attributes[i] = new CoreDictionary.Attribute(Length);
                attributes[i].totalFrequency = currentTotalFrequency;
                for (int j = 0; j < Length; ++j)
                {
                    attributes[i].nature[j] = natureIndexArray[byteArray.Next()];
                    attributes[i].frequency[j] = byteArray.Next();
                }
            }
            if (!trie.load(byteArray, attributes) || byteArray.hasMore()) return false;
        }
        catch (Exception e)
        {
            logger.warning("读取失败，问题发生在" + e);
            return false;
        }
        return true;
    }

    /**
     * 获取条目
     * @param key
     * @return
     */
    public static Attribute get(string key)
    {
        return trie.get(key);
    }

    /**
     * 获取条目
     * @param wordID
     * @return
     */
    public static Attribute get(int wordID)
    {
        return trie.get(wordID);
    }

    /**
     * 获取词频
     *
     * @param term
     * @return
     */
    public static int getTermFrequency(string term)
    {
        Attribute attribute = get(term);
        if (attribute == null) return 0;
        return attribute.totalFrequency;
    }

    /**
     * 是否包含词语
     * @param key
     * @return
     */
    public static bool Contains(string key)
    {
        return trie.get(key) != null;
    }

    /**
     * 核心词典中的词属性
     */
    public class Attribute : Serializable
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

        // 几个预定义的变量

//        public static Attribute NUMBER = new Attribute()

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
            : this(1)
        {
            this.nature[0] = nature;
            this.frequency[0] = frequency;
            totalFrequency = frequency;
        }

        public Attribute(Nature[] nature, int[] frequency, int totalFrequency)
        {
            this.nature = nature;
            this.frequency = frequency;
            this.totalFrequency = totalFrequency;
        }

        /**
         * 使用单个词性，默认词频1000构造
         *
         * @param nature
         */
        public Attribute(Nature nature)
            : this(nature, 1000)
        {
        }

        public static Attribute create(string natureWithFrequency)
        {
            try
            {
                string[] param = natureWithFrequency.Split(" ");
                if (param.Length % 2 != 0)
                {
                    return new Attribute(Nature.create(natureWithFrequency.Trim()), 1); // 儿童锁
                }
                int natureCount = param.Length / 2;
                Attribute attribute = new Attribute(natureCount);
                for (int i = 0; i < natureCount; ++i)
                {
                    attribute.nature[i] = Nature.create(param[2 * i]);
                    attribute.frequency[i] = int.parseInt(param[1 + 2 * i]);
                    attribute.totalFrequency += attribute.frequency[i];
                }
                return attribute;
            }
            catch (Exception e)
            {
                logger.warning("使用字符串" + natureWithFrequency + "创建词条属性失败！" + TextUtility.exceptionToString(e));
                return null;
            }
        }

        /**
         * 从字节流中加载
         * @param byteArray
         * @param natureIndexArray
         * @return
         */
        public static Attribute create(ByteArray byteArray, Nature[] natureIndexArray)
        {
            int currentTotalFrequency = byteArray.Next();
            int Length = byteArray.Next();
            Attribute attribute = new Attribute(Length);
            attribute.totalFrequency = currentTotalFrequency;
            for (int j = 0; j < Length; ++j)
            {
                attribute.nature[j] = natureIndexArray[byteArray.Next()];
                attribute.frequency[j] = byteArray.Next();
            }

            return attribute;
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
            catch (ArgumentException e)
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
        public int getNatureFrequency(Nature nature)
        {
            int i = 0;
            foreach (Nature pos in this.nature)
            {
                if (nature == pos)
                {
                    return frequency[i];
                }
                ++i;
            }
            return 0;
        }

        /**
         * 是否有某个词性
         * @param nature
         * @return
         */
        public bool hasNature(Nature nature)
        {
            return getNatureFrequency(nature) > 0;
        }

        /**
         * 是否有以某个前缀开头的词性
         * @param prefix 词性前缀，比如u会查询是否有ude, uzhe等等
         * @return
         */
        public bool hasNatureStartsWith(string prefix)
        {
            foreach (Nature n in nature)
            {
                if (n.StartsWith(prefix)) return true;
            }
            return false;
        }

        //@Override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nature.Length; ++i)
            {
                sb.Append(nature[i]).Append(' ').Append(frequency[i]).Append(' ');
            }
            return sb.ToString();
        }

        public void save(Stream Out) 
        {
            Out.writeInt(totalFrequency);
            Out.writeInt(nature.Length);
            for (int i = 0; i < nature.Length; ++i)
            {
                Out.writeInt(nature[i].Ordinal);
                Out.writeInt(frequency[i]);
            }
        }
    }

    /**
     * 获取词语的ID
     * @param a 词语
     * @return ID,如果不存在,则返回-1
     */
    public static int getWordID(string a)
    {
        return CoreDictionary.trie.exactMatchSearch(a);
    }
}
