/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/25 12:42</create-date>
 *
 * <copyright file="Model.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.maxent;




/**
 * 最大熵模型，采用双数组Trie树加速，值得拥有
 *
 * @author hankcs
 */
public class MaxEntModel
{
    /**
     * 常数C，训练的时候用到
     */
    int correctionConstant;
    /**
     * 为修正特征函数对应的参数，在预测的时候并没有用到
     */
    double correctionParam;
    /**
     * 归一化
     */
    UniformPrior prior;
    /**
     * 事件名
     */
    protected string[] outcomeNames;
    /**
     * 衡量参数
     */
    EvalParameters evalParams;

    /**
     * 将特征与一个数字（下标）对应起来的映射map
     */
    DoubleArrayTrie<int> pmap;

    /**
     * 预测分布
     *
     * @param context 环境
     * @return 概率数组
     */
    public double[] eval(string[] context)
    {
        return (eval(context, new double[evalParams.getNumOutcomes()]));
    }

    /**
     * 预测分布
     *
     * @param context
     * @return
     */
    public List<Pair<string, Double>> predict(string[] context)
    {
        List<Pair<string, Double>> result = new ArrayList<Pair<string, Double>>(outcomeNames.Length);
        double[] p = eval(context);
        for (int i = 0; i < p.Length; ++i)
        {
            result.add(new Pair<string, Double>(outcomeNames[i], p[i]));
        }
        return result;
    }

    /**
     * 预测概率最高的分类
     *
     * @param context
     * @return
     */
    public KeyValuePair<string, Double> predictBest(string[] context)
    {
        List<KeyValuePair<string, Double>> resultList = predict(context);
        double bestP = -1.0;
        KeyValuePair<string, Double> bestPair;
        for (KeyValuePair<string, Double> pair : resultList)
        {
            if (pair.getSecond() > bestP)
            {
                bestP = pair.getSecond();
                bestPair = pair;
            }
        }

        return bestPair;
    }

    /**
     * 预测分布
     *
     * @param context
     * @return
     */
    public List<KeyValuePair<string, Double>> predict(ICollection<string> context)
    {
        return predict(context.ToArray());
    }

    /**
     * 预测分布
     *
     * @param context 环境
     * @param outsums 先验分布
     * @return 概率数组
     */
    public double[] eval(string[] context, double[] outsums)
    {
        assert context != null;
        int[] scontexts = new int[context.Length];
        for (int i = 0; i < context.Length; i++)
        {
            int ci = pmap.get(context[i]);
            scontexts[i] = ci == null ? -1 : ci;
        }
        prior.logPrior(outsums);
        return eval(scontexts, outsums, evalParams);
    }

    /**
     * 预测
     * @param context 环境
     * @param prior 先验概率
     * @param model 特征函数
     * @return
     */
    public static double[] eval(int[] context, double[] prior, EvalParameters model)
    {
        Context[] _params = model.getParams();
        int[] numfeats = new int[model.getNumOutcomes()];
        int[] activeOutcomes;
        double[] activeParameters;
        double value = 1;
        for (int ci = 0; ci < context.Length; ci++)
        {
            if (context[ci] >= 0)
            {
                Context predParams = _params[context[ci]];
                activeOutcomes = predParams.getOutcomes();
                activeParameters = predParams.getParameters();
                for (int ai = 0; ai < activeOutcomes.Length; ai++)
                {
                    int oid = activeOutcomes[ai];
                    numfeats[oid]++;
                    prior[oid] += activeParameters[ai] * value;
                }
            }
        }

        double normal = 0.0;
        for (int oid = 0; oid < model.getNumOutcomes(); oid++)
        {
            if (model.getCorrectionParam() != 0)
            {
                prior[oid] = Math
                        .Exp(prior[oid]
                                     * model.getConstantInverse()
                                     + ((1.0 - ((double) numfeats[oid] / model
                                .getCorrectionConstant())) * model.getCorrectionParam()));
            }
            else
            {
                prior[oid] = Math.Exp(prior[oid] * model.getConstantInverse());
            }
            normal += prior[oid];
        }

        for (int oid = 0; oid < model.getNumOutcomes(); oid++)
        {
            prior[oid] /= normal;
        }
        return prior;
    }

    /**
     * 从文件加载，同时缓存为二进制文件
     * @param path
     * @return
     */
    public static MaxEntModel create(string path)
    {
        MaxEntModel m = new MaxEntModel();
        try
        {
            BufferedReader br = new BufferedReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            DataOutputStream _out = new DataOutputStream(IOUtil.newOutputStream(path + Predefine.BIN_EXT));
            br.readLine();  // type
            m.correctionConstant = int.parseInt(br.readLine());  // correctionConstant
            _out.writeInt(m.correctionConstant);
            m.correctionParam = Double.parseDouble(br.readLine());  // getCorrectionParameter
            _out.writeDouble(m.correctionParam);
            // label
            int numOutcomes = int.parseInt(br.readLine());
            _out.writeInt(numOutcomes);
            string[] outcomeLabels = new string[numOutcomes];
            m.outcomeNames = outcomeLabels;
            for (int i = 0; i < numOutcomes; i++)
            {
                outcomeLabels[i] = br.readLine();
                TextUtility.writeString(outcomeLabels[i], _out);
            }
            // pattern
            int numOCTypes = int.parseInt(br.readLine());
            _out.writeInt(numOCTypes);
            int[][] outcomePatterns = new int[numOCTypes][];
            for (int i = 0; i < numOCTypes; i++)
            {
                StringTokenizer tok = new StringTokenizer(br.readLine(), " ");
                int[] infoInts = new int[tok.countTokens()];
                _out.writeInt(infoInts.Length);
                for (int j = 0; tok.hasMoreTokens(); j++)
                {
                    infoInts[j] = int.parseInt(tok.nextToken());
                    _out.writeInt(infoInts[j]);
                }
                outcomePatterns[i] = infoInts;
            }
            // feature
            int NUM_PREDS = int.parseInt(br.readLine());
            _out.writeInt(NUM_PREDS);
            string[] predLabels = new string[NUM_PREDS];
            m.pmap = new DoubleArrayTrie<int>();
            var tmpMap = new Dictionary<string, int>();
            for (int i = 0; i < NUM_PREDS; i++)
            {
                predLabels[i] = br.readLine();
                //assert !tmpMap.containsKey(predLabels[i]) : "重复的键： " + predLabels[i] + " 请使用 -Dfile.encoding=UTF-8 训练";
                TextUtility.writeString(predLabels[i], _out);
                tmpMap.put(predLabels[i], i);
            }
            m.pmap.build(tmpMap);
            for (KeyValuePair<string, int> entry : tmpMap.entrySet())
            {
                _out.writeInt(entry.getValue());
            }
            m.pmap.save(_out);
            // params
            Context[] _params = new Context[NUM_PREDS];
            int pid = 0;
            for (int i = 0; i < outcomePatterns.Length; i++)
            {
                int[] outcomePattern = new int[outcomePatterns[i].Length - 1];
                for (int k = 1; k < outcomePatterns[i].Length; k++)
                {
                    outcomePattern[k - 1] = outcomePatterns[i][k];
                }
                for (int j = 0; j < outcomePatterns[i][0]; j++)
                {
                    double[] contextParameters = new double[outcomePatterns[i].Length - 1];
                    for (int k = 1; k < outcomePatterns[i].Length; k++)
                    {
                        contextParameters[k - 1] = Double.parseDouble(br.readLine());
                        _out.writeDouble(contextParameters[k - 1]);
                    }
                    _params[pid] = new Context(outcomePattern, contextParameters);
                    pid++;
                }
            }
            // prior
            m.prior = new UniformPrior();
            m.prior.setLabels(outcomeLabels);
            // eval
            m.evalParams = new EvalParameters(_params, m.correctionParam, m.correctionConstant, outcomeLabels.Length);
            _out.close();
        }
        catch (Exception e)
        {
            logger.severe("从" + path + "加载最大熵模型失败！" + TextUtility.exceptionToString(e));
            return null;
        }
        return m;
    }

    /**
     * 从字节流快速加载
     * @param byteArray
     * @return
     */
    public static MaxEntModel create(ByteArray byteArray)
    {
        MaxEntModel m = new MaxEntModel();
        m.correctionConstant = byteArray.nextInt();  // correctionConstant
        m.correctionParam = byteArray.nextDouble();  // getCorrectionParameter
        // label
        int numOutcomes = byteArray.nextInt();
        string[] outcomeLabels = new string[numOutcomes];
        m.outcomeNames = outcomeLabels;
        for (int i = 0; i < numOutcomes; i++) outcomeLabels[i] = byteArray.nextString();
        // pattern
        int numOCTypes = byteArray.nextInt();
        int[][] outcomePatterns = new int[numOCTypes][];
        for (int i = 0; i < numOCTypes; i++)
        {
            int Length = byteArray.nextInt();
            int[] infoInts = new int[Length];
            for (int j = 0; j < Length; j++)
            {
                infoInts[j] = byteArray.nextInt();
            }
            outcomePatterns[i] = infoInts;
        }
        // feature
        int NUM_PREDS = byteArray.nextInt();
        string[] predLabels = new string[NUM_PREDS];
        m.pmap = new DoubleArrayTrie<int>();
        for (int i = 0; i < NUM_PREDS; i++)
        {
            predLabels[i] = byteArray.nextString();
        }
        int[] v = new int[NUM_PREDS];
        for (int i = 0; i < v.Length; i++)
        {
            v[i] = byteArray.nextInt();
        }
        m.pmap.load(byteArray, v);
        // params
        Context[] _params = new Context[NUM_PREDS];
        int pid = 0;
        for (int i = 0; i < outcomePatterns.Length; i++)
        {
            int[] outcomePattern = new int[outcomePatterns[i].Length - 1];
            for (int k = 1; k < outcomePatterns[i].Length; k++)
            {
                outcomePattern[k - 1] = outcomePatterns[i][k];
            }
            for (int j = 0; j < outcomePatterns[i][0]; j++)
            {
                double[] contextParameters = new double[outcomePatterns[i].Length - 1];
                for (int k = 1; k < outcomePatterns[i].Length; k++)
                {
                    contextParameters[k - 1] = byteArray.nextDouble();
                }
                _params[pid] = new Context(outcomePattern, contextParameters);
                pid++;
            }
        }
        // prior
        m.prior = new UniformPrior();
        m.prior.setLabels(outcomeLabels);
        // eval
        m.evalParams = new EvalParameters(_params, m.correctionParam, m.correctionConstant, outcomeLabels.Length);
        return m;
    }

    /**
     * 加载最大熵模型<br>
     *     如果存在缓存的话，优先读取缓存，否则读取txt，并且建立缓存
     * @param txtPath txt的路径，即使不存在.txt，只存在.bin，也应传入txt的路径，方法内部会自动加.bin后缀
     * @return
     */
    public static MaxEntModel load(string txtPath)
    {
        ByteArray byteArray = ByteArray.createByteArray(txtPath + Predefine.BIN_EXT);
        if (byteArray != null) return create(byteArray);
        return create(txtPath);
    }
}
