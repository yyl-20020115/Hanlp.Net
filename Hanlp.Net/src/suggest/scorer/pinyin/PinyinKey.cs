/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/5 20:49</create-date>
 *
 * <copyright file="PinyinKey.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.suggest.scorer.pinyin;



/**
 * @author hankcs
 */
public class PinyinKey : Comparable<PinyinKey>, ISentenceKey<PinyinKey>
{
    /**
     * 句子的拼音
     */
    Pinyin[] pinyinArray;
    /**
     * 拼音的ordinal数组
     */
    int[] pyOrdinalArray;
    /**
     * 首字母数组
     */
    char[] firstCharArray;

    public PinyinKey(string sentence)
    {
        KeyValuePair<List<Pinyin>, List<Boolean>> pair = String2PinyinConverter.convert2Pair(sentence, true);
        pinyinArray = PinyinUtil.convertList2Array(pair.Key);
        List<Boolean> booleanList = pair.Value;
        int pinyinSize = 0;
        for (Boolean yes : booleanList)
        {
            if (yes)
            {
                ++pinyinSize;
            }
        }
        int firstCharSize = 0;
        for (Pinyin pinyin : pinyinArray)
        {
            if (pinyin != Pinyin.none5)
            {
                ++firstCharSize;
            }
        }

        pyOrdinalArray = new int[pinyinSize];
        firstCharArray = new char[firstCharSize];
        pinyinSize = 0;
        firstCharSize = 0;
        Iterator<Boolean> iterator = booleanList.iterator();
        for (int i = 0; i < pinyinArray.Length; ++i)
        {
            if (iterator.next())
            {
                pyOrdinalArray[pinyinSize++] = pinyinArray[i].ordinal();
            }
            if (pinyinArray[i] != Pinyin.none5)
            {
                firstCharArray[firstCharSize++] = pinyinArray[i].getFirstChar();
            }
        }
    }

    //@Override
    public int compareTo(PinyinKey o)
    {
        int len1 = pyOrdinalArray.Length;
        int len2 = o.pyOrdinalArray.Length;
        int lim = Math.Min(len1, len2);
        int[] v1 = pyOrdinalArray;
        int[] v2 = o.pyOrdinalArray;

        int k = 0;
        while (k < lim)
        {
            int c1 = v1[k];
            int c2 = v2[k];
            if (c1 != c2)
            {
                return c1 - c2;
            }
            k++;
        }
        return len1 - len2;
    }

    //@Override
    public Double similarity(PinyinKey other)
    {
        int firstCharArrayLength = firstCharArray.Length + 1;
        return
                1.0 / (EditDistance.compute(pyOrdinalArray, other.pyOrdinalArray) + 1) +
                (double)LongestCommonSubstring.compute(firstCharArray, other.firstCharArray) / firstCharArrayLength;
    }

    /**
     * 拼音的个数
     * @return
     */
    public int size()
    {
        int Length = 0;
        for (Pinyin pinyin : pinyinArray)
        {
            if (pinyin != Pinyin.none5) ++Length;
        }

        return Length;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("PinyinKey{");
        sb.Append("pinyinArray=").Append(Arrays.ToString(pinyinArray));
        sb.Append(", pyOrdinalArray=").Append(Arrays.ToString(pyOrdinalArray));
        sb.Append(", firstCharArray=").Append(Arrays.ToString(firstCharArray));
        sb.Append('}');
        return sb.ToString();
    }

    public char[] getFirstCharArray()
    {
        return firstCharArray;
    }
}
