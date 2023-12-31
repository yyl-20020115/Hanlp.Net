/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM10:29</create-date>
 *
 * <copyright file="LinearModel.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.common;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.perceptron.model;




/**
 * 在线学习标注模型
 *
 * @author hankcs
 */
public class LinearModel : ICacheAble
{
    /**
     * 特征函数
     */
    public FeatureMap featureMap;
    /**
     * 特征权重
     */
    public float[] parameter;


    public LinearModel(FeatureMap featureMap, float[] parameter)
    {
        this.featureMap = featureMap;
        this.parameter = parameter;
    }

    public LinearModel(FeatureMap featureMap)
    {
        this.featureMap = featureMap;
        parameter = new float[featureMap.size() * featureMap.tagSet.size()];
    }

    public LinearModel(string modelFile) 
    {
        load(modelFile);
    }

    /**
     * 模型压缩
     * @param ratio 压缩比c（压缩掉的体积，压缩后体积变为1-c）
     * @return
     */
    public LinearModel compress(double ratio)
    {
        return compress(ratio, 1e-3f);
    }

    /**
     * @param ratio 压缩比c（压缩掉的体积，压缩后体积变为1-c）
     * @param threshold 特征权重绝对值之和最低阈值
     * @return
     */
    public LinearModel compress(double ratio, double threshold)
    {
        if (ratio < 0 || ratio >= 1)
        {
            throw new ArgumentException("压缩比必须介于 0 和 1 之间");
        }
        if (ratio == 0) return this;
        var featureIdSet = featureMap.entrySet();
        TagSet tagSet = featureMap.tagSet;
        MaxHeap<FeatureSortItem> heap = new MaxHeap<FeatureSortItem>(
            (int) ((featureIdSet.size() - tagSet.sizeIncludingBos()) * (1.0f - ratio)),
            new COMP()
        );

        logger.start("裁剪特征...\n");
        int logEvery = (int) Math.Ceiling(featureMap.size() / 10000f);
        int n = 0;
        foreach (KeyValuePair<string, int> entry in featureIdSet)
        {
            if (++n % logEvery == 0 || n == featureMap.size())
            {
                logger._out("\r%.2f%% ", MathUtility.percentage(n, featureMap.size()));
            }
            if (entry.Value < tagSet.sizeIncludingBos())
            {
                continue;
            }
            FeatureSortItem item = new FeatureSortItem(entry, this.parameter, tagSet.size());
            if (item.total < threshold) continue;
            heap.Add(item);
        }
        logger.finish("\n裁剪完毕\n");

        int size = heap.size() + tagSet.sizeIncludingBos();
        float[] parameter = new float[size * tagSet.size()];
        MutableDoubleArrayTrieInteger mdat = new MutableDoubleArrayTrieInteger();
        foreach (KeyValuePair<string, int> tag in tagSet)
        {
            mdat.Add("BL=" + tag.Key);
        }
        mdat.Add("BL=_BL_");
        for (int i = 0; i < tagSet.size() * tagSet.sizeIncludingBos(); i++)
        {
            parameter[i] = this.parameter[i];
        }
        logger.start("构建双数组trie树...\n");
        logEvery = (int) Math.Ceiling(heap.size() / 10000f);
        n = 0;
        foreach (FeatureSortItem item in heap)
        {
            if (++n % logEvery == 0 || n == heap.size())
            {
                logger._out("\r%.2f%% ", MathUtility.percentage(n, heap.size()));
            }
            int id = mdat.size();
            mdat.Add(item.key, id);
            for (int i = 0; i < tagSet.size(); ++i)
            {
                parameter[id * tagSet.size() + i] = this.parameter[item.id * tagSet.size() + i];
            }
        }
        logger.finish("\n构建完毕\n");
        this.featureMap = new ImmutableFeatureMDatMap(mdat, tagSet);
        this.parameter = parameter;
        return this;
    }
    public class COMP :IComparer<FeatureSortItem> 
    {
        //@Override
        public int Compare(FeatureSortItem? o1, FeatureSortItem? o2)
        {
            return float.Compare(o1.total, o2.total);
        }
    }
    /**
     * 保存到路径
     *
     * @param modelFile
     * @
     */
    public void save(string modelFile) 
    {
        Stream _out = new Stream(new BufferedOutputStream(IOUtil.newOutputStream(modelFile)));
        save(_out);
        _out.Close();
    }

    /**
     * 压缩并保存
     *
     * @param modelFile 路径
     * @param ratio     压缩比c（压缩掉的体积，压缩后体积变为1-c）
     * @
     */
    public void save(string modelFile,  double ratio) 
    {
        save(modelFile, featureMap.entrySet(), ratio);
    }

    public void save(string modelFile, HashSet<KeyValuePair<string, int>> featureIdSet, double ratio) 
    {
        save(modelFile, featureIdSet, ratio, false);
    }

    /**
     * 保存
     *
     * @param modelFile    路径
     * @param featureIdSet 特征集（有些数据结构不支持遍历，可以提供构造时用到的特征集来规避这个缺陷）
     * @param ratio        压缩比
     * @param text         是否输出文本以供调试
     * @
     */
    public void save(string modelFile, HashSet<KeyValuePair<string, int>> featureIdSet, double ratio, bool text) 
    {
        float[] parameter = this.parameter;
        this.compress(ratio, 1e-3f);

        Stream _out = new Stream(new BufferedOutputStream(IOUtil.newOutputStream(modelFile)));
        save(_out);
        _out.Close();

        if (text)
        {
            TextWriter bw = new TextWriter(new StreamWriter(IOUtil.newOutputStream(modelFile + ".txt"), "UTF-8"));
            TagSet tagSet = featureMap.tagSet;
            for (KeyValuePair<string, int> entry : featureIdSet)
            {
                bw.write(entry.Key);
                if (featureIdSet.size() == parameter.Length)
                {
                    bw.write("\t");
                    bw.write(string.valueOf(parameter[entry.Value]));
                }
                else
                {
                    for (int i = 0; i < tagSet.size(); ++i)
                    {
                        bw.write("\t");
                        bw.write(string.valueOf(parameter[entry.Value * tagSet.size() + i]));
                    }
                }
                bw.newLine();
            }
            bw.Close();
        }
    }

    /**
     * 参数更新
     *
     * @param x 特征向量
     * @param y 正确答案
     */
    public void update(ICollection<int> x, int y)
    {
        //assert y == 1 || y == -1 : "感知机的标签y必须是±1";
        foreach (int f in x)
            parameter[f] += y;
    }

    /**
     * 分离超平面解码
     *
     * @param x 特征向量
     * @return sign(wx)
     */
    public int decode(ICollection<int> x)
    {
        float y = 0;
        foreach (int f in x)
            y += parameter[f];
        return y < 0 ? -1 : 1;
    }

    /**
     * 维特比解码
     *
     * @param instance 实例
     * @return
     */
    public double viterbiDecode(Instance instance)
    {
        return viterbiDecode(instance, instance.tagArray);
    }

