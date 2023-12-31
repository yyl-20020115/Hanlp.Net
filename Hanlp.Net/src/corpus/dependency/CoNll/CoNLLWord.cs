/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 12:38</create-date>
 *
 * <copyright file="CoNLLWord.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using System.Text;

namespace com.hankcs.hanlp.corpus.dependency.CoNll;

/**
 * @author hankcs
 */
public class CoNLLWord
{
    /**
     * ID	当前词在句子中的序号，１开始.
     */
    public int ID;
    /**
     * 当前词语（或标点）的原型或词干，在中文中，此列与FORM相同
     */
    public string LEMMA;
    /**
     * 当前词语的词性（粗粒度）
     */
    public string CPOSTAG;
    /**
     * 当前词语的词性（细粒度）
     */
    public string POSTAG;
    /**
     * 当前词语的中心词
     */
    public CoNLLWord HEAD;
    /**
     * 当前词语与中心词的依存关系
     */
    public string DEPREL;

    /**
     * 等效字符串
     */
    public string NAME;

    /**
     * 根节点
     */
    public static readonly CoNLLWord ROOT = new CoNLLWord(0, "##核心##", "ROOT", "root");
    /**
     * 空白节点，用于描述下标超出word数组的词语
     */
    public static readonly CoNLLWord NULL = new CoNLLWord(-1, "##空白##", "NULL", "null");

    /**
     *
     * @param ID 当前词在句子中的序号，１开始.
     * @param LEMMA 当前词语（或标点）的原型或词干，在中文中，此列与FORM相同
     * @param POSTAG 当前词语的词性（细粒度）
     */
    public CoNLLWord(int ID, string LEMMA, string POSTAG)
    {
        this.ID = ID;
        this.LEMMA = LEMMA;
        this.CPOSTAG = POSTAG.substring(0, 1);   // 取首字母作为粗粒度词性
        this.POSTAG = POSTAG;
        compile();
    }

    /**
     *
     * @param ID 当前词在句子中的序号，１开始.
     * @param LEMMA 当前词语（或标点）的原型或词干，在中文中，此列与FORM相同
     * @param CPOSTAG 当前词语的词性（粗粒度）
     * @param POSTAG 当前词语的词性（细粒度）
     */
    public CoNLLWord(int ID, string LEMMA, string CPOSTAG, string POSTAG)
    {
        this.ID = ID;
        this.LEMMA = LEMMA;
        this.CPOSTAG = CPOSTAG;
        this.POSTAG = POSTAG;
        compile();
    }

    private void compile()
    {
        this.NAME = PosTagCompiler.compile(POSTAG, LEMMA);
    }

    public CoNLLWord(CoNllLine line)
    {
        LEMMA = line.value[2];
        CPOSTAG = line.value[3];
        POSTAG = line.value[4];
        DEPREL = line.value[7];
        ID = line.id;
        compile();
    }

    public CoNLLWord(CoNllLine[] lineArray, int index)
    {
        this(lineArray[index]);
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        // ID为0时为根节点，ID为-1时为空白节点
        if (ID!=0 && ID!=-1){
            sb.Append(ID).Append('\t').Append(LEMMA).Append('\t').Append(LEMMA).Append('\t').Append(CPOSTAG).Append('\t')
                .Append(POSTAG).Append('\t').Append('_').Append('\t').Append(HEAD.ID).Append('\t').Append(DEPREL).Append('\t')
                .Append('_').Append('\t').Append('_');  
        } else {
            sb.Append(ID).Append('\t').Append(LEMMA).Append('\t').Append(LEMMA).Append('\t').Append(CPOSTAG).Append('\t')
                .Append(POSTAG).Append('\t').Append('_').Append('\t').Append('_').Append('\t').Append(DEPREL).Append('\t')
                .Append('_').Append('\t').Append('_');  
        }
        return sb.ToString();
    }
}
