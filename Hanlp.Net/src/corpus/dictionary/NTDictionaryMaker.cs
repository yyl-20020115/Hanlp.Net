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
using System.Text;

namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * @author hankcs
 */
public class NTDictionaryMaker : CommonDictionaryMaker
{
    TFDictionary tfDictionary = new TFDictionary();

    public NTDictionaryMaker(EasyDictionary dictionary)
        : base(dictionary)
    {
        ;
    }

    //@Override
    protected void addToDictionary(List<List<IWord>> sentenceList)
    {
        //        logger.warning("开始制作词典");
        // 将非A的词语保存下来
        foreach (List<IWord> wordList in sentenceList)
        {
            foreach (IWord word in wordList)
            {
                if (!word.getLabel().Equals(NT.Z.ToString()))
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
        int i = 0;
        foreach (List<IWord> wordList in sentenceList)
        {
            Precompiler.compileWithoutNT(wordList);
            if (verbose)
            {
                Console.Write(++i + " / " + sentenceList.size() + " ");
                Console.WriteLine("原始语料 " + wordList);
            }
            LinkedList<IWord> wordLinkedList = (LinkedList<IWord>) wordList;
            wordLinkedList.addFirst(new Word(Predefine.TAG_BIGIN, "S"));
            wordLinkedList.addLast(new Word(Predefine.TAG_END, "Z"));
            if (verbose) Console.WriteLine("添加首尾 " + wordList);
            // 标注上文
            Iterator<IWord> iterator = wordLinkedList.iterator();
            IWord pre = iterator.next();
            while (iterator.MoveNext())
            {
                IWord current = iterator.next();
                if (current.getLabel().StartsWith("nt") && !pre.getLabel().StartsWith("nt"))
                {
                    pre.setLabel(NT.A.ToString());
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
                if (current.getLabel().StartsWith("nt") && !pre.getLabel().StartsWith("nt"))
                {
                    pre.setLabel(NT.B.ToString());
                }
                pre = current;
            }
            if (verbose) Console.WriteLine("标注下文 " + wordList);
            // 标注中间
            {
                iterator = wordLinkedList.iterator();
                IWord first = iterator.next();
                IWord second = iterator.next();
                while (iterator.MoveNext())
                {
                    IWord third = iterator.next();
                    if (first.getLabel().StartsWith("nt") && third.getLabel().StartsWith("nt") && !second.getLabel().StartsWith("nt"))
                    {
                        second.setLabel(NT.X.ToString());
                    }
                    first = second;
                    second = third;
                }
                if (verbose) Console.WriteLine("标注中间 " + wordList);
            }
            // 处理整个
            ListIterator<IWord> listIterator = wordLinkedList.GetEnumerator();
            while (listIterator.MoveNext())
            {
                IWord word = listIterator.next();
                string label = word.getLabel();
                if (label.Equals(label.toUpperCase())) continue;
                if (label.StartsWith("nt"))
                {
                    StringBuilder sbPattern = new StringBuilder();
                    // 复杂机构
                    if (word is CompoundWord)
                    {
                        listIterator.Remove();
                        Word last = null;
                        foreach (Word inner in ((CompoundWord) word).innerList)
                        {
                            last = inner;
                            string innerLabel = inner.label;
                            if (innerLabel.StartsWith("ns"))
                            {
                                inner.setValue(Predefine.TAG_PLACE);
                                inner.setLabel(NT.G.ToString());
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.StartsWith("nt"))
                            {
                                inner.value = Predefine.TAG_GROUP;
                                inner.label = NT.K.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.Equals("b") || innerLabel.Equals("ng") || innerLabel.Equals("j"))
                            {
                                inner.label = NT.J.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("n".Equals(innerLabel) ||
                                    "an".Equals(innerLabel) ||
                                    "a".Equals(innerLabel) ||
                                    "vn".Equals(innerLabel) ||
                                    "vd".Equals(innerLabel) ||
                                    "vl".Equals(innerLabel) ||
                                    "v".Equals(innerLabel) ||
                                    "vi".Equals(innerLabel) ||
                                    "nnt".Equals(innerLabel) ||
                                    "nnd".Equals(innerLabel) ||
                                    "nf".Equals(innerLabel) ||
                                    "cc".Equals(innerLabel) ||
                                    "t".Equals(innerLabel) ||
                                    "z".Equals(innerLabel)
                                    )
                            {
                                inner.label = NT.C.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("nz".Equals(innerLabel))
                            {
                                inner.label = NT.I.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("m".Equals(innerLabel))
                            {
                                inner.value = Predefine.TAG_NUMBER;
                                inner.label = NT.M.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("w".Equals(innerLabel))
                            {
                                inner.label = NT.W.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.StartsWith("nr") || "x".Equals(innerLabel) || "nx".Equals(innerLabel))
                            {
                                inner.value = Predefine.TAG_PEOPLE;
                                inner.label = NT.F.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.StartsWith("ni"))
                            {
                                inner.label = NT.D.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("f".Equals(innerLabel) || "s".Equals(innerLabel))
                            {
                                inner.label = NT.L.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else
                            {
                                inner.label = NT.P.ToString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                    }
                    if (last != null)
                    {
                        last.label = NT.D.ToString();
                        sbPattern.deleteCharAt(sbPattern.Length - 1);
                        sbPattern.Append(last.label);
                        tfDictionary.Add(sbPattern.ToString());
                        sbPattern.setLength(0);
                    }
                }
                else
                {
                    word.setLabel(NT.K.ToString());
                }
            }
            else
            {
                word.setLabel(NT.Z.ToString());
            }
        }
        if (verbose) Console.WriteLine("处理整个 " + wordList);
        wordLinkedList.getFirst().setLabel(NT.S.ToString());
    }

}

    //@Override
    public bool saveTxtTo(string path)
    {
        if (!base.saveTxtTo(path)) return false;
        return tfDictionary.saveKeyTo(path + ".pattern.txt");
    }
}
