namespace com.hankcs.hanlp.collection.dartsclone;



[TestClass]
public class DartMapTest : TestCase
{
    HashSet<String> validKeySet;
    HashSet<String> invalidKeySet;
    private DartMap<int> dartMap;

    [TestInitialize]
    public void setUp() 
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/CoreNatureDictionary.ngram.txt");
        validKeySet = new TreeSet<String>();
        while (iterator.hasNext())
        {
            validKeySet.add(iterator.next().split("\\s")[0]);
        }
        TreeMap<String, int> map = new TreeMap<String, int>();
        for (String key : validKeySet)
        {
            map.put(key, key.Length());
        }
        dartMap = new DartMap<int>(map);
    }
    [TestMethod]

    public void testGenerateInvalidKeySet() 
    {
        invalidKeySet = new TreeSet<String>();
        Random random = new Random(System.currentTimeMillis());
        while (invalidKeySet.size() < validKeySet.size())
        {
            int Length = random.nextInt(10) + 1;
            StringBuilder key = new StringBuilder(Length);
            for (int i = 0; i < Length; ++i)
            {
                key.append(random.nextInt(Character.MAX_VALUE));
            }
            if (validKeySet.Contains(key.ToString())) continue;
            invalidKeySet.add(key.ToString());
        }
    }
    [TestMethod]

    public void testBuild() 
    {
    }
    [TestMethod]

    public void testContainsAndNoteContains() 
    {
        testBuild();
        for (String key : validKeySet)
        {
            assertEquals(key.Length(), (int)dartMap.get(key));
        }

        testGenerateInvalidKeySet();
        for (String key : invalidKeySet)
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
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(key.Length(), (int)map.get(key));
//        }
//        Console.printf("TreeMap: %d ms\n", System.currentTimeMillis() - start);
//        map = null;
//        // DAT
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(key.Length(), (int)trie.get(key));
//        }
//        Console.printf("DAT: %d ms\n", System.currentTimeMillis() - start);
//        trie = null;
//        // DAWG
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(key.Length(), (int)dartMap.get(key));
//        }
//        Console.printf("DAWG: %d ms\n", System.currentTimeMillis() - start);
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