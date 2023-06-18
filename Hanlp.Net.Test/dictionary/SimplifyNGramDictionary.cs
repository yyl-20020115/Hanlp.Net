/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/7 21:06</create-date>
 *
 * <copyright file="SimplifyNGramDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dictionary;



/**
 * 有一些类似于 工程@学 1 的条目会干扰 工程学家 的识别，这类@后接短字符的可以过滤掉
 * @author hankcs
 */
[TestClass]

public class SimplifyNGramDictionary : TestCase
{
//    String path = "data/dictionary/CoreNatureDictionary.ngram.txt";
//    public void testSimplify() 
//    {
//        BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path)));
//        Dictionary<String, int> map = new Dictionary<String, int>();
//        String line;
//        while ((line = br.readLine()) != null)
//        {
//            String[] param = line.Split("\\s");
//            map.put(param[0], int.valueOf(param[1]));
//        }
//        br.close();
//        HashSet<Map.Entry<String, int>> entrySet = map.descendingMap().entrySet();
//        Iterator<Map.Entry<String, int>> iterator = entrySet.iterator();
//        // 第一步去包含
////        Map.Entry<String, int> pre = new AbstractMap.SimpleEntry<>(" @ ", 1);
////        while (iterator.hasNext())
////        {
////            Map.Entry<String, int> current = iterator.next();
////            if (current.getKey().Length - current.getKey().indexOf('@') == 2 && pre.getKey().indexOf(current.getKey()) == 0 && current.getValue() <= 2)
////            {
////                Console.WriteLine("应当删除 " + current + " 保留 " + pre);
////                iterator.Remove();
////            }
////            pre = current;
////        }
//        // 第二步，尝试移除“学@家”这样的短共现
////        iterator = entrySet.iterator();
////        while (iterator.hasNext())
////        {
////            Map.Entry<String, int> current = iterator.next();
////            if (current.getKey().Length == 3)
////            {
////                Console.WriteLine("应当删除 " + current);
////            }
////        }
//        // 第三步，对某些@后面的词语太短了，也移除
////        iterator = entrySet.iterator();
////        while (iterator.hasNext())
////        {
////            Map.Entry<String, int> current = iterator.next();
////            String[] termArray = current.getKey().Split("@", 2);
////            if (termArray[0].Equals("未##人") && termArray[1].Length < 2)
////            {
////                Console.WriteLine("删除 " + current.getKey());
////                iterator.Remove();
////            }
////        }
//        // 第四步，人名接续对识别产生太多误命中影响，也删除
////        iterator = entrySet.iterator();
////        while (iterator.hasNext())
////        {
////            Map.Entry<String, int> current = iterator.next();
////            if (current.getKey().Contains("未##人") && current.getValue() < 10)
////            {
////                Console.WriteLine("删除 " + current.getKey());
////                iterator.Remove();
////            }
////        }
//        // 对人名的终极调优
//        TFDictionary dictionary = new TFDictionary();
//        dictionary.load("D:\\JavaProjects\\HanLP\\data\\dictionary\\CoreNatureDictionary.ngram.mini.txt");
//        iterator = entrySet.iterator();
//        while (iterator.hasNext())
//        {
//            Map.Entry<String, int> current = iterator.next();
//            if (current.getKey().Contains("未##人") && dictionary.getFrequency(current.getKey()) < 10)
//            {
//                Console.WriteLine("删除 " + current.getKey());
//                iterator.Remove();
//            }
//        }
//        // 输出
//        BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(path)));
//        for (Map.Entry<String, int> entry : map.entrySet())
//        {
//            bw.write(entry.getKey());
//            bw.write(' ');
//            bw.write(String.valueOf(entry.getValue()));
//            bw.newLine();
//        }
//        bw.close();
//    }
//
//    /**
//     * 有些词条不在CoreDictionary里面，那就把它们删掉
//     * @
//     */
//    public void testLoseWeight() 
//    {
//        BufferedReader br = new BufferedReader(new InputStreamReader(new FileInputStream(path), "UTF-8"));
//        Dictionary<String, int> map = new Dictionary<String, int>();
//        String line;
//        while ((line = br.readLine()) != null)
//        {
//            String[] param = line.Split(" ");
//            map.put(param[0], int.valueOf(param[1]));
//        }
//        br.close();
//        Iterator<String> iterator = map.Keys.iterator();
//        while (iterator.hasNext())
//        {
//            line = iterator.next();
//            String[] params = line.Split("@", 2);
//            String one = params[0];
//            String two = params[1];
//            if (!CoreDictionary.Contains(one) || !CoreDictionary.Contains(two))
//                iterator.Remove();
//        }
//
//        // 输出
//        BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(path), "UTF-8"));
//        for (Map.Entry<String, int> entry : map.entrySet())
//        {
//            bw.write(entry.getKey());
//            bw.write(' ');
//            bw.write(String.valueOf(entry.getValue()));
//            bw.newLine();
//        }
//        bw.close();
//    }
}
