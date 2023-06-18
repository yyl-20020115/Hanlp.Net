/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/5/10 12:42</create-date>
 *
 * <copyright file="WordDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary;





/**
 * 用户自定义词典<br>
 *     注意自定义词典的动态增删改不是线程安全的。
 *
 * @author He Han
 */
public class CustomDictionary
{
    /**
     * 用于储存用户动态插入词条的二分trie树
     */
    public static BinTrie<CoreDictionary.Attribute> trie;
    public static DoubleArrayTrie<CoreDictionary.Attribute> dat = new DoubleArrayTrie<CoreDictionary.Attribute>();

    // 自动加载词典
    static CustomDictionary()
    {
        string[] path = HanLP.Config.CustomDictionaryPath;
        long start = DateTime.Now.Microsecond;
        if (!loadMainDictionary(path[0]))
        {
            logger.warning("自定义词典" + Arrays.toString(path) + "加载失败");
        }
        else
        {
            logger.info("自定义词典加载成功:" + dat.size() + "个词条，耗时" + (DateTime.Now.Microsecond - start) + "ms");
        }
    }

    /**
     * 加载词典
     * @param mainPath 缓存文件文件名
     * @param path 自定义词典
     * @param isCache 是否缓存结果
     */
    public static bool loadMainDictionary(string mainPath, string[] path, DoubleArrayTrie<CoreDictionary.Attribute> dat, bool isCache)
    {
        logger.info("自定义词典开始加载:" + mainPath);
        if (loadDat(mainPath, dat)) return true;
        var map = new Dictionary<string, CoreDictionary.Attribute>();
        HashSet<Nature> customNatureCollector = new();
        try
        {
            //string path[] = HanLP.Config.CustomDictionaryPath;
            foreach (string p in path)
            {
                Nature defaultNature = Nature.n;
                File file = new File(p);
                string fileName = file.getName();
                int cut = fileName.lastIndexOf(' ');
                if (cut > 0)
                {
                    // 有默认词性
                    string nature = fileName.substring(cut + 1);
                    p = file.getParent() + File.separator + fileName.substring(0, cut);
                    try
                    {
                        defaultNature = LexiconUtility.convertStringToNature(nature, customNatureCollector);
                    }
                    catch (Exception e)
                    {
                        logger.severe("配置文件【" + p + "】写错了！" + e);
                        continue;
                    }
                }
                logger.info("以默认词性[" + defaultNature + "]加载自定义词典" + p + "中……");
                bool success = load(p, defaultNature, map, customNatureCollector);
                if (!success) logger.warning("失败：" + p);
            }
            if (map.size() == 0)
            {
                logger.warning("没有加载到任何词条");
                map.put(Predefine.TAG_OTHER, null);     // 当作空白占位符
            }
            logger.info("正在构建DoubleArrayTrie……");
            dat.build(map);
            if (isCache)
            {
                // 缓存成dat文件，下次加载会快很多
                logger.info("正在缓存词典为dat文件……");
                // 缓存值文件
                List<CoreDictionary.Attribute> attributeList = new LinkedList<CoreDictionary.Attribute>();
                for (KeyValuePair<string, CoreDictionary.Attribute> entry : map.entrySet())
                {
                    attributeList.Add(entry.getValue());
                }
                DataOutputStream _out = new DataOutputStream(new BufferedOutputStream(IOUtil.newOutputStream(mainPath + Predefine.BIN_EXT)));
                // 缓存用户词性
                if (customNatureCollector.isEmpty()) // 热更新
                {
                    for (int i = Nature.begin.ordinal() + 1; i < Nature.values().Length; ++i)
                    {
                        customNatureCollector.Add(Nature.values()[i]);
                    }
                }
                IOUtil.writeCustomNature(_out, customNatureCollector);
                // 缓存正文
                _out.writeInt(attributeList.size());
                for (CoreDictionary.Attribute attribute : attributeList)
                {
                    attribute.save(_out);
                }
                dat.save(_out);
                _out.close();
            }
        }
        catch (FileNotFoundException e)
        {
            logger.severe("自定义词典" + mainPath + "不存在！" + e);
            return false;
        }
        catch (IOException e)
        {
            logger.severe("自定义词典" + mainPath + "读取错误！" + e);
            return false;
        }
        catch (Exception e)
        {
            logger.warning("自定义词典" + mainPath + "缓存失败！\n" + TextUtility.exceptionToString(e));
        }
        return true;
    }

    private static bool loadMainDictionary(string mainPath)
    {
        return loadMainDictionary(mainPath, HanLP.Config.CustomDictionaryPath, CustomDictionary.dat, true);
    }


    /**
     * 加载用户词典（追加）
     *
     * @param path          词典路径
     * @param defaultNature 默认词性
     * @param customNatureCollector 收集用户词性
     * @return
     */
    public static bool load(string path, Nature defaultNature, Dictionary<string, CoreDictionary.Attribute> map, HashSet<Nature> customNatureCollector)
    {
        try
        {
            string splitter = "\\s";
            if (path.EndsWith(".csv"))
            {
                splitter = ",";
            }
            BufferedReader br = new BufferedReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            string line;
            bool firstLine = true;
            while ((line = br.readLine()) != null)
            {
                if (firstLine)
                {
                    line = IOUtil.removeUTF8BOM(line);
                    firstLine = false;
                }
                string[] param = line.Split(splitter);
                if (param[0].Length == 0) continue;   // 排除空行
                if (HanLP.Config.Normalization) param[0] = CharTable.convert(param[0]); // 正规化

                int natureCount = (param.Length - 1) / 2;
                CoreDictionary.Attribute attribute;
                if (natureCount == 0)
                {
                    attribute = new CoreDictionary.Attribute(defaultNature);
                }
                else
                {
                    attribute = new CoreDictionary.Attribute(natureCount);
                    for (int i = 0; i < natureCount; ++i)
                    {
                        attribute.nature[i] = LexiconUtility.convertStringToNature(param[1 + 2 * i], customNatureCollector);
                        attribute.frequency[i] = int.parseInt(param[2 + 2 * i]);
                        attribute.totalFrequency += attribute.frequency[i];
                    }
                }
                //                if (updateAttributeIfExist(param[0], attribute, map, rewriteTable)) continue;
                map.Add(param[0], attribute);
            }
            br.close();
        }
        catch (Exception e)
        {
            logger.severe("自定义词典" + path + "读取错误！" + e);
            return false;
        }

        return true;
    }

    /**
     * 如果已经存在该词条,直接更新该词条的属性
     * @param key 词语
     * @param attribute 词语的属性
     * @param map 加载期间的map
     * @param rewriteTable
     * @return 是否更新了
     */
    private static bool updateAttributeIfExist(string key, CoreDictionary.Attribute attribute, Dictionary<string, CoreDictionary.Attribute> map, Dictionary<int, CoreDictionary.Attribute> rewriteTable)
    {
        int wordID = CoreDictionary.getWordID(key);
        CoreDictionary.Attribute attributeExisted;
        if (wordID != -1)
        {
            attributeExisted = CoreDictionary.get(wordID);
            attributeExisted.nature = attribute.nature;
            attributeExisted.frequency = attribute.frequency;
            attributeExisted.totalFrequency = attribute.totalFrequency;
            // 收集该覆写
            rewriteTable.Add(wordID, attribute);
            return true;
        }

        attributeExisted = map.get(key);
        if (attributeExisted != null)
        {
            attributeExisted.nature = attribute.nature;
            attributeExisted.frequency = attribute.frequency;
            attributeExisted.totalFrequency = attribute.totalFrequency;
            return true;
        }

        return false;
    }

    /**
     * 往自定义词典中插入一个新词（非覆盖模式）<br>
     *     动态增删不会持久化到词典文件
     *
     * @param word                新词 如“裸婚”
     * @param natureWithFrequency 词性和其对应的频次，比如“nz 1 v 2”，null时表示“nz 1”
     * @return 是否插入成功（失败的原因可能是不覆盖、natureWithFrequency有问题等，后者可以通过调试模式了解原因）
     */
    public static bool Add(string word, string natureWithFrequency)
    {
        if (contains(word)) return false;
        return insert(word, natureWithFrequency);
    }

    /**
     * 往自定义词典中插入一个新词（非覆盖模式）<br>
     *     动态增删不会持久化到词典文件
     *
     * @param word                新词 如“裸婚”
     * @return 是否插入成功（失败的原因可能是不覆盖等，可以通过调试模式了解原因）
     */
    public static bool Add(string word)
    {
        if (HanLP.Config.Normalization) word = CharTable.convert(word);
        if (contains(word)) return false;
        return insert(word, null);
    }

    /**
     * 往自定义词典中插入一个新词（覆盖模式）<br>
     *     动态增删不会持久化到词典文件
     *
     * @param word                新词 如“裸婚”
     * @param natureWithFrequency 词性和其对应的频次，比如“nz 1 v 2”，null时表示“nz 1”。
     * @return 是否插入成功（失败的原因可能是natureWithFrequency问题，可以通过调试模式了解原因）
     */
    public static bool insert(string word, string natureWithFrequency)
    {
        if (word == null) return false;
        if (HanLP.Config.Normalization) word = CharTable.convert(word);
        CoreDictionary.Attribute att = natureWithFrequency == null ? new CoreDictionary.Attribute(Nature.nz, 1) : CoreDictionary.Attribute.create(natureWithFrequency);
        if (att == null) return false;
        if (dat.set(word, att)) return true;
        if (trie == null) trie = new BinTrie<CoreDictionary.Attribute>();
        trie.put(word, att);
        return true;
    }

    /**
     * 以覆盖模式增加新词<br>
     *     动态增删不会持久化到词典文件
     *
     * @param word
     * @return
     */
    public static bool insert(string word)
    {
        return insert(word, null);
    }

    public static bool loadDat(string path, DoubleArrayTrie<CoreDictionary.Attribute> dat)
    {
        return loadDat(path, HanLP.Config.CustomDictionaryPath, dat);
    }

    /**
     * 从磁盘加载双数组
     *
     * @param path 主词典路径
     * @param customDicPath 用户词典路径
     * @return
     */
    public static bool loadDat(string path, string[] customDicPath, DoubleArrayTrie<CoreDictionary.Attribute> dat)
    {
        try
        {
            if (isDicNeedUpdate(path, customDicPath))
            {
                return false;
            }
            ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
            if (byteArray == null) return false;
            int size = byteArray.nextInt();
            if (size < 0)   // 一种兼容措施,当size小于零表示文件头部储存了-size个用户词性
            {
                while (++size <= 0)
                {
                    Nature.create(byteArray.nextString());
                }
                size = byteArray.nextInt();
            }
            CoreDictionary.Attribute[] attributes = new CoreDictionary.Attribute[size];
            Nature[] natureIndexArray = Nature.values();
            for (int i = 0; i < size; ++i)
            {
                // 第一个是全部频次，第二个是词性个数
                int currentTotalFrequency = byteArray.nextInt();
                int Length = byteArray.nextInt();
                attributes[i] = new CoreDictionary.Attribute(Length);
                attributes[i].totalFrequency = currentTotalFrequency;
                for (int j = 0; j < Length; ++j)
                {
                    attributes[i].nature[j] = natureIndexArray[byteArray.nextInt()];
                    attributes[i].frequency[j] = byteArray.nextInt();
                }
            }
            if (!dat.load(byteArray, attributes)) return false;
        }
        catch (Exception e)
        {
            logger.warning("读取失败，问题发生在" + TextUtility.exceptionToString(e));
            return false;
        }
        return true;
    }

    /**
     * 获取本地词典更新状态
     * @return true 表示本地词典比缓存文件新，需要删除缓存
     */
    private static bool isDicNeedUpdate(string mainPath, string[] path)
    {
        if (HanLP.Config.IOAdapter != null &&
            !HanLP.Config.IOAdapter.getClass().getName().contains("com.hankcs.hanlp.corpus.io.FileIOAdapter"))
        {
            return false;
        }
        string binPath = mainPath + Predefine.BIN_EXT;
        File binFile = new File(binPath);
        if (!binFile.exists())
        {
            return true;
        }
        long lastModified = binFile.lastModified();
        //string path[] = HanLP.Config.CustomDictionaryPath;
        foreach (string p in path)
        {
            File f = new File(p);
            string fileName = f.getName();
            int cut = fileName.lastIndexOf(' ');
            if (cut > 0)
            {
                p = f.getParent() + File.separator + fileName.substring(0, cut);
            }
            f = new File(p);
            if (f.exists() && f.lastModified() > lastModified)
            {
                IOUtil.deleteFile(binPath); // 删掉缓存
                logger.info("已清除自定义词典缓存文件！");
                return true;
            }
        }
        return false;
    }

    /**
     * 查单词
     *
     * @param key
     * @return
     */
    public static CoreDictionary.Attribute get(string key)
    {
        if (HanLP.Config.Normalization) key = CharTable.convert(key);
        CoreDictionary.Attribute attribute = dat.get(key);
        if (attribute != null) return attribute;
        if (trie == null) return null;
        return trie.get(key);
    }

    /**
     * 删除单词<br>
     *     动态增删不会持久化到词典文件
     *
     * @param key
     */
    public static void Remove(string key)
    {
        if (HanLP.Config.Normalization) key = CharTable.convert(key);
        if (trie == null) return;
        trie.Remove(key);
    }

    /**
     * 前缀查询
     *
     * @param key
     * @return
     */
    public static LinkedList<KeyValuePair<string, CoreDictionary.Attribute>> commonPrefixSearch(string key)
    {
        return trie.commonPrefixSearchWithValue(key);
    }

    /**
     * 前缀查询
     *
     * @param chars
     * @param begin
     * @return
     */
    public static LinkedList<KeyValuePair<string, CoreDictionary.Attribute>> commonPrefixSearch(char[] chars, int begin)
    {
        return trie.commonPrefixSearchWithValue(chars, begin);
    }

    public static BaseSearcher getSearcher(string text)
    {
        return new Searcher(text);
    }

    //@Override
    public string toString()
    {
        return "CustomDictionary{" +
                "trie=" + trie +
                '}';
    }

    /**
     * 词典中是否含有词语
     * @param key 词语
     * @return 是否包含
     */
    public static bool contains(string key)
    {
        if (dat.exactMatchSearch(key) >= 0) return true;
        return trie != null && trie.containsKey(key);
    }

    /**
     * 获取一个BinTrie的查询工具
     * @param charArray 文本
     * @return 查询者
     */
    public static BaseSearcher getSearcher(char[] charArray)
    {
        return new Searcher(charArray);
    }

    public class Searcher : BaseSearcher<CoreDictionary.Attribute>
    {
        /**
         * 分词从何处开始，这是一个状态
         */
        int begin;

        private LinkedList<KeyValuePair<string, CoreDictionary.Attribute>> entryList;

        protected Searcher(char[] c)
            : base(c)
        {
            entryList = new LinkedList<KeyValuePair<string, CoreDictionary.Attribute>>();
        }

        protected Searcher(string text)
            : base(c)
        {
            entryList = new LinkedList<KeyValuePair<string, CoreDictionary.Attribute>>();
        }

        //@Override
        public override KeyValuePair<string, CoreDictionary.Attribute> next()
        {
            // 保证首次调用找到一个词语
            while (entryList.size() == 0 && begin < c.Length)
            {
                entryList = trie.commonPrefixSearchWithValue(c, begin);
                ++begin;
            }
            // 之后调用仅在缓存用完的时候调用一次
            if (entryList.size() == 0 && begin < c.Length)
            {
                entryList = trie.commonPrefixSearchWithValue(c, begin);
                ++begin;
            }
            if (entryList.size() == 0)
            {
                return null;
            }
            KeyValuePair<string, CoreDictionary.Attribute> result = entryList.getFirst();
            entryList.removeFirst();
            offset = begin - 1;
            return result;
        }
    }

    /**
     * 获取词典对应的trie树
     *
     * @return
     * @deprecated 谨慎操作，有可能废弃此接口
     */
    public static BinTrie<CoreDictionary.Attribute> getTrie()
    {
        return trie;
    }

    /**
     * 解析一段文本（目前采用了BinTrie+DAT的混合储存形式，此方法可以统一两个数据结构）
     * @param text         文本
     * @param processor    处理器
     */
    public static void parseText(char[] text, AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute> processor)
    {
        if (trie != null)
        {
            trie.parseText(text, processor);
        }
        DoubleArrayTrie<CoreDictionary.Attribute>.Searcher searcher = dat.getSearcher(text, 0);
        while (searcher.next())
        {
            processor.hit(searcher.begin, searcher.begin + searcher.Length, searcher.value);
        }
    }

    /**
     * 解析一段文本（目前采用了BinTrie+DAT的混合储存形式，此方法可以统一两个数据结构）
     * @param text         文本
     * @param processor    处理器
     */
    public static void parseText(string text, AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute> processor)
    {
        if (trie != null)
        {
            BaseSearcher searcher = CustomDictionary.getSearcher(text);
            int offset;
            KeyValuePair<string, CoreDictionary.Attribute> entry;
            while ((entry = searcher.next()) != null)
            {
                offset = searcher.getOffset();
                processor.hit(offset, offset + entry.getKey().Length, entry.getValue());
            }
        }
        DoubleArrayTrie<CoreDictionary.Attribute>.Searcher searcher = dat.getSearcher(text, 0);
        while (searcher.next())
        {
            processor.hit(searcher.begin, searcher.begin + searcher.Length, searcher.value);
        }
    }

    /**
     * 最长匹配
     *
     * @param text      文本
     * @param processor 处理器
     */
    public static void parseLongestText(string text, AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute> processor)
    {
        if (trie != null)
        {
            int[] lengthArray = new int[text.Length];
            CoreDictionary.Attribute[] attributeArray = new CoreDictionary.Attribute[text.Length];
            char[] charArray = text.ToCharArray();
            DoubleArrayTrie<CoreDictionary.Attribute>.Searcher searcher = dat.getSearcher(charArray, 0);
            while (searcher.next())
            {
                lengthArray[searcher.begin] = searcher.Length;
                attributeArray[searcher.begin] = searcher.value;
            }
            trie.parseText(charArray, new HT());
            for (int i = 0; i < charArray.Length;)
            {
                if (lengthArray[i] == 0)
                {
                    ++i;
                }
                else
                {
                    processor.hit(i, i + lengthArray[i], attributeArray[i]);
                    i += lengthArray[i];
                }
            }
        }
        else
            dat.parseLongestText(text, processor);
    }
    public class HT : AhoCorasickDoubleArrayTrie<string>.IHit<CoreDictionary.Attribute>
    {
        //@Override
        public void hit(int begin, int end, CoreDictionary.Attribute value)
        {
            int Length = end - begin;
            if (Length > lengthArray[begin])
            {
                lengthArray[begin] = Length;
                attributeArray[begin] = value;
            }
        }
    }
    /**
     * 热更新（重新加载）<br>
     * 集群环境（或其他IOAdapter）需要自行删除缓存文件（路径 = HanLP.Config.CustomDictionaryPath[0] + Predefine.BIN_EXT）
     * @return 是否加载成功
     */
    public static bool reload()
    {
        string[] path = HanLP.Config.CustomDictionaryPath;
        if (path == null || path.Length == 0) return false;
        IOUtil.deleteFile(path[0] + Predefine.BIN_EXT); // 删掉缓存
        return loadMainDictionary(path[0]);
    }
}
