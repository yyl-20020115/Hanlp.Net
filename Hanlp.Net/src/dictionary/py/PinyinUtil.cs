/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/2 11:19</create-date>
 *
 * <copyright file="PinyinUtil.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.dictionary.py;


/**
 * @author hankcs
 */
public class PinyinUtil
{
    /**
     * Convert tone numbers to tone marks using Unicode <br/><br/>
     * <p/>
     * <b>Algorithm for determining location of tone mark</b><br/>
     * <p/>
     * A simple algorithm for determining the vowel on which the tone mark
     * appears is as follows:<br/>
     * <p/>
     * <ol>
     * <li>First, look for an "a" or an "e". If either vowel appears, it takes
     * the tone mark. There are no possible pinyin syllables that contain both
     * an "a" and an "e".
     * <p/>
     * <li>If there is no "a" or "e", look for an "ou". If "ou" appears, then
     * the "o" takes the tone mark.
     * <p/>
     * <li>If none of the above cases hold, then the last vowel in the syllable
     * takes the tone mark.
     * <p/>
     * </ol>
     *
     * @param pinyinStr the ascii represention with tone numbers
     * @return the unicode represention with tone marks
     */
    public static string convertToneNumber2ToneMark(string pinyinStr)
    {
        string lowerCasePinyinStr = pinyinStr.ToLower();

        if (lowerCasePinyinStr.matches("[a-z]*[1-5]?"))
        {
            char defautlCharValue = '$';
            int defautlIndexValue = -1;

            char unmarkedVowel = defautlCharValue;
            int indexOfUnmarkedVowel = defautlIndexValue;

            char charA = 'a';
            char charE = 'e';
            string ouStr = "ou";
            string allUnmarkedVowelStr = "aeiouv";
            string allMarkedVowelStr = "āáǎàaēéěèeīíǐìiōóǒòoūúǔùuǖǘǚǜü";

            if (lowerCasePinyinStr.matches("[a-z]*[1-5]"))
            {

                int tuneNumber =
                        char.getNumericValue(lowerCasePinyinStr.charAt(lowerCasePinyinStr.Length - 1));

                int indexOfA = lowerCasePinyinStr.IndexOf(charA);
                int indexOfE = lowerCasePinyinStr.IndexOf(charE);
                int ouIndex = lowerCasePinyinStr.IndexOf(ouStr);

                if (-1 != indexOfA)
                {
                    indexOfUnmarkedVowel = indexOfA;
                    unmarkedVowel = charA;
                }
                else if (-1 != indexOfE)
                {
                    indexOfUnmarkedVowel = indexOfE;
                    unmarkedVowel = charE;
                }
                else if (-1 != ouIndex)
                {
                    indexOfUnmarkedVowel = ouIndex;
                    unmarkedVowel = ouStr[0];
                }
                else
                {
                    for (int i = lowerCasePinyinStr.Length - 1; i >= 0; i--)
                    {
                        if (string.valueOf(lowerCasePinyinStr[i]).matches(
                                "[" + allUnmarkedVowelStr + "]"))
                        {
                            indexOfUnmarkedVowel = i;
                            unmarkedVowel = lowerCasePinyinStr[i];
                            break;
                        }
                    }
                }

                if ((defautlCharValue != unmarkedVowel) && (defautlIndexValue != indexOfUnmarkedVowel))
                {
                    int rowIndex = allUnmarkedVowelStr.IndexOf(unmarkedVowel);
                    int columnIndex = tuneNumber - 1;

                    int vowelLocation = rowIndex * 5 + columnIndex;

                    char markedVowel = allMarkedVowelStr.charAt(vowelLocation);

                    StringBuilder resultBuffer = new StringBuilder();

                    resultBuffer.Append(lowerCasePinyinStr.substring(0, indexOfUnmarkedVowel).replaceAll("v",
                                                                                                         "ü"));
                    resultBuffer.Append(markedVowel);
                    resultBuffer.Append(lowerCasePinyinStr.substring(indexOfUnmarkedVowel + 1,
                                                                     lowerCasePinyinStr.Length - 1).replaceAll("v", "ü"));

                    return resultBuffer.ToString();

                }
                else
                // error happens in the procedure of locating vowel
                {
                    return lowerCasePinyinStr;
                }
            }
            else
            // input string has no any tune number
            {
                // only replace v with ü (umlat) character
                return lowerCasePinyinStr.Replace("v", "ü");
            }
        }
        else
        // bad Format
        {
            return lowerCasePinyinStr;
        }
    }

    /**
     * 将列表转为数组
     * @param pinyinList
     * @return
     */
    public static Pinyin[] convertList2Array(List<Pinyin> pinyinList)
    {
        return pinyinList.ToArray();
    }

    public static Pinyin removeTone(Pinyin p)
    {
        return Pinyin.none5;
    }

    /**
     * 转换List<Pinyin> pinyinList到List<string>，其中的string为带声调符号形式
     * @param pinyinList
     * @return
     */
    public static List<string> convertPinyinList2TonePinyinList(List<Pinyin> pinyinList)
    {
        List<string> tonePinyinList = new (pinyinList.Count);
        foreach (Pinyin pinyin in pinyinList)
        {
            tonePinyinList.Add(pinyin.PinyinWithToneMark);
        }

        return tonePinyinList;
    }
}
