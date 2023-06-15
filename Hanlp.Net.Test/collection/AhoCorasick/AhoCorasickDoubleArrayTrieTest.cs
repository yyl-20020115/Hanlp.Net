using com.hankcs.hanlp.algorithm.ahocorasick.trie;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.AhoCorasick;



[TestClass]
public class AhoCorasickDoubleArrayTrieTest : TestCase
{
    [TestMethod]

    public void TestTwoAC()
    {
        var map = new Dictionary<String, String>();
        IOUtil.LineIterator iterator = new IOUtil.LineIterator(
            "data/dictionary/CoreNatureDictionary.mini.txt");
        while (iterator.hasNext())
        {
            String line = iterator.next().Split("\\s")[0];
            map.Add(line, line);
        }

        Trie trie = new Trie();
        trie.addAllKeyword(map.Keys);
        AhoCorasickDoubleArrayTrie<String> act = new AhoCorasickDoubleArrayTrie<String>();
        act.build(map);

        for (String key : map.Keys)
        {
            Collection<Emit> emits = trie.parseText(key);
            Set<String> otherSet = new HashSet<String>();
            for (Emit emit : emits)
            {
                otherSet.add(emit.getKeyword() + emit.getEnd());
            }

            List<AhoCorasickDoubleArrayTrie<String>.Hit<String>> entries = act.parseText(key);
            Set<String> mySet = new HashSet<String>();
            for (AhoCorasickDoubleArrayTrie<String>.Hit<String> entry : entries)
            {
                mySet.add(entry.value + (entry.end - 1));
            }

            assertEquals(otherSet, mySet);
        }
    }

    /**
     * 测试构建和匹配，使用《我的团长我的团》.txt作为测试数据，并且判断匹配是否正确
     * @
     */
//    public void testSegment() 
//    {
//        TreeMap<String, String> map = new TreeMap<String, String>();
//        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/CoreNatureDictionary.txt");
//        while (iterator.hasNext())
//        {
//            String line = iterator.next().split("\\s")[0];
//            map.put(line, line);
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