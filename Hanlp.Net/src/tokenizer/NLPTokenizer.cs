/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 20:20</create-date>
 *
 * <copyright file="NLPTokenizer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.tokenizer;



/**
 * 可供自然语言处理用的分词器，更重视准确率。
 *
 * @author hankcs
 */
public class NLPTokenizer
{
    /**
     * 预置分词器
     */
    public static AbstractLexicalAnalyzer ANALYZER;

    static NLPTokenizer()
    {
        try
        {
            // 目前感知机的效果相当不错，如果能在更大的语料库上训练就更好了
            ANALYZER = new PerceptronLexicalAnalyzer();
        }
        catch (IOException e)
        {
            throw new Exception(e.Message,e);
        }
    }

    public static List<Term> segment(string text)
    {
        return ANALYZER.seg(text);
    }

    /**
     * 分词
     *
     * @param text 文本
     * @return 分词结果
     */
    public static List<Term> segment(char[] text)
    {
        return ANALYZER.seg(text);
    }

    /**
     * 切分为句子形式
     *
     * @param text 文本
     * @return 句子列表
     */
    public static List<List<Term>> seg2sentence(string text)
    {
        return ANALYZER.seg2sentence(text);
    }

    /**
     * 词法分析
     *
     * @param sentence
     * @return 结构化句子
     */
    public static Sentence analyze(string sentence)
    {
        return ANALYZER.analyze(sentence);
    }

    /**
     * 分词断句 输出句子形式
     *
     * @param text     待分词句子
     * @param shortest 是否断句为最细的子句（将逗号也视作分隔符）
     * @return 句子列表，每个句子由一个单词列表组成
     */
    public List<List<Term>> seg2sentence(string text, bool shortest)
    {
        return ANALYZER.seg2sentence(text, shortest);
    }
}
