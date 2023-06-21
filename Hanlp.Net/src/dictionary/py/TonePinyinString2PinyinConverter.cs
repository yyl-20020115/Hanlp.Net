/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/6 16:02</create-date>
 *
 * <copyright file="SingleString2PinyinConverter.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm.ahocorasick.trie;

namespace com.hankcs.hanlp.dictionary.py;



/**
 * 将类似āiyā的词语转为拼音的转换器
 * @author hankcs
 */
public class TonePinyinString2PinyinConverter
{
    /**
     * 带音调的字母到Pinyin的map
     */
    static Dictionary<string, Pinyin> mapKey;
    /**
     * 带数字音调的字幕到Pinyin的map
     */
    static Dictionary<string, Pinyin> mapNumberKey;
    static Trie trie;
    static TonePinyinString2PinyinConverter()
    {
        mapNumberKey = new Dictionary<string, Pinyin>();
        mapKey = new Dictionary<string, Pinyin>();
        foreach (Pinyin pinyin in Integer2PinyinConverter.pinyins)
        {
            mapNumberKey.Add(pinyin.ToString(), pinyin);
            string pinyinWithToneMark = pinyin.getPinyinWithToneMark();
            string pinyinWithoutTone = pinyin.getPinyinWithoutTone();
            Pinyin tone5 = String2PinyinConverter.convert2Tone5(pinyin);
            mapKey.Add(pinyinWithToneMark, pinyin);
            mapKey.Add(pinyinWithoutTone, tone5);
        }
        trie = new Trie().remainLongest();
        trie.addAllKeyword(mapKey.Keys);
    }

    /**
     * 这个拼音是否合格
     * @param singlePinyin
     * @return
     */
    public static bool valid(string singlePinyin)
    {
        if (mapNumberKey.ContainsKey(singlePinyin)) return true;

        return false;
    }

    public static Pinyin convertFromToneNumber(string singlePinyin)
    {
        return mapNumberKey.get(singlePinyin);
    }

    public static List<Pinyin> convert(string[] pinyinArray)
    {
        List<Pinyin> pinyinList = new (pinyinArray.Length);
        for (int i = 0; i < pinyinArray.Length; i++)
        {
            pinyinList.Add(mapKey.get(pinyinArray[i]));
        }

        return pinyinList;
    }

    public static Pinyin convert(string singlePinyin)
    {
        return mapKey.get(singlePinyin);
    }

    /**
     *
     * @param tonePinyinText
     * @return
     */
    public static List<Pinyin> convert(string tonePinyinText, bool removeNull)
    {
        List<Pinyin> pinyinList = new ();
        ICollection<Token> tokenize = trie.tokenize(tonePinyinText);
        foreach (Token token in tokenize)
        {
            Pinyin pinyin = mapKey.get(token.getFragment());
            if (removeNull && pinyin == null) continue;
            pinyinList.Add(pinyin);
        }

        return pinyinList;
    }

    /**
     * 这些拼音是否全部合格
     * @param pinyinStringArray
     * @return
     */
    public static bool valid(string[] pinyinStringArray)
    {
        foreach (string p in pinyinStringArray)
        {
            if (!valid(p)) return false;
        }

        return true;
    }

    public static List<Pinyin> convertFromToneNumber(string[] pinyinArray)
    {
        List<Pinyin> pinyinList = new (pinyinArray.Length);
        foreach (string py in pinyinArray)
        {
            pinyinList.Add(convertFromToneNumber(py));
        }
        return pinyinList;
    }
}
