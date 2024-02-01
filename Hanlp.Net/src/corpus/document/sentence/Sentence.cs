/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/8 18:04</create-date>
 *
 * <copyright file="Sentence.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.model.perceptron.utility;
using System.Text;
using System.Text.RegularExpressions;

namespace com.hankcs.hanlp.corpus.document.sentence;




/**
 * 句子，指的是以。！等标点结尾的句子
 *
 * @author hankcs
 */
public class Sentence : /*Serializable,*/ IEnumerable<IWord>
{
    /**
     * 词语列表（复合或简单单词的列表）
     */
    public List<IWord> wordList;

    public Sentence(List<IWord> wordList)
    {
        this.wordList = wordList;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(Count() * 4);
        int i = 1;
        foreach (IWord word in wordList)
        {
            sb.Append(word);
            if (i != wordList.Count) sb.Append(' ');
            ++i;
        }
        return sb.ToString();
    }

    /**
     * 转换为空格分割无标签的string
     *
     * @return
     */
    public string toStringWithoutLabels()
    {
        StringBuilder sb = new StringBuilder(Count() * 4);
        int i = 1;
        foreach (IWord word in wordList)
        {
            if (word is CompoundWord)
            {
                int j = 0;
                foreach (Word w in ((CompoundWord) word).innerList)
                {
                    sb.Append(w.Value);
                    if (++j != ((CompoundWord) word).innerList.Count)
                        sb.Append(' ');
                }
            }
            else
                sb.Append(word.Value);
            if (i != wordList.Count) sb.Append(' ');
            ++i;
        }
        return sb.ToString();
    }

    /**
     * brat standoff Format<br>
     * http://brat.nlplab.org/standoff.html
     *
     * @return
     */
    public string toStandoff()
    {
        return toStandoff(false);
    }

    /**
     * brat standoff Format<br>
     * http://brat.nlplab.org/standoff.html
     *
     * @param withComment
     * @return
     */
    public string toStandoff(bool withComment)
    {
        StringBuilder sb = new StringBuilder(Count() * 4);
        string delimiter = " ";
        string text2 = text(delimiter);
        sb.Append(text2).Append('\n');
        int i = 1;
        int offset = 0;
        foreach (IWord word in wordList)
        {
            //assert text.charAt(offset) == word.Value[0];
            printWord(word, sb, i, offset, withComment);
            ++i;
            if (word is CompoundWord)
            {
                int offsetChild = offset;
                foreach (Word child in ((CompoundWord) word).innerList)
                {
                    printWord(child, sb, i, offsetChild, withComment);
                    offsetChild += child.Length();
                    offsetChild += delimiter.Length;
                    ++i;
                }
                offset += delimiter.Length * ((CompoundWord) word).innerList.Count;
            }
            else
            {
                offset += delimiter.Length;
            }
            offset += word.Length();
        }
        return sb.ToString();
    }

    /**
     * 按照 PartOfSpeechTagDictionary 指定的映射表将词语词性翻译过去
     *
     * @return
     */
    public Sentence translateLabels()
    {
        foreach (IWord word in wordList)
        {
            word.setLabel(PartOfSpeechTagDictionary.translate(word.getLabel()));
            if (word is CompoundWord)
            {
                foreach (Word child in ((CompoundWord) word).innerList)
                {
                    child.setLabel(PartOfSpeechTagDictionary.translate(child.getLabel()));
                }
            }
        }
        return this;
    }

    /**
     * 按照 PartOfSpeechTagDictionary 指定的映射表将复合词词语词性翻译过去
     *
     * @return
     */
    public Sentence translateCompoundWordLabels()
    {
        foreach (IWord word in wordList)
        {
            if (word is CompoundWord)
                word.setLabel(PartOfSpeechTagDictionary.translate(word.getLabel()));
        }
        return this;
    }

    private void printWord(IWord word, StringBuilder sb, int id, int offset)
    {
        printWord(word, sb, id, offset, false);
    }

    private void printWord(IWord word, StringBuilder sb, int id, int offset, bool withComment)
    {
        char delimiter = '\t';
        char endLine = '\n';
        sb.Append('T').Append(id).Append(delimiter);
        sb.Append(word.getLabel()).Append(delimiter);
        int Length = word.Length();
        if (word is CompoundWord)
        {
            Length += ((CompoundWord) word).innerList.Count - 1;
        }
        sb.Append(offset).Append(delimiter).Append(offset + Length).Append(delimiter);
        sb.Append(word.Value).Append(endLine);
        string translated = PartOfSpeechTagDictionary.translate(word.getLabel());
        if (withComment && !word.getLabel().Equals(translated))
        {
            sb.Append('#').Append(id).Append(delimiter).Append("AnnotatorNotes").Append(delimiter)
                .Append('T').Append(id).Append(delimiter).Append(translated)
                .Append(endLine);
        }
    }

    /**
     * 以人民日报2014语料格式的字符串创建一个结构化句子
     *
     * @param param
     * @return
     */
    public static Sentence create(string param)
    {
        if (param == null)
        {
            return null;
        }
        param = param.Trim();
        if (param.Length == 0)
        {
            return null;
        }
        var pattern = new Regex("(\\[(([^\\s]+/[0-9a-zA-Z]+)\\s+)+?([^\\s]+/[0-9a-zA-Z]+)]/?[0-9a-zA-Z]+)|([^\\s]+/[0-9a-zA-Z]+)");
        var matcher = pattern.matcher(param);
        List<IWord> wordList = new ();
        while (matcher.find())
        {
            string single = matcher.group();
            IWord word = WordFactory.create(single);
            if (word == null)
            {
                logger.warning("在用 " + single + " 构造单词时失败，句子构造参数为 " + param);
                return null;
            }
            wordList.Add(word);
        }
        if (wordList.Count==0) // 按照无词性来解析
        {
            foreach (string w in param.Split("\\s+"))
            {
                wordList.Add(new Word(w, null));
            }
        }

        return new Sentence(wordList);
    }

    /**
     * 句子中单词（复合词或简单词）的数量
     *
     * @return
     */
    public int Count()
    {
        return wordList.Count;
    }

    /**
     * 句子文本长度
     *
     * @return
     */
    public int Length()
    {
        int Length = 0;
        foreach (IWord word in this)
        {
            Length += word.Value.Length;
        }

        return Length;
    }

    /**
     * 原始文本形式（无标注，raw text）
     *
     * @return
     */
    public string text()
    {
        return text(null);
    }

    /**
     * 原始文本形式（无标注，raw text）
     *
     * @param delimiter 词语之间的分隔符
     * @return
     */
    public string text(string delimiter)
    {
        if (delimiter == null) delimiter = "";
        StringBuilder sb = new StringBuilder(Count() * 3);
        foreach (IWord word in this)
        {
            if (word is CompoundWord)
            {
                foreach (Word child in ((CompoundWord) word).innerList)
                {
                    sb.Append(child.Value).Append(delimiter);
                }
            }
            else
            {
                sb.Append(word.Value).Append(delimiter);
            }
        }
        sb.Length=(sb.Length - delimiter.Length);

        return sb.ToString();
    }

    //@Override
    public IEnumerator<IWord> GetEnumerator()
    {
        return wordList.GetEnumerator();
    }

    /**
     * 找出所有词性为label的单词（不检查复合词内部的简单词）
     *
     * @param label
     * @return
     */
    public List<IWord> findWordsByLabel(string label)
    {
        List<IWord> wordList = new ();
        foreach (IWord word in this)
        {
            if (label.Equals(word.getLabel()))
            {
                wordList.Add(word);
            }
        }
        return wordList;
    }

    /**
     * 找出第一个词性为label的单词（不检查复合词内部的简单词）
     *
     * @param label
     * @return
     */
    public IWord findFirstWordByLabel(string label)
    {
        foreach (IWord word in this)
        {
            if (label.Equals(word.getLabel()))
            {
                return word;
            }
        }
        return null;
    }

    /**
     * 找出第一个词性为label的单词的指针（不检查复合词内部的简单词）<br>
     * 若要查看该单词，请调用 previous<br>
     * 若要删除该单词，请调用 Remove<br>
     *
     * @param label
     * @return
     */
    public IEnumerator<IWord> findFirstWordIteratorByLabel(string label)
    {
        var listIterator = this.wordList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            IWord word = listIterator.Current;
            if (label.Equals(word.getLabel()))
            {
                return listIterator;
            }
        }
        return null;
    }

    /**
     * 是否含有词性为label的单词
     *
     * @param label
     * @return
     */
    public bool containsWordWithLabel(string label)
    {
        return findFirstWordByLabel(label) != null;
    }

    /**
     * 转换为简单单词列表
     *
     * @return
     */
    public List<Word> toSimpleWordList()
    {
        List<Word> wordList = new ();
        foreach (IWord word in this.wordList)
        {
            if (word is CompoundWord)
            {
                wordList.AddRange(((CompoundWord) word).innerList);
            }
            else
            {
                wordList.Add((Word) word);
            }
        }

        return wordList;
    }

    /**
     * 获取所有单词构成的数组
     *
     * @return
     */
    public string[] toWordArray()
    {
        List<Word> wordList = toSimpleWordList();
        string[] wordArray = new string[wordList.Count];
        IEnumerator<Word> iterator = wordList.GetEnumerator();
        for (int i = 0; i < wordArray.Length; i++)
        {
            iterator.MoveNext();
            wordArray[i] = iterator.Current.value;
        }
        return wordArray;
    }

    /**
     * word pos
     *
     * @return
     */
    public string[][] toWordTagArray()
    {
        List<Word> wordList = toSimpleWordList();
        string[][] pair = new string[2][wordList.Count];
        IEnumerator<Word> iterator = wordList.GetEnumerator();
        for (int i = 0; i < pair[0].Length; i++)
        {
            iterator.MoveNext();
            Word word = iterator.Current;
            pair[0][i] = word.value;
            pair[1][i] = word.label;
        }
        return pair;
    }

    /**
     * word pos ner
     *
     * @param tagSet
     * @return
     */
    public string[][] toWordTagNerArray(NERTagSet tagSet)
    {
        List<string[]> tupleList = Utility.convertSentenceToNER(this, tagSet);
        string[][] result = new string[3][tupleList.Count];
        var iterator = tupleList.GetEnumerator();
        for (int i = 0; i < result[0].Length; i++)
        {
            iterator.MoveNext();
            string[] tuple = iterator.Current;
            for (int j = 0; j < 3; ++j)
            {
                result[j][i] = tuple[j];
            }
        }
        return result;
    }

    public Sentence mergeCompoundWords()
    {
        IEnumerator<IWord> listIterator = wordList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            IWord word = listIterator.next();
            if (word is CompoundWord)
            {
                listIterator.set(new Word(word.Value, word.getLabel()));
            }
        }
        return this;
    }

    //@Override
    public override bool Equals(Object? o)
    {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        Sentence sentence = (Sentence) o;
        return ToString().Equals(sentence.ToString());
    }

    //@Override
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}

