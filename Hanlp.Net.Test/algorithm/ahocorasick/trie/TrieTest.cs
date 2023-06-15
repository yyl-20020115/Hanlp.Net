namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

using com.hankcs.hanlp;
using com.hankcs.hanlp.collection.AhoCorasick;

[TestClass]
public class TrieTest : TestCase
{
    [TestMethod]
    public void testHasKeyword()
    {
        var map = new Dictionary<String, String>();
        String[] keyArray = new String[]
            {
                "hers",
                "his",
                "she",
                "he"
            };
        foreach (String key in keyArray)
        {
            map.Add(key, key);
        }
        Trie trie = new Trie();
        trie.addAllKeyword(map.Keys);
        foreach (String key in keyArray)
        {
            assertTrue(trie.hasKeyword(key));
        }
        assertTrue(trie.hasKeyword("ushers"));
        assertFalse(trie.hasKeyword("构建耗时"));
    }
    [TestMethod]
    public void testParseText()
    {
        var map = new Dictionary<String, String>();
        String[] keyArray = new String[]
            {
                "hers",
                "his",
                "she",
                "he"
            };
        foreach (String key in keyArray)
        {
            map.Add(key, key);
        }
        AhoCorasickDoubleArrayTrie<String> act = new AhoCorasickDoubleArrayTrie<String>();
        act.build(map);
        //        act.debug();
        String text = "uhers";
        act.parseText(text, new TestHit() { text = text });
    }
    public class TestHit : TestCase, IHit<String>
    {
        public string text;
        //@Override
        public void hit(int begin, int end, String value)
        {
            //                Console.printf("[%d:%d]=%s\n", begin, end, value);
            assertEquals(value, text[begin..end]);
        }
    }

}