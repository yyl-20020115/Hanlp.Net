using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.MDAG;



[TestClass]
public class MDAGMapTest : TestCase
{
    MDAGMap<int> mdagMap = new MDAGMap<int>();
    HashSet<String> validKeySet;
    [TestInitialize]
    public void setUp() 
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/custom/CustomDictionary.txt");
        validKeySet = new ();
        while (iterator.hasNext())
        {
            validKeySet.add(iterator.next().split("\\s")[0]);
        }
        for (String word : validKeySet)
        {
            mdagMap.put(word, word.Length());
        }
    }
    [TestMethod]

    public void testPut() 
    {
    }
    [TestMethod]

    public void testGet() 
    {
        testPut();
        mdagMap.simplify();
//        mdagMap.unSimplify();
        foreach (String word in validKeySet)
        {
            assertEquals(word.Length, (int) mdagMap.get(word));
        }
    }
    [TestMethod]

    public void testSingle() 
    {
        testPut();
        mdagMap.simplify();
        assertEquals(null, mdagMap.get("齿轮厂"));
    }
    [TestMethod]

    public void testCommonPrefixSearch() 
    {
        testPut();
        assertEquals("[hankcs=6]", mdagMap.commonPrefixSearchWithValue("hankcs").ToString());
    }

//    public void testBenchmark() 
//    {
//        testPut();
//        BinTrie<int> binTrie = new BinTrie<int>();
//        for (String key : validKeySet)
//        {
//            binTrie.put(key, key.Length());
//        }
//        mdagMap.simplify();
//        for (String key : validKeySet)
//        {
//            assertEquals(binTrie.commonPrefixSearchWithValue(key).size(), mdagMap.commonPrefixSearchWithValue(key).size());
//        }
//
//        long start;
//        start = DateTime.Now.Microsecond;
//        for (String key : validKeySet)
//        {
//            binTrie.commonPrefixSearchWithValue(key);
//        }
//        Console.printf("binTrie: %d ms\n", DateTime.Now.Microsecond - start);
//
//        start = DateTime.Now.Microsecond;
//        for (String key : validKeySet)
//        {
//            mdagMap.commonPrefixSearchWithValue(key);
//        }
//        Console.printf("mdagMap: %d ms\n", DateTime.Now.Microsecond - start);
//    }
}