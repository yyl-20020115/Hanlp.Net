/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-28 14:35</create-date>
 *
 * <copyright file="NERInstance.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.mining.word2vec;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.tagset;
using System.Text;

namespace com.hankcs.hanlp.model.perceptron.instance;



/**
 * @author hankcs
 */
public class NERInstance : Instance
{
    public NERInstance(string[] wordArray, string[] posArray, string[] nerArray, NERTagSet tagSet, FeatureMap featureMap)
    {
        this(wordArray, posArray, featureMap);

        tagArray = new int[wordArray.Length];
        for (int i = 0; i < wordArray.Length; i++)
        {
            tagArray[i] = tagSet.Add(nerArray[i]);
        }
    }

    public NERInstance(string[] wordArray, string[] posArray, FeatureMap featureMap)
    {
        initFeatureMatrix(wordArray, posArray, featureMap);
    }

    private void initFeatureMatrix(string[] wordArray, string[] posArray, FeatureMap featureMap)
    {
        featureMatrix = new int[wordArray.Length][];
        for (int i = 0; i < featureMatrix.Length; i++)
        {
            featureMatrix[i] = extractFeature(wordArray, posArray, featureMap, i);
        }
    }

    /**
     * 提取特征，override此方法来拓展自己的特征模板
     *
     * @param wordArray  词语
     * @param posArray   词性
     * @param featureMap 储存特征的结构
     * @param position   当前提取的词语所在的位置
     * @return 特征向量
     */
    protected int[] extractFeature(string[] wordArray, string[] posArray, FeatureMap featureMap, int position)
    {
        List<int> featVec = new ();

        string pre2Word = position >= 2 ? wordArray[position - 2] : "_B_";
        string preWord = position >= 1 ? wordArray[position - 1] : "_B_";
        string curWord = wordArray[position];
        string nextWord = position <= wordArray.Length - 2 ? wordArray[position + 1] : "_E_";
        string next2Word = position <= wordArray.Length - 3 ? wordArray[position + 2] : "_E_";

        string pre2Pos = position >= 2 ? posArray[position - 2] : "_B_";
        string prePos = position >= 1 ? posArray[position - 1] : "_B_";
        string curPos = posArray[position];
        string nextPos = position <= posArray.Length - 2 ? posArray[position + 1] : "_E_";
        string next2Pos = position <= posArray.Length - 3 ? posArray[position + 2] : "_E_";

        StringBuilder sb = new StringBuilder();
        addFeatureThenClear(sb.Append(pre2Word).Append('1'), featVec, featureMap);
        addFeatureThenClear(sb.Append(preWord).Append('2'), featVec, featureMap);
        addFeatureThenClear(sb.Append(curWord).Append('3'), featVec, featureMap);
        addFeatureThenClear(sb.Append(nextWord).Append('4'), featVec, featureMap);
        addFeatureThenClear(sb.Append(next2Word).Append('5'), featVec, featureMap);
//        addFeatureThenClear(sb.Append(pre2Word).Append(preWord).Append('6'), featVec, featureMap);
//        addFeatureThenClear(sb.Append(preWord).Append(curWord).Append('7'), featVec, featureMap);
//        addFeatureThenClear(sb.Append(curWord).Append(nextWord).Append('8'), featVec, featureMap);
//        addFeatureThenClear(sb.Append(nextWord).Append(next2Word).Append('9'), featVec, featureMap);

        addFeatureThenClear(sb.Append(pre2Pos).Append('A'), featVec, featureMap);
        addFeatureThenClear(sb.Append(prePos).Append('B'), featVec, featureMap);
        addFeatureThenClear(sb.Append(curPos).Append('C'), featVec, featureMap);
        addFeatureThenClear(sb.Append(nextPos).Append('D'), featVec, featureMap);
        addFeatureThenClear(sb.Append(next2Pos).Append('E'), featVec, featureMap);
        addFeatureThenClear(sb.Append(pre2Pos).Append(prePos).Append('F'), featVec, featureMap);
        addFeatureThenClear(sb.Append(prePos).Append(curPos).Append('G'), featVec, featureMap);
        addFeatureThenClear(sb.Append(curPos).Append(nextPos).Append('H'), featVec, featureMap);
        addFeatureThenClear(sb.Append(nextPos).Append(next2Pos).Append('I'), featVec, featureMap);

        return toFeatureArray(featVec);
    }

    public static NERInstance create(string segmentedTaggedNERSentence, FeatureMap featureMap)
    {
        return create(Sentence.create(segmentedTaggedNERSentence), featureMap);
    }

    public static NERInstance create(Sentence sentence, FeatureMap featureMap)
    {
        if (sentence == null || featureMap == null) return null;

        NERTagSet tagSet = (NERTagSet) featureMap.tagSet;
        List<string[]> collector = Utility.convertSentenceToNER(sentence, tagSet);
        string[] wordArray = new string[collector.Count];
        string[] posArray = new string[collector.Count];
        string[] tagArray = new string[collector.Count];
        Utility.reshapeNER(collector, wordArray, posArray, tagArray);
        return new NERInstance(wordArray, posArray, tagArray, tagSet, featureMap);
    }

}
