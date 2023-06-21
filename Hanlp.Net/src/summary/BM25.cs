/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/8/22 14:17</create-date>
 *
 * <copyright file="BM25.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.summary;


/**
 * 搜索相关性评分算法
 * @author hankcs
 */
public class BM25
{
    /**
     * 文档句子的个数
     */
    int D;

    /**
     * 文档句子的平均长度
     */
    double avgdl;

    /**
     * 拆分为[句子[单词]]形式的文档
     */
    List<List<string>> docs;

    /**
     * 文档中每个句子中的每个词与词频
     */
    Dictionary<string, int>[] f;

    /**
     * 文档中全部词语与出现在几个句子中
     */
    Dictionary<string, int> df;

    /**
     * IDF
     */
    Dictionary<string, Double> idf;

    /**
     * 调节因子
     */
    static float k1 = 1.5f;

    /**
     * 调节因子
     */
    static float b = 0.75f;

    public BM25(List<List<string>> docs)
    {
        this.docs = docs;
        D = docs.Count;
        foreach (List<string> sentence in docs)
        {
            avgdl += sentence.Count;
        }
        avgdl /= D;
        f = new Dictionary<string, int>[D];
        df = new Dictionary<string, int>();
        idf = new Dictionary<string, Double>();
        init();
    }

    /**
     * 在构造时初始化自己的所有参数
     */
    private void init()
    {
        int index = 0;
        foreach (List<string> sentence in docs)
        {
            Dictionary<string, int> tf = new Dictionary<string, int>();
            foreach (string word in sentence)
            {
                freq = (!tf.TryGetValue(word,out var freq)  ? 0 : freq) + 1;
                tf.Add(word, freq);
            }
            f[index] = tf;
            foreach (var entry in tf)
            {
                string word = entry.Key;
                int freq = df.get(word);
                freq = (freq == null ? 0 : freq) + 1;
                df.Add(word, freq);
            }
            ++index;
        }
        foreach (var entry in df)
        {
            string word = entry.Key;
            int freq = entry.Value;
            idf.Add(word, Math.Log(D - freq + 0.5) - Math.Log(freq + 0.5));
        }
    }

    /**
     * 计算一个句子与一个文档的BM25相似度
     *
     * @param sentence 句子（查询语句）
     * @param index    文档（用语料库中的下标表示）
     * @return BM25 score
     */
    public double sim(List<string> sentence, int index)
    {
        double score = 0;
        foreach (string word in sentence)
        {
            if (!f[index].ContainsKey(word)) continue;
            int d = docs[(index)].Count;
            int tf = f[index].get(word);
            score += (idf.get(word) * tf * (k1 + 1)
                    / (tf + k1 * (1 - b + b * d
                                                / avgdl)));
        }

        return score;
    }

    public double[] simAll(List<string> sentence)
    {
        double[] scores = new double[D];
        for (int i = 0; i < D; ++i)
        {
            scores[i] = sim(sentence, i);
        }
        return scores;
    }
}
