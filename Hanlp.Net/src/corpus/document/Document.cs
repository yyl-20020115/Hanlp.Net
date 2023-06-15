/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/8 19:01</create-date>
 *
 * <copyright file="Document.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.tokenizers;
using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.corpus.document;




/**
 * @author hankcs
 */
public class Document : Serializable
{
    public List<Sentence> sentenceList;

    public Document(List<Sentence> sentenceList)
    {
        this.sentenceList = sentenceList;
    }

    public static Document create(string param)
    {
        var pattern = Pattern.compile(".+?((。/w)|(！/w )|(？/w )|\\n|$)");
        var matcher = pattern.matcher(param);
        List<Sentence> sentenceList = new LinkedList<Sentence>();
        while (matcher.find())
        {
            string single = matcher.group();
            Sentence sentence = Sentence.create(single);
            if (sentence == null)
            {
                logger.warning("使用" + single + "构建句子失败");
                return null;
            }
            sentenceList.add(sentence);
        }
        return new Document(sentenceList);
    }

    /**
     * 获取单词序列
     *
     * @return
     */
    public List<IWord> getWordList()
    {
        List<IWord> wordList = new ();
        foreach (Sentence sentence in sentenceList)
        {
            wordList.addAll(sentence.wordList);
        }
        return wordList;
    }

    public List<Word> getSimpleWordList()
    {
        List<IWord> wordList = getWordList();
        List<Word> simpleWordList = new ();
        foreach (IWord word in wordList)
        {
            if (word is CompoundWord)
            {
                simpleWordList.addAll(((CompoundWord) word).innerList);
            }
            else
            {
                simpleWordList.add((Word) word);
            }
        }

        return simpleWordList;
    }

    /**
     * 获取简单的句子列表，其中复合词会被拆分为简单词
     *
     * @return
     */
    public List<List<Word>> getSimpleSentenceList()
    {
        List<List<Word>> simpleList = new ();
        foreach (Sentence sentence in sentenceList)
        {
            List<Word> wordList = new ();
            foreach (IWord word in sentence.wordList)
            {
                if (word is CompoundWord)
                {
                    foreach (Word inner in ((CompoundWord) word).innerList)
                    {
                        wordList.Add(inner);
                    }
                }
                else
                {
                    wordList.Add((Word) word);
                }
            }
            simpleList.Add(wordList);
        }

        return simpleList;
    }

    /**
     * 获取复杂句子列表，句子中的每个单词有可能是复合词，有可能是简单词
     *
     * @return
     */
    public List<List<IWord>> getComplexSentenceList()
    {
        List<List<IWord>> complexList = new ();
        for (Sentence sentence in sentenceList)
        {
            complexList.add(sentence.wordList);
        }

        return complexList;
    }

    /**
     * 获取简单的句子列表
     *
     * @param spilt 如果为真，其中复合词会被拆分为简单词
     * @return
     */
    public List<List<Word>> getSimpleSentenceList(bool spilt)
    {
        List<List<Word>> simpleList = new ();
        foreach (Sentence sentence in sentenceList)
        {
            List<Word> wordList = new ();
            for (IWord word in sentence.wordList)
            {
                if (word is CompoundWord)
                {
                    if (spilt)
                    {
                        for (Word inner in ((CompoundWord) word).innerList)
                        {
                            wordList.add(inner);
                        }
                    }
                    else
                    {
                        wordList.add(((CompoundWord) word).toWord());
                    }
                }
                else
                {
                    wordList.add((Word) word);
                }
            }
            simpleList.add(wordList);
        }

        return simpleList;
    }

    /**
     * 获取简单的句子列表，其中复合词的标签如果是set中指定的话会被拆分为简单词
     *
     * @param labelSet
     * @return
     */
    public List<List<Word>> getSimpleSentenceList(HashSet<string> labelSet)
    {
        List<List<Word>> simpleList = new LinkedList<List<Word>>();
        for (Sentence sentence in sentenceList)
        {
            List<Word> wordList = new LinkedList<Word>();
            for (IWord word in sentence.wordList)
            {
                if (word is CompoundWord)
                {
                    if (labelSet.contains(word.getLabel()))
                    {
                        for (Word inner : ((CompoundWord) word).innerList)
                        {
                            wordList.add(inner);
                        }
                    }
                    else
                    {
                        wordList.add(((CompoundWord) word).toWord());
                    }
                }
                else
                {
                    wordList.add((Word) word);
                }
            }
            simpleList.add(wordList);
        }

        return simpleList;
    }

    //@Override
    public string toString()
    {
        StringBuilder sb = new StringBuilder();
        for (Sentence sentence in sentenceList)
        {
            sb.Append(sentence);
            sb.Append(' ');
        }
        if (sb.length() > 0) sb.deleteCharAt(sb.length() - 1);
        return sb.toString();
    }

    public static Document create(File file)
    {
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(file.getAbsolutePath());
        List<Sentence> sentenceList = new LinkedList<Sentence>();
        for (string line in lineIterator)
        {
            line = line.trim();
            if (line.isEmpty()) continue;
            Sentence sentence = Sentence.create(line);
            if (sentence == null)
            {
                logger.warning("使用 " + line + " 创建句子失败");
                return null;
            }
            sentenceList.add(sentence);
        }
        return new Document(sentenceList);
    }
}
