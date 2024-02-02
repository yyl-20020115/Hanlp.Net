/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/17 19:34</create-date>
 *
 * <copyright file="PlaceRecognition.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.liNTunsoft.com/
 * This source is subject to the LiNTunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.corpus.dictionary.item;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.nt;
using com.hankcs.hanlp.seg.common;
using System.Text;

namespace com.hankcs.hanlp.recognition.nt;




/**
 * 地址识别
 *
 * @author hankcs
 */
public class OrganizationRecognition
{
    public static bool recognition(List<Vertex> pWordSegResult, WordNet wordNetOptimum, WordNet wordNetAll)
    {
        List<EnumItem<NT>> roleTagList = roleTag(pWordSegResult, wordNetAll);
        if (HanLP.Config.DEBUG)
        {
            StringBuilder sbLog = new StringBuilder();
            var iterator = pWordSegResult.GetEnumerator();
            foreach (EnumItem<NT> NTEnumItem in roleTagList)
            {
                sbLog.Append('[');
                sbLog.Append(iterator.next().realWord);
                sbLog.Append(' ');
                sbLog.Append(NTEnumItem);
                sbLog.Append(']');
            }
            Console.WriteLine("机构名角色观察：%s\n", sbLog.ToString());
        }
        List<NT> NTList = viterbiCompute(roleTagList);
        if (HanLP.Config.DEBUG)
        {
            StringBuilder sbLog = new StringBuilder();
            var iterator = pWordSegResult.GetEnumerator();
            sbLog.Append('[');
            foreach (NT NT in NTList)
            {
                sbLog.Append(iterator.next().realWord);
                sbLog.Append('/');
                sbLog.Append(NT);
                sbLog.Append(" ,");
            }
            if (sbLog.Length > 1) sbLog.delete(sbLog.Length - 2, sbLog.Length);
            sbLog.Append(']');
            Console.WriteLine("机构名角色标注：%s\n", sbLog.ToString());
        }

        OrganizationDictionary.parsePattern(NTList, pWordSegResult, wordNetOptimum, wordNetAll);
        return true;
    }

    public static List<EnumItem<NT>> roleTag(List<Vertex> vertexList, WordNet wordNetAll)
    {
        List<EnumItem<NT>> tagList = new ();
        //        int line = 0;
        foreach (Vertex vertex in vertexList)
        {
            // 构成更长的
            Nature nature = vertex.guessNature();
            if (nature == nrf)
            {
                if (vertex.getAttribute().totalFrequency <= 1000)
                {
                    tagList.Add(new EnumItem<NT>(NT.F, 1000));
                    continue;
                }
            }
            else if (nature == ni || nature == nic || nature == nis || nature == nit)
            {
                EnumItem<NT> ntEnumItem = new EnumItem<NT>(NT.K, 1000);
                ntEnumItem.addLabel(NT.D, 1000);
                tagList.Add(ntEnumItem);
                continue;
            }
            else if (nature == m)
            {
                EnumItem<NT> ntEnumItem = new EnumItem<NT>(NT.M, 1000);
                tagList.Add(ntEnumItem);
                continue;
            }

            EnumItem<NT> NTEnumItem = OrganizationDictionary.dictionary.get(vertex.word);  // 此处用等效词，更加精准
            if (NTEnumItem == null)
            {
                NTEnumItem = new EnumItem<NT>(NT.Z, OrganizationDictionary.transformMatrixDictionary.getTotalFrequency(NT.Z));
            }
            tagList.Add(NTEnumItem);
//            line += vertex.realWord.Length;
        }
        return tagList;
    }

    /**
     * 维特比算法求解最优标签
     *
     * @param roleTagList
     * @return
     */
    public static List<NT> viterbiCompute(List<EnumItem<NT>> roleTagList)
    {
        return Viterbi.ComputeEnum(roleTagList, OrganizationDictionary.transformMatrixDictionary);
    }
}
