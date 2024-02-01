/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 14:46</create-date>
 *
 * <copyright file="NRDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * nr词典（词典+ngram转移+词性转移矩阵）制作工具
 * @author hankcs
 */
public class NRDictionaryMaker : CommonDictionaryMaker
{

    public NRDictionaryMaker(EasyDictionary dictionary)
        :base(dictionary)
    {
    }

    //@Override
    protected void addToDictionary(List<List<IWord>> sentenceList)
    {
        if (verbose)
            Console.WriteLine("开始制作词典");
        // 将非A的词语保存下来
        foreach (List<IWord> wordList in sentenceList)
        {
            foreach (IWord word in wordList)
            {
                if (!word.Label.Equals(NR.A.ToString()))
                {
                    dictionaryMaker.Add(word);
                }
            }
        }
        // 制作NGram词典
        foreach (List<IWord> wordList in sentenceList)
        {
            IWord pre = null;
            foreach (IWord word in wordList)
            {
                if (pre != null)
                {
                    nGramDictionaryMaker.addPair(pre, word);
                }
                pre = word;
            }
        }
    }

    //@Override
    protected void roleTag(List<List<IWord>> sentenceList)
    {
        if (verbose)
            Console.WriteLine("开始标注角色");
        int i = 0;
        foreach (List<IWord> wordList in sentenceList)
        {
            if (verbose)
            {
                Console.WriteLine(++i + " / " + sentenceList.Count);
                Console.WriteLine("原始语料 " + wordList);
            }
            // 先标注A和K
            IWord pre = new Word("##始##", "begin");
            var listIterator = wordList.GetEnumerator();
            while (listIterator.MoveNext())
            {
                IWord word = listIterator.next();
                if (!word.Label.Equals(Nature.nr.ToString()))
                {
                    word.                    Label = NR.A.ToString();
                }
                else
                {
                    if (!pre.Label.Equals(Nature.nr.ToString()))
                    {
                        pre.                        Label = NR.K.ToString();
                    }
                }
                pre = word;
            }
            if (verbose) Console.WriteLine("标注非前 " + wordList);
            // 然后标注LM
            IWord next = new Word("##末##", "end");
            while (listIterator.hasPrevious())
            {
                IWord word = listIterator.previous();
                if (word.Label.Equals(Nature.nr.ToString()))
                {
                    string label = next.Label;
                    if (label.Equals("A")) next.Label = "L";
                    else if (label.Equals("K")) next.Label = "M";
                }
                next = word;
            }
            if (verbose) Console.WriteLine("标注中后 " + wordList);
            // 拆分名字
            listIterator = wordList.GetEnumerator();
            while (listIterator.MoveNext())
            {
                IWord word = listIterator.next();
                if (word.Label.Equals(Nature.nr.ToString()))
                {
                    switch (word.Value.Length)
                    {
                        case 2:
                            if (word.Value.StartsWith("大")
                                    || word.Value.StartsWith("老")
                                    || word.Value.StartsWith("小")
                                    )
                            {
                                listIterator.Add(new Word(word.Value.substring(1, 2), NR.B.ToString()));
                                word.                                Value = word.Value.substring(0, 1);
                                word.                                Label = NR.F.ToString();
                            }
                            else if (word.Value.EndsWith("哥")
                                    || word.Value.EndsWith("公")
                                    || word.Value.EndsWith("姐")
                                    || word.Value.EndsWith("老")
                                    || word.Value.EndsWith("某")
                                    || word.Value.EndsWith("嫂")
                                    || word.Value.EndsWith("氏")
                                    || word.Value.EndsWith("总")
                                    )

                            {
                                listIterator.Add(new Word(word.Value.substring(1, 2), NR.G.ToString()));
                                word.                                Value = word.Value.substring(0, 1);
                                word.                                Label = NR.B.ToString();
                            }
                            else
                            {
                                listIterator.Add(new Word(word.Value.substring(1, 2), NR.E.ToString()));
                                word.                                Value = word.Value.substring(0, 1);
                                word.                                Label = NR.B.ToString();
                            }
                            break;
                        case 3:
                            listIterator.Add(new Word(word.Value.substring(1, 2), NR.C.ToString()));
                            listIterator.Add(new Word(word.Value.substring(2, 3), NR.D.ToString()));
                            word.                            Value = word.Value.substring(0, 1);
                            word.                            Label = NR.B.ToString();
                            break;
                        default:
                            word.                            Label = NR.A.ToString(); // 非中国人名
                    }
                }
            }
            if (verbose) Console.WriteLine("姓名拆分 " + wordList);
            // 上文成词
            listIterator = wordList.GetEnumerator();
            pre = new Word("##始##", "begin");
            while (listIterator.MoveNext())
            {
                IWord word = listIterator.next();
                if (word.Label.Equals(NR.B.ToString()))
                {
                    string combine = pre.Value + word.Value;
                    if (dictionary.Contains(combine))
                    {
                        pre.                        Value = combine;
                        pre.                        Label = "U";
                        listIterator.Remove();
                    }
                }
                pre = word;
            }
            if (verbose) Console.WriteLine("上文成词 " + wordList);
            // 头部成词
            next = new Word("##末##", "end");
            while (listIterator.hasPrevious())
            {
                IWord word = listIterator.previous();
                if (word.Label.Equals(NR.B.ToString()))
                {
                    string combine = word.Value + next.Value;
                    if (dictionary.Contains(combine))
                    {
                        next.                        Value = combine;
                        next.                        Label = next.Label.Equals(NR.C.ToString()) ? NR.X.ToString() : NR.Y.ToString();
                        listIterator.Remove();
                    }
                }
                next = word;
            }
            if (verbose) Console.WriteLine("头部成词 " + wordList);
            // 尾部成词
            pre = new Word("##始##", "begin");
            while (listIterator.MoveNext())
            {
                IWord word = listIterator.next();
                if (word.Label.Equals(NR.D.ToString()))
                {
                    string combine = pre.Value + word.Value;
                    if (dictionary.Contains(combine))
                    {
                        pre.                        Value = combine;
                        pre.                        Label = NR.Z.ToString();
                        listIterator.Remove();
                    }
                }
                pre = word;
            }
            if (verbose) Console.WriteLine("尾部成词 " + wordList);
            // 下文成词
            next = new Word("##末##", "end");
            while (listIterator.hasPrevious())
            {
                IWord word = listIterator.previous();
                if (word.Label.Equals(NR.D.ToString()))
                {
                    string combine = word.Value + next.Value;
                    if (dictionary.Contains(combine))
                    {
                        next.                        Value = combine;
                        next.                        Label = NR.V.ToString();
                        listIterator.Remove();
                    }
                }
                next = word;
            }
            if (verbose) Console.WriteLine("头部成词 " + wordList);
            List<IWord> wordLinkedList = (List<IWord>) wordList;
            wordLinkedList.addFirst(new Word(Predefine.TAG_BIGIN, "S"));
            wordLinkedList.addLast(new Word(Predefine.TAG_END, "A"));
            if (verbose) Console.WriteLine("添加首尾 " + wordList);
        }
    }
}
