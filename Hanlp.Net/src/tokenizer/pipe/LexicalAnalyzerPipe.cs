/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-11-10 10:36 AM</create-date>
 *
 * <copyright file="PipeLexicalAnalyzer.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * See LICENSE file in the project root for full license information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.tokenizer.pipe;



/**
 * 词法分析器管道。约定将IWord的label设为非null表示本级管道已经处理
 *
 * @author hankcs
 */
public class LexicalAnalyzerPipe : Pipe<List<IWord>, List<IWord>>
{
    /**
     * 代理的词法分析器
     */
    public LexicalAnalyzer analyzer;

    public LexicalAnalyzerPipe(LexicalAnalyzer analyzer)
    {
        this.analyzer = analyzer;
    }

    //@Override
    public List<IWord> flow(List<IWord> input)
    {
        IEnumerator<IWord> listIterator = input.GetEnumerator();
        while (listIterator.MoveNext())
        {
            IWord wordOrSentence = listIterator.next();
            if (wordOrSentence.getLabel() != null)
                continue; // 这是别的管道已经处理过的单词，跳过
            listIterator.Remove(); // 否则是句子
            string sentence = wordOrSentence.Value;
            foreach (IWord word in analyzer.analyze(sentence))
            {
                listIterator.Add(word);
            }
        }
        return input;
    }
}
