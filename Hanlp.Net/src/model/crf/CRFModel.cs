/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/9 20:53</create-date>
 *
 * <copyright file="Model.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.crf;




/**
 * 这份代码目前做到了与CRF++解码结果完全一致。也可以直接使用移植版的CRF++ {@link CRFLexicalAnalyzer}
 *
 * @author hankcs
 */
public class CRFModel : ICacheAble
{
    /**
     * 标签和id的相互转换
     */
    Dictionary<string, int> tag2id;
    /**
     * id转标签
     */
    protected string[] id2tag;
    /**
     * 特征函数
     */
    ITrie<FeatureFunction> featureFunctionTrie;
    /**
     * 特征模板
     */
    List<FeatureTemplate> featureTemplateList;
    /**
     * tag的二元转移矩阵，适用于BiGram Feature
     */
    protected double[][] matrix;

    public CRFModel()
    {
        featureFunctionTrie = new DoubleArrayTrie<FeatureFunction>();
    }

    /**
     * 以指定的trie树结构储存内部特征函数
     *
     * @param featureFunctionTrie
     */
    public CRFModel(ITrie<FeatureFunction> featureFunctionTrie)
    {
        this.featureFunctionTrie = featureFunctionTrie;
    }

    protected void onLoadTxtFinished()
    {
        // do nothing
    }