    /**
     * 维特比解码
     *
     * @param instance   实例
     * @param guessLabel 输出标签
     * @return
     */
    public double viterbiDecode(Instance instance, int[] guessLabel)
    {
         int[] allLabel = featureMap.allLabels();
         int bos = featureMap.bosTag();
         int sentenceLength = instance.tagArray.Length;
         int labelSize = allLabel.Length;

        int[][] preMatrix = new int[sentenceLength][labelSize];
        double[][] scoreMatrix = new double[2][labelSize];

        for (int i = 0; i < sentenceLength; i++)
        {
            int _i = i & 1;
            int _i_1 = 1 - _i;
            int[] allFeature = instance.getFeatureAt(i);
            int transitionFeatureIndex = allFeature.Length - 1;
            if (0 == i)
            {
                allFeature[transitionFeatureIndex] = bos;
                for (int j = 0; j < allLabel.Length; j++)
                {
                    preMatrix[0][j] = j;

                    double score = score(allFeature, j);

                    scoreMatrix[0][j] = score;
                }
            }
            else
            {
                for (int curLabel = 0; curLabel < allLabel.Length; curLabel++)
                {

                    double maxScore = int.MinValue;

                    for (int preLabel = 0; preLabel < allLabel.Length; preLabel++)
                    {

                        allFeature[transitionFeatureIndex] = preLabel;
                        double score2 = score(allFeature, curLabel);

                        double curScore = scoreMatrix[_i_1][preLabel] + score2;

                        if (maxScore < curScore)
                        {
                            maxScore = curScore;
                            preMatrix[i][curLabel] = preLabel;
                            scoreMatrix[_i][curLabel] = maxScore;
                        }
                    }
                }

            }
        }

        int maxIndex = 0;
        double maxScore = scoreMatrix[(sentenceLength - 1) & 1][0];

        for (int index = 1; index < allLabel.Length; index++)
        {
            if (maxScore < scoreMatrix[(sentenceLength - 1) & 1][index])
            {
                maxIndex = index;
                maxScore = scoreMatrix[(sentenceLength - 1) & 1][index];
            }
        }

        for (int i = sentenceLength - 1; i >= 0; --i)
        {
            guessLabel[i] = allLabel[maxIndex];
            maxIndex = preMatrix[i][maxIndex];
        }

        return maxScore;
    }

    /**
     * 通过命中的特征函数计算得分
     *
     * @param featureVector 压缩形式的特征id构成的特征向量
     * @return
     */
    public double score(int[] featureVector, int currentTag)
    {
        double score = 0;
        foreach (int index in featureVector)
        {
            if (index == -1)
            {
                continue;
            }
            else if (index < -1 || index >= featureMap.size())
            {
                throw new ArgumentException("在打分时传入了非法的下标");
            }
            else
            {
                var index2 = index * featureMap.tagSet.size() + currentTag;
                score += parameter[index2];    // 其实就是特征权重的累加
            }
        }
        return score;
    }

    /**
     * 加载模型
     *
     * @param modelFile
     * @
     */
    public void load(string modelFile) 
    {
        if (HanLP.Config.DEBUG)
            logger.start("加载 %s ... ", modelFile);
        ByteArrayStream byteArray = ByteArrayStream.createByteArrayStream(modelFile);
        if (!load(byteArray))
        {
            throw new IOException(string.Format("%s 加载失败", modelFile));
        }
        if (HanLP.Config.DEBUG)
            logger.finish(" 加载完毕\n");
    }

    public TagSet tagSet()
    {
        return featureMap.tagSet;
    }

    //@Override
    public void save(Stream _out) 
    {
        if (!(featureMap is ImmutableFeatureMDatMap))
        {
            featureMap = new ImmutableFeatureMDatMap(featureMap.entrySet(), tagSet());
        }
        featureMap.save(_out);
        foreach (float aParameter in this.parameter)
        {
            _out.writeFloat(aParameter);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        if (byteArray == null)
            return false;
        featureMap = new ImmutableFeatureMDatMap();
        featureMap.load(byteArray);
        int size = featureMap.size();
        TagSet tagSet = featureMap.tagSet;
        if (tagSet.type == TaskType.CLASSIFICATION)
        {
            parameter = new float[size];
            for (int i = 0; i < size; i++)
            {
                parameter[i] = byteArray.nextFloat();
            }
        }
        else
        {
            parameter = new float[size * tagSet.size()];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < tagSet.size(); ++j)
                {
                    parameter[i * tagSet.size() + j] = byteArray.nextFloat();
                }
            }
        }
//        assert !byteArray.hasMore();
//        byteArray.Close();
        if (!byteArray.hasMore())
            byteArray.Close();
        return true;
    }

    public TaskType taskType()
    {
        return featureMap.tagSet.type;
    }
}
