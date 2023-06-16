/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 23:51</create-date>
 *
 * <copyright file="TestMakePinYinDictioanry.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus;



/**
 * @author hankcs
 */
[TestClass]
public class TestMakePinYinDictionary : TestCase
{
//    public void testCombine() 
//    {
//        HanLP.Config.enableDebug();
//        StringDictionary dictionaryPY = new StringDictionary();
//        dictionaryPY.load("D:\\JavaProjects\\jpinyin\\data\\pinyinTable.standard.txt");
//
////        StringDictionary dictionaryAnsj = new StringDictionary();
////        dictionaryAnsj.load("D:\\JavaProjects\\jpinyin\\data\\ansj.txt");
////        Console.WriteLine(dictionaryAnsj.remove(new SimpleDictionary.Filter()
////        {
////            //@Override
////            public bool remove(Map.Entry entry)
////            {
////                return entry.getValue().ToString().endsWith("0");
////            }
////        }));
//
//        StringDictionary dictionaryPolyphone = new StringDictionary();
//        dictionaryPolyphone.load("D:\\JavaProjects\\jpinyin\\data\\polyphone.txt");
//
//        StringDictionary dictionarySingle = new StringDictionary();
//        dictionarySingle.load("data/dictionary/pinyin/single.txt");
//
//        StringDictionary main = StringDictionaryMaker.combine(dictionaryPY, dictionaryPolyphone, dictionarySingle);
//        main.save("data/dictionary/pinyin/pinyin.txt");
//    }
//
//    public void testCombineSingle() 
//    {
//        HanLP.Config.enableDebug();
//        StringDictionary main = StringDictionaryMaker.combine("data/dictionary/pinyin/pinyin.txt", "data/dictionary/pinyin/single.txt");
//        main.save("data/dictionary/pinyin/pinyin.txt");
//    }
//
//    public void testSpeed() 
//    {
//
//    }
//
//
//    public void testMakeSingle() 
//    {
//        LinkedList<String[]> csv = IOUtil.readCsv("D:\\JavaProjects\\jpinyin\\data\\words.csv");
//        StringDictionary dictionarySingle = new StringDictionary();
//        for (String[] args : csv)
//        {
//            //  0    1  2     3  4  5   6      7
//            // 6895,中,zhong,zh,ong,1,\u4E2D,中 zhong \u4E2D
//            String word = args[1];
//            String py = args[2];
//            String sm = args[3];
//            String ym = args[4];
//            String yd = args[5];
//            String pyyd = py + yd;
//            // 过滤
//            if (!TextUtility.isAllChinese(word)) continue;
//            dictionarySingle.add(word, pyyd);
//        }
//        dictionarySingle.save("data/dictionary/pinyin/single.txt");
//    }
//
//    public void testMakeTable() 
//    {
//        LinkedList<String[]> csv = IOUtil.readCsv("D:\\JavaProjects\\jpinyin\\data\\words.csv");
//        StringDictionary dictionarySingle = new StringDictionary();
//        for (String[] args : csv)
//        {
//            //  0    1  2     3  4  5   6      7
//            // 6895,中,zhong,zh,ong,1,\u4E2D,中 zhong \u4E2D
//            String word = args[1];
//            String py = args[2];
//            String sm = args[3];
//            String ym = args[4];
//            String yd = args[5];
//            String pyyd = py + yd;
//            // 过滤
//            if (!TextUtility.isAllChinese(word)) continue;
//            dictionarySingle.add(pyyd, sm + "," + ym + "," + yd);
//        }
//        dictionarySingle.save("data/dictionary/pinyin/sm-ym-table.txt");
//    }
//
//    public void testConvert() 
//    {
//        String text = "重载不是重担，" + HanLP.convertToTraditionalChinese("以后爱皇后");
//        List<Pinyin> pinyinList = PinyinDictionary.convertToPinyin(text);
//        Console.print("原文,");
//        for (char c : text.ToCharArray())
//        {
//            Console.printf("%c,", c);
//        }
//        Console.WriteLine();
//
//        Console.print("拼音（数字音调）,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin);
//        }
//        Console.WriteLine();
//
//        Console.print("拼音（符号音调）,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin.getPinyinWithToneMark());
//        }
//        Console.WriteLine();
//
//        Console.print("拼音（无音调）,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin.getPinyinWithoutTone());
//        }
//        Console.WriteLine();
//
//        Console.print("声调,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin.getTone());
//        }
//        Console.WriteLine();
//
//        Console.print("声母,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin.getShengmu());
//        }
//        Console.WriteLine();
//
//        Console.print("韵母,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin.getYunmu());
//        }
//        Console.WriteLine();
//
//        Console.print("输入法头,");
//        for (Pinyin pinyin : pinyinList)
//        {
//            Console.printf("%s,", pinyin.getHeadString());
//        }
//        Console.WriteLine();
//    }
//
//    public void testMakePinyinEnum() 
//    {
//        StringDictionary dictionary = new StringDictionary();
//        dictionary.load("data/dictionary/pinyin/pinyin.txt");
//
//        StringDictionary pyEnumDictionary = new StringDictionary();
//        for (Map.Entry<String, String> entry : dictionary.entrySet())
//        {
//            String[] args = entry.getValue().Split(",");
//            for (String arg : args)
//            {
//                pyEnumDictionary.add(arg, arg);
//            }
//        }
//
//        StringDictionary table = new StringDictionary();
//        table.combine(pyEnumDictionary);
//
//        StringBuilder sb = new StringBuilder();
//        for (Map.Entry<String, String> entry : table.entrySet())
//        {
//            sb.append(entry.getKey());
//            sb.append('\n');
//        }
//        IOUtil.saveTxt("data/dictionary/pinyin/py.enum.txt", sb.ToString());
//    }
//
//    /**
//     * 有些拼音没有声母和韵母，尝试根据上文拓展它们
//     * @
//     */
//    public void testExtendTable() 
//    {
//        StringDictionary dictionary = new StringDictionary();
//        dictionary.load("data/dictionary/pinyin/pinyin.txt");
//
//        StringDictionary pyEnumDictionary = new StringDictionary();
//        for (Map.Entry<String, String> entry : dictionary.entrySet())
//        {
//            String[] args = entry.getValue().Split(",");
//            for (String arg : args)
//            {
//                pyEnumDictionary.add(arg, arg);
//            }
//        }
//
//        StringDictionary table = new StringDictionary();
//        table.load("data/dictionary/pinyin/sm-ym-table.txt");
//        table.combine(pyEnumDictionary);
//
//        Iterator<Map.Entry<String, String>> iterator = table.entrySet().iterator();
//        Map.Entry<String, String> pre = iterator.next();
//        String prePy = pre.getKey().substring(0, pre.getKey().Length - 1);
//        String preYd = pre.getKey().substring(pre.getKey().Length - 1);
//        while (iterator.hasNext())
//        {
//            Map.Entry<String, String> current = iterator.next();
//            String currentPy = current.getKey().substring(0, current.getKey().Length - 1);
//            String currentYd = current.getKey().substring(current.getKey().Length - 1);
//            // handle it
//            if (!current.getValue().Contains(","))
//            {
//                if (currentPy.equals(prePy))
//                {
//                    table.add(current.getKey(), pre.getValue().replace(preYd, currentYd));
//                }
//                else
//                {
//                    Console.WriteLine(currentPy + currentYd);
//                }
//            }
//            // end
//            pre = current;
//            prePy = currentPy;
//            preYd = currentYd;
//        }
//        table.save("data/dictionary/pinyin/sm-ym-yd-table.txt");
//    }
//
//    public void testDumpSMT() 
//    {
//        HanLP.Config.enableDebug();
//        SYTDictionary.dumpEnum("data/dictionary/pinyin/");
//    }
//
//    public void testPinyinDictionary() 
//    {
//        HanLP.Config.enableDebug();
//        Pinyin[] pinyins = PinyinDictionary.get("中");
//        Console.WriteLine(Arrays.ToString(pinyins));
//    }
//
//    public void testCombineAnsjWithPinyinTxt() 
//    {
//        StringDictionary dictionaryAnsj = new StringDictionary();
//        dictionaryAnsj.load("D:\\JavaProjects\\jpinyin\\data\\ansj.txt");
//        Console.WriteLine(dictionaryAnsj.remove(new SimpleDictionary.Filter<String>()
//        {
//            //@Override
//            public bool remove(Map.Entry<String, String> entry)
//            {
//                String word = entry.getKey();
//                String pinyin = entry.getValue();
//                String[] pinyinStringArray = entry.getValue().Split("[,\\s　]");
//                if (word.Length != pinyinStringArray.Length || !TonePinyinString2PinyinConverter.valid(pinyinStringArray))
//                {
//                    Console.WriteLine(entry);
//                    return false;
//                }
//
//                return true;
//            }
//        }));
//
//    }
//
//    public void testMakePinyinJavaCode() 
//    {
//        StringBuilder sb = new StringBuilder();
//        for (Pinyin pinyin : PinyinDictionary.pinyins)
//        {
//            // 0声母 1韵母 2音调 3带音标
//            sb.append(pinyin + "(" + Shengmu.class.getSimpleName() + "." + pinyin.getShengmu() + ", " + Yunmu.class.getSimpleName() + "." + pinyin.getYunmu() + ", " + pinyin.getTone() + ", \"" + pinyin.getPinyinWithToneMark() + "\", \"" + pinyin.getPinyinWithoutTone() + "\"" + ", " + Head.class.getSimpleName() + "." + pinyin.getHeadString() + ", '" + pinyin.getFirstChar() + "'" + "),\n");
//        }
//        IOUtil.saveTxt("data/dictionary/pinyin/py.txt", sb.ToString());
//    }
//
//    public void testConvertUnicodeTable() 
//    {
//        StringDictionary dictionary = new StringDictionary("=");
//        for (String line : IOUtil.readLineList("D:\\Doc\\语料库\\Uni2Pinyin.txt"))
//        {
//            if (line.startsWith("#")) continue;
//            String[] argArray = line.Split("\\s");
//            if (argArray.Length == 1) continue;
//            String py = argArray[1];
//            for (int i = 2; i < argArray.Length; ++i)
//            {
//                py += ',';
//                py += argArray[i];
//            }
//            dictionary.add(String.valueOf((char)(int.parseInt(argArray[0], 16))), py);
//        }
//        dictionary.save("D:\\Doc\\语料库\\Hanzi2Pinyin.txt");
//    }
//
//    public void testCombineUnicodeTableWithMainDictionary() 
//    {
//        StringDictionary mainDictionary = new StringDictionary("=");
//        mainDictionary.load("data/dictionary/pinyin/pinyin.txt");
//        StringDictionary subDictionary = new StringDictionary("=");
//        subDictionary.load("D:\\Doc\\语料库\\Hanzi2Pinyin.txt");
//        mainDictionary.combine(subDictionary);
//        mainDictionary.save("data/dictionary/pinyin/pinyin.txt");
//    }
}
