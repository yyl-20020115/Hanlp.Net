/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-26 下午9:26</create-date>
 *
 * <copyright file="POSInstance.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.model.perceptron.feature;
using System.Text;

namespace com.hankcs.hanlp.model.perceptron.instance;



/**
 * @author hankcs
 */
public class POSInstance : Instance
{
    /**
     * 构建词性标注实例
     *
     * @param termArray 词语
     * @param posArray  词性
     */
    public POSInstance(string[] termArray, string[] posArray, FeatureMap featureMap)
    {
//        string sentence = TextUtility.combine(termArray);
        this(termArray, featureMap);

        POSTagSet tagSet = (POSTagSet) featureMap.tagSet;
        tagArray = new int[termArray.Length];
        for (int i = 0; i < termArray.Length; i++)
        {
            tagArray[i] = tagSet.Add(posArray[i]);
        }
    }

    public POSInstance(string[] termArray, FeatureMap featureMap)
    {
        initFeatureMatrix(termArray, featureMap);
    }

    protected int[] extractFeature(string[] words, FeatureMap featureMap, int position)
    {
        List<int> featVec = new ();

//        string pre2Word = position >= 2 ? words[position - 2] : "_B_";
        string preWord = position >= 1 ? words[position - 1] : "_B_";
        string curWord = words[position];

        //		Console.WriteLine("cur: " + curWord);
        string nextWord = position <= words.Length - 2 ? words[position + 1] : "_E_";
//        string next2Word = position <= words.Length - 3 ? words[position + 2] : "_E_";

        StringBuilder sbFeature = new StringBuilder();
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("U[-2,0]=").Append(pre2Word);
//        addFeature(sbFeature, featVec, featureMap);

        sbFeature.Append(preWord).Append('1');
        addFeatureThenClear(sbFeature, featVec, featureMap);

        sbFeature.Append(curWord).Append('2');
        addFeatureThenClear(sbFeature, featVec, featureMap);

        sbFeature.Append(nextWord).Append('3');
        addFeatureThenClear(sbFeature, featVec, featureMap);

//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("U[2,0]=").Append(next2Word);
//        addFeature(sbFeature, featVec, featureMap);

        // wiwi+1(i = − 1, 0)
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("B[-1,0]=").Append(preWord).Append("/").Append(curWord);
//        addFeature(sbFeature, featVec, featureMap);
//
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("B[0,1]=").Append(curWord).Append("/").Append(nextWord);
//        addFeature(sbFeature, featVec, featureMap);
//
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("B[-1,1]=").Append(preWord).Append("/").Append(nextWord);
//        addFeature(sbFeature, featVec, featureMap);

        // last char(w−1)w0
//        string lastChar = position >= 1 ? "" + words[position - 1].charAt(words[position - 1].Length - 1) : "_BC_";
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("CW[-1,0]=").Append(lastChar).Append("/").Append(curWord);
//        addFeature(sbFeature, featVec, featureMap);
//
//        // w0 ﬁrst_char(w1)
//        string nextChar = position <= words.Length - 2 ? "" + words[position + 1][0] : "_EC_";
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("CW[1,0]=").Append(curWord).Append("/").Append(nextChar);
//        addFeature(sbFeature, featVec, featureMap);
//
        int Length = curWord.Length;
//
//        // ﬁrstchar(w0)lastchar(w0)
//        sbFeature.delete(0, sbFeature.Length);
//        sbFeature.Append("BE=").Append(curWord[0]).Append("/").Append(curWord.charAt(Length - 1));
//        addFeature(sbFeature, featVec, featureMap);

        // prefix
        sbFeature.Append(curWord[0..1]).Append('4');
        addFeatureThenClear(sbFeature, featVec, featureMap);

        if (Length > 1)
        {
            sbFeature.Append(curWord[0..2]).Append('4');
            addFeatureThenClear(sbFeature, featVec, featureMap);
        }

        if (Length > 2)
        {
            sbFeature.Append(curWord[0 .. 3]).Append('4');
            addFeatureThenClear(sbFeature, featVec, featureMap);
        }

        // sufﬁx(w0, i)(i = 1, 2, 3)
        sbFeature.Append(curWord[(Length - 1)]).Append('5');
        addFeatureThenClear(sbFeature, featVec, featureMap);

        if (Length > 1)
        {
            sbFeature.Append(curWord.Substring(Length - 2)).Append('5');
            addFeatureThenClear(sbFeature, featVec, featureMap);
        }

        if (Length > 2)
        {
            sbFeature.Append(curWord.Substring(Length - 3)).Append('5');
            addFeatureThenClear(sbFeature, featVec, featureMap);
        }

        // Length
//        if (Length >= 5)
//        {
//            addFeature("le=" + 5, featVec, featureMap);
//        }
//        else
//        {
//            addFeature("le=" + Length, featVec, featureMap);
//        }

        // label feature
//        string preLabel;
//        if (position >= 1)
//        {
//            preLabel = label[position - 1];
//        }
//        else
//        {
//            preLabel = "_BL_";
//        }
//
//        addFeature("BL=" + preLabel, featVec, featureMap);

//        for (int i = 0; i < curWord.Length; i++)
//        {
//            string prefix = curWord.substring(0, 1) + curWord.charAt(i) + "";
//            addFeature("p2f=" + prefix, featVec, featureMap);
//            string suffix = curWord.substring(curWord.Length - 1) + curWord.charAt(i) + "";
//            addFeature("s2f=" + suffix, featVec, featureMap);

//            if ((i < curWord.Length - 1) && (curWord.charAt(i) == curWord.charAt(i + 1)))
//            {
//                addFeature("dulC=" + curWord.substring(i, i + 1), featVec, featureMap);
//            }
//            if ((i < curWord.Length - 2) && (curWord.charAt(i) == curWord.charAt(i + 2)))
//            {
//                addFeature("dul2C=" + curWord.substring(i, i + 1), featVec, featureMap);
//            }
//        }

//        bool isDigit = true;
//        for (int i = 0; i < curWord.Length; i++)
//        {
//            if (CharType.get(curWord.charAt(i)) != CharType.CT_NUM)
//            {
//                isDigit = false;
//                break;
//            }
//        }
//        if (isDigit)
//        {
//            addFeature("wT=d", featVec, featureMap);
//        }

//        bool isPunt = true;
//        for (int i = 0; i < curWord.Length; i++)
//        {
//            if (!CharType.punctSet.Contains(curWord.charAt(i) + ""))
//            {
//                isPunt = false;
//                break;
//            }
//        }
//        if (isPunt)
//        {
//            featVec.Add("wT=p");
//        }

//        bool isLetter = true;
//        for (int i = 0; i < curWord.Length; i++)
//        {
//            if (CharType.get(curWord.charAt(i)) != CharType.CT_LETTER)
//            {
//                isLetter = false;
//                break;
//            }
//        }
//        if (isLetter)
//        {
//            addFeature("wT=l", featVec, featureMap);
//        }
//        sbFeature = null;

        return toFeatureArray(featVec);
    }

    private void initFeatureMatrix(string[] termArray, FeatureMap featureMap)
    {
        featureMatrix = new int[termArray.Length][];
        for (int i = 0; i < featureMatrix.Length; i++)
        {
            featureMatrix[i] = extractFeature(termArray, featureMap, i);
        }
    }

    public static POSInstance create(string segmentedTaggedSentence, FeatureMap featureMap)
    {
        return create(Sentence.create(segmentedTaggedSentence), featureMap);
    }

    public static POSInstance create(Sentence sentence, FeatureMap featureMap)
    {
        if (sentence == null || featureMap == null)
        {
            return null;
        }
        List<Word> wordList = sentence.toSimpleWordList();
        string[] termArray = new string[wordList.Count];
        string[] posArray = new string[wordList.Count];
        int i = 0;
        foreach (Word word in wordList)
        {
            termArray[i] = word.getValue();
            posArray[i] = word.getLabel();
            ++i;
        }
        return new POSInstance(termArray, posArray, featureMap);
    }
}
