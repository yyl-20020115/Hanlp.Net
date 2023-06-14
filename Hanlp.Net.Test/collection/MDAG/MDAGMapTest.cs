namespace com.hankcs.hanlp.collection.MDAG;



public class MDAGMapTest : TestCase
{
    MDAGMap<Integer> mdagMap = new MDAGMap<Integer>();
    Set<String> validKeySet;

    public void setUp() 
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/custom/CustomDictionary.txt");
        validKeySet = new TreeSet<String>();
        while (iterator.hasNext())
        {
            validKeySet.add(iterator.next().split("\\s")[0]);
        }
        for (String word : validKeySet)
        {
            mdagMap.put(word, word.length());
        }
    }

    public void testPut() 
    {
    }

    public void testGet() 
    {
        testPut();
        mdagMap.simplify();
//        mdagMap.unSimplify();
        for (String word : validKeySet)
        {
            assertEquals(word.length(), (int) mdagMap.get(word));
        }
    }

    public void testSingle() 
    {
        testPut();
        mdagMap.simplify();
        assertEquals(null, mdagMap.get("齿轮厂"));
    }

    public void testCommonPrefixSearch() 
    {
        testPut();
        assertEquals("[hankcs=6]", mdagMap.commonPrefixSearchWithValue("hankcs").toString());
    }

//    public void testBenchmark() 
//    {
//        testPut();
//        BinTrie<Integer> binTrie = new BinTrie<Integer>();
//        for (String key : validKeySet)
//        {
//            binTrie.put(key, key.length());
//        }
//        mdagMap.simplify();
//        for (String key : validKeySet)
//        {
//            assertEquals(binTrie.commonPrefixSearchWithValue(key).size(), mdagMap.commonPrefixSearchWithValue(key).size());
//        }
//
//        long start;
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            binTrie.commonPrefixSearchWithValue(key);
//        }
//        System.out.printf("binTrie: %d ms\n", System.currentTimeMillis() - start);
//
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            mdagMap.commonPrefixSearchWithValue(key);
//        }
//        System.out.printf("mdagMap: %d ms\n", System.currentTimeMillis() - start);
//    }
}