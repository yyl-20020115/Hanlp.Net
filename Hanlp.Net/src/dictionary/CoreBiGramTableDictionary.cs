/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/24 12:46</create-date>
 *
 * <copyright file="CoreBiGramDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary;




/**
 * 核心词典的二元接续词典，采用整型储存，高性能
 *
 * @author hankcs
 */
public class CoreBiGramTableDictionary
{
    /**
     * 描述了词在pair中的范围，具体说来<br>
     * 给定一个词idA，从pair[start[idA]]开始的start[idA + 1] - start[idA]描述了一些接续的频次
     */
    static int[] start;
    /**
     * pair[偶数n]表示key，pair[n+1]表示frequency
     */
    static int[] pair;

    static CoreBiGramTableDictionary()
    {
        string path = HanLP.Config.BiGramDictionaryPath;
        logger.info("开始加载二元词典" + path + ".table");
        long start = DateTime.Now.Microsecond;
        if (!load(path))
        {
            throw new ArgumentException("二元词典加载失败");
        }
        else
        {
            logger.info(path + ".table" + "加载成功，耗时" + (DateTime.Now.Microsecond - start) + "ms");
        }
    }

    static bool load(string path)
    {
        string datPath = HanLP.Config.BiGramDictionaryPath + ".table" + Predefine.BIN_EXT;
        if (loadDat(datPath)) return true;
        TextReader br;
        var map = new Dictionary<int, Dictionary<int, int>>();
        try
        {
            br = new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            string line;
            int total = 0;
            int maxWordId = CoreDictionary.trie.size();
            while ((line = br.ReadLine()) != null)
            {
                string[] p = line.Split("\\s");
                string[] twoWord = p[0].Split("@", 2);
                string a = twoWord[0];
                int idA = CoreDictionary.trie.exactMatchSearch(a);
                if (idA == -1)
                {
//                    if (HanLP.Config.DEBUG)
//                        logger.warning(line + " 中的 " + a + "不存在于核心词典，将会忽略这一行");
                    continue;
                }
                string b = twoWord[1];
                int idB = CoreDictionary.trie.exactMatchSearch(b);
                if (idB == -1)
                {
//                    if (HanLP.Config.DEBUG)
//                        logger.warning(line + " 中的 " + b + "不存在于核心词典，将会忽略这一行");
                    continue;
                }
                int freq = int.parseInt(p[1]);
                Dictionary<int, int> biMap = map.get(idA);
                if (biMap == null)
                {
                    biMap = new Dictionary<int, int>();
                    map.Add(idA, biMap);
                }
                biMap.Add(idB, freq);
                total += 2;
            }
            br.Close();
            start = new int[maxWordId + 1];
            pair = new int[total];  // total是接续的个数*2
            int offset = 0;

            for (int i = 0; i < maxWordId; ++i)
            {
                Dictionary<int, int> bMap = map.get(i);
                if (bMap != null)
                {
                    for (KeyValuePair<int, int> entry : bMap.entrySet())
                    {
                        int index = offset << 1;
                        pair[index] = entry.Key;
                        pair[index + 1] = entry.Value;
                        ++offset;
                    }
                }
                start[i + 1] = offset;
            }

            logger.info("二元词典读取完毕:" + path + "，构建为TableBin结构");
        }
        catch (FileNotFoundException e)
        {
            logger.severe("二元词典" + path + "不存在！" + e);
            return false;
        }
        catch (IOException e)
        {
            logger.severe("二元词典" + path + "读取错误！" + e);
            return false;
        }
        logger.info("开始缓存二元词典到" + datPath);
        if (!saveDat(datPath))
        {
            logger.warning("缓存二元词典到" + datPath + "失败");
        }
        return true;
    }

    static bool saveDat(string path)
    {
        try
        {
//            Stream _out = new Stream(new FileStream(path));
//            _out.writeInt(start.Length);
//            for (int i : start)
//            {
//                _out.writeInt(i);
//            }
//            _out.writeInt(pair.Length);
//            for (int i : pair)
//            {
//                _out.writeInt(i);
//            }
//            _out.Close();
            ObjectOutputStream _out = new ObjectOutputStream(IOUtil.newOutputStream(path));
            _out.writeObject(start);
            _out.writeObject(pair);
            _out.Close();
        }
        catch (Exception e)
        {
            logger.log(Level.WARNING, "在缓存" + path + "时发生异常", e);
            return false;
        }

        return true;
    }

    static bool loadDat(string path)
    {
//        ByteArray byteArray = ByteArray.createByteArray(path);
//        if (byteArray == null) return false;
//
//        int size = byteArray.Next(); // 这两个数组从byte转为int竟然要花4秒钟
//        start = new int[size];
//        for (int i = 0; i < size; ++i)
//        {
//            start[i] = byteArray.Next();
//        }
//
//        size = byteArray.Next();
//        pair = new int[size];
//        for (int i = 0; i < size; ++i)
//        {
//            pair[i] = byteArray.Next();
//        }

        try
        {
            ObjectInputStream _in = new ObjectInputStream(IOUtil.newInputStream(path));
            start = (int[])_in.readObject();
            if (CoreDictionary.trie.size() != start.Length - 1)     // 目前CoreNatureDictionary.ngram.txt的缓存依赖于CoreNatureDictionary.txt的缓存
            {                                                       // 所以这里校验一下二者的一致性，不然可能导致下标越界或者ngram错乱的情况
                _in.Close();
                return false;
            }
            pair = (int[])_in.readObject();
            _in.Close();
        }
        catch (Exception e)
        {
            logger.warning("尝试载入缓存文件" + path + "发生异常[" + e + "]，下面将载入源文件并自动缓存……");
            return false;
        }
        return true;
    }

    /**
     * 二分搜索，由于二元接续前一个词固定时，后一个词比较少，所以二分也能取得很高的性能
     * @param a 目标数组
     * @param fromIndex 开始下标
     * @param Length 长度
     * @param key 词的id
     * @return 共现频次
     */
    private static int binarySearch(int[] a, int fromIndex, int Length, int key)
    {
        int low = fromIndex;
        int high = fromIndex + Length - 1;

        while (low <= high)
        {
            int mid = (low + high) >>> 1;
            int midVal = a[mid << 1];

            if (midVal < key)
                low = mid + 1;
            else if (midVal > key)
                high = mid - 1;
            else
                return mid; // key found
        }
        return -(low + 1);  // key not found.
    }

    /**
     * 获取共现频次
     *
     * @param a 第一个词
     * @param b 第二个词
     * @return 第一个词@第二个词出现的频次
     */
    public static int getBiFrequency(string a, string b)
    {
        int idA = CoreDictionary.trie.exactMatchSearch(a);
        if (idA == -1)
        {
            return 0;
        }
        int idB = CoreDictionary.trie.exactMatchSearch(b);
        if (idB == -1)
        {
            return 0;
        }
        int index = binarySearch(pair, start[idA], start[idA + 1] - start[idA], idB);
        if (index < 0) return 0;
        index <<= 1;
        return pair[index + 1];
    }

    /**
     * 获取共现频次
     * @param idA 第一个词的id
     * @param idB 第二个词的id
     * @return 共现频次
     */
    public static int getBiFrequency(int idA, int idB)
    {
        // 负数id表示来自用户词典的词语的词频（用户自定义词语没有id），返回正值增加其亲和度
        if (idA < 0)
        {
            return -idA;
        }
        if (idB < 0)
        {
            return -idB;
        }
        int index = binarySearch(pair, start[idA], start[idA + 1] - start[idA], idB);
        if (index < 0) return 0;
        index <<= 1;
        return pair[index + 1];
    }

    /**
     * 获取词语的ID
     *
     * @param a 词语
     * @return id
     */
    public static int getWordID(string a)
    {
        return CoreDictionary.trie.exactMatchSearch(a);
    }

    /**
     * 热更新二元接续词典<br>
     *     集群环境（或其他IOAdapter）需要自行删除缓存文件
     * @return 是否成功
     */
    public static bool reload()
    {
        string biGramDictionaryPath = HanLP.Config.BiGramDictionaryPath;
        IOUtil.deleteFile(biGramDictionaryPath + ".table" + Predefine.BIN_EXT);

        return load(biGramDictionaryPath);
    }
}
