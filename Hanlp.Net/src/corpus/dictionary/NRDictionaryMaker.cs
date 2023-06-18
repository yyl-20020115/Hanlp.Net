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
            System._out.println("开始制作词典");
        // 将非A的词语保存下来
        for (List<IWord> wordList : sentenceList)
        {
            for (IWord word : wordList)
            {
                if (!word.getLabel().Equals(NR.A.toString()))
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
        if (verbose)
            System._out.println("开始标注角色");
        int i = 0;
        for (List<IWord> wordList : sentenceList)
        {
            if (verbose)
            {
                System._out.println(++i + " / " + sentenceList.size());
                System._out.println("原始语料 " + wordList);
            }
            // 先标注A和K
            IWord pre = new Word("##始##", "begin");
            ListIterator<IWord> listIterator = wordList.listIterator();
            while (listIterator.hasNext())
            {
                IWord word = listIterator.next();
                if (!word.getLabel().Equals(Nature.nr.toString()))
                {
                    word.setLabel(NR.A.toString());
                }
                else
                {
                    if (!pre.getLabel().Equals(Nature.nr.toString()))
                    {
                        pre.setLabel(NR.K.toString());
                    }
                }
                pre = word;
            }
            if (verbose) System._out.println("标注非前 " + wordList);
            // 然后标注LM
            IWord next = new Word("##末##", "end");
            while (listIterator.hasPrevious())
            {
                IWord word = listIterator.previous();
                if (word.getLabel().Equals(Nature.nr.toString()))
                {
                    string label = next.getLabel();
                    if (label.Equals("A")) next.setLabel("L");
                    else if (label.Equals("K")) next.setLabel("M");
                }
                next = word;
            }
            if (verbose) System._out.println("标注中后 " + wordList);
            // 拆分名字
            listIterator = wordList.listIterator();
            while (listIterator.hasNext())
            {
                IWord word = listIterator.next();
                if (word.getLabel().Equals(Nature.nr.toString()))
                {
                    switch (word.getValue().Length)
                    {
                        case 2:
                            if (word.getValue().StartsWith("大")
                                    || word.getValue().StartsWith("老")
                                    || word.getValue().StartsWith("小")
                                    )
                            {
                                listIterator.Add(new Word(word.getValue().substring(1, 2), NR.B.toString()));
                                word.setValue(word.getValue().substring(0, 1));
                                word.setLabel(NR.F.toString());
                            }
                            else if (word.getValue().EndsWith("哥")
                                    || word.getValue().EndsWith("公")
                                    || word.getValue().EndsWith("姐")
                                    || word.getValue().EndsWith("老")
                                    || word.getValue().EndsWith("某")
                                    || word.getValue().EndsWith("嫂")
                                    || word.getValue().EndsWith("氏")
                                    || word.getValue().EndsWith("总")
                                    )

                            {
                                listIterator.Add(new Word(word.getValue().substring(1, 2), NR.G.toString()));
                                word.setValue(word.getValue().substring(0, 1));
                                word.setLabel(NR.B.toString());
                            }
                            else
                            {
                                listIterator.Add(new Word(word.getValue().substring(1, 2), NR.E.toString()));
                                word.setValue(word.getValue().substring(0, 1));
                                word.setLabel(NR.B.toString());
                            }
                            break;
                        case 3:
                            listIterator.Add(new Word(word.getValue().substring(1, 2), NR.C.toString()));
                            listIterator.Add(new Word(word.getValue().substring(2, 3), NR.D.toString()));
                            word.setValue(word.getValue().substring(0, 1));
                            word.setLabel(NR.B.toString());
                            break;
                        default:
                            word.setLabel(NR.A.toString()); // 非中国人名
                    }
                }
            }
            if (verbose) System._out.println("姓名拆分 " + wordList);
            // 上文成词
            listIterator = wordList.listIterator();
            pre = new Word("##始##", "begin");
            while (listIterator.hasNext())
            {
                IWord word = listIterator.next();
                if (word.getLabel().Equals(NR.B.toString()))
                {
                    string combine = pre.getValue() + word.getValue();
                    if (dictionary.contains(combine))
                    {
                        pre.setValue(combine);
                        pre.setLabel("U");
                        listIterator.Remove();
                    }
                }
                pre = word;
            }
            if (verbose) System._out.println("上文成词 " + wordList);
            // 头部成词
            next = new Word("##末##", "end");
            while (listIterator.hasPrevious())
            {
                IWord word = listIterator.previous();
                if (word.getLabel().Equals(NR.B.toString()))
                {
                    string combine = word.getValue() + next.getValue();
                    if (dictionary.contains(combine))
                    {
                        next.setValue(combine);
                        next.setLabel(next.getLabel().Equals(NR.C.toString()) ? NR.X.toString() : NR.Y.toString());
                        listIterator.Remove();
                    }
                }
                next = word;
            }
            if (verbose) System._out.println("头部成词 " + wordList);
            // 尾部成词
            pre = new Word("##始##", "begin");
            while (listIterator.hasNext())
            {
                IWord word = listIterator.next();
                if (word.getLabel().Equals(NR.D.toString()))
                {
                    string combine = pre.getValue() + word.getValue();
                    if (dictionary.contains(combine))
                    {
                        pre.setValue(combine);
                        pre.setLabel(NR.Z.toString());
                        listIterator.Remove();
                    }
                }
                pre = word;
            }
            if (verbose) System._out.println("尾部成词 " + wordList);
            // 下文成词
            next = new Word("##末##", "end");
            while (listIterator.hasPrevious())
            {
                IWord word = listIterator.previous();
                if (word.getLabel().Equals(NR.D.toString()))
                {
                    string combine = word.getValue() + next.getValue();
                    if (dictionary.contains(combine))
                    {
                        next.setValue(combine);
                        next.setLabel(NR.V.toString());
                        listIterator.Remove();
                    }
                }
                next = word;
            }
            if (verbose) System._out.println("头部成词 " + wordList);
            LinkedList<IWord> wordLinkedList = (LinkedList<IWord>) wordList;
            wordLinkedList.addFirst(new Word(Predefine.TAG_BIGIN, "S"));
            wordLinkedList.addLast(new Word(Predefine.TAG_END, "A"));
            if (verbose) System._out.println("添加首尾 " + wordList);
        }
    }
}
