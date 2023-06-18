/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/05/2014/5/26 13:52</create-date>
 *
 * <copyright file="UnknowWord.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.recognition.nr;




/**
 * 人名识别
 * @author hankcs
 */
public class PersonRecognition
{
    public static bool recognition(List<Vertex> pWordSegResult, WordNet wordNetOptimum, WordNet wordNetAll)
    {
        List<EnumItem<NR>> roleTagList = roleObserve(pWordSegResult);
        if (HanLP.Config.DEBUG)
        {
            StringBuilder sbLog = new StringBuilder();
            Iterator<Vertex> iterator = pWordSegResult.iterator();
            for (EnumItem<NR> nrEnumItem : roleTagList)
            {
                sbLog.Append('[');
                sbLog.Append(iterator.next().realWord);
                sbLog.Append(' ');
                sbLog.Append(nrEnumItem);
                sbLog.Append(']');
            }
            System._out.printf("人名角色观察：%s\n", sbLog.ToString());
        }
        List<NR> nrList = viterbiComputeSimply(roleTagList);
        if (HanLP.Config.DEBUG)
        {
            StringBuilder sbLog = new StringBuilder();
            Iterator<Vertex> iterator = pWordSegResult.iterator();
            sbLog.Append('[');
            for (NR nr : nrList)
            {
                sbLog.Append(iterator.next().realWord);
                sbLog.Append('/');
                sbLog.Append(nr);
                sbLog.Append(" ,");
            }
            if (sbLog.Length > 1) sbLog.delete(sbLog.Length - 2, sbLog.Length);
            sbLog.Append(']');
            System._out.printf("人名角色标注：%s\n", sbLog.ToString());
        }

        PersonDictionary.parsePattern(nrList, pWordSegResult, wordNetOptimum, wordNetAll);
        return true;
    }

    /**
     * 角色观察(从模型中加载所有词语对应的所有角色,允许进行一些规则补充)
     * @param wordSegResult 粗分结果
     * @return
     */
    public static List<EnumItem<NR>> roleObserve(List<Vertex> wordSegResult)
    {
        List<EnumItem<NR>> tagList = new LinkedList<EnumItem<NR>>();
        Iterator<Vertex> iterator = wordSegResult.iterator();
        iterator.next();
        tagList.Add(new EnumItem<NR>(NR.A, NR.K)); //  始##始 A K
        while (iterator.hasNext())
        {
            Vertex vertex = iterator.next();
            EnumItem<NR> nrEnumItem = PersonDictionary.dictionary.get(vertex.realWord);
            if (nrEnumItem == null)
            {
                Nature nature = vertex.guessNature();
                if (nature == nr)
                {
                    // 有些双名实际上可以构成更长的三名
                    if (vertex.getAttribute().totalFrequency <= 1000 && vertex.realWord.Length == 2)
                    {
                        nrEnumItem = new EnumItem<NR>();
                        nrEnumItem.labelMap.put(NR.X, 2); // 认为是三字人名前2个字=双字人名的可能性更高
                        nrEnumItem.labelMap.put(NR.G, 1);
                    }
                    else
                        nrEnumItem = new EnumItem<NR>(NR.A, PersonDictionary.transformMatrixDictionary.getTotalFrequency(NR.A));
                }
                else if (nature == nnt)
                {
                    // 姓+职位
                    nrEnumItem = new EnumItem<NR>(NR.G, NR.K);
                }
                else
                {
                    nrEnumItem = new EnumItem<NR>(NR.A, PersonDictionary.transformMatrixDictionary.getTotalFrequency(NR.A));
                }
            }
            tagList.Add(nrEnumItem);
        }
        return tagList;
    }

    /**
     * 维特比算法求解最优标签
     * @param roleTagList
     * @return
     */
    public static List<NR> viterbiCompute(List<EnumItem<NR>> roleTagList)
    {
        return Viterbi.computeEnum(roleTagList, PersonDictionary.transformMatrixDictionary);
    }

    /**
     * 简化的"维特比算法"求解最优标签
     * @param roleTagList
     * @return
     */
    public static List<NR> viterbiComputeSimply(List<EnumItem<NR>> roleTagList)
    {
        return Viterbi.computeEnumSimply(roleTagList, PersonDictionary.transformMatrixDictionary);
    }
}
