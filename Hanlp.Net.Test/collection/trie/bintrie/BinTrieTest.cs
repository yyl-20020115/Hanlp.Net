namespace com.hankcs.hanlp.collection.trie.bintrie;



public class BinTrieTest : TestCase
{
    static String DATA_TEST_OUT_BIN;
    private File tempFile;

    //@Override
    public void setUp() 
    {
        tempFile = File.createTempFile("hanlp-", ".dat");
        DATA_TEST_OUT_BIN = tempFile.getAbsolutePath();
    }

    public void testParseText() 
    {
        BinTrie<String> trie = new BinTrie<String>();
        String[] keys = new String[]{"he", "her", "his"};
        for (String key : keys)
        {
            trie.put(key, key);
        }
        final String text = " her4he7his ";
        AhoCorasickDoubleArrayTrie.IHit<String> processor = new AhoCorasickDoubleArrayTrie.IHit<String>()
        {
            //@Override
            public void hit(int begin, int end, String value)
            {
//                System.out.printf("[%d, %d)=%s\n", begin, end, value);
                assertEquals(value, text.substring(begin, end));
            }
        };
//        trie.parseLongestText(text, processor);
        trie.parseText(text, processor);
    }

    public void testPut() 
    {
        BinTrie<Boolean> trie = new BinTrie<Boolean>();
        trie.put("加入", true);
        trie.put("加入", false);

        assertEquals(new Boolean(false), trie.get("加入"));
    }

    public void testArrayIndexOutOfBoundsException() 
    {
        BinTrie<Boolean> trie = new BinTrie<Boolean>();
        trie.put(new char[]{'\uffff'}, true);
    }

    public void testSaveAndLoad() 
    {
        BinTrie<Integer> trie = new BinTrie<Integer>();
        trie.put("haha", 0);
        trie.put("hankcs", 1);
        trie.put("hello", 2);
        trie.put("za", 3);
        trie.put("zb", 4);
        trie.put("zzz", 5);
        assertTrue(trie.save(DATA_TEST_OUT_BIN));
        trie = new BinTrie<Integer>();
        Integer[] value = new Integer[100];
        for (int i = 0; i < value.length; ++i)
        {
            value[i] = i;
        }
        assertTrue(trie.load(DATA_TEST_OUT_BIN, value));
        Set<Map.Entry<String, Integer>> entrySet = trie.entrySet();
        assertEquals("[haha=0, hankcs=1, hello=2, za=3, zb=4, zzz=5]", entrySet.toString());
    }

//    public void testCustomDictionary() 
//    {
//        HanLP.Config.enableDebug(true);
//        Console.WriteLine(CustomDictionary.get("龟兔赛跑"));
//    }
//
//    public void testSortCustomDictionary() 
//    {
//        DictionaryUtil.sortDictionary(HanLP.Config.CustomDictionaryPath[0]);
//    }
}