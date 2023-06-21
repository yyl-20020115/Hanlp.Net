using com.hankcs.hanlp.algorithm.ahocorasick.trie;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.AhoCorasick;



[TestClass]
public class AhoCorasickDoubleArrayTrieTest : TestCase
{
    [TestMethod]
    public void TestTwoAC()
    {
        var map = new Dictionary<string, string>();
        IOUtil.LineIterator iterator = new IOUtil.LineIterator(
            "data/dictionary/CoreNatureDictionary.mini.txt");
        while (iterator.MoveNext())
        {
            String line = iterator.next().Split("\\s")[0];
            map.Add(line, line);
        }

        Trie trie = new Trie();
        trie.addAllKeyword(map.Keys);
        AhoCorasickDoubleArrayTrie<String> act = new AhoCorasickDoubleArrayTrie<String>();
        act.build(map);

        foreach (String key in map.Keys)
        {
            var emits = trie.parseText(key);
            var otherSet = new HashSet<String>();
            foreach (Emit emit in emits)
            {
                otherSet.Add(emit.getKeyword() + emit.getEnd());
            }

            var entries = act.parseText(key);
            var mySet = new HashSet<String>();
            foreach (AhoCorasickDoubleArrayTrie<String>.Hit<String> entry in entries)
            {
                mySet.Add(entry.value + (entry.end - 1));
            }

            AssertEquals(otherSet, mySet);
        }
    }

    /**
     * 测试构建和匹配，使用《我的团长我的团》.txt作为测试数据，并且判断匹配是否正确
     * @
     */
//    public void testSegment() 
//    {
//        Dictionary<String, String> map = new Dictionary<String, String>();
//        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/CoreNatureDictionary.txt");
//        while (iterator.MoveNext())
//        {
//            String line = iterator.next().Split("\\s")[0];
//            map.Add(line, line);
//        }
//
//        Trie trie = new Trie();
//        trie.addAllKeyword(map.Keys);
//        AhoCorasickDoubleArrayTrie<String> act = new AhoCorasickDoubleArrayTrie<String>();
//        long timeMillis = DateTime.Now.Microsecond;
//        act.build(map);
//        Console.WriteLine("构建耗时：" + (DateTime.Now.Microsecond - timeMillis) + " ms");
//
//        LinkedList<String> lineList = IOUtil.readLineList("D:\\Doc\\语料库\\《我的团长我的团》.txt");
//        timeMillis = DateTime.Now.Microsecond;
//        for (String sentence : lineList)
//        {
////            Console.WriteLine(sentence);
//            List<AhoCorasickDoubleArrayTrie<String>.Hit<String>> entryList = act.parseText(sentence);
//            for (AhoCorasickDoubleArrayTrie<String>.Hit<String> entry : entryList)
//            {
//                int end = entry.end;
//                int start = entry.begin;
////                Console.printf("[%d:%d]=%s\n", start, end, entry.value);
//
//                assertEquals(sentence.substring(start, end), entry.value);
//            }
//        }
//        Console.printf("%d ms\n", DateTime.Now.Microsecond - timeMillis);
//    }
}