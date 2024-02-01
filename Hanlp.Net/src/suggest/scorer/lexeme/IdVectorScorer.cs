/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/5 15:49</create-date>
 *
 * <copyright file="IdVectorScorer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.suggest.scorer.lexeme;


/**
 * 单词语义向量打分器
 * @author hankcs
 */
public class IdVectorScorer : BaseScorer<IdVector>
{
    //@Override
    protected override IdVector generateKey(string sentence)
    {
        IdVector idVector = new IdVector(sentence);
        if (idVector.idArrayList.Count == 0) return null;
        return idVector;
    }
}
