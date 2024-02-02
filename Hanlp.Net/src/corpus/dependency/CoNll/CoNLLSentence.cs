/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 11:05</create-date>
 *
 * <copyright file="CoNLLSentence.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.corpus.dependency.CoNll;


/**
 * CoNLL中的一个句子
 * @author hankcs
 */
public class CoNLLSentence : IEnumerable<CoNLLWord>
{
    /**
     * 有许多行，每行是一个单词
     */
    public CoNLLWord[] word;

    /**
     * 构造一个句子
     * @param lineList
     */
    public CoNLLSentence(List<CoNllLine> lineList)
    {
        CoNllLine[] lineArray = lineList.ToArray();
        this.word = new CoNLLWord[lineList.Count];
        int i = 0;
        foreach(CoNllLine line in lineList)
        {
            word[i++] = new CoNLLWord(line);
        }
        foreach (CoNLLWord nllWord in word)
        {
            int head = int.parseInt(lineArray[nllWord.ID - 1].value[6]) - 1;
            if (head != -1)
            {
                nllWord.HEAD = word[head];
            }
            else
            {
                nllWord.HEAD = CoNLLWord.ROOT;
            }
        }
    }

    public CoNLLSentence(CoNLLWord[] word)
    {
        this.word = word;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(word.Length * 50);
        foreach (CoNLLWord word in this.word)
        {
            sb.Append(word);
            sb.Append('\n');
        }
        return sb.ToString();
    }

    /**
     * 获取边的列表，edge[i][j]表示id为i的词语与j存在一条依存关系为该值的边，否则为null
     * @return
     */
    public string[][] GetEdgeArray()
    {
        string[][] edge = new string[word.Length + 1][word.Length + 1];
        foreach (CoNLLWord coNLLWord in word)
        {
            edge[coNLLWord.ID][coNLLWord.HEAD.ID] = coNLLWord.DEPREL;
        }

        return edge;
    }

    /**
     * 获取包含根节点在内的单词数组
     * @return
     */
    public CoNLLWord[] getWordArrayWithRoot()
    {
        CoNLLWord[] wordArray = new CoNLLWord[word.Length + 1];
        wordArray[0] = CoNLLWord.ROOT;
        Array.Copy(word, 0, wordArray, 1, word.Length);

        return wordArray;
    }

    public CoNLLWord[] getWordArray()
    {
        return word;
    }

    //@Override
    public IEnumerator<CoNLLWord> GetEnumerator()
    {
        return new IT();
    }

    public class IT: IEnumerator<CoNLLWord>
    {
        int index;
        //@Override
        public bool MoveNext()
        {
            return index < word.Length;
        }

        //@Override
        public CoNLLWord next()
        {
            return word[index++];
        }

        //@Override
        public void Remove()
        {
            throw new InvalidOperationException("CoNLLSentence是只读对象，不允许删除");
        }
    }
}
