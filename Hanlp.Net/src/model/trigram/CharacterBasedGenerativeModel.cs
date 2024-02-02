/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/6 19:48</create-date>
 *
 * <copyright file="CharacterBasedGenerativeModel.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.trigram.frequency;

namespace com.hankcs.hanlp.model.trigram;



/**
 * 基于字符的生成模型（其实就是一个TriGram文法模型，或称2阶隐马模型）
 *
 * @author hankcs
 */
public class CharacterBasedGenerativeModel : ICacheAble
{
    /**
     * 2阶隐马的三个参数
     */
    double l1, l2, l3;
    /**
     * 频次统计
     */
    Probability tf;
    /**
     * BMES标签转移矩阵, 用于限定输出结果
     */
    int[][][] transMatrix;
    /**
     * 用到的标签
     */
    static readonly char[] id2tag = new char[]{'b', 'm', 'e', 's', 'x'};
    /**
     * 视野范围外的事件
     */
    static readonly char[] bos = {'\b', 'x'};
    /**
     * 无穷小
     */
    static readonly double inf = -1e10;
    /**
     * 最后两个字的状态
     * 只可能是 "be" "me" "es" "ss"
     */
    static readonly int[][] probableTail = { new int[]{0,2}, new int[] { 1,2}, new int[] { 2,3}, new int[] { 3,3}};

    public CharacterBasedGenerativeModel()
    {
        tf = new Probability();
        // 构建有限转移矩阵
        // 矩阵的数值根据《人民日报》语料估算 
        int [] nullArray = {0, 0, 0, 0, 0};
        transMatrix = new int[5][][];
        transMatrix[0] = new int[][]{nullArray, new int[] { 0, 150, 330, 0, 0}, new int[] { 160, 0, 0, 168, 20}, nullArray, nullArray};
        transMatrix[1] = new int[][]{nullArray, new int[] { 0, 35, 150, 0, 0}, new int[] { 210, 0, 0, 263, 3}, nullArray, nullArray};
        transMatrix[2] = new int[][]{ new int[] { 0, 200, 1600, 0, 0}, nullArray, nullArray, new int[] { 1080, 0, 0, 650, 205}, nullArray};
        transMatrix[3] = new int[][]{ new int[] { 0, 200, 1600, 0, 0}, nullArray, nullArray, new int[] { 640, 0, 0, 700, 63}, nullArray};
        transMatrix[4] = new int[][]{ new int[] { 0, 30, 150, 0, 0}, nullArray, nullArray, new int[] { 60, 0, 0, 50, 3}, new int[] { 180, 0, 0, 120, 0}};
   }

    /**
     * 让模型观测一个句子
     * @param wordList
     */
    public void learn(List<Word> wordList)
    {
        List<char[]> sentence = new ();
        foreach (IWord iWord in wordList)
        {
            string word = iWord.Value;
            if (word.Length == 1)
            {
                sentence.Add(new char[]{word[0], 's'});
            }
            else
            {
                sentence.Add(new char[]{word[0], 'b'});
                for (int i = 1; i < word.Length - 1; ++i)
                {
                    sentence.Add(new char[]{word[i], 'm'});
                }
                sentence.Add(new char[] { word[^1], 'e' });
            }
        }
        // 转换完毕，开始统计
        char[][] now = new char[3][];   // 定长3的队列
        now[1] = bos;
        now[2] = bos;
        tf.Add(1, bos, bos);
        tf.Add(2, bos);
        foreach (char[] i in sentence)
        {
            Array.Copy(now, 1, now, 0, 2);
            now[2] = i;
            tf.Add(1, i);   // uni
            tf.Add(1, now[1], now[2]);   // bi
            tf.Add(1, now);   // tri
        }
    }

    /**
     * 观测结束，开始训练
     */
    public void train()
    {
        double tl1 = 0.0;
        double tl2 = 0.0;
        double tl3 = 0.0;
        foreach (string key in tf.d.Keys())
        {
            if (key.Length != 6) continue;    // tri samples
            char[][] now = new char[][]
                    {
                            new char[]{key[0], key[1] },
                            new char[]{key[2], key[3] },
                            new char[]{key[4], key[5] },
                    };
            double c3 = div(tf.get(now) - 1, tf.get(now[0], now[1]) - 1);
            double c2 = div(tf.get(now[1], now[2]) - 1, tf.get(now[1]) - 1);
            double c1 = div(tf.get(now[2]) - 1, tf.getsum() - 1);
            if (c3 >= c1 && c3 >= c2)
                tl3 += tf.get(key.ToCharArray());
            else if (c2 >= c1 && c2 >= c3)
                tl2 += tf.get(key.ToCharArray());
            else if (c1 >= c2 && c1 >= c3)
                tl1 += tf.get(key.ToCharArray());
        }

        l1 = div(tl1, tl1 + tl2 + tl3);
        l2 = div(tl2, tl1 + tl2 + tl3);
        l3 = div(tl3, tl1 + tl2 + tl3);
    }

