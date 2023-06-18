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
        for (List<IWord> wordList : sentenceList)
        {
            for (IWord word : wordList)
            {
                if (!word.getLabel().Equals(NT.Z.toString()))
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
            Precompiler.compileWithoutNT(wordList);
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
                if (current.getLabel().StartsWith("nt") && !pre.getLabel().StartsWith("nt"))
                {
                    pre.setLabel(NT.A.toString());
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
                if (current.getLabel().StartsWith("nt") && !pre.getLabel().StartsWith("nt"))
                {
                    pre.setLabel(NT.B.toString());
                }
                pre = current;
            }
            if (verbose) System._out.println("标注下文 " + wordList);
            // 标注中间
            {
                iterator = wordLinkedList.iterator();
                IWord first = iterator.next();
                IWord second = iterator.next();
                while (iterator.hasNext())
                {
                    IWord third = iterator.next();
                    if (first.getLabel().StartsWith("nt") && third.getLabel().StartsWith("nt") && !second.getLabel().StartsWith("nt"))
                    {
                        second.setLabel(NT.X.toString());
                    }
                    first = second;
                    second = third;
                }
                if (verbose) System._out.println("标注中间 " + wordList);
            }
            // 处理整个
            ListIterator<IWord> listIterator = wordLinkedList.listIterator();
            while (listIterator.hasNext())
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
                        for (Word inner : ((CompoundWord) word).innerList)
                        {
                            last = inner;
                            string innerLabel = inner.label;
                            if (innerLabel.StartsWith("ns"))
                            {
                                inner.setValue(Predefine.TAG_PLACE);
                                inner.setLabel(NT.G.toString());
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.StartsWith("nt"))
                            {
                                inner.value = Predefine.TAG_GROUP;
                                inner.label = NT.K.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.Equals("b") || innerLabel.Equals("ng") || innerLabel.Equals("j"))
                            {
                                inner.label = NT.J.toString();
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
                                inner.label = NT.C.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("nz".Equals(innerLabel))
                            {
                                inner.label = NT.I.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("m".Equals(innerLabel))
                            {
                                inner.value = Predefine.TAG_NUMBER;
                                inner.label = NT.M.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("w".Equals(innerLabel))
                            {
                                inner.label = NT.W.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.StartsWith("nr") || "x".Equals(innerLabel) || "nx".Equals(innerLabel))
                            {
                                inner.value = Predefine.TAG_PEOPLE;
                                inner.label = NT.F.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if (innerLabel.StartsWith("ni"))
                            {
                                inner.label = NT.D.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else if ("f".Equals(innerLabel) || "s".Equals(innerLabel))
                            {
                                inner.label = NT.L.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                            else
                            {
                                inner.label = NT.P.toString();
                                listIterator.Add(inner);
                                sbPattern.Append(inner.label);
                            }
                    }
                    if (last != null)
                    {
                        last.label = NT.D.toString();
                        sbPattern.deleteCharAt(sbPattern.Length - 1);
                        sbPattern.Append(last.label);
                        tfDictionary.Add(sbPattern.toString());
                        sbPattern.setLength(0);
                    }
                }
                else
                {
                    word.setLabel(NT.K.toString());
                }
            }
            else
            {
                word.setLabel(NT.Z.toString());
            }
        }
        if (verbose) System._out.println("处理整个 " + wordList);
        wordLinkedList.getFirst().setLabel(NT.S.toString());
    }

}

    //@Override
    public bool saveTxtTo(string path)
    {
        if (!super.saveTxtTo(path)) return false;
        return tfDictionary.saveKeyTo(path + ".pattern.txt");
    }
}
