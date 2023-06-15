namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

using com.hankcs.hanlp;
using com.hankcs.hanlp.collection.AhoCorasick;

[TestClass]
public class TrieTest : TestCase
{
    [TestMethod]
    public void TestHasKeyword()
    {
        var map = new Dictionary<string, string>();
        var keyArray = new string[]
            {
                "hers",
                "his",
                "she",
                "he"
            };
        foreach (var key in keyArray)
        {
            map.Add(key, key);
        }
        var trie = new Trie();
        trie.addAllKeyword(map.Keys);
        foreach (var key in keyArray)
        {
            assertTrue(trie.hasKeyword(key));
        }
        assertTrue(trie.hasKeyword("ushers"));
        assertFalse(trie.hasKeyword("构建耗时"));
    }
    [TestMethod]
    public void TestParseText()
    {
        var map = new Dictionary<string, string>();
        var keyArray = new string[]
            {
                "hers",
                "his",
                "she",
                "he"
            };
        foreach (var key in keyArray)
        {
            map.Add(key, key);
        }
        AhoCorasickDoubleArrayTrie<string> act = new AhoCorasickDoubleArrayTrie<string>();
        act.build(map);
        //        act.debug();
        String text = "uhers";
        act.parseText(text, new TestHit(text));
    }
    public class TestHit : TestCase, AhoCorasickDoubleArrayTrie<string>.IHit<string>
    {
        private readonly string text;
        public TestHit(string text)
        {
            this.text = text;
        }
        public void hit(int begin, int end, String value)
        {
            //                Console.printf("[%d:%d]=%s\n", begin, end, value);
            assertEquals(value, text[begin..end]);
        }
    }

}