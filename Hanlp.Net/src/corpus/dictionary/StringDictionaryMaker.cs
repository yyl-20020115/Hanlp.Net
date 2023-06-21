/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 19:52</create-date>
 *
 * <copyright file="SimpleDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.dictionary;


/**
 * 方便的工厂类
 * @author hankcs
 */
public class StringDictionaryMaker
{
    /**
     * 加载词典
     * @param path
     * @param separator
     * @return
     */
    public static StringDictionary load(string path, string separator)
    {
        StringDictionary dictionary = new StringDictionary(separator);
        if (dictionary.load(path)) return dictionary;
        return null;
    }

    /**
     * 加载词典
     * @param path
     * @return
     */
    public static StringDictionary load(string path)
    {
        return load(path, "=");
    }

    /**
     * 合并词典，第一个为主词典
     * @param args
     * @return
     */
    public static StringDictionary combine(params StringDictionary[] args)
    {
        StringDictionary[] dictionaries = args.clone();
        StringDictionary mainDictionary = dictionaries[0];
        for (int i = 1; i < dictionaries.Length; ++i)
        {
            mainDictionary.combine(dictionaries[i]);
        }

        return mainDictionary;
    }

    public static StringDictionary combine(params string[] args)
    {
        string[] pathArray = args.clone();
        List<StringDictionary> dictionaryList = new ();
        foreach (string path in pathArray)
        {
            StringDictionary dictionary = load(path);
            if (dictionary == null) continue;
            dictionaryList.Add(dictionary);
        }

        return combine(dictionaryList.ToArray());
    }
}
