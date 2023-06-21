/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-23 8:30 PM</create-date>
 *
 * <copyright file="TransforMatrix.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.dictionary;




/**
 * @author hankcs
 */
public abstract class TransformMatrix
{
    // HMM的五元组
    //int[] observations;
    /**
     * 隐状态
     */
    public int[] states;
    /**
     * 初始概率
     */
    public double[] start_probability;
    /**
     * 转移概率
     */
    public double[][] transititon_probability;
    /**
     * 内部标签下标最大值不超过这个值，用于矩阵创建
     */
    protected int ordinaryMax;
    /**
     * 储存转移矩阵
     */
    int[][] matrix;
    /**
     * 储存每个标签出现的次数
     */
    int[] total;
    /**
     * 所有标签出现的总次数
     */
    int totalFrequency;

    public bool load(string path)
    {
        try
        {
            TextReader br = new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            // 第一行是矩阵的各个类型
            string line = br.ReadLine();
            string[] _param = line.Split(",");
            // 为了制表方便，第一个label是废物，所以要抹掉它
            string[] labels = new string[_param.Length - 1];
            Array.Copy(_param, 1, labels, 0, labels.Length);
            int[] ordinaryArray = new int[labels.Length];
            ordinaryMax = 0;
            for (int i = 0; i < ordinaryArray.Length; ++i)
            {
                ordinaryArray[i] = ordinal(labels[i]);
                ordinaryMax = Math.Max(ordinaryMax, ordinaryArray[i]);
            }
            ++ordinaryMax;
            matrix = new int[ordinaryMax][ordinaryMax];
            for (int i = 0; i < ordinaryMax; ++i)
            {
                for (int j = 0; j < ordinaryMax; ++j)
                {
                    matrix[i][j] = 0;
                }
            }
            // 之后就描述了矩阵
            while ((line = br.ReadLine()) != null)
            {
                string[] paramArray = line.Split(",");
                int currentOrdinary = ordinal(paramArray[0]);
                for (int i = 0; i < ordinaryArray.Length; ++i)
                {
                    matrix[currentOrdinary][ordinaryArray[i]] = int.valueOf(paramArray[1 + i]);
                }
            }
            br.Close();
            // 需要统计一下每个标签出现的次数
            total = new int[ordinaryMax];
            for (int j = 0; j < ordinaryMax; ++j)
            {
                total[j] = 0;
                for (int i = 0; i < ordinaryMax; ++i)
                {
                    total[j] += matrix[j][i]; // 按行累加
                }
            }
            for (int j = 0; j < ordinaryMax; ++j)
            {
                if (total[j] == 0)
                {
                    for (int i = 0; i < ordinaryMax; ++i)
                    {
                        total[j] += matrix[i][j]; // 按列累加
                    }
                }
            }
            for (int j = 0; j < ordinaryMax; ++j)
            {
                totalFrequency += total[j];
            }
            // 下面计算HMM四元组
            states = ordinaryArray;
            start_probability = new double[ordinaryMax];
            foreach (int s in states)
            {
                double frequency = total[s] + 1e-8;
                start_probability[s] = -Math.Log(frequency / totalFrequency);
            }
            transititon_probability = new double[ordinaryMax][ordinaryMax];
            foreach (int from in states)
            {
                foreach (int to in states)
                {
                    double frequency = matrix[from][to] + 1e-8;
                    transititon_probability[from][to] = -Math.Log(frequency / total[from]);
//                    Console.WriteLine("from" + NR.values()[from] + " to" + NR.values()[to] + " = " + transititon_probability[from][to]);
                }
            }
        }
        catch (Exception e)
        {
            logger.warning("读取" + path + "失败" + e);
            return false;
        }

        return true;
    }

    /**
     * 拓展内部矩阵,仅用于通过反射新增了枚举实例之后的兼容措施
     */
    public void extend(int ordinaryMax)
    {
        this.ordinaryMax = ordinaryMax;
        double[][] n_transititon_probability = new double[ordinaryMax][ordinaryMax];
        for (int i = 0; i < transititon_probability.Length; i++)
        {
            Array.Copy(transititon_probability[i], 0, n_transititon_probability[i], 0, transititon_probability.Length);
        }
        transititon_probability = n_transititon_probability;

        int[] n_total = new int[ordinaryMax];
        Array.Copy(total, 0, n_total, 0, total.Length);
        total = n_total;

        double[] n_start_probability = new double[ordinaryMax];
        Array.Copy(start_probability, 0, n_start_probability, 0, start_probability.Length);
        start_probability = n_start_probability;

        int[][] n_matrix = new int[ordinaryMax][ordinaryMax];
        for (int i = 0; i < matrix.Length; i++)
        {
            Array.Copy(matrix[i], 0, n_matrix[i], 0, matrix.Length);
        }
        matrix = n_matrix;
    }

    public abstract int ordinal(string tag);

    public int getTotalFrequency(int ordinal)
    {
        return total[ordinal];
    }
}
