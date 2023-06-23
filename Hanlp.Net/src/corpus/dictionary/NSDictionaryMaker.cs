/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/17 14:39</create-date>
 *
 * <copyright file="NSDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.corpus.util;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.corpus.dictionary;




/**
 * @author hankcs
 */
public class NSDictionaryMaker : CommonDictionaryMaker
{
    public NSDictionaryMaker(EasyDictionary dictionary)
        : base(dictionary)
    {
        ;
    }

    //@Override
    protected override void addToDictionary(List<List<IWord>> sentenceList)
    {
//        logger.warning("开始制作词典");
        // 将非A的词语保存下来
        foreach (List<IWord> wordList in sentenceList)
        {
            foreach (IWord word in wordList)
            {
                if (!word.getLabel().Equals(NS.Z.ToString()))
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
    protected override void roleTag(List<List<IWord>> sentenceList)
    {
        int i = 0;
        foreach (List<IWord> wordList in sentenceList)
        {
            Precompiler.compileWithoutNS(wordList);
            if (verbose)
            {
                Console.Write(++i + " / " + sentenceList.Count + " ");
                Console.WriteLine("原始语料 " + wordList);
            }
            List<IWord> wordLinkedList = (List<IWord>) wordList;
            wordLinkedList.addFirst(new Word(Predefine.TAG_BIGIN, "S"));
            wordLinkedList.addLast(new Word(Predefine.TAG_END, "Z"));
            if (verbose) Console.WriteLine("添加首尾 " + wordList);
            // 标注上文
            IEnumerator<IWord> iterator = wordLinkedList.GetEnumerator();
            IWord pre = iterator.next();
            while (iterator.MoveNext())
            {
                IWord current = iterator.next();
                if (current.getLabel().StartsWith("ns") && !pre.getLabel().StartsWith("ns"))
                {
                    pre.setLabel(NS.A.ToString());
                }
                pre = current;
            }
            if (verbose) Console.WriteLine("标注上文 " + wordList);
            // 标注下文
            iterator = wordLinkedList.descendingIterator();
            pre = iterator.next();
            while (iterator.MoveNext())
            {
                IWord current = iterator.next();
                if (current.getLabel().StartsWith("ns") && !pre.getLabel().StartsWith("ns"))
                {
                    pre.setLabel(NS.B.ToString());
                }
                pre = current;
            }
            if (verbose) Console.WriteLine("标注下文 " + wordList);
            // 标注中间
            iterator = wordLinkedList.GetEnumerator();
            IWord first = iterator.next();
            IWord second = iterator.next();
            while (iterator.MoveNext())
            {
                IWord third = iterator.next();
                if (first.getLabel().StartsWith("ns") && third.getLabel().StartsWith("ns") && !second.getLabel().StartsWith("ns"))
                {
                    second.setLabel(NS.X.ToString());
                }
                first = second;
                second = third;
            }
            if (verbose) Console.WriteLine("标注中间 " + wordList);
            // 拆分地名
            CorpusUtil.spilt(wordList);
            if (verbose) Console.WriteLine("拆分地名 " + wordList);
            // 处理整个
            var listIterator = wordLinkedList.GetEnumerator();
            while (listIterator.MoveNext())
            {
                IWord word = listIterator.next();
                string label = word.getLabel();
                if (label.Equals(label.ToUpper())) continue;
                if (label.StartsWith("ns"))
                {
                    string value = word.Value;
                    int longestSuffixLength = PlaceSuffixDictionary.dictionary.getLongestSuffixLength(value);
                    int wordLength = value.Length - longestSuffixLength;
                    if (longestSuffixLength == 0 || wordLength == 0)
                    {
                        word.setLabel(NS.G.ToString());
                        continue;
                    }
                    listIterator.Remove();
                    if (wordLength > 3)
                    {
                        listIterator.Add(new Word(value.substring(0, wordLength), NS.G.ToString()));
                        listIterator.Add(new Word(value.substring(wordLength), NS.H.ToString()));
                        continue;
                    }
                    for (int l = 1, tag = NS.C.ordinal(); l <= wordLength; ++l, ++tag)
                    {
                        listIterator.Add(new Word(value.substring(l - 1, l), NS.values()[tag].ToString()));
                    }
                    listIterator.Add(new Word(value.substring(wordLength), NS.H.ToString()));
                }
                else
                {
                    word.setLabel(NS.Z.ToString());
                }
            }
            if (verbose) Console.WriteLine("处理整个 " + wordList);
        }
    }
}
