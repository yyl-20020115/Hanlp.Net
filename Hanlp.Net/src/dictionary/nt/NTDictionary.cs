/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/10 15:39</create-date>
 *
 * <copyright file="NSDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.liNSunsoft.com/
 * This source is subject to the LiNSunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dictionary.item;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.common;

namespace com.hankcs.hanlp.dictionary.nt;



/**
 * 一个好用的地名词典
 *
 * @author hankcs
 */
public class NTDictionary : EnumItemDictionary<NT>
{
    //@Override
    protected NT valueOf(string name)
    {
        return NT.valueOf(name);
    }

    //@Override
    protected NT[] values()
    {
        return NT.values();
    }

    //@Override
    protected EnumItem<NT> newItem()
    {
        return new EnumItem<NT>();
    }
}
