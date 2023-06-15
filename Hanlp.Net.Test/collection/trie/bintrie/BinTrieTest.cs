
namespace com.hankcs.hanlp.collection.trie.bintrie;



[TestClass]
public class BinTrieTest : TestCase
{
    static string DATA_TEST_OUT_BIN;
    private string tempFile;

    [TestInitialize]
    public override void SetUp()
    {
        tempFile = File.createTempFile("hanlp-", ".dat");
        DATA_TEST_OUT_BIN = tempFile.getAbsolutePath();
    }
    [TestMethod]
    public void TestParseText()
    {
        BinTrie<String> trie = new BinTrie<String>();
        String[] keys = new String[] { "he", "her", "his" };
        foreach (String key in keys)
        {
            trie.put(key, key);
        }
        String text = " her4he7his ";
        AhoCorasick.AhoCorasickDoubleArrayTrie<String>.IHit<String> processor = new TestHit() { text = text };
        //        trie.parseLongestText(text, processor);
        trie.parseText(text, processor);
    }
    public class TestHit : AhoCorasick.AhoCorasickDoubleArrayTrie<String>.IHit<String>
    {
        public string text;
        //@Override
        public void hit(int begin, int end, String value)
        {
            //                Console.printf("[%d, %d)=%s\n", begin, end, value);
            AssertEquals(value, text[begin .. end]);
        }
    }
    [TestMethod]

    public void TestPut()
    {
        BinTrie<bool> trie = new BinTrie<bool>();
        trie.put("加入", true);
        trie.put("加入", false);

        AssertEquals(false, trie.get("加入"));
    }
    [TestMethod]

    public void TestArrayIndexOutOfBoundsException()
    {
        BinTrie<bool> trie = new BinTrie<bool>();
        trie.put(new char[] { '\uffff' }, true);
    }

    [TestMethod]
    public void TestSaveAndLoad()
    {
        BinTrie<int> trie = new BinTrie<int>();
        trie.put("haha", 0);
        trie.put("hankcs", 1);
        trie.put("hello", 2);
        trie.put("za", 3);
        trie.put("zb", 4);
        trie.put("zzz", 5);
        AssertTrue(trie.save(DATA_TEST_OUT_BIN));
        trie = new BinTrie<int>();
        int[] value = new int[100];
        for (int i = 0; i < value.Length; ++i)
        {
            value[i] = i;
        }
        AssertTrue(trie.load(DATA_TEST_OUT_BIN, value));
        var entrySet = trie.entrySet();
        AssertEquals("[haha=0, hankcs=1, hello=2, za=3, zb=4, zzz=5]", entrySet.ToString());
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