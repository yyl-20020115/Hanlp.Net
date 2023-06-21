/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 19:46</create-date>
 *
 * <copyright file="TestJianFanDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus;



/**
 * @author hankcs
 */
[TestClass]
public class TestJianFanDictionaryMaker : TestCase
{

    private String cc = "/Users/hankcs/CppProjects/OpenCC/data/dictionary/";
    private String root = "data/dictionary/tc/";
    [TestMethod]

    public void TestCombine() 
    {
//        StringDictionary dictionaryHanLP = new StringDictionary("=");
//        dictionaryHanLP.load(HanLP.Config.t2sDictionaryPath);
//
//        StringDictionary dictionaryOuter = new StringDictionary("=");
//        dictionaryOuter.load("D:\\Doc\\语料库\\简繁分歧词表.txt");
//
//        for (Map.KeyValuePair<String, String> entry : dictionaryOuter.entrySet())
//        {
//            String t = entry.Key;
//            String s = entry.Value;
//            if (t.Length == 1) continue;
//            if (HanLP.convertToTraditionalChinese(s).Equals(t)) continue;
//            dictionaryHanLP.Add(t, s);
//        }
//
//        dictionaryHanLP.save(HanLP.Config.t2sDictionaryPath);
    }

//    public void testConvertSingle() 
//    {
//        Console.WriteLine(HanLP.convertToTraditionalChinese("一个劲"));
//    }
//
//    public void testIssue() 
//    {
//        Console.WriteLine(HanLP.convertToSimplifiedChinese("缐"));
//        Console.WriteLine(CharTable.convert("缐"));
//    }
//
//    public void testImportOpenCC() 
//    {
//        // 转换OpenCC的词库
//        Map<String, String> s2t = new Dictionary<String, String>();
//        combine("\t", s2t, cc + "STCharacters.txt",
//                cc + "STPhrases.txt"
//        );
//        save(s2t, "data/dictionary/tc/s2t.txt");
//        Map<String, String> t2s = new Dictionary<String, String>();
//        combine("=", t2s, "data/dictionary/tc/TraditionalChinese.txt");
//        combine("\t", t2s, cc + "TSCharacters.txt",
//                cc + "TSPhrases.txt"
//        );
//        save(t2s, "data/dictionary/tc/t2s.txt");
//    }
//
//    public void testMakeHK() 
//    {
//        Map<String, String> t2hk = new Dictionary<String, String>();
//        combine("\t", t2hk,
//                cc + "HKVariantsPhrases.txt",
//                cc + "HKVariants.txt"
//                );
//        save(t2hk, "data/dictionary/tc/t2hk.txt");
//    }
//
//    public void testMakeTW() 
//    {
//        Map<String, String> t2tw = new Dictionary<String, String>();
//        combine("\t", t2tw,
//                cc + "TWPhrasesIT.txt",
//                cc + "TWPhrasesName.txt",
//                cc + "TWPhrasesOther.txt",
//                cc + "TWVariants.txt"
//        );
//        save(t2tw, "data/dictionary/tc/t2tw.txt");
//    }
//
//    private void save(Map<String, String> storage, String path) 
//    {
//        TextWriter bw = IOUtil.newBufferedWriter(path);
//        for (Map.KeyValuePair<String, String> entry : storage.entrySet())
//        {
//            String line = entry.ToString();
//            int firstBlank = line.IndexOf(' ');
//            if (firstBlank != -1)
//            {
//                line = line.substring(0, firstBlank);
//            }
//            bw.Write(line);
//            bw.newLine();
//        }
//        bw.Close();
//    }
//
//    private Map<String, HashSet<String>> combine(Map<String, String> s2t, Map<String, String> t2s)
//    {
//        Map<String, HashSet<String>> all = new Dictionary<String, HashSet<String>>();
//        for (Map.KeyValuePair<String, String> entry : s2t.entrySet())
//        {
//            String key = entry.Key;
//            HashSet<String> value = all.get(key);
//            if (value == null)
//            {
//                value = new TreeSet<String>();
//                all.Add(key, value);
//            }
//            for (String v : entry.Value.Split(" "))
//            {
//                if (key.Length == 1 && key.Equals(v))
//                {
//                    continue;
//                }
//                value.Add(v);
//            }
//        }
//
//        for (Map.KeyValuePair<String, String> entry : t2s.entrySet())
//        {
//            for (String key : entry.Value.Split(" "))
//            {
//                if (key.Length == 1 && key.Equals(entry.Key))
//                {
//                    continue;
//                }
//                HashSet<String> value = all.get(key);
//                if (value == null)
//                {
//                    value = new TreeSet<String>();
//                    all.Add(key, value);
//                }
//
//                value.Add(entry.Key);
//            }
//        }
//
//        return all;
//    }
//
//    private Map<String, String> combine(String delimiter, Map<String, String> storage, String... pathArray)
//    {
//        for (String path : pathArray)
//        {
//            IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(path);
//            while (lineIterator.MoveNext())
//            {
//                String line = lineIterator.next();
//                String[] args = line.Split(delimiter);
//                if (args.Length != 2)
//                {
//                    Console.Error.WriteLine(line);
//                    Environment.Exit(-1);
//                }
//                storage.Add(args[0], args[1]);
//            }
//        }
//
//        return storage;
//    }
//
//    public void testChar() 
//    {
//        String line = "㐹\t㑶 㐹";
//        Console.WriteLine('㐹' == '㐹');
//    }
}