    /**
     * 求概率
     * @param s1 前2个字
     * @param s1 前2个状态的下标
     * @param s2 前1个字
     * @param s2 前1个状态的下标
     * @param s3 当前字
     * @param s3 当前状态的下标
     * @return 序列的概率
     */
    double log_prob(char s1, int i1, char s2, int i2, char s3, int i3)
    {
        if (transMatrix[i1][i2][i3] == 0)
            return inf;
        char t1 = id2tag[i1];
        char t2 = id2tag[i2];
        char t3 = id2tag[i3];
        double uni = l1 * tf.freq(s3,t3);
        double bi = div(l2 * tf.get(s2,t2, s3,t3), tf.get(s2,t2));
        double tri = div(l3 * tf.get(s1,t1, s2,t2, s3,t3), tf.get(s1,t1, s2,t2));
        if (uni + bi + tri == 0)
            return inf;
        return Math.Log(uni + bi + tri);
    }

    /**
     * 序列标注
     * @param charArray 观测序列
     * @return 标注序列
     */
    public char[] tag(char[] charArray)
    {
        if (charArray.Length == 0) return new char[0];
        if (charArray.Length == 1) return new char[]{'s'};
        char[] tag = new char[charArray.Length];
        double[][] now = new double[4][4];
        double[] first = new double[4];

        // link[i][s][t] := 第i个节点在前一个状态是s，当前状态是t时，前2个状态的tag的值
        int[][][] link = new int[charArray.Length][4][4];
        // 第一个字，只可能是bs
        for (int s = 0; s < 4; ++s)
        {
            double p = (s == 1 || s == 2) ? inf : log_prob(bos[0], 4, bos[0], 4, charArray[0],s);
            first[s] = p;
        }

        // 第二个字，尚不能完全利用TriGram
        for (int f = 0; f < 4; ++f)
        {
            for (int s = 0; s < 4; ++s)
            {
                double p = first[f] + log_prob(bos[0],4, charArray[0],f, charArray[1],s);
                now[f][s] = p;
                link[1][f][s] = f;
            }
        }

        // 第三个字开始，利用TriGram标注
        double[,] pre = new double[4,4];
        for (int i = 2; i < charArray.Length; i++)
        {
            // swap(now, pre)
            double[][] _ = pre;
            pre = now;
            now = _;
            // end of swap
            for (int s = 0; s < 4; ++s)
            {
                for (int t = 0; t < 4; ++t)
                {
                    now[s][t] = -1e20;
                    for (int f = 0; f < 4; ++f)
                    {
                        double p = pre[f,s] + log_prob(charArray[i - 2], f,
                                                        charArray[i - 1], s,
                                                        charArray[i],     t);
                        if (p > now[s][t])
                        {
                            now[s][t] = p;
                            link[i][s][t] = f;
                        }
                    }
                }
            }
        }
        // 无法保证最优路径每个状态的概率都是非最小值, 所以回溯路径得分最小值必须小于inf
        double score = charArray.Length*inf;
        int s = 0;
        int t = 0;
        for (int i = 0; i < probableTail.Length; i++)
        {
            int [] state = probableTail[i];
            if (now[state[0]][state[1]] > score)
            {
                score = now[state[0]][state[1]];
                s = state[0];
                t = state[1];
            }
        }
        for (int i = link.Length - 1; i >= 0; --i)
        {
            tag[i] = id2tag[t];
            int f = link[i][s][t];
            t = s;
            s = f;
        }
        return tag;
    }

    /**
     * 安全除法
     * @param v1
     * @param v2
     * @return
     */
    private static double div(int v1, int v2)
    {
        if (v2 == 0) return 0.0;
        return v1 / (double) v2;
    }

    /**
     * 安全除法
     * @param v1
     * @param v2
     * @return
     */
    private static double div(double v1, double v2)
    {
        if (v2 == 0) return 0.0;
        return v1 / v2;
    }

    //@Override
    public void save(Stream Out)
    {
        Out.writeDouble(l1);
        Out.writeDouble(l2);
        Out.writeDouble(l3);
        tf.save(Out);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        l1 = byteArray.nextDouble();
        l2 = byteArray.nextDouble();
        l3 = byteArray.nextDouble();
        tf.load(byteArray);
        return true;
    }
}
