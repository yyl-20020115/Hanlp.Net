/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 17:29</create-date>
 *
 * <copyright file="AbstractDependencyParser.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dependency.CoNll;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.tokenizer;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dependency;



/**
 * @author hankcs
 */
public abstract class AbstractDependencyParser : IDependencyParser
{
    /**
     * 本Parser使用的分词器，可以自由替换
     */
    private Segment segment;
    /**
     * 依存关系映射表（可以将英文标签映射为中文）
     */
    private Dictionary<string, string> deprelTranslater;
    /**
     * 是否自动转换依存关系
     */
    private bool enableDeprelTranslater;

    public AbstractDependencyParser(Segment segment)
    {
        this.segment = segment;
    }

    public AbstractDependencyParser()
        : this(NLPTokenizer.ANALYZER)
    {
       
    }

    //@Override
    public CoNLLSentence Parse(string sentence)
    {
        //assert sentence != null;
        CoNLLSentence output = Parse(segment.seg(sentence.ToCharArray()));
        if (enableDeprelTranslater && deprelTranslater != null)
        {
            foreach (CoNLLWord word in output)
            {
                string translatedDeprel = deprelTranslater[(word.DEPREL)];
                word.DEPREL = translatedDeprel;
            }
        }
        return output;
    }

    //@Override
    public Segment GetSegment()
    {
        return segment;
    }

    //@Override
    public IDependencyParser SetSegment(Segment segment)
    {
        this.segment = segment;
        return this;
    }

    //@Override
    public Dictionary<string, string> GetDeprelTranslator()
    {
        return deprelTranslater;
    }

    //@Override
    public IDependencyParser SetDeprelTranslator(Dictionary<string, string> deprelTranslator)
    {
        this.deprelTranslater = deprelTranslator;
        return this;
    }

    /**
     * 设置映射表
     * @param deprelTranslatorPath 映射表路径
     * @return
     */
    public IDependencyParser setDeprelTranslater(string deprelTranslatorPath)
    {
        deprelTranslater = GlobalObjectPool.get(deprelTranslatorPath);
        if (deprelTranslater != null) return this;

        IOUtil.LineIterator iterator = new IOUtil.LineIterator(deprelTranslatorPath);
        deprelTranslater = new ();
        while (iterator.MoveNext())
        {
            string[] args = iterator.next().Split("\\s");
            deprelTranslater.Add(args[0], args[1]);
        }
        if (deprelTranslater.Count == 0)
        {
            deprelTranslater = null;
        }
        GlobalObjectPool.Add(deprelTranslatorPath, deprelTranslater);

        return this;
    }

    //@Override
    public IDependencyParser EnableDeprelTranslator(bool enable)
    {
        enableDeprelTranslater = enable;
        return this;
    }
}