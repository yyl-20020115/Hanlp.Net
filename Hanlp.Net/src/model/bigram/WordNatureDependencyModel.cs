/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 15:00</create-date>
 *
 * <copyright file="WordNatureDependencyModel.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.dependency.model;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dependency.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.bigram;




/**
 * 词、词性相互构成依存关系的统计句法分析模型
 * @author hankcs
 */
public class WordNatureDependencyModel
{
    DoubleArrayTrie<Attribute> trie;

    public WordNatureDependencyModel(string path)
    {
        long start = DateTime.Now.Microsecond;
        if (load(path))
        {
            logger.info("加载依存句法生成模型" + path + "成功，耗时：" + (DateTime.Now.Microsecond - start) + " ms");
        }
        else
        {
            throw new ArgumentException("加载依存句法生成模型" + path + "失败，耗时：" + (DateTime.Now.Microsecond - start) + " ms");
        }
    }

    bool load(string path)
    {
        trie = new DoubleArrayTrie<Attribute>();
        if (loadDat(path)) return true;
        Dictionary<string, Attribute> map = new Dictionary<string, Attribute>();
        Dictionary<string, int> tagMap = new Dictionary<string, int>();
        foreach (string line in IOUtil.readLineListWithLessMemory(path))
        {
            string[] param = line.Split(" ");
            if (param[0].EndsWith("@"))
            {
                tagMap.Add(param[0], int.parseInt(param[2]));
                continue;
            }
            int natureCount = (param.Length - 1) / 2;
            Attribute attribute = new Attribute(natureCount);
            for (int i = 0; i < natureCount; ++i)
            {
                attribute.dependencyRelation[i] = param[1 + 2 * i];
                attribute.p[i] = int.parseInt(param[2 + 2 * i]);
            }
            map.Add(param[0], attribute);
        }
        if (map.size() == 0) return false;
        // 为它们计算概率
        foreach (KeyValuePair<string, Attribute> entry in map.entrySet())
        {
            string key = entry.Key;
            string[] param = key.Split("@", 2);
            Attribute attribute = entry.Value;
            int total = tagMap.get(param[0] + "@");
            for (int i = 0; i < attribute.p.Length; ++i)
            {
                attribute.p[i] = (float) -Math.Log(attribute.p[i] / total);
            }
            // 必须降低平滑处理的权重
            float boost = 1.0f;
            if (key.StartsWith("<"))
            {
                boost *= 10.0f;
            }
            if (key.EndsWith(">"))
            {
                boost *= 10.0f;
            }
            if (boost != 1.0f)
                attribute.setBoost(boost);
        }

        trie.build(map);
        if (!saveDat(path, map)) logger.warning("缓存" + path + "失败");
        return true;
    }

    bool saveDat(string path, Dictionary<string, Attribute> map)
    {
        var attributeList = map.values();
        // 缓存值文件
        try
        {
            Stream _out = new Stream(IOUtil.newOutputStream(path + Predefine.BIN_EXT));
            _out.writeInt(attributeList.size());
            foreach (Attribute attribute in attributeList)
            {
                _out.writeInt(attribute.p.Length);
                for (int i = 0; i < attribute.p.Length; ++i)
                {
                    char[] charArray = attribute.dependencyRelation[i].ToCharArray();
                    _out.writeInt(charArray.Length);
                    foreach (char c in charArray)
                    {
                        _out.writeChar(c);
                    }
                    _out.writeFloat(attribute.p[i]);
                }
            }
            if (!trie.save(_out)) return false;
            _out.Close();
        }
        catch (Exception e)
        {
            logger.warning("保存失败" + e);
            return false;
        }
        return true;
    }

    bool loadDat(string path)
    {
        ByteArray byteArray = ByteArray.createByteArray(path + Predefine.BIN_EXT);
        if (byteArray == null) return false;
        int size = byteArray.Next();
        Attribute[] attributeArray = new Attribute[size];
        for (int i = 0; i < attributeArray.Length; ++i)
        {
            int Length = byteArray.Next();
            Attribute attribute = new Attribute(Length);
            for (int j = 0; j < attribute.dependencyRelation.Length; ++j)
            {
                attribute.dependencyRelation[j] = byteArray.nextString();
                attribute.p[j] = byteArray.nextFloat();
            }
            attributeArray[i] = attribute;
        }

        return trie.load(byteArray, attributeArray);
    }

    public Attribute get(string key)
    {
        return trie.get(key);
    }

    /**
     * 打分
     * @param from
     * @param to
     * @return
     */
    public Edge getEdge(Node from, Node to)
    {
        if (from is null)
        {
            throw new ArgumentNullException(nameof(from));
        }
        // 首先尝试词+词
        Attribute attribute = get(from.compiledWord, to.compiledWord);
        if (attribute == null) attribute = get(from.compiledWord, WordNatureWeightModelMaker.wrapTag(to.label));
        if (attribute == null) attribute = get(WordNatureWeightModelMaker.wrapTag(from.label), to.compiledWord);
        if (attribute == null) attribute = get(WordNatureWeightModelMaker.wrapTag(from.label), WordNatureWeightModelMaker.wrapTag(to.label));
        if (attribute == null)
        {
            attribute = Attribute.NULL;
        }
        if (HanLP.Config.DEBUG)
        {
            Console.WriteLine(from + " 到 " + to + " : " + attribute);
        }
        return new Edge(from.id, to.id, attribute.dependencyRelation[0], attribute.p[0]);
    }

    public Attribute get(string from, string to)
    {
        return get(from + "@" + to);
    }

    class Attribute
    {
        public static Attribute NULL = new Attribute("未知", 10000.0f);
        /**
         * 依存关系
         */
        public string[] dependencyRelation;
        /**
         * 概率
         */
        public float[] p;

        public Attribute(int size)
        {
            dependencyRelation = new string[size];
            p = new float[size];
        }

        Attribute(string dr, float p)
        {
            dependencyRelation = new string[]{dr};
            this.p = new float[]{p};
        }

        /**
         * 加权
         * @param boost
         */
        public void setBoost(float boost)
        {
            for (int i = 0; i < p.Length; ++i)
            {
                p[i] *= boost;
            }
        }

        //@Override
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(dependencyRelation.Length * 10);
            for (int i = 0; i < dependencyRelation.Length; ++i)
            {
                sb.Append(dependencyRelation[i]).Append(' ').Append(p[i]).Append(' ');
            }
            return sb.ToString();
        }
    }
}
