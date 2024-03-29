/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-05 PM7:56</create-date>
 *
 * <copyright file="AveragedPerceptronSegment.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * 感知机词法分析器，支持简繁全半角和大小写
 *
 * @author hankcs
 */
public class PerceptronLexicalAnalyzer : AbstractLexicalAnalyzer
{
    public PerceptronLexicalAnalyzer(PerceptronSegmenter segmenter)
        :base(segmenter)
    {
    }

    public PerceptronLexicalAnalyzer(PerceptronSegmenter segmenter, PerceptronPOSTagger posTagger)
        : base(segmenter, posTagger)
    {
        ;
    }

    public PerceptronLexicalAnalyzer(PerceptronSegmenter segmenter, PerceptronPOSTagger posTagger, PerceptronNERecognizer neRecognizer)
        : base(segmenter, posTagger, neRecognizer)
    {
    }

    public PerceptronLexicalAnalyzer(LinearModel cwsModel, LinearModel posModel, LinearModel nerModel)
    {
        segmenter = new PerceptronSegmenter(cwsModel);
        if (posModel != null)
        {
            this.posTagger = new PerceptronPOSTagger(posModel);
            config.speechTagging = true;
        }
        else
        {
            this.posTagger = null;
        }
        if (nerModel != null)
        {
            neRecognizer = new PerceptronNERecognizer(nerModel);
            config.ner = true;
        }
        else
        {
            neRecognizer = null;
        }
    }

    public PerceptronLexicalAnalyzer(string cwsModelFile, string posModelFile, string nerModelFile)
        : this(new LinearModel(cwsModelFile), posModelFile == null ? null : new LinearModel(posModelFile), nerModelFile == null ? null : new LinearModel(nerModelFile))
    {
       ;
    }

    public PerceptronLexicalAnalyzer(string cwsModelFile, string posModelFile) 
        : this(new LinearModel(cwsModelFile), posModelFile == null ? null : new LinearModel(posModelFile), null)
    {
        ;
    }

    public PerceptronLexicalAnalyzer(string cwsModelFile) 
        : this(new LinearModel(cwsModelFile), null, null)
    {
        ;
    }

    public PerceptronLexicalAnalyzer(LinearModel CWSModel)
        : this(CWSModel, null, null)
    {
        ;
    }

    /**
     * 加载配置文件指定的模型构造词法分析器
     *
     * @
     */
    public PerceptronLexicalAnalyzer() 
        : this(HanLP.Config.PerceptronCWSModelPath, HanLP.Config.PerceptronPOSModelPath, HanLP.Config.PerceptronNERModelPath)
    {
        ;
    }

    /**
     * 中文分词
     *
     * @param text
     * @param output
     */
    public void segment(string text, List<string> output)
    {
        string normalized = CharTable.convert(text);
        Segment(text, normalized, output);
    }

    /**
     * 词性标注
     *
     * @param wordList
     * @return
     */
    public string[] partOfSpeechTag(List<string> wordList)
    {
        if (posTagger == null)
        {
            throw new ArgumentException("未提供词性标注模型");
        }
        return Tag(wordList);
    }

    /**
     * 命名实体识别
     *
     * @param wordArray
     * @param posArray
     * @return
     */
    public string[] namedEntityRecognize(string[] wordArray, string[] posArray)
    {
        if (neRecognizer == null)
        {
            throw new ArgumentException("未提供命名实体识别模型");
        }
        return Recognize(wordArray, posArray);
    }

    /**
     * 在线学习
     *
     * @param segmentedTaggedSentence 已分词、标好词性和命名实体的人民日报2014格式的句子
     * @return 是否学习成果（失败的原因是句子格式不合法）
     */
    public bool learn(string segmentedTaggedSentence)
    {
        Sentence sentence = Sentence.create(segmentedTaggedSentence);
        return learn(sentence);
    }

    /**
     * 在线学习
     *
     * @param sentence 已分词、标好词性和命名实体的人民日报2014格式的句子
     * @return 是否学习成果（失败的原因是句子格式不合法）
     */
    public bool learn(Sentence sentence)
    {
        CharTable.normalize(sentence);
        if (!getPerceptronSegmenter().learn(sentence)) return false;
        if (posTagger != null && !getPerceptronPOSTagger().learn(sentence)) return false;
        if (neRecognizer != null && !getPerceptionNERecognizer().learn(sentence)) return false;
        return true;
    }

    /**
     * 获取分词器
     *
     * @return
     */
    public PerceptronSegmenter getPerceptronSegmenter()
    {
        return (PerceptronSegmenter) segmenter;
    }

    /**
     * 获取词性标注器
     *
     * @return
     */
    public PerceptronPOSTagger getPerceptronPOSTagger()
    {
        return (PerceptronPOSTagger) posTagger;
    }

    /**
     * 获取命名实体识别器
     *
     * @return
     */
    public PerceptronNERecognizer getPerceptionNERecognizer()
    {
        return (PerceptronNERecognizer) neRecognizer;
    }

}