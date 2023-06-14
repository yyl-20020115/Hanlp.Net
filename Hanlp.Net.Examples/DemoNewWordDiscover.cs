/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-11-02 11:21</create-date>
 *
 * <copyright file="Demo.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.demo;



/**
 * 词语提取、新词发现
 *
 * @author hankcs
 */
public class DemoNewWordDiscover
{
    static readonly string CORPUS_PATH = TestUtility.ensureTestData("红楼梦.txt", "http://hanlp.linrunsoft.com/release/corpus/红楼梦.zip");

    public static void Main(String[] args) 
    {
        // 文本长度越大越好，试试红楼梦？
        List<WordInfo> wordInfoList = HanLP.extractWords(IOUtil.newBufferedReader(CORPUS_PATH), 100);
        Console.WriteLine(wordInfoList);
    }
}
