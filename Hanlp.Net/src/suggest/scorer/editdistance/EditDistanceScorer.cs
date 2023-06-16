/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/5 17:06</create-date>
 *
 * <copyright file="EditDistanceScorer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.suggest.scorer.editdistance;


/**
 * 编辑距离打分器
 * @author hankcs
 */
public class EditDistanceScorer : BaseScorer<CharArray>
{
    //@Override
    protected CharArray generateKey(string sentence)
    {
        char[] charArray = sentence.ToCharArray();
        if (charArray.Length == 0) return null;
        return new CharArray(charArray);
    }
}