    /**
     * 加载Txt形式的CRF++模型
     *
     * @param path     模型路径
     * @param instance 模型的实例（这里允许用户构造不同的CRFModel来储存最终读取的结果）
     * @return 该模型
     */
    public static CRFModel loadTxt(string path, CRFModel instance)
    {
        CRFModel CRFModel = instance;
        // 先尝试从bin加载
        if (CRFModel.load(ByteArray.createByteArray(path + Predefine.BIN_EXT))) return CRFModel;
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(path);
        if (!lineIterator.MoveNext()) return null;
        logger.info(lineIterator.next());   // verson
        logger.info(lineIterator.next());   // cost-factor
        int maxid = int.parseInt(lineIterator.next().substring("maxid:".Length).Trim());
        logger.info(lineIterator.next());   // xsize
        lineIterator.next();    // blank
        string line;
        int id = 0;
        CRFModel.tag2id = new ();
        while ((line = lineIterator.next()).Length != 0)
        {
            CRFModel.tag2id.Add(line, id);
            ++id;
        }
        CRFModel.id2tag = new string[CRFModel.tag2id.Count];
        int size = CRFModel.id2tag.Length;
        foreach (KeyValuePair<string, int> entry in CRFModel.tag2id)
        {
            CRFModel.id2tag[entry.Value] = entry.Key;
        }
        Dictionary<string, FeatureFunction> featureFunctionMap = new Dictionary<string, FeatureFunction>();  // 构建trie树的时候用
        Dictionary<int, FeatureFunction> featureFunctionList = new Dictionary<int, FeatureFunction>(); // 读取权值的时候用
        CRFModel.featureTemplateList = new ();
        while ((line = lineIterator.next()).Length != 0)
        {
            if (!"B".Equals(line))
            {
                FeatureTemplate featureTemplate = FeatureTemplate.create(line);
                CRFModel.featureTemplateList.Add(featureTemplate);
            }
            else
            {
                CRFModel.matrix = new double[size][size];
            }
        }

        int b = -1;// 转换矩阵的权重位置
        if (CRFModel.matrix != null)
        {
            string[] args = lineIterator.next().Split(" ", 2);    // 0 B
            b = int.valueOf(args[0]);
            featureFunctionList.Add(b, null);
        }

        while ((line = lineIterator.next()).Length != 0)
        {
            string[] args = line.Split(" ", 2);
            char[] charArray = args[1].ToCharArray();
            FeatureFunction featureFunction = new FeatureFunction(charArray, size);
            featureFunctionMap.Add(args[1], featureFunction);
            featureFunctionList.Add(int.parseInt(args[0]), featureFunction);
        }

        foreach (KeyValuePair<int, FeatureFunction> entry in featureFunctionList)
        {
            int fid = entry.Key;
            FeatureFunction featureFunction = entry.Value;
            if (fid == b)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        CRFModel.matrix[i][j] = Double.parseDouble(lineIterator.next());
                    }
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    featureFunction.w[i] = Double.parseDouble(lineIterator.next());
                }
            }
        }
        if (lineIterator.MoveNext())
        {
            logger.warning("文本读取有残留，可能会出问题！" + path);
        }
        lineIterator.Close();
        logger.info("开始构建trie树");
        CRFModel.featureFunctionTrie.build(featureFunctionMap);
        // 缓存bin
        try
        {
            logger.info("开始缓存" + path + Predefine.BIN_EXT);
            Stream Out = new Stream(IOUtil.newOutputStream(path + Predefine.BIN_EXT));
            CRFModel.save(Out);
            Out.Close();
        }
        catch (Exception e)
        {
            logger.warning("在缓存" + path + Predefine.BIN_EXT + "时发生错误" + TextUtility.exceptionToString(e));
        }
        CRFModel.onLoadTxtFinished();
        return CRFModel;
    }

    /**
     * 维特比后向算法标注
     *
     * @param table
     */
    public void tag(Table table)
    {
        int size = table.Count;
        if (size == 0) return;
        int tagSize = id2tag.Length;
        double[][] net = new double[size][tagSize];
        for (int i = 0; i < size; ++i)
        {
            LinkedList<double[]> scoreList = computeScoreList(table, i);
            for (int tag = 0; tag < tagSize; ++tag)
            {
                net[i][tag] = computeScore(scoreList, tag);
            }
        }

        if (size == 1)
        {
            double maxScore = -1e10;
            int bestTag = 0;
            for (int tag = 0; tag < net[0].Length; ++tag)
            {
                if (net[0][tag] > maxScore)
                {
                    maxScore = net[0][tag];
                    bestTag = tag;
                }
            }
            table.setLast(0, id2tag[bestTag]);
            return;
        }

        int[][] from = new int[size][tagSize];
        double[][] maxScoreAt = new double[2][tagSize]; // 滚动数组
        Array.Copy(net[0], 0, maxScoreAt[0], 0, tagSize); // 初始preI=0,  maxScoreAt[preI][pre] = net[0][pre]
        int curI = 0;
        for (int i = 1; i < size; ++i)
        {
            curI = i & 1;
            int preI = 1 - curI;
            for (int now = 0; now < tagSize; ++now)
            {
                double maxScore = -1e10;
                for (int pre = 0; pre < tagSize; ++pre)
                {
                    double score = maxScoreAt[preI][pre] + matrix[pre][now] + net[i][now];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        from[i][now] = pre;
                        maxScoreAt[curI][now] = maxScore;
                    }
                }
                net[i][now] = maxScore;
            }
        }
        // 反向回溯最佳路径
        double maxScore = -1e10;
        int maxTag = 0;
        for (int tag = 0; tag < tagSize; ++tag)
        {
            if (maxScoreAt[curI][tag] > maxScore)
            {
                maxScore = maxScoreAt[curI][tag];
                maxTag = tag;
            }
        }

        table.setLast(size - 1, id2tag[maxTag]);
        maxTag = from[size - 1][maxTag];
        for (int i = size - 2; i > 0; --i)
        {
            table.setLast(i, id2tag[maxTag]);
            maxTag = from[i][maxTag];
        }
        table.setLast(0, id2tag[maxTag]);
    }

    /**
     * 根据特征函数计算输出
     *
     * @param table
     * @param current
     * @return
     */
    protected List<double[]> computeScoreList(Table table, int current)
    {
        List<double[]> scoreList = new ();
        foreach (FeatureTemplate featureTemplate in featureTemplateList)
        {
            char[] o = featureTemplate.generateParameter(table, current);
            FeatureFunction featureFunction = featureFunctionTrie.get(o);
            if (featureFunction == null) continue;
            scoreList.Add(featureFunction.w);
        }

        return scoreList;
    }

    /**
     * 给一系列特征函数结合tag打分
     *
     * @param scoreList
     * @param tag
     * @return
     */
    protected static double computeScore(List<double[]> scoreList, int tag)
    {
        double score = 0;
        foreach (double[] w in scoreList)
        {
            score += w[tag];
        }
        return score;
    }

    //@Override
    public void save(Stream Out)
    {
        Out.writeInt(id2tag.Length);
        foreach (string tag in id2tag)
        {
            Out.writeUTF(tag);
        }
        FeatureFunction[] valueArray = featureFunctionTrie.getValueArray(new FeatureFunction[0]);
        Out.writeInt(valueArray.Length);
        foreach (FeatureFunction featureFunction in valueArray)
        {
            featureFunction.save(Out);
        }
        featureFunctionTrie.save(Out);
        Out.writeInt(featureTemplateList.Count);
        foreach (FeatureTemplate featureTemplate in featureTemplateList)
        {
            featureTemplate.save(Out);
        }
        if (matrix != null)
        {
            Out.writeInt(matrix.Length);
            foreach (double[] line in matrix)
            {
                foreach (double v in line)
                {
                    Out.writeDouble(v);
                }
            }
        }
        else
        {
            Out.writeInt(0);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        if (byteArray == null) return false;
        try
        {
            int size = byteArray.Next();
            id2tag = new string[size];
            tag2id = new (size);
            for (int i = 0; i < id2tag.Length; i++)
            {
                id2tag[i] = byteArray.nextUTF();
                tag2id.Add(id2tag[i], i);
            }
            FeatureFunction[] valueArray = new FeatureFunction[byteArray.Next()];
            for (int i = 0; i < valueArray.Length; i++)
            {
                valueArray[i] = new FeatureFunction();
                valueArray[i].load(byteArray);
            }
            featureFunctionTrie.load(byteArray, valueArray);
            size = byteArray.Next();
            featureTemplateList = new (size);
            for (int i = 0; i < size; ++i)
            {
                FeatureTemplate featureTemplate = new FeatureTemplate();
                featureTemplate.load(byteArray);
                featureTemplateList.Add(featureTemplate);
            }
            size = byteArray.Next();
            if (size == 0) return true;
            matrix = new double[size][size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i][j] = byteArray.nextDouble();
                }
            }
        }
        catch (Exception e)
        {
            logger.warning("缓存载入失败，可能是由于版本变迁带来的不兼容。具体异常是：\n" + TextUtility.exceptionToString(e));
            return false;
        }

        return true;
    }

    /**
     * 加载Txt形式的CRF++模型<br>
     * 同时生成path.bin模型缓存
     *
     * @param path 模型路径
     * @return 该模型
     */
    public static CRFModel loadTxt(string path)
    {
        return loadTxt(path, new CRFModel(new DoubleArrayTrie<FeatureFunction>()));
    }

    /**
     * 加载CRF++模型<br>
     * 如果存在缓存的话，优先读取缓存，否则读取txt，并且建立缓存
     *
     * @param path txt的路径，即使不存在.txt，只存在.bin，也应传入txt的路径，方法内部会自动加.bin后缀
     * @return
     */
    public static CRFModel load(string path)
    {
        CRFModel model = loadBin(path + BIN_EXT);
        if (model != null) return model;
        return loadTxt(path, new CRFModel(new DoubleArrayTrie<FeatureFunction>()));
    }

    /**
     * 加载Bin形式的CRF++模型<br>
     * 注意该Bin形式不是CRF++的二进制模型,而是HanLP由CRF++的文本模型转换过来的私有格式
     *
     * @param path
     * @return
     */
    public static CRFModel loadBin(string path)
    {
        ByteArray byteArray = ByteArray.createByteArray(path);
        if (byteArray == null) return null;
        CRFModel model = new CRFModel();
        if (model.load(byteArray)) return model;
        return null;
    }

    /**
     * 获取某个tag的ID
     *
     * @param tag
     * @return
     */
    public int getTagId(string tag)
    {
        return tag2id.get(tag);
    }
}
