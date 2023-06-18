/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 12:27</create-date>
 *
 * <copyright file="WordNatureWeightScorer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dependency.CoNll;
using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.corpus.dependency.model;



/**
 * 生成模型打分器模型构建工具
 *
 * @author hankcs
 */
public class WordNatureWeightModelMaker
{
    public static bool makeModel(string corpusLoadPath, string modelSavePath)
    {
        HashSet<string> posSet = new ();
        DictionaryMaker dictionaryMaker = new DictionaryMaker();
        foreach (CoNLLSentence sentence in CoNLLLoader.loadSentenceList(corpusLoadPath))
        {
            foreach (CoNLLWord word in sentence.word)
            {
                addPair(word.NAME, word.HEAD.NAME, word.DEPREL, dictionaryMaker);
                addPair(word.NAME, wrapTag(word.HEAD.POSTAG ), word.DEPREL, dictionaryMaker);
                addPair(wrapTag(word.POSTAG), word.HEAD.NAME, word.DEPREL, dictionaryMaker);
                addPair(wrapTag(word.POSTAG), wrapTag(word.HEAD.POSTAG), word.DEPREL, dictionaryMaker);
                posSet.Add(word.POSTAG);
            }
        }
        foreach (CoNLLSentence sentence in CoNLLLoader.loadSentenceList(corpusLoadPath))
        {
            foreach (CoNLLWord word in sentence.word)
            {
                addPair(word.NAME, word.HEAD.NAME, word.DEPREL, dictionaryMaker);
                addPair(word.NAME, wrapTag(word.HEAD.POSTAG ), word.DEPREL, dictionaryMaker);
                addPair(wrapTag(word.POSTAG), word.HEAD.NAME, word.DEPREL, dictionaryMaker);
                addPair(wrapTag(word.POSTAG), wrapTag(word.HEAD.POSTAG), word.DEPREL, dictionaryMaker);
                posSet.Add(word.POSTAG);
            }
        }
        StringBuilder sb = new StringBuilder();
        foreach (string pos in posSet)
        {
            sb.Append("case \"" + pos + "\":\n");
        }
        IOUtil.saveTxt("data/model/dependency/pos-thu.txt", sb.ToString());
        return dictionaryMaker.saveTxtTo(modelSavePath);
    }

    private static void addPair(string from, string to, string label, DictionaryMaker dictionaryMaker)
    {
        dictionaryMaker.Add(new Word(from + "@" + to, label));
        dictionaryMaker.Add(new Word(from + "@", "频次"));
    }

    /**
     * 用尖括号将标签包起来
     * @param tag
     * @return
     */
    public static string wrapTag(string tag)
    {
        return "<" + tag + ">";
    }
}
