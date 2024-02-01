/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 21:15</create-date>
 *
 * <copyright file="Util.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;

namespace com.hankcs.hanlp.corpus.util;




/**
 * @author hankcs
 */
public class CorpusUtil
{
    public const string TAG_PLACE = "未##地";
    public const string TAG_BIGIN = "始##始";
    public const string TAG_OTHER = "未##它";
    public const string TAG_GROUP = "未##团";
    public const string TAG_NUMBER = "未##数";
    public const string TAG_PROPER = "未##专";
    public const string TAG_TIME = "未##时";
    public const string TAG_CLUSTER = "未##串";
    public const string TAG_END = "末##末";
    public const string TAG_PEOPLE = "未##人";

    /**
     * 编译单词
     *
     * @param word
     * @return
     */
    public static IWord compile(IWord word)
    {
        string label = word.Label;
        if ("nr".Equals(label)) return new Word(word.Value, TAG_PEOPLE);
        else if ("m".Equals(label) || "mq".Equals(label)) return new Word(word.Value, TAG_NUMBER);
        else if ("t".Equals(label)) return new Word(word.Value, TAG_TIME);
        else if ("ns".Equals(label)) return new Word(word.Value, TAG_PLACE);
//        switch (word.getLabel())
//        {
//            case "nr":
//                return new Word(word.Value, TAG_PEOPLE);
//            case "m":
//            case "mq":
//                return new Word(word.Value, TAG_NUMBER);
//            case "t":
//                return new Word(word.Value, TAG_TIME);
//            case "ns":
//                return new Word(word.Value, TAG_TIME);
//        }

        return word;
    }

    /**
     * 将word列表转为兼容的IWord列表
     *
     * @param simpleSentenceList
     * @return
     */
    public static List<List<IWord>> convert2CompatibleList(List<List<Word>> simpleSentenceList)
    {
        List<List<IWord>> compatibleList = new ();
        foreach (List<Word> wordList in simpleSentenceList)
        {
            compatibleList.Add(new (wordList));
        }
        return compatibleList;
    }

    public static List<IWord> spilt(List<IWord> wordList)
    {
        var listIterator = wordList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            IWord word = listIterator.next();
            if (word is CompoundWord)
            {
                listIterator.Remove();
                foreach (Word inner in ((CompoundWord) word).innerList)
                {
                    listIterator.Add(inner);
                }
            }
        }
        return wordList;
    }
}
