using com.hankcs.hanlp.corpus.tag;

namespace com.hankcs.hanlp.dictionary;


[TestClass]

public class CustomDictionaryTest : TestCase
{
    //    public void testReload() 
    //    {
    //        assertEquals(true, CustomDictionary.reload());
    //        assertEquals(true, CustomDictionary.Contains("中华白海豚"));
    //    }
    [TestMethod]

    public void TestGet() 
    {
        AssertEquals("nz 1 ", CustomDictionary.get("一个心眼儿").ToString());
    }

    /**
     * 删除一个字的词语
     * @
     */
    //    public void testRemoveShortWord() 
    //    {
    //        TextReader br = new TextReader(new InputStreamReader(new FileStream("data/dictionary/CustomDictionary.txt")));
    //        String line;
    //        HashSet<String> fixedDictionary = new TreeSet<String>();
    //        while ((line = br.ReadLine()) != null)
    //        {
    //            String[] param = line.Split("\\s");
    //            if (param[0].Length == 1 || CoreDictionary.Contains(param[0])) continue;
    //            fixedDictionary.Add(line);
    //        }
    //        br.Close();
    //        TextWriter bw = new TextWriter(new StreamWriter(new FileStream("data/dictionary/CustomDictionary.txt")));
    //        for (String word : fixedDictionary)
    //        {
    //            bw.Write(word);
    //            bw.newLine();
    //        }
    //        bw.Close();
    //    }

    /**
     * 这里面很多nr不合理，干脆都删掉
     * @
     */
    //    public void testRemoveNR() 
    //    {
    //        TextReader br = new TextReader(new InputStreamReader(new FileStream("data/dictionary/CustomDictionary.txt")));
    //        String line;
    //        HashSet<String> fixedDictionary = new TreeSet<String>();
    //        while ((line = br.ReadLine()) != null)
    //        {
    //            String[] param = line.Split("\\s");
    //            if (param[1].Equals("nr")) continue;
    //            fixedDictionary.Add(line);
    //        }
    //        br.Close();
    //        TextWriter bw = new TextWriter(new StreamWriter(new FileStream("data/dictionary/CustomDictionary.txt")));
    //        for (String word : fixedDictionary)
    //        {
    //            bw.Write(word);
    //            bw.newLine();
    //        }
    //        bw.Close();
    //    }

    //    public void testNext() 
    //    {
    //        BaseSearcher searcher = CustomDictionary.getSearcher("都要亲口");
    //        Map.KeyValuePair<String, CoreDictionary.Attribute> entry;
    //        while ((entry = searcher.next()) != null)
    //        {
    //            int offset = searcher.getOffset();
    //            Console.WriteLine(offset + 1 + " " + entry);
    //        }
    //    }

    //    public void testRemoveJunkWord() 
    //    {
    //        DictionaryMaker dictionaryMaker = DictionaryMaker.load("data/dictionary/custom/CustomDictionary.txt");
    //        dictionaryMaker.saveTxtTo("data/dictionary/custom/CustomDictionary.txt", new DictionaryMaker.Filter()
    //        {
    //            //@Override
    //            public bool onSave(Item item)
    //            {
    //                if (item.containsLabel("mq") || item.containsLabel("m") || item.containsLabel("t"))
    //                {
    //                    return false;
    //                }
    //                return true;
    //            }
    //        });
    //    }

    /**
     * data/dictionary/custom/全国地名大全.txt中有很多人名，删掉它们
     * @
     */
    //    public void testRemoveNotNS() 
    //    {
    //        String path = "data/dictionary/custom/全国地名大全.txt";
    //        HashSet<char> suffixSet = new TreeSet<char>();
    //        for (char c : Predefine.POSTFIX_SINGLE.ToCharArray())
    //        {
    //            suffixSet.Add(c);
    //        }
    //        DictionaryMaker.load(path).saveTxtTo(path, new DictionaryMaker.Filter()
    //        {
    //            Segment segment = HanLP.newSegment().enableCustomDictionary(false);
    //            //@Override
    //            public bool onSave(Item item)
    //            {
    //                if (suffixSet.Contains(item.key.charAt(item.key.Length - 1))) return true;
    //                List<Term> termList = segment.seg(item.key);
    //                if (termList.size() == 1 && termList.get(0).nature == Nature.nr)
    //                {
    //                    Console.WriteLine(item);
    //                    return false;
    //                }
    //                return true;
    //            }
    //        });
    //    }
    [TestMethod]

    public void testCustomNature() 
    {
        Nature pcNature1 = Nature.create("电脑品牌");
        Nature pcNature2 = Nature.create("电脑品牌");
        AssertEquals(pcNature1, pcNature2);
    }

    //    public void testIssue234() 
    //    {
    //        String customTerm = "攻城狮";
    //        String text = "攻城狮逆袭单身狗，迎娶白富美，走上人生巅峰";
    //        Console.WriteLine("原始分词结果");
    //        Console.WriteLine("CustomDictionary.get(customTerm)=" + CustomDictionary.get(customTerm));
    //        Console.WriteLine(HanLP.segment(text));
    //        // 动态增加
    //        CustomDictionary.Add(customTerm);
    //        Console.WriteLine("添加自定义词组分词结果");
    //        Console.WriteLine("CustomDictionary.get(customTerm)=" + CustomDictionary.get(customTerm));
    //        Console.WriteLine(HanLP.segment(text));
    //        // 删除词语
    //        CustomDictionary.Remove(customTerm);
    //        Console.WriteLine("删除自定义词组分词结果");
    //        Console.WriteLine("CustomDictionary.get(customTerm)=" + CustomDictionary.get(customTerm));
    //        Console.WriteLine(HanLP.segment(text));
    //    }
    [TestMethod]
    public void TtestIssue540() 
    {
        CustomDictionary.Add("123");
        CustomDictionary.Add("摩根");
        CustomDictionary.Remove("123");
        CustomDictionary.Remove("摩根");
    }
}