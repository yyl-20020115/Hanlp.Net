namespace com.hankcs.hanlp.collection.dartsclone;



public class DartMapTest : TestCase
{
    Set<String> validKeySet;
    Set<String> invalidKeySet;
    private DartMap<Integer> dartMap;

    public void setUp() 
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/CoreNatureDictionary.ngram.txt");
        validKeySet = new TreeSet<String>();
        while (iterator.hasNext())
        {
            validKeySet.add(iterator.next().split("\\s")[0]);
        }
        TreeMap<String, Integer> map = new TreeMap<String, Integer>();
        for (String key : validKeySet)
        {
            map.put(key, key.length());
        }
        dartMap = new DartMap<Integer>(map);
    }

    public void testGenerateInvalidKeySet() 
    {
        invalidKeySet = new TreeSet<String>();
        Random random = new Random(System.currentTimeMillis());
        while (invalidKeySet.size() < validKeySet.size())
        {
            int length = random.nextInt(10) + 1;
            StringBuilder key = new StringBuilder(length);
            for (int i = 0; i < length; ++i)
            {
                key.append(random.nextInt(Character.MAX_VALUE));
            }
            if (validKeySet.contains(key.toString())) continue;
            invalidKeySet.add(key.toString());
        }
    }

    public void testBuild() 
    {
    }

    public void testContainsAndNoteContains() 
    {
        testBuild();
        for (String key : validKeySet)
        {
            assertEquals(key.length(), (int)dartMap.get(key));
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
//        DoubleArrayTrie<Integer> trie = new DoubleArrayTrie<Integer>();
//        TreeMap<String, Integer> map = new TreeMap<String, Integer>();
//        for (String key : validKeySet)
//        {
//            map.put(key, key.length());
//        }
//        trie.build(map);
//
//        // TreeMap
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(key.length(), (int)map.get(key));
//        }
//        System.out.printf("TreeMap: %d ms\n", System.currentTimeMillis() - start);
//        map = null;
//        // DAT
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(key.length(), (int)trie.get(key));
//        }
//        System.out.printf("DAT: %d ms\n", System.currentTimeMillis() - start);
//        trie = null;
//        // DAWG
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(key.length(), (int)dartMap.get(key));
//        }
//        System.out.printf("DAWG: %d ms\n", System.currentTimeMillis() - start);
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