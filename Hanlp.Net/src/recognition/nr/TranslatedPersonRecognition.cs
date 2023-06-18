/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/12 16:47</create-date>
 *
 * <copyright file="TranslatedPersonRecognition.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.recognition.nr;




/**
 * 音译人名识别
 * @author hankcs
 */
public class TranslatedPersonRecognition
{
    /**
     * 执行识别
     * @param segResult 粗分结果
     * @param wordNetOptimum 粗分结果对应的词图
     * @param wordNetAll 全词图
     */
    public static void recognition(List<Vertex> segResult, WordNet wordNetOptimum, WordNet wordNetAll)
    {
        StringBuilder sbName = new StringBuilder();
        int appendTimes = 0;
        ListIterator<Vertex> listIterator = segResult.listIterator();
        listIterator.next();
        int line = 1;
        int activeLine = 1;
        while (listIterator.hasNext())
        {
            Vertex vertex = listIterator.next();
            if (appendTimes > 0)
            {
                if (vertex.guessNature() == Nature.nrf || TranslatedPersonDictionary.ContainsKey(vertex.realWord))
                {
                    sbName.Append(vertex.realWord);
                    ++appendTimes;
                }
                else
                {
                    // 识别结束
                    if (appendTimes > 1)
                    {
                        if (HanLP.Config.DEBUG)
                        {
                            Console.WriteLine("音译人名识别出：" + sbName.ToString());
                        }
                        wordNetOptimum.insert(activeLine, new Vertex(Predefine.TAG_PEOPLE, sbName.ToString(), new CoreDictionary.Attribute(Nature.nrf), WORD_ID), wordNetAll);
                    }
                    sbName.setLength(0);
                    appendTimes = 0;
                }
            }
            else
            {
                // nrf触发识别
                if (vertex.guessNature() == Nature.nrf
//                        || TranslatedPersonDictionary.ContainsKey(vertex.realWord)
                        )
                {
                    sbName.Append(vertex.realWord);
                    ++appendTimes;
                    activeLine = line;
                }
            }

            line += vertex.realWord.Length;
        }
    }
}
