/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/2 12:08</create-date>
 *
 * <copyright file="UnEmptyStringSet.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.collection.set;


/**
 * 一个不接受空白的字符串set
 * @author hankcs
 */
public class UnEmptyStringSet : HashSet<string>
{
    //@Override
    public bool Add(string s)
    {
        if (s.Trim().Length == 0) return false;

        return base.Add(s);
    }
}
