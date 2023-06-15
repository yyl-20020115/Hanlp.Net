/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-26 下午9:21</create-date>
 *
 * <copyright file="CWSInstance.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron.instance;



/**
 * @author hankcs
 */
public class CWSInstance : Instance
{
    private static readonly char CHAR_BEGIN = '\u0001';
    private static readonly char CHAR_END = '\u0002';

    /**
     * 生成分词实例
     *
     * @param termArray  分词序列
     * @param featureMap 特征收集
     */
    public CWSInstance(String[] termArray, FeatureMap featureMap)
    {
        String sentence = com.hankcs.hanlp.utility.TextUtility.combine(termArray);
        CWSTagSet tagSet = (CWSTagSet) featureMap.tagSet;

        tagArray = new int[sentence.length()];
        for (int i = 0, j = 0; i < termArray.length; i++)
        {
            assert termArray[i].length() > 0 : "句子中出现了长度为0的单词，不合法：" + sentence;
            if (termArray[i].length() == 1)
                tagArray[j++] = tagSet.S;
            else
            {
                tagArray[j++] = tagSet.B;
                for (int k = 1; k < termArray[i].length() - 1; k++)
                    tagArray[j++] = tagSet.M;
                tagArray[j++] = tagSet.E;
            }
        }

        initFeatureMatrix(sentence, featureMap);
    }

    public CWSInstance(String sentence, FeatureMap featureMap)
    {
        initFeatureMatrix(sentence, featureMap);
        tagArray = new int[sentence.length()];
    }

