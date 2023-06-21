/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/7 10:59</create-date>
 *
 * <copyright file="CharacterBasedGenerativeModelSegment.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.seg;



/**
 * 基于“由字构词”方法分词器基类
 * @author hankcs
 */
public abstract class CharacterBasedSegment : Segment
{

    /**
     * 查询或猜测一个词语的属性，
     * 先查词典，然后对字母、数字串的属性进行判断，最后猜测未登录词
     * @param term
     * @return
     */
    public static CoreDictionary.Attribute guessAttribute(Term term)
    {
        CoreDictionary.Attribute attribute = CoreDictionary.get(term.word);
        if (attribute == null)
        {
            attribute = CustomDictionary.get(term.word);
        }
        if (attribute == null)
        {
            if (term.nature != null)
            {
                if (Nature.nx == term.nature)
                    attribute = new CoreDictionary.Attribute(Nature.nx);
                else if (Nature.m == term.nature)
                    attribute = CoreDictionary.get(CoreDictionary.M_WORD_ID);
            }
            else if (term.word.Trim().Length == 0)
                attribute = new CoreDictionary.Attribute(Nature.x);
            else attribute = new CoreDictionary.Attribute(Nature.nz);
        }
        else term.nature = attribute.nature[0];
        return attribute;
    }


    /**
     * 以下方法用于纯分词模型
     * 分词、词性标注联合模型则直接重载segSentence
     */
    //@Override
    protected List<Term> segSentence(char[] sentence)
    {
        if (sentence.Length == 0) return new();
        List<Term> termList = roughSegSentence(sentence);
        if (!(config.ner || config.useCustomDictionary || config.speechTagging))
            return termList;
        List<Vertex> vertexList = toVertexList(termList, true);
        if (config.speechTagging)
        {
            Viterbi.compute(vertexList, CoreDictionaryTransformMatrixDictionary.transformMatrixDictionary);
            int i = 0;
            foreach (Term term in termList)
            {
                if (term.nature != null) term.nature = vertexList.get(i + 1).guessNature();
                ++i;
            }
        }
        if (config.useCustomDictionary)
        {
            combineByCustomDictionary(vertexList);
            termList = convert(vertexList, config.offset);
        }
        return termList;
    }

    /**
     * 单纯的分词模型实现该方法，仅输出词
     * @param sentence
     * @return
     */
    protected abstract List<Term> roughSegSentence(char[] sentence);

    /**
     * 将中间结果转换为词网顶点,
     * 这样就可以利用基于Vertex开发的功能, 如词性标注、NER等
     * @param wordList
     * @param appendStart
     * @return
     */
    protected List<Vertex> toVertexList(List<Term> wordList, bool appendStart)
    {
        var vertexList = new List<Vertex>(wordList.Count + 2);
        if (appendStart) vertexList.Add(Vertex.newB());
        foreach (Term word in wordList)
        {
            CoreDictionary.Attribute attribute = guessAttribute(word);
            Vertex vertex = new Vertex(word.word, attribute);
            vertexList.Add(vertex);
        }
        if (appendStart) vertexList.Add(Vertex.newE());
        return vertexList;
    }

}
