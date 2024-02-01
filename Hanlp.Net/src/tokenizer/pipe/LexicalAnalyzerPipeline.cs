/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-11-10 10:23 AM</create-date>
 *
 * <copyright file="PipelineLexicalAnalyzer.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * See LICENSE file in the project root for full license information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.tokenizer.pipe;



/**
 * 流水线式词法分析器
 * @author hankcs
 */
public class LexicalAnalyzerPipeline : Pipeline<string, List<IWord>, List<IWord>>, LexicalAnalyzer
{
    public LexicalAnalyzerPipeline(Pipe<string, List<IWord>> first, Pipe<List<IWord>, List<IWord>> last)
        : base(first, last)
    {

    }

    public LexicalAnalyzerPipeline(LexicalAnalyzer analyzer)
        : this(new LexicalAnalyzerPipe(analyzer))
    {
        ;
    }

    public LexicalAnalyzerPipeline(LexicalAnalyzerPipe analyzer)
        : this(new PL(), new PW())
    {
        Add(analyzer);
    }

    public class PL : Pipe<string, List<IWord>>
    {
        //@Override
        public List<IWord> flow(string input)
        {
            List<IWord> output = new();
            output.Add(new Word(input, null));
            return output;
        }
    }
    public class PW : Pipe<List<IWord>, List<IWord>>
    {
        //@Override
        public List<IWord> flow(List<IWord> input)
        {
            return input;
        }
    }

    /**
     * 获取代理的词法分析器
     *
     * @return
     */
    public LexicalAnalyzer getAnalyzer()
    {
        foreach (Pipe<List<IWord>, List<IWord>> pipe in this.GetEnumerator())
        {
            if (pipe is LexicalAnalyzerPipe)
            {
                return ((LexicalAnalyzerPipe)pipe).analyzer;
            }
        }
        return null;
    }

    //@Override
    public void segment(string sentence, string normalized, List<string> wordList)
    {
        LexicalAnalyzer analyzer = getAnalyzer();
        if (analyzer == null)
            throw new InvalidOperationException("流水线中没有LexicalAnalyzerPipe");
        analyzer.segment(sentence, normalized, wordList);
    }

    //@Override
    public List<string> segment(string sentence)
    {
        LexicalAnalyzer analyzer = getAnalyzer();
        if (analyzer == null)
            throw new InvalidOperationException("流水线中没有LexicalAnalyzerPipe");
        return analyzer.segment(sentence);
    }

    //@Override
    public string[] recognize(string[] wordArray, string[] posArray)
    {
        LexicalAnalyzer analyzer = getAnalyzer();
        if (analyzer == null)
            throw new InvalidOperationException("流水线中没有LexicalAnalyzerPipe");
        return analyzer.recognize(wordArray, posArray);
    }

    //@Override
    public string[] tag(params string[] words)
    {
        LexicalAnalyzer analyzer = getAnalyzer();
        if (analyzer == null)
            throw new InvalidOperationException("流水线中没有LexicalAnalyzerPipe");
        return analyzer.tag(words);
    }

    //@Override
    public string[] tag(List<string> wordList)
    {
        LexicalAnalyzer analyzer = getAnalyzer();
        if (analyzer == null)
            throw new InvalidOperationException("流水线中没有LexicalAnalyzerPipe");
        return analyzer.tag(wordList);
    }

    //@Override
    public NERTagSet getNERTagSet()
    {
        LexicalAnalyzer analyzer = getAnalyzer();
        if (analyzer == null)
            throw new InvalidOperationException("流水线中没有LexicalAnalyzerPipe");
        return analyzer.getNERTagSet();
    }

    //@Override
    public Sentence analyze(string sentence)
    {
        return new Sentence(flow(sentence));
    }
}
