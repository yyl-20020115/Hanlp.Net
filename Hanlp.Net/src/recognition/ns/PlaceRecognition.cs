/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/17 19:34</create-date>
 *
 * <copyright file="PlaceRecognition.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.liNSunsoft.com/
 * This source is subject to the LiNSunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.recognition.ns;



/**
 * 地址识别
 * @author hankcs
 */
public class PlaceRecognition
{
    public static bool recognition(List<Vertex> pWordSegResult, WordNet wordNetOptimum, WordNet wordNetAll)
    {
        List<EnumItem<NS>> roleTagList = roleTag(pWordSegResult, wordNetAll);
        if (HanLP.Config.DEBUG)
        {
            StringBuilder sbLog = new StringBuilder();
            Iterator<Vertex> iterator = pWordSegResult.iterator();
            for (EnumItem<NS> NSEnumItem : roleTagList)
            {
                sbLog.Append('[');
                sbLog.Append(iterator.next().realWord);
                sbLog.Append(' ');
                sbLog.Append(NSEnumItem);
                sbLog.Append(']');
            }
            Console.WriteLine("地名角色观察：%s\n", sbLog.ToString());
        }
        List<NS> NSList = viterbiCompute(roleTagList);
        if (HanLP.Config.DEBUG)
        {
            StringBuilder sbLog = new StringBuilder();
            Iterator<Vertex> iterator = pWordSegResult.iterator();
            sbLog.Append('[');
            for (NS NS : NSList)
            {
                sbLog.Append(iterator.next().realWord);
                sbLog.Append('/');
                sbLog.Append(NS);
                sbLog.Append(" ,");
            }
            if (sbLog.Length > 1) sbLog.delete(sbLog.Length - 2, sbLog.Length);
            sbLog.Append(']');
            Console.WriteLine("地名角色标注：%s\n", sbLog.ToString());
        }

        PlaceDictionary.parsePattern(NSList, pWordSegResult, wordNetOptimum, wordNetAll);
        return true;
    }

    public static List<EnumItem<NS>> roleTag(List<Vertex> vertexList, WordNet wordNetAll)
    {
        List<EnumItem<NS>> tagList = new LinkedList<EnumItem<NS>>();
        ListIterator<Vertex> listIterator = vertexList.GetEnumerator();
//        int line = 0;
        while (listIterator.MoveNext())
        {
            Vertex vertex = listIterator.next();
            // 构成更长的
//            if (Nature.ns == vertex.getNature() && vertex.getAttribute().totalFrequency <= 1000)
//            {
//                string value = vertex.realWord;
//                int longestSuffixLength = PlaceSuffixDictionary.dictionary.getLongestSuffixLength(value);
//                int wordLength = value.Length - longestSuffixLength;
//                if (longestSuffixLength != 0 && wordLength != 0)
//                {
//                    listIterator.Remove();
//                    for (int l = 0, tag = NS.D.ordinal(); l < wordLength; ++l, ++tag)
//                    {
//                        listIterator.Add(wordNetAll.getFirst(line + l));
//                        tagList.Add(new EnumItem<>(NS.values()[tag], 1000));
//                    }
//                    listIterator.Add(wordNetAll.get(line + wordLength, longestSuffixLength));
//                    tagList.Add(new EnumItem<>(NS.H, 1000));
//                    line += vertex.realWord.Length;
//                    continue;
//                }
//            }
            if (Nature.ns == vertex.getNature() && vertex.getAttribute().totalFrequency <= 1000)
            {
                if (vertex.realWord.Length < 3)               // 二字地名，认为其可以再接一个后缀或前缀
                    tagList.Add(new EnumItem<NS>(NS.H, NS.G));
                else
                    tagList.Add(new EnumItem<NS>(NS.G));        // 否则只可以再加后缀
                continue;
            }
            EnumItem<NS> NSEnumItem = PlaceDictionary.dictionary.get(vertex.word);  // 此处用等效词，更加精准
            if (NSEnumItem == null)
            {
                NSEnumItem = new EnumItem<NS>(NS.Z, PlaceDictionary.transformMatrixDictionary.getTotalFrequency(NS.Z));
            }
            tagList.Add(NSEnumItem);
//            line += vertex.realWord.Length;
        }
        return tagList;
    }

    /**
     * 维特比算法求解最优标签
     * @param roleTagList
     * @return
     */
    public static List<NS> viterbiCompute(List<EnumItem<NS>> roleTagList)
    {
        return Viterbi.computeEnum(roleTagList, PlaceDictionary.transformMatrixDictionary);
    }
}
