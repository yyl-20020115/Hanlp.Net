/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/13 18:36</create-date>
 *
 * <copyright file="DemoRewriteDocument.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
namespace com.hankcs.demo;


/**
 * @author hankcs
 */
public class DemoRewriteText
{
    public static void Main(String[] args)
    {
        String text = "这个方法可以利用同义词词典将一段文本改写成意思相似的另一段文本，而且差不多符合语法";
        Console.WriteLine(CoreSynonymDictionary.rewrite(text));
    }
}
