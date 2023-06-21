using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dictionary;

namespace com.hankcs.hanlp.collection.trie;



[TestClass]
public class DoubleArrayTrieTest : TestCase
{
    [TestMethod]

    public void TestDatFromFile()
    {
        var map = new Dictionary<String, String>();
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/CoreNatureDictionary.mini.txt");
        while (iterator.MoveNext())
        {
            String line = iterator.next();
            map.Add(line, line);
        }
        DoubleArrayTrie<String> trie = new DoubleArrayTrie<String>();
        trie.build(map);
        foreach (String key in map.Keys)
        {
            AssertEquals(key, trie.get(key));
        }
    }
    [TestMethod]

    public void TestGet() 
    {
    }
    [TestMethod]

    public void TestLongestSearcher() 
    {
        var buildFrom = new Dictionary<String, String>();
        String[] keys = new String[]{"he", "her", "his"};
        foreach (String key in keys)
        {
            buildFrom.Add(key, key);
        }
        DoubleArrayTrie<String> trie = new DoubleArrayTrie<String>(buildFrom);
        String text = "her3he6his-hers! ";
        DoubleArrayTrie<String>.LongestSearcher searcher = trie.getLongestSearcher(text.ToCharArray(), 0);
        while (searcher.next())
        {
//            Console.printf("[%d, %d)=%s\n", searcher.begin, searcher.begin + searcher.Length, searcher.value);
            AssertEquals(searcher.value, text[searcher.begin .. (searcher.begin + searcher.Length)]);
        }
    }
    [TestMethod]

    public void TestTransmit() 
    {
        DoubleArrayTrie<CoreDictionary.Attribute> dat = CustomDictionary.dat;
        int index = dat.transition("龙", 1);
        //assertNull(dat.output(index));
        index = dat.transition("窝", index);
        AssertEquals("nz 183 ", dat.output(index).ToString());
    }

    //    public void testCombine() 
    //    {
    //        DoubleArrayTrie<CoreDictionary.Attribute> dat = CustomDictionary.dat;
    //        String[] wordNet = new String[]
    //            {
    //                "他",
    //                "一",
    //                "丁",
    //                "不",
    //                "识",
    //                "一",
    //                "丁",
    //                "呀",
    //            };
    //        for (int i = 0; i < wordNet.Length; ++i)
    //        {
    //            int state = 1;
    //            state = dat.transition(wordNet[i], state);
    //            if (state > 0)
    //            {
    //                int start = i;
    //                int to = i + 1;
    //                int end = - 1;
    //                CoreDictionary.Attribute value = null;
    //                for (; to < wordNet.Length; ++to)
    //                {
    //                    state = dat.transition(wordNet[to], state);
    //                    if (state < 0) break;
    //                    CoreDictionary.Attribute output = dat.output(state);
    //                    if (output != null)
    //                    {
    //                        value = output;
    //                        end = to + 1;
    //                    }
    //                }
    //                if (value != null)
    //                {
    //                    StringBuilder sbTerm = new StringBuilder();
    //                    for (int j = start; j < end; ++j)
    //                    {
    //                        sbTerm.append(wordNet[j]);
    //                    }
    //                    Console.WriteLine(sbTerm.ToString() + "/" + value);
    //                    i = end - 1;
    //                }
    //            }
    //        }
    //    }
    [TestMethod]

    public void TestHandleEmptyString()
    {
        String emptyString = "";
        DoubleArrayTrie<String> dat = new DoubleArrayTrie<String>();
        var dictionary = new Dictionary<String, String>();
        dictionary.Add("bug", "问题");
        dat.build(dictionary);
        DoubleArrayTrie<String>.Searcher searcher = dat.getSearcher(emptyString, 0);
        while (searcher.next())
        {
        }
    }
    [TestMethod]

    public void TestIssue966() 
    {
        var map = new Dictionary<String, String>();
        foreach (String word in "001乡道, 北京, 北京市通信公司, 来广营乡, 通州区".Split(", "))
        {
            map.Add(word, word);
        }
        DoubleArrayTrie<String> trie = new DoubleArrayTrie<String>(map);
        DoubleArrayTrie<String>.LongestSearcher searcher = trie.getLongestSearcher("北京市通州区001乡道发生了一件有意思的事情，来广营乡歌舞队正在跳舞", 0);
        while (searcher.next())
        {
            Console.Write("{0} {1}\n", searcher.begin, searcher.value);
        }
    }
}