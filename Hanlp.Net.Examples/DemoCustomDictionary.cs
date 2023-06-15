/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/9 13:04</create-date>
 *
 * <copyright file="DemoCustomDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.dictionary;

namespace com.hankcs.demo;



/**
 * 演示用户词典的动态增删
 *
 * @author hankcs
 */
public class DemoCustomDictionary
{
    public static void Main(String[] args)
    {
        // 动态增加
        CustomDictionary.add("攻城狮");
        // 强行插入
        CustomDictionary.insert("白富美", "nz 1024");
        // 删除词语（注释掉试试）
        //        CustomDictionary.remove("攻城狮");
        Console.WriteLine(CustomDictionary.add("单身狗", "nz 1024 n 1"));
        Console.WriteLine(CustomDictionary.get("单身狗"));

        String text = "攻城狮逆袭单身狗，迎娶白富美，走上人生巅峰";  // 怎么可能噗哈哈！

        // DoubleArrayTrie分词
        char[] charArray = text.ToCharArray();
        CustomDictionary.parseText(charArray, new CT3());
        // 首字哈希之后二分的trie树分词
        var searcher = CustomDictionary.getSearcher(text);
        Map.Entry entry;
        while ((entry = searcher.next()) != null)
        {
            Console.WriteLine(entry);
        }

        // 标准分词
        Console.WriteLine(HanLP.segment(text));

        // Note:动态增删不会影响词典文件
        // 目前CustomDictionary使用DAT储存词典文件中的词语，用BinTrie储存动态加入的词语，前者性能高，后者性能低
        // 之所以保留动态增删功能，一方面是历史遗留特性，另一方面是调试用；未来可能会去掉动态增删特性。
    }
    public class CT3 : AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute>
    {

        public void hit(int begin, int end, CoreDictionary.Attribute value)
        {
            Console.WriteLine("[%d:%d]=%s %s\n", begin, end, new String(charArray, begin, end - begin), value);
        }
    }
}
