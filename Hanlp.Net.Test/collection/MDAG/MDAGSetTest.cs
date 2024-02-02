using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.collection.MDAG;



/**
 * 测试MDAG
 */
[TestClass]
public class MDAGSetTest : TestCase
{
    HashSet<String> validKeySet;
    HashSet<String> invalidKeySet;
    MDAGSet mdagSet;

    static String DATA_TEST_OUT_BIN;
    private string tempFile;

    [TestInitialize]
    public override void SetUp() 
    {
        TestUtility.EnsureFullData();
        tempFile = createTempFile("hanlp-", ".bin");
        DATA_TEST_OUT_BIN = tempFile; 
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/custom/CustomDictionary.txt");
        validKeySet = new ();
        while (iterator.MoveNext())
        {
            validKeySet.Add(iterator.next().Split("\\s")[0]);
        }
        mdagSet = new MDAGSet(validKeySet);
    }
    [TestMethod]

    public void TestSize()
    {
        AssertEquals(validKeySet.Count, mdagSet.Count);
    }
    [TestMethod]

    public void TestContains()
    {
        foreach (String key in validKeySet)
        {
//            assertEquals(true, mdagSet.Contains(key));
            //assert mdagSet.Contains(key) : "本来应该有 " + key;
        }
    }
    [TestMethod]

    public void TestNotContains() 
    {
        invalidKeySet = new ();
        Random random = new Random(DateTime.Now.Microsecond);
        mdagSet.simplify();
        mdagSet.unSimplify();
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

        foreach (String key in invalidKeySet)
        {
            AssertEquals(false, mdagSet.Contains(key));
        }
    }
    [TestMethod]

    public void TestToArray() 
    {
        String[] keyArray = mdagSet.ToArray(new String[0]);
        AssertEquals(validKeySet.Count, keyArray.Length);
        foreach (String key in keyArray)
        {
            AssertEquals(true, mdagSet.Contains(key));
        }
    }
    [TestMethod]

    public void TestRemove() 
    {
        String[] keyArray = mdagSet.ToArray();
        foreach (String key in keyArray)
        {
            mdagSet.Remove(key);
            AssertEquals(false, mdagSet.Contains(key));
        }
    }
    [TestMethod]

    public void TestAdd() 
    {
        AssertEquals(true, mdagSet.Add("成功啦"));
        AssertEquals(true, mdagSet.Contains("成功啦"));
    }
    [TestMethod]

    public void TestSimplify() 
    {
        var equivalenceClassMDAGNodeHashMapBefore = mdagSet._getEquivalenceClassMDAGNodeHashMap();
        mdagSet.simplify();
        mdagSet.unSimplify();
        var equivalenceClassMDAGNodeHashMapAfter = mdagSet._getEquivalenceClassMDAGNodeHashMap();
        AssertEquals(equivalenceClassMDAGNodeHashMapBefore, equivalenceClassMDAGNodeHashMapAfter);
    }
    [TestMethod]


    public void TestSimplifyAndContains() 
    {
        mdagSet.simplify();
        TestContains();
        TestNotContains();
    }
    [TestMethod]

    public void TestSaveAndLoad() 
    {
        var Out = new FileStream(DATA_TEST_OUT_BIN,FileMode.Create);
        mdagSet.save(Out);
        Out.Close();

        mdagSet = new MDAGSet();
        mdagSet.load(ByteArray.createByteArray(DATA_TEST_OUT_BIN));
        TestContains();
        TestNotContains();
    }
    [TestMethod]

    public void TestSingle() 
    {
        mdagSet.simplify();
        AssertTrue(mdagSet.Contains("hankcs"));
    }

    //    public void testBenchmark() 
    //    {
    //        BinTrie<Boolean> binTrie = new BinTrie<Boolean>();
    //        for (String key : validKeySet)
    //        {
    //            binTrie.Add(key, true);
    //        }
    //        long start = DateTime.Now.Microsecond;
    //        for (String key : validKeySet)
    //        {
    //            assertEquals(true, (bool)binTrie.get(key));
    //        }
    //        Console.printf("binTrie用时 %d ms\n", DateTime.Now.Microsecond - start);
    //
    //        mdagSet.simplify();
    //        start = DateTime.Now.Microsecond;
    //        for (String key : validKeySet)
    //        {
    //            assertEquals(true, (bool)mdagSet.Contains(key));
    //        }
    //        Console.printf("mdagSet用时 %d ms\n", DateTime.Now.Microsecond - start);
    //    }
    [TestMethod]

    public void TestCommPrefix() 
    {
        MDAGSet setTwo = new MDAGSet(validKeySet);
        setTwo.simplify();
        foreach(String key in validKeySet)
        {
            AssertEquals(mdagSet.getStringsStartingWith(key), setTwo.getStringsStartingWith(key));
        }
    }

    [TestMethod]

    public void TestSimplifyWithoutSave() 
    {
        MDAG mdag = new MDAG();
        mdag.addString("hers");
        mdag.addString("his");
        mdag.addString("she");
        mdag.addString("he");

        mdag.simplify();
        AssertTrue(mdag.Contains("hers"));
    }
    [TestMethod]

    public void TestSimplifyMap() 
    {
        MDAGMap<String> mdagMap = new ();
        List<String> validKeySet = new ();
        validKeySet.Add("hers");
        validKeySet.Add("his");
        validKeySet.Add("she");
        validKeySet.Add("he");
        foreach (String key in validKeySet)
        {
            mdagMap.Add(key, key);
        }
        mdagMap.simplify();

        AssertEquals("he", mdagMap.get("he"));
    }
}