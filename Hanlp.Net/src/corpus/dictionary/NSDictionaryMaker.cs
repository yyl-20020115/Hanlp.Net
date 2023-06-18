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
using com.hankcs.hanlp.corpus.util;

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
    protected void addToDictionary(List<List<IWord>> sentenceList)
    {
//        logger.warning("开始制作词典");
        // 将非A的词语保存下来
        for (List<IWord> wordList : sentenceList)
        {
            for (IWord word : wordList)
            {
                if (!word.getLabel().Equals(NS.Z.toString()))
                {
                    dictionaryMaker.Add(word);
                }
            }
        }
        // 制作NGram词典
        for (List<IWord> wordList : sentenceList)
        {
            IWord pre = null;
            for (IWord word : wordList)
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
        int i = 0;
        for (List<IWord> wordList : sentenceList)
        {
            Precompiler.compileWithoutNS(wordList);
            if (verbose)
            {
                System._out.print(++i + " / " + sentenceList.size() + " ");
                System._out.println("原始语料 " + wordList);
            }
            LinkedList<IWord> wordLinkedList = (LinkedList<IWord>) wordList;
            wordLinkedList.addFirst(new Word(Predefine.TAG_BIGIN, "S"));
            wordLinkedList.addLast(new Word(Predefine.TAG_END, "Z"));
            if (verbose) System._out.println("添加首尾 " + wordList);
            // 标注上文
            Iterator<IWord> iterator = wordLinkedList.iterator();
            IWord pre = iterator.next();
            while (iterator.hasNext())
            {
                IWord current = iterator.next();
                if (current.getLabel().StartsWith("ns") && !pre.getLabel().StartsWith("ns"))
                {
                    pre.setLabel(NS.A.toString());
                }
                pre = current;
            }
            if (verbose) System._out.println("标注上文 " + wordList);
            // 标注下文
            iterator = wordLinkedList.descendingIterator();
            pre = iterator.next();
            while (iterator.hasNext())
            {
                IWord current = iterator.next();
                if (current.getLabel().StartsWith("ns") && !pre.getLabel().StartsWith("ns"))
                {
                    pre.setLabel(NS.B.toString());
                }
                pre = current;
            }
            if (verbose) System._out.println("标注下文 " + wordList);
            // 标注中间
            iterator = wordLinkedList.iterator();
            IWord first = iterator.next();
            IWord second = iterator.next();
            while (iterator.hasNext())
            {
                IWord third = iterator.next();
                if (first.getLabel().StartsWith("ns") && third.getLabel().StartsWith("ns") && !second.getLabel().StartsWith("ns"))
                {
                    second.setLabel(NS.X.toString());
                }
                first = second;
                second = third;
            }
            if (verbose) System._out.println("标注中间 " + wordList);
            // 拆分地名
            CorpusUtil.spilt(wordList);
            if (verbose) System._out.println("拆分地名 " + wordList);
            // 处理整个
            ListIterator<IWord> listIterator = wordLinkedList.listIterator();
            while (listIterator.hasNext())
            {
                IWord word = listIterator.next();
                string label = word.getLabel();
                if (label.Equals(label.toUpperCase())) continue;
                if (label.StartsWith("ns"))
                {
                    string value = word.getValue();
                    int longestSuffixLength = PlaceSuffixDictionary.dictionary.getLongestSuffixLength(value);
                    int wordLength = value.Length - longestSuffixLength;
                    if (longestSuffixLength == 0 || wordLength == 0)
                    {
                        word.setLabel(NS.G.toString());
                        continue;
                    }
                    listIterator.Remove();
                    if (wordLength > 3)
                    {
                        listIterator.Add(new Word(value.substring(0, wordLength), NS.G.toString()));
                        listIterator.Add(new Word(value.substring(wordLength), NS.H.toString()));
                        continue;
                    }
                    for (int l = 1, tag = NS.C.ordinal(); l <= wordLength; ++l, ++tag)
                    {
                        listIterator.Add(new Word(value.substring(l - 1, l), NS.values()[tag].toString()));
                    }
                    listIterator.Add(new Word(value.substring(wordLength), NS.H.toString()));
                }
                else
                {
                    word.setLabel(NS.Z.toString());
                }
            }
            if (verbose) System._out.println("处理整个 " + wordList);
        }
    }
}
