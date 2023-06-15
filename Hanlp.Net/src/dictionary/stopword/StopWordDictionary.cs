/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/15 19:38</create-date>
 *
 * <copyright file="StopwordDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dictionary.stopword;




/**
 * @author hankcs
 */
public class StopWordDictionary : MDAGSet : Filter
{
    public StopWordDictionary(File file) 
    {
        super(file);
    }

    public StopWordDictionary(Collection<string> strCollection)
    {
        super(strCollection);
    }

    public StopWordDictionary()
    {
    }

    public StopWordDictionary(string stopWordDictionaryPath) 
    {
        super(stopWordDictionaryPath);
    }

    //@Override
    public bool shouldInclude(Term term)
    {
        return contains(term.word);
    }
}
