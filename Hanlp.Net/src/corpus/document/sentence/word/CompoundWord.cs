/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/8 17:42</create-date>
 *
 * <copyright file="CompoundWord.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using System.Text;

namespace com.hankcs.hanlp.corpus.document.sentence.word;


/**
 * 复合词，由两个或以上的word构成
 * @author hankcs
 */
public class CompoundWord : IWord, IEnumerable<Word>
{
    /**
     * 由这些词复合而来
     */
    public List<Word> innerList;

    /**
     * 标签，通常是词性
     */
    public string label;

    //@Override
    public string Value()
    {
        StringBuilder sb =  new StringBuilder();
        foreach (Word word in innerList)
        {
            sb.Append(word.value);
        }
        return sb.ToString();
    }

    //@Override
    //@Override
    public string Label { get => label; set => this.label = value; }

    //@Override
    public int Length => Value().Length;

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('[');
        int i = 1;
        foreach (Word word in innerList)
        {
            sb.Append(word.Value);
            string label = word.Label;
            if (label != null)
            {
                sb.Append('/').Append(label);
            }
            if (i != innerList.Count)
            {
                sb.Append(' ');
            }
            ++i;
        }
        sb.Append("]/");
        sb.Append(label);
        return sb.ToString();
    }

    /**
     * 转换为一个简单词
     * @return
     */
    public Word toWord()
    {
        return new Word(Value(), Label);
    }

    public CompoundWord(List<Word> innerList, string label)
    {
        this.innerList = innerList;
        this.label = label;
    }

    public static CompoundWord create(string param)
    {
        if (param == null) return null;
        int cutIndex = param.LastIndexOf(']');
        if (cutIndex <= 2 || cutIndex == param.Length - 1) return null;
        string wordParam  = param.substring(1, cutIndex);
        List<Word> wordList = new ();
        foreach (string single in wordParam.Split("\\s+"))
        {
            if (single.Length == 0) continue;
            Word word = Word.create(single);
            if (word == null)
            {
                logger.warning("使用参数" + single + "构造单词时发生错误");
                return null;
            }
            wordList.Add(word);
        }
        string labelParam = param.Substring(cutIndex + 1);
        if (labelParam.StartsWith("/"))
        {
            labelParam = labelParam.Substring(1);
        }
        return new CompoundWord(wordList, labelParam);
    }

    //@Override
    public IEnumerator<Word> GetEnumerator()
    {
        return innerList.GetEnumerator();
    }
}