    protected int[] extractFeature(String sentence, FeatureMap featureMap, int position)
    {
        List<int> featureVec = new LinkedList<int>();

        char pre2Char = position >= 2 ? sentence.charAt(position - 2) : CHAR_BEGIN;
        char preChar = position >= 1 ? sentence.charAt(position - 1) : CHAR_BEGIN;
        char curChar = sentence.charAt(position);
        char nextChar = position < sentence.length() - 1 ? sentence.charAt(position + 1) : CHAR_END;
        char next2Char = position < sentence.length() - 2 ? sentence.charAt(position + 2) : CHAR_END;

        StringBuilder sbFeature = new StringBuilder();
        //char unigram feature
//        sbFeature.delete(0, sbFeature.length());
//        sbFeature.Append("U[-2,0]=").Append(pre2Char);
//        addFeature(sbFeature, featureVec, featureMap);

        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(preChar).Append('1');
        addFeature(sbFeature, featureVec, featureMap);

        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(curChar).Append('2');
        addFeature(sbFeature, featureVec, featureMap);

        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(nextChar).Append('3');
        addFeature(sbFeature, featureVec, featureMap);

//        sbFeature.delete(0, sbFeature.length());
//        sbFeature.Append("U[2,0]=").Append(next2Char);
//        addFeature(sbFeature, featureVec, featureMap);

        //char bigram feature
        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(pre2Char).Append("/").Append(preChar).Append('4');
        addFeature(sbFeature, featureVec, featureMap);

        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(preChar).Append("/").Append(curChar).Append('5');
        addFeature(sbFeature, featureVec, featureMap);

        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(curChar).Append("/").Append(nextChar).Append('6');
        addFeature(sbFeature, featureVec, featureMap);

        sbFeature.delete(0, sbFeature.length());
        sbFeature.Append(nextChar).Append("/").Append(next2Char).Append('7');
        addFeature(sbFeature, featureVec, featureMap);

//        sbFeature.delete(0, sbFeature.length());
//        sbFeature.Append("B[-2,0]=").Append(pre2Char).Append("/").Append(curChar);
//        addFeature(sbFeature, featureVec, featureMap);
//
//        sbFeature.delete(0, sbFeature.length());
//        sbFeature.Append("B[-1,1]=").Append(preChar).Append("/").Append(nextChar);
//        addFeature(sbFeature, featureVec, featureMap);
//
//        sbFeature.delete(0, sbFeature.length());
//        sbFeature.Append("B[0,2]=").Append(curChar).Append("/").Append(next2Char);
//        addFeature(sbFeature, featureVec, featureMap);

        //char trigram feature
//        sbFeature.delete(0, sbFeature.length());
//        sbFeature.Append("T[-1,0]=").Append(preChar).Append("/").Append(curChar).Append("/").Append(nextChar);
//        addFeature(sbFeature, featureVec, featureMap);
        sbFeature = null;

//        if (preChar == curChar)
//            addFeature("-1AABBT", featureVec, featureMap);
//        if (curChar == nextChar)
//            addFeature("0AABBT", featureVec, featureMap);
//
//        if (pre2Char == curChar)
//            addFeature("-2ABABT", featureVec, featureMap);
//        if (preChar == nextChar)
//            addFeature("-1ABABT", featureVec, featureMap);
//        if (curChar == next2Char)
//            addFeature("0ABABT", featureVec, featureMap);

        //char type unigram feature
//        addFeature("cT=" + CharType.get(sentence.charAt(position)), featureVec, featureMap);
//
//        //char type trigram feature
//        StringBuffer trigram = new StringBuffer();
//
//        if (position > 0)
//            trigram.Append(CharType.get(sentence.charAt(position - 1)));
//        else
//            trigram.Append("_BT_");
//
//        trigram.Append("/" + CharType.get(sentence.charAt(position)));
//
//        if (position < sentence.length() - 1)
//            trigram.Append("/" + CharType.get(sentence.charAt(position + 1)));
//        else
//            trigram.Append("/_EL_");
//
//        addFeature("cTT=" + trigram, featureVec, featureMap);

        //dictionary feature
//        int[] begin = new int[sentence.length()];
//        int[] middle = new int[sentence.length()];
//        int[] end = new int[sentence.length()];
//        // 查词典
//        for (int i = 0; i < sentence.length(); i++)
//        {
//            int maxPre = 0;
//            int offset = -1;
//            int state = 1;
//            while (state > 0 && i + ++offset < sentence.length())
//            {
//                state = dat.transition(sentence.charAt(i + offset), state);
//                if (dat.output(state) != null)
//                {
//                    maxPre = offset + 1;
//                }
//            }
//
//            begin[i] = maxPre;
//
//            if (maxPre > 0 && end[i + maxPre - 1] < maxPre)
//                end[i + maxPre - 1] = maxPre;
//            for (int k = i + 1; k < i + maxPre - 1; k++)
//                if (middle[k] < maxPre)
//                    middle[k] = maxPre;
//        }
//        addFeature("b=" + begin[position], featureVec, featureMap);
//        addFeature("m=" + middle[position], featureVec, featureMap);
//        addFeature("e=" + end[position], featureVec, featureMap);

        //label bigram feature
//        char preLabel = position > 0 ? tagArray[position - 1].toChar() : CHAR_BEGIN;
//
//        addFeature("BL=" + preLabel, featureVec, featureMap);    // 虽然有preLabel，但并没有加上当前label，当前label是由调用者自行加的

        return toFeatureArray(featureVec);
    }

    protected void initFeatureMatrix(String sentence, FeatureMap featureMap)
    {
        featureMatrix = new int[sentence.length()][];
        for (int i = 0; i < sentence.length(); i++)
        {
            featureMatrix[i] = extractFeature(sentence, featureMap, i);
        }
    }

    public static CWSInstance create(Sentence sentence, FeatureMap featureMap)
    {
        if (sentence == null || featureMap == null)
        {
            return null;
        }
        List<Word> wordList = sentence.toSimpleWordList();
        String[] termArray = new String[wordList.size()];
        int i = 0;
        for (Word word : wordList)
        {
            termArray[i] = word.getValue();
            ++i;
        }
        return new CWSInstance(termArray, featureMap);
    }
}
