/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/15 19:39</create-date>
 *
 * <copyright file="CoreStopwordDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.stopword;




/**
 * 核心停用词词典
 * @author hankcs
 */
public class CoreStopWordDictionary
{
    static StopWordDictionary dictionary;
    static CoreStopWordDictionary()
    {
        ByteArray byteArray = ByteArray.createByteArray(HanLP.Config.CoreStopWordDictionaryPath + Predefine.BIN_EXT);
        if (byteArray == null)
        {
            try
            {
                dictionary = new StopWordDictionary(HanLP.Config.CoreStopWordDictionaryPath);
                Stream _out = new Stream(new BufferedOutputStream(IOUtil.newOutputStream(HanLP.Config.CoreStopWordDictionaryPath + Predefine.BIN_EXT)));
                dictionary.save(_out);
                _out.Close();
            }
            catch (Exception e)
            {
                logger.severe("载入停用词词典" + HanLP.Config.CoreStopWordDictionaryPath + "失败"  + TextUtility.exceptionToString(e));
                throw new RuntimeException("载入停用词词典" + HanLP.Config.CoreStopWordDictionaryPath + "失败");
            }
        }
        else
        {
            dictionary = new StopWordDictionary();
            dictionary.load(byteArray);
        }
    }

    public static bool Contains(string key)
    {
        return dictionary.Contains(key);
    }

    /**
     * 核心停用词典的核心过滤器，词性属于名词、动词、副词、形容词，并且不在停用词表中才不会被过滤
     */
    public static Filter FILTER = new DF();
    public class DF:Filter
    {
        //@Override
        public bool shouldInclude(Term term)
        {
            // 除掉停用词
            string nature = term.nature != null ? term.nature.ToString() : "空";
            char firstChar = nature[0];
            switch (firstChar)
            {
                case 'm':
                case 'b':
                case 'c':
                case 'e':
                case 'o':
                case 'p':
                case 'q':
                case 'u':
                case 'y':
                case 'z':
                case 'r':
                case 'w':
                {
                    return false;
                }
                default:
                {
                    if (!CoreStopWordDictionary.Contains(term.word))
                    {
                        return true;
                    }
                }
                break;
            }

            return false;
        }
    };

    /**
     * 是否应当将这个term纳入计算
     *
     * @param term
     * @return 是否应当
     */
    public static bool shouldInclude(Term term)
    {
        return FILTER.shouldInclude(term);
    }

    /**
     * 是否应当去掉这个词
     * @param term 词
     * @return 是否应当去掉
     */
    public static bool shouldRemove(Term term)
    {
        return !shouldInclude(term);
    }

    /**
     * 加入停用词到停用词词典中
     * @param stopWord 停用词
     * @return 词典是否发生了改变
     */
    public static bool Add(string stopWord)
    {
        return dictionary.Add(stopWord);
    }

    /**
     * 从停用词词典中删除停用词
     * @param stopWord 停用词
     * @return 词典是否发生了改变
     */
    public static bool Remove(string stopWord)
    {
        return dictionary.Remove(stopWord);
    }

    /**
     * 对分词结果应用过滤
     * @param termList
     */
    public static void apply(List<Term> termList)
    {
        IEnumerator<Term> listIterator = termList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            if (shouldRemove(listIterator.next())) listIterator.Remove();
        }
    }
}
