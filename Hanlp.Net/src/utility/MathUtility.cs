/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/10 PM6:51</create-date>
 *
 * <copyright file="MathUtility.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.utility;

/**
 * 一些数学小工具
 * @author hankcs
 */
public class MathUtility
{
    public static int sum(params int[] var)
    {
        int sum = 0;
        foreach (int x in var)
        {
            sum += x;
        }

        return sum;
    }

    public static float sum(params float[] var)
    {
        float sum = 0;
        foreach (float x in var)
        {
            sum += x;
        }

        return sum;
    }

    public static double percentage(double current, double total)
    {
        return current / total * 100.0;
    }

    public static double average(double[] array)
    {
        double sum = 0;
        for (int i = 0; i < array.Length; i++)
            sum += array[i];
        return sum / array.Length;
    }

    /**
     * 使用log-sum-exp技巧来归一化一组对数值
     *
     * @param predictionScores
     */
    public static void normalizeExp(Dictionary<string, Double> predictionScores)
    {
        HashSet<KeyValuePair<string, Double>> entrySet = predictionScores.entrySet();
        double max = Double.NEGATIVE_INFINITY;
        for (KeyValuePair<string, Double> entry : entrySet)
        {
            max = Math.Max(max, entry.getValue());
        }

        double sum = 0.0;
        //通过减去最大值防止浮点数溢出
        foreach (KeyValuePair<string, Double> entry in entrySet)
        {
            Double value = Math.exp(entry.getValue() - max);
            entry.setValue(value);

            sum += value;
        }

        if (sum != 0.0)
        {
            foreach (KeyValuePair<string, Double> entry in entrySet)
            {
                predictionScores.put(entry.getKey(), entry.getValue() / sum);
            }
        }
    }

    public static void normalizeExp(double[] predictionScores)
    {
        double max = Double.NEGATIVE_INFINITY;
        foreach (double value in predictionScores)
        {
            max = Math.Max(max, value);
        }

        double sum = 0.0;
        //通过减去最大值防止浮点数溢出
        for (int i = 0; i < predictionScores.Length; i++)
        {
            predictionScores[i] = Math.Exp(predictionScores[i] - max);
            sum += predictionScores[i];
        }

        if (sum != 0.0)
        {
            for (int i = 0; i < predictionScores.Length; i++)
            {
                predictionScores[i] /= sum;
            }
        }
    }

    /**
     * 从一个词到另一个词的词的花费
     *
     * @param from 前面的词
     * @param to   后面的词
     * @return 分数
     */
    public static double calculateWeight(Vertex from, Vertex to)
    {
        int frequency = from.getAttribute().totalFrequency;
        if (frequency == 0)
        {
            frequency = 1;  // 防止发生除零错误
        }
//        int nTwoWordsFreq = BiGramDictionary.getBiFrequency(from.word, to.word);
        int nTwoWordsFreq = CoreBiGramTableDictionary.getBiFrequency(from.wordID, to.wordID);
        double value = -Math.Log(dSmoothingPara * frequency / (MAX_FREQUENCY) + (1 - dSmoothingPara) * ((1 - dTemp) * nTwoWordsFreq / frequency + dTemp));
        if (value < 0.0)
        {
            value = -value;
        }
//        logger.info(string.Format("%5s frequency:%6d, %s nTwoWordsFreq:%3d, weight:%.2f", from.word, frequency, from.word + "@" + to.word, nTwoWordsFreq, value));
        return value;
    }

}