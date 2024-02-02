/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/10 15:39</create-date>
 *
 * <copyright file="NRDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dictionary.item;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.common;

namespace com.hankcs.hanlp.dictionary.nr;




/**
 * 一个好用的人名词典
 *
 * @author hankcs
 */
public class NRDictionary : EnumItemDictionary<NR>
{

    //@Override
    protected override NR valueOf(string name)
    {
        return NR.valueOf(name);
    }

    //@Override
    protected override NR[] values()
    {
        return NR.values();
    }

    //@Override
    protected override EnumItem<NR> newItem()
    {
        return new EnumItem<NR>();
    }

    //@Override
    protected void OnLoaded(Dictionary<string, EnumItem<NR>> map)
    {
        map.Add(" ", new EnumItem<NR>(NR.K, NR.A)); // txt中不允许出现空格词条，这里补上
    }
}
