/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/11 17:20</create-date>
 *
 * <copyright file="BigramDependencyModel.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.dependency.model;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.bigram;




/**
 * 2-gram依存模型，根据两个词的词和词性猜测它们最可能的依存关系
 * @author hankcs
 */
public class BigramDependencyModel
{
    static DoubleArrayTrie<string> trie;

    static BigramDependencyModel()
    {
        long start = DateTime.Now.Microsecond;
        if (load(HanLP.Config.WordNatureModelPath))
        {
            logger.info("加载依存句法二元模型" + HanLP.Config.WordNatureModelPath + "成功，耗时：" + (DateTime.Now.Microsecond - start) + " ms");
        }
        else
        {
            logger.warning("加载依存句法二元模型" + HanLP.Config.WordNatureModelPath + "失败，耗时：" + (DateTime.Now.Microsecond - start) + " ms");
        }
    }

    static bool load(string path)
    {
        trie = new DoubleArrayTrie<string>();
        if (loadDat(path + ".bi" + Predefine.BIN_EXT)) return true;
        var map = new Dictionary<string, string>();
        foreach (string line in IOUtil.readLineListWithLessMemory(path))
        {
            string[] param = line.Split(" ");
            if (param[0].EndsWith("@"))
            {
                continue;
            }
            string dependency = param[1];
            map.Add(param[0], dependency);
        }
        if (map.Count == 0) return false;
        trie.build(map);
        if (!saveDat(path, map)) logger.warning("缓存" + path + Predefine.BIN_EXT + "失败");
        return true;
    }

    private static bool loadDat(string path)
    {
        ByteArray byteArray = ByteArray.createByteArray(path);
        if (byteArray == null) return false;
        int size = byteArray.Next();
        string[] valueArray = new string[size];
        for (int i = 0; i < valueArray.Length; ++i)
        {
            valueArray[i] = byteArray.nextUTF();
        }
        return trie.load(byteArray, valueArray);
    }

    static bool saveDat(string path, Dictionary<string, string> map)
    {
        Collection<string> dependencyList = map.values();
        // 缓存值文件
        try
        {
            Stream Out = new Stream(IOUtil.newOutputStream(path +  ".bi" + Predefine.BIN_EXT));
            Out.writeInt(dependencyList.Count);
            foreach (string dependency in dependencyList)
            {
                Out.writeUTF(dependency);
            }
            if (!trie.save(Out)) return false;
            Out.Close();
        }
        catch (Exception e)
        {
            logger.warning("保存失败" + e);
            return false;
        }
        return true;
    }

    public static string get(string key)
    {
        return trie.get(key);
    }

    /**
     * 获取一个词和另一个词最可能的依存关系
     * @param fromWord
     * @param fromPos
     * @param toWord
     * @param toPos
     * @return
     */
    public static string get(string fromWord, string fromPos, string toWord, string toPos)
    {
        string dependency = get(fromWord + "@" + toWord);
        if (dependency == null) dependency = get(fromWord + "@" + WordNatureWeightModelMaker.wrapTag(toPos));
        if (dependency == null) dependency = get(WordNatureWeightModelMaker.wrapTag(fromPos) + "@" + toWord);
        if (dependency == null) dependency = get(WordNatureWeightModelMaker.wrapTag(fromPos) + "@" + WordNatureWeightModelMaker.wrapTag(toPos));
        if (dependency == null) dependency = "未知";

        return dependency;
    }
}
