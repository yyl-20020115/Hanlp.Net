/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-09 7:47 PM</create-date>
 *
 * <copyright file="SecondOrderHiddenMarkovModel.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.hmm;


/**
 * @author hankcs
 */
public class SecondOrderHiddenMarkovModel : HiddenMarkovModel
{
    /**
     * 状态转移概率矩阵
     */
    float[][][] transition_probability2;

    /**
     * 构造隐马模型
     *
     * @param start_probability      初始状态概率向量
     * @param transition_probability 状态转移概率矩阵
     * @param emission_probability   观测概率矩阵
     */
    private SecondOrderHiddenMarkovModel(float[] start_probability, float[][] transition_probability, float[][] emission_probability)
        : base(start_probability, transition_probability, emission_probability)
    {
        ;
    }

    public SecondOrderHiddenMarkovModel(float[] start_probability, float[][] transition_probability, float[][] emission_probability, float[][][] transition_probability2)
        : this(start_probability, transition_probability, emission_probability)
    {
        ;
        this.transition_probability2 = transition_probability2;
        toLog();
    }

    public SecondOrderHiddenMarkovModel()
    {
        this(null, null, null, null);
    }

    //@Override
    protected void estimateTransitionProbability(Collection<int[][]> samples, int max_state)
    {
        transition_probability = new float[max_state + 1][max_state + 1];
        transition_probability2 = new float[max_state + 1][max_state + 1][max_state + 1];
        foreach (int[][] sample in samples)
        {
            int prev_s = sample[1][0];
            int prev_prev_s = -1;
            for (int i = 1; i < sample[0].Length; i++)
            {
                int s = sample[1][i];
                if (i == 1)
                    ++transition_probability[prev_s][s];
                else
                    ++transition_probability2[prev_prev_s][prev_s][s];
                prev_prev_s = prev_s;
                prev_s = s;
            }
        }
        foreach (float[] p in transition_probability)
            normalize(p);
        foreach (float[][] pp in transition_probability2)
            foreach (float[] p in pp)
                normalize(p);
    }

    //@Override
    public int[][] generate(int Length)
    {
        double[] pi = logToCdf(start_probability);
        double[][] A = logToCdf(transition_probability);
        double[][][] A2 = logToCdf(transition_probability2);
        double[][] B = logToCdf(emission_probability);
        int[][] os = new int[2][Length];
        os[1][0] = drawFrom(pi); // 采样首个隐状态
        os[0][0] = drawFrom(B[os[1][0]]); // 根据首个隐状态采样它的显状态

        os[1][1] = drawFrom(A[os[1][0]]);
        os[0][1] = drawFrom(B[os[1][1]]);

        for (int t = 2; t < Length; t++)
        {
            os[1][t] = drawFrom(A2[os[1][t - 2]][os[1][t - 1]]);
            os[0][t] = drawFrom(B[os[1][t]]);
        }

        return os;
    }

    private double[][][] logToCdf(float[][][] log)
    {
        double[][][] cdf = new double[log.Length][log[0].Length][log[0][0].Length];
        for (int i = 0; i < log.Length; i++)
        {
            cdf[i] = logToCdf(log[i]);
        }
        return cdf;
    }

    //@Override
    protected void toLog()
    {
        base.toLog();
        if (transition_probability2 != null)
        {
            foreach (float[][] m in transition_probability2)
            {
                foreach (float[] v in m)
                {
                    for (int i = 0; i < v.Length; i++)
                    {
                        v[i] = (float) Math.Log(v[i]);
                    }
                }
            }
        }
    }

    //@Override
    public float predict(int[] observation, int[] state)
    {
        int time = observation.Length; // 序列长度
        int max_s = start_probability.Length; // 状态种数

        float[][] score = new float[max_s][max_s];
        float[] first = new float[max_s];

        // link[i][s][t] := 第i个时刻在前一个状态是s，当前状态是t时，前2个状态是什么
        int[][][] link = new int[time][max_s][max_s];
        // 第一个时刻，使用初始概率向量乘以发射概率矩阵
        for (int cur_s = 0; cur_s < max_s; ++cur_s)
        {
            first[cur_s] = start_probability[cur_s] + emission_probability[cur_s][observation[0]];
        }

        if (time == 1)
        {
            int best_s = 0;
            float max_score = int.MinValue;
            for (int cur_s = 0; cur_s < max_s; ++cur_s)
            {
                if (first[cur_s] > max_score)
                {
                    best_s = cur_s;
                    max_score = first[cur_s];
                }
            }
            state[0] = best_s;
            return max_score;
        }

        // 第二个时刻，使用前一个时刻的概率向量乘以一阶转移矩阵乘以发射概率矩阵
        for (int f = 0; f < max_s; ++f)
        {
            for (int s = 0; s < max_s; ++s)
            {
                float p = first[f] + transition_probability[f][s] + emission_probability[s][observation[1]];
                score[f][s] = p;
                link[1][f][s] = f;
            }
        }

        // 从第三个时刻开始，使用前一个时刻的概率矩阵乘以二阶转移张量乘以发射概率矩阵
        float[][] pre = new float[max_s][max_s];
        for (int i = 2; i < observation.Length; i++)
        {
            // swap(now, pre)
            float[][] _ = pre;
            pre = score;
            score = _;
            // end of swap
            for (int s = 0; s < max_s; ++s)
            {
                for (int t = 0; t < max_s; ++t)
                {
                    score[s][t] = int.MinValue;
                    for (int f = 0; f < max_s; ++f)
                    {
                        float p = pre[f][s] + transition_probability2[f][s][t] + emission_probability[t][observation[i]];
                        if (p > score[s][t])
                        {
                            score[s][t] = p;
                            link[i][s][t] = f;
                        }
                    }
                }
            }
        }

        float max_score = int.MinValue;
        int best_s = 0, best_t = 0;
        for (int s = 0; s < max_s; s++)
        {
            for (int t = 0; t < max_s; t++)
            {
                if (score[s][t] > max_score)
                {
                    max_score = score[s][t];
                    best_s = s;
                    best_t = t;
                }
            }
        }

        for (int i = link.Length - 1; i >= 0; --i)
        {
            state[i] = best_t;
            int best_f = link[i][best_s][best_t];
            best_t = best_s;
            best_s = best_f;
        }

        return max_score;
    }

    //@Override
    public void unLog()
    {
        base.unLog();
        foreach (float[][] m in transition_probability2)
        {
            foreach (float[] v in m)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    v[i] = (float) Math.Exp(v[i]);
                }
            }
        }
    }

    //@Override
    public bool similar(HiddenMarkovModel model)
    {
        if (!(model is SecondOrderHiddenMarkovModel)) return false;
        SecondOrderHiddenMarkovModel hmm2 = (SecondOrderHiddenMarkovModel) model;
        for (int i = 0; i < transition_probability.Length; i++)
        {
            for (int j = 0; j < transition_probability.Length; j++)
            {
                if (!similar(transition_probability2[i][j], hmm2.transition_probability2[i][j]))
                    return false;
            }
        }
        return base.similar(model);
    }
}
