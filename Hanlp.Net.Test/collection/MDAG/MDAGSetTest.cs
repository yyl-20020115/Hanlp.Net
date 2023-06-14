namespace com.hankcs.hanlp.collection.MDAG;



/**
 * 测试MDAG
 */
public class MDAGSetTest : TestCase
{
    Set<String> validKeySet;
    Set<String> invalidKeySet;
    MDAGSet mdagSet;

    static String DATA_TEST_OUT_BIN;
    private File tempFile;


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

    public void testSize() 
    {
        assertEquals(validKeySet.size(), mdagSet.size());
    }

    public void testContains() 
    {
        for (String key : validKeySet)
        {
//            assertEquals(true, mdagSet.contains(key));
            assert mdagSet.contains(key) : "本来应该有 " + key;
        }
    }

    public void testNotContains() 
    {
        invalidKeySet = new TreeSet<String>();
        Random random = new Random(System.currentTimeMillis());
        mdagSet.simplify();
        mdagSet.unSimplify();
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

        for (String key : invalidKeySet)
        {
            assertEquals(false, mdagSet.contains(key));
        }
    }

    public void testToArray() 
    {
        String[] keyArray = mdagSet.toArray(new String[0]);
        assertEquals(validKeySet.size(), keyArray.length);
        for (String key : keyArray)
        {
            assertEquals(true, mdagSet.contains(key));
        }
    }

    public void testRemove() 
    {
        String[] keyArray = mdagSet.toArray(new String[0]);
        for (String key : keyArray)
        {
            mdagSet.remove(key);
            assertEquals(false, mdagSet.contains(key));
        }
    }

    public void testAdd() 
    {
        assertEquals(true, mdagSet.add("成功啦"));
        assertEquals(true, mdagSet.contains("成功啦"));
    }

    public void testSimplify() 
    {
        HashMap<MDAGNode, MDAGNode> equivalenceClassMDAGNodeHashMapBefore = mdagSet._getEquivalenceClassMDAGNodeHashMap();
        mdagSet.simplify();
        mdagSet.unSimplify();
        HashMap<MDAGNode, MDAGNode> equivalenceClassMDAGNodeHashMapAfter = mdagSet._getEquivalenceClassMDAGNodeHashMap();
        assertEquals(equivalenceClassMDAGNodeHashMapBefore, equivalenceClassMDAGNodeHashMapAfter);
    }

    public void testSimplifyAndContains() 
    {
        mdagSet.simplify();
        testContains();
        testNotContains();
    }

    public void testSaveAndLoad() 
    {
        DataOutputStream out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        mdagSet.save(out);
        out.close();

        mdagSet = new MDAGSet();
        mdagSet.load(ByteArray.createByteArray(DATA_TEST_OUT_BIN));
        testContains();
        testNotContains();
    }

    public void testSingle() 
    {
        mdagSet.simplify();
        assertTrue(mdagSet.contains("hankcs"));
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
//            assertEquals(true, (boolean)binTrie.get(key));
//        }
//        System.out.printf("binTrie用时 %d ms\n", System.currentTimeMillis() - start);
//
//        mdagSet.simplify();
//        start = System.currentTimeMillis();
//        for (String key : validKeySet)
//        {
//            assertEquals(true, (boolean)mdagSet.contains(key));
//        }
//        System.out.printf("mdagSet用时 %d ms\n", System.currentTimeMillis() - start);
//    }

    public void testCommPrefix() 
    {
        MDAGSet setTwo = new MDAGSet(validKeySet);
        setTwo.simplify();
        for (String key : validKeySet)
        {
            assertEquals(mdagSet.getStringsStartingWith(key), setTwo.getStringsStartingWith(key));
        }
    }


    public void testSimplifyWithoutSave() 
    {
        MDAG mdag = new MDAG();
        mdag.addString("hers");
        mdag.addString("his");
        mdag.addString("she");
        mdag.addString("he");

        mdag.simplify();
        assertTrue(mdag.contains("hers"));
    }

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