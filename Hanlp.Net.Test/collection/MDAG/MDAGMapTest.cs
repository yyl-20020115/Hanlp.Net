using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.MDAG;



[TestClass]
public class MDAGMapTest : TestCase
{
    MDAGMap<int> mdagMap = new MDAGMap<int>();
    HashSet<String> validKeySet;
    [TestInitialize]
    public override void SetUp() 
    {
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/custom/CustomDictionary.txt");
        validKeySet = new ();
        while (iterator.hasNext())
        {
            validKeySet.Add(iterator.next().Split("\\s")[0]);
        }
        foreach (String word in validKeySet)
        {
            mdagMap.Add(word, word.Length);
        }
    }
    [TestMethod]

    public void TestPut()
    {
    }
    [TestMethod]

    public void TestGet() 
    {
        TestPut();
        mdagMap.simplify();
//        mdagMap.unSimplify();
        foreach (String word in validKeySet)
        {
            AssertEquals(word.Length, (int) mdagMap.get(word));
        }
    }
    [TestMethod]

    public void TestSingle() 
    {
        TestPut();
        mdagMap.simplify();
        AssertEquals(null, mdagMap.get("齿轮厂"));
    }
    [TestMethod]

    public void TestCommonPrefixSearch() 
    {
        TestPut();
        AssertEquals("[hankcs=6]", mdagMap.commonPrefixSearchWithValue("hankcs").ToString());
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