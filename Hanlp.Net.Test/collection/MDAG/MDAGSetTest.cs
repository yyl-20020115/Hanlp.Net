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
    private File tempFile;

    [TestInitialize]
    public void setUp() 
    {
        TestUtility.ensureFullData();
        tempFile = File.createTempFile("hanlp-", ".bin");
        DATA_TEST_OUT_BIN = tempFile.getAbsolutePath();
        IOUtil.LineIterator iterator = new IOUtil.LineIterator("data/dictionary/custom/CustomDictionary.txt");
        validKeySet = new TreeSet<String>();
        while (iterator.hasNext())
        {
            validKeySet.add(iterator.next().split("\\s")[0]);
        }
        mdagSet = new MDAGSet(validKeySet);
    }
    [TestMethod]

    public void testSize() 
    {
        assertEquals(validKeySet.size(), mdagSet.size());
    }
    [TestMethod]

    public void testContains() 
    {
        for (String key : validKeySet)
        {
//            assertEquals(true, mdagSet.Contains(key));
            assert mdagSet.Contains(key) : "本来应该有 " + key;
        }
    }
    [TestMethod]

    public void testNotContains() 
    {
        invalidKeySet = new TreeSet<String>();
        Random random = new Random(System.currentTimeMillis());
        mdagSet.simplify();
        mdagSet.unSimplify();
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

        for (String key : invalidKeySet)
        {
            assertEquals(false, mdagSet.Contains(key));
        }
    }
    [TestMethod]

    public void testToArray() 
    {
        String[] keyArray = mdagSet.toArray(new String[0]);
        assertEquals(validKeySet.size(), keyArray.Length);
        for (String key : keyArray)
        {
            assertEquals(true, mdagSet.Contains(key));
        }
    }
    [TestMethod]

    public void testRemove() 
    {
        String[] keyArray = mdagSet.toArray(new String[0]);
        for (String key : keyArray)
        {
            mdagSet.remove(key);
            assertEquals(false, mdagSet.Contains(key));
        }
    }
    [TestMethod]

    public void testAdd() 
    {
        assertEquals(true, mdagSet.add("成功啦"));
        assertEquals(true, mdagSet.Contains("成功啦"));
    }
    [TestMethod]

    public void testSimplify() 
    {
        HashMap<MDAGNode, MDAGNode> equivalenceClassMDAGNodeHashMapBefore = mdagSet._getEquivalenceClassMDAGNodeHashMap();
        mdagSet.simplify();
        mdagSet.unSimplify();
        HashMap<MDAGNode, MDAGNode> equivalenceClassMDAGNodeHashMapAfter = mdagSet._getEquivalenceClassMDAGNodeHashMap();
        assertEquals(equivalenceClassMDAGNodeHashMapBefore, equivalenceClassMDAGNodeHashMapAfter);
    }
    [TestMethod]


    public void testSimplifyAndContains() 
    {
        mdagSet.simplify();
        testContains();
        testNotContains();
    }
    [TestMethod]

    public void testSaveAndLoad() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        mdagSet.save(_out);
        _out.close();

        mdagSet = new MDAGSet();
        mdagSet.load(ByteArray.createByteArray(DATA_TEST_OUT_BIN));
        testContains();
        testNotContains();
    }
    [TestMethod]

    public void testSingle() 
    {
        mdagSet.simplify();
        assertTrue(mdagSet.Contains("hankcs"));
    }

    //    public void testBenchmark() 
    //    {
    //        BinTrie<Boolean> binTrie = new BinTrie<Boolean>();
    //        for (String key : validKeySet)
    //        {
    //            binTrie.put(key, true);
    //        }
    //        long start = System.currentTimeMillis();
    //        for (String key : validKeySet)
    //        {
    //            assertEquals(true, (bool)binTrie.get(key));
    //        }
    //        Console.printf("binTrie用时 %d ms\n", System.currentTimeMillis() - start);
    //
    //        mdagSet.simplify();
    //        start = System.currentTimeMillis();
    //        for (String key : validKeySet)
    //        {
    //            assertEquals(true, (bool)mdagSet.Contains(key));
    //        }
    //        Console.printf("mdagSet用时 %d ms\n", System.currentTimeMillis() - start);
    //    }
    [TestMethod]

    public void testCommPrefix() 
    {
        MDAGSet setTwo = new MDAGSet(validKeySet);
        setTwo.simplify();
        for (String key : validKeySet)
        {
            assertEquals(mdagSet.getStringsStartingWith(key), setTwo.getStringsStartingWith(key));
        }
    }

    [TestMethod]

    public void testSimplifyWithoutSave() 
    {
        MDAG mdag = new MDAG();
        mdag.addString("hers");
        mdag.addString("his");
        mdag.addString("she");
        mdag.addString("he");

        mdag.simplify();
        assertTrue(mdag.Contains("hers"));
    }
    [TestMethod]

    public void testSimplifyMap() 
    {
        MDAGMap<String> mdagMap = new MDAGMap<String>();
        List<String> validKeySet = new ArrayList<String>();
        validKeySet.add("hers");
        validKeySet.add("his");
        validKeySet.add("she");
        validKeySet.add("he");
        for (String key : validKeySet)
        {
            mdagMap.put(key, key);
        }
        mdagMap.simplify();

        assertEquals("he", mdagMap.get("he"));
    }
}