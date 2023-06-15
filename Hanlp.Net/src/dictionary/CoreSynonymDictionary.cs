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
using com.hankcs.hanlp.dictionary.common;

namespace com.hankcs.hanlp.dictionary;


/**
 * 核心同义词词典
 *
 * @author hankcs
 */
public class CoreSynonymDictionary
{
    static CommonSynonymDictionary dictionary;

    static CoreSynonymDictionary()
    {
        try
        {
            long start = DateTime.Now.Microsecond;
            dictionary = CommonSynonymDictionary.create(IOUtil.newInputStream(HanLP.Config.CoreSynonymDictionaryDictionaryPath));
            logger.info("载入核心同义词词典成功，耗时 " + (DateTime.Now.Microsecond - start) + " ms");
        }
        catch (Exception e)
        {
            throw new IllegalArgumentException("载入核心同义词词典失败" + e);
        }
    }

    /**
     * 获取一个词的同义词（意义完全相同的，即{@link com.hankcs.hanlp.dictionary.common.CommonSynonymDictionary.SynonymItem#type}
     * == {@link com.hankcs.hanlp.corpus.synonym.Synonym.Type#EQUAL}的）列表
     * @param key
     * @return
     */
    public static CommonSynonymDictionary.SynonymItem get(string key)
    {
        return dictionary.get(key);
    }

    /**
     * 不分词直接转换
     * @param text
     * @return
     */
    public static string rewriteQuickly(string text)
    {
        return dictionary.rewriteQuickly(text);
    }

    public static string rewrite(string text)
    {
        return dictionary.rewrite(text);
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
     * 判断两个单词之间的语义距离
     * @param A
     * @param B
     * @return
     */
    public static long distance(string A, string B)
    {
        CommonSynonymDictionary.SynonymItem itemA = get(A);
        CommonSynonymDictionary.SynonymItem itemB = get(B);
        if (itemA == null || itemB == null) return long.MAX_VALUE;

        return distance(itemA, itemB);
    }

    /**
     * 计算两个单词之间的相似度，0表示不相似，1表示完全相似
     * @param A
     * @param B
     * @return
     */
    public static double similarity(string A, string B)
    {
        long distance = distance(A, B);
        if (distance > dictionary.getMaxSynonymItemIdDistance()) return 0.0;

        return (dictionary.getMaxSynonymItemIdDistance() - distance) / (double) dictionary.getMaxSynonymItemIdDistance();
    }

    /**
     * 将分词结果转换为同义词列表
     * @param sentence 句子
     * @param withUndefinedItem 是否保留词典中没有的词语
     * @return
     */
    public static List<CommonSynonymDictionary.SynonymItem> convert(List<Term> sentence, bool withUndefinedItem)
    {
        List<CommonSynonymDictionary.SynonymItem> synonymItemList = new ArrayList<CommonSynonymDictionary.SynonymItem>(sentence.size());
        for (Term term : sentence)
        {
            CommonSynonymDictionary.SynonymItem item = get(term.word);
            if (item == null)
            {
                if (withUndefinedItem)
                {
                    item = CommonSynonymDictionary.SynonymItem.createUndefined(term.word);
                    synonymItemList.add(item);
                }

            }
            else
            {
                synonymItemList.add(item);
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
        long[] array = new long[synonymItemList.size()];
        int i = 0;
        for (CommonSynonymDictionary.SynonymItem item : synonymItemList)
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
