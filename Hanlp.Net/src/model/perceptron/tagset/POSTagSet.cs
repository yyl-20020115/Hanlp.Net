/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-26 下午5:49</create-date>
 *
 * <copyright file="POSTagSet.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron.tagset;


/**
 * 词性标注集
 * @author hankcs
 */
public class POSTagSet : TagSet
{
    public POSTagSet()
    {
        base(TaskType.POS);
    }
}
