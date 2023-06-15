using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.collection.dartsclone;



[TestClass]
public class DartMapTest : TestCase
{
    HashSet<String> validKeySet;
    HashSet<String> invalidKeySet;
    private DartMap<int> dartMap;

    [TestInitialize]
    public override void setUp()
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/CoreNatureDictionary.ngram.txt");
        validKeySet = new ();
        while (iterator.hasNext())
        {
            validKeySet.Add(iterator.next().Split("\\s")[0]);
        }
        var map = new Dictionary<String, int>();
        foreach (String key in validKeySet)
        {
            map.Add(key, key.Length);
        }

        dartMap = new DartMap<int>(map);
    }
    [TestMethod]

    public void TestGenerateInvalidKeySet()
    {
        invalidKeySet = new ();
        Random random = new Random(DateTime.Now.Microsecond);
        while (invalidKeySet.Count < validKeySet.Count)
        {
            int Length = random.Next(10) + 1;
            StringBuilder key = new StringBuilder(Length);
            for (int i = 0; i < Length; ++i)
            {
                key.Append(random.Next(char.MaxValue));
            }
            if (validKeySet.Contains(key.ToString())) continue;
            invalidKeySet.Add(key.ToString());
        }
    }
    [TestMethod]

    public void TestBuild() 
    {
    }
    [TestMethod]

    public void TestContainsAndNoteContains() 
    {
        TestBuild();
        foreach (String key in validKeySet)
        {
            assertEquals(key.Length, (int)dartMap.get(key));
        }

        TestGenerateInvalidKeySet();
        foreach (String key in invalidKeySet)
        {
            assertEquals(null, dartMap.get(key));
        }
    }

//    public void testCommPrefixSearch() 
//    {
//        testBuild();
//        assertEquals(true, dartMap.commonPrefixSearch("一举一动"));
//    }

//    public void testBenchmark() 
//    {
//        testBuild();
//        long start;
//        {
//
//        }
//        DoubleArrayTrie<int> trie = new DoubleArrayTrie<int>();
//        TreeMap<String, int> map = new TreeMap<String, int>();
//        for (String key : validKeySet)
//        {
//            map.put(key, key.Length());
//        }
//        trie.build(map);
//
//        // TreeMap
//        start = DateTime.Now.Microsecond;
//        for (String key : validKeySet)
//        {
//            assertEquals(key.Length(), (int)map.get(key));
//        }
//        Console.printf("TreeMap: %d ms\n", DateTime.Now.Microsecond - start);
//        map = null;
//        // DAT
//        start = DateTime.Now.Microsecond;
//        for (String key : validKeySet)
//        {
//            assertEquals(key.Length(), (int)trie.get(key));
//        }
//        Console.printf("DAT: %d ms\n", DateTime.Now.Microsecond - start);
//        trie = null;
//        // DAWG
//        start = DateTime.Now.Microsecond;
//        for (String key : validKeySet)
//        {
//            assertEquals(key.Length(), (int)dartMap.get(key));
//        }
//        Console.printf("DAWG: %d ms\n", DateTime.Now.Microsecond - start);
//
//        /**
//         * result:
//         * TreeMap: 677 ms
//         * DAT: 310 ms
//         * DAWG: 858 ms
//         *
//         * 结论，虽然很省内存，但是速度不理想
//         */
//    }
}