/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/13 13:12</create-date>
 *
 * <copyright file="CoreSynonymDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dictionary.common;
using com.hankcs.hanlp.dictionary.stopword;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary;


/**
 * 核心同义词词典(使用语义id作为value）
 *
 * @author hankcs
 */
public class CoreSynonymDictionaryEx
{
    static CommonSynonymDictionaryEx dictionary;

    static CoreSynonymDictionaryEx()
    {
        try
        {
            dictionary = CommonSynonymDictionaryEx.create(IOUtil.newInputStream(HanLP.Config.CoreSynonymDictionaryDictionaryPath));
        }
        catch (Exception e)
        {
            logger.severe("载入核心同义词词典失败");
            throw new ArgumentException(e);
        }
    }

    public static long[] get(string key)
    {
        return dictionary.get(key);
    }

    /**
     * 语义距离
     * @param itemA
     * @param itemB
     * @return
     */
    public static long distance(CommonSynonymDictionary.SynonymItem itemA, CommonSynonymDictionary.SynonymItem itemB)
    {
        return itemA.distance(itemB);
    }

    /**
     * 将分词结果转换为同义词列表
     * @param sentence 句子
     * @param withUndefinedItem 是否保留词典中没有的词语
     * @return
     */
    public static List<long[]> convert(List<Term> sentence, bool withUndefinedItem)
    {
        List<long[]> synonymItemList = new (sentence.Count);
        foreach (Term term in sentence)
        {
            // 除掉停用词
            if (term.nature == null) continue;
            string nature = term.nature.ToString();
            char firstChar = nature[0];
            switch (firstChar)
            {
                case 'm':
                {
                    if (!TextUtility.isAllChinese(term.word)) continue;
                }break;
                case 'w':
                {
                    continue;
                }
            }
            // 停用词
            if (CoreStopWordDictionary.Contains(term.word)) continue;
            long[] item = get(term.word);
//            logger.trace("{} {}", wordResult.word, Arrays.ToString(item));
            if (item == null)
            {
                if (withUndefinedItem)
                {
                    item = new long[]{long.MaxValue / 3};
                    synonymItemList.Add(item);
                }

            }
            else
            {
                synonymItemList.Add(item);
            }
        }

        return synonymItemList;
    }

    /**
     * 获取语义标签
     * @return
     */
    public static long[] getLexemeArray(List<CommonSynonymDictionary.SynonymItem> synonymItemList)
    {
        long[] array = new long[synonymItemList.Count];
        int i = 0;
        foreach (CommonSynonymDictionary.SynonymItem item in synonymItemList)
        {
            array[i++] = item.entry.id;
        }
        return array;
    }


    public long distance(List<CommonSynonymDictionary.SynonymItem> synonymItemListA, List<CommonSynonymDictionary.SynonymItem> synonymItemListB)
    {
        return EditDistance.compute(synonymItemListA, synonymItemListB);
    }

    public long distance(long[] arrayA, long[] arrayB)
    {
        return EditDistance.compute(arrayA, arrayB);
    }
}
