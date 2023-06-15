/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-24 下午6:46</create-date>
 *
 * <copyright file="PartOfSpeechTagDictionary.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.dictionary.other;



/**
 * 词性标注集中英映射表
 *
 * @author hankcs
 */
public class PartOfSpeechTagDictionary
{
    /**
     * 词性映射表
     */
    public static Dictionary<string, string> translator = new ();

    static PartOfSpeechTagDictionary()
    {
        load(HanLP.Config.PartOfSpeechTagDictionary);
    }

    public static void load(string path)
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator(path);
        iterator.next(); // header
        while (iterator.hasNext())
        {
            string[] args = iterator.next().Split(",");
            if (args.Length < 3) continue;
            translator.Add(args[1], args[2]);
        }
    }

    /**
     * 翻译词性
     *
     * @param tag
     * @return
     */
    public static string translate(string tag)
    {
        return !translator.TryGetValue(tag,out var cn) ? tag : cn;
    }
}
