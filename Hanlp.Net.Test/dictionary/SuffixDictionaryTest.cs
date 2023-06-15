using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary;


[TestClass]

public class SuffixDictionaryTest : TestCase
{
    SuffixDictionary dictionary = new SuffixDictionary();
    [TestInitialize]
    public override void setUp() 
    {
        base.setUp();
        dictionary.addAll(Predefine.POSTFIX_SINGLE);
        dictionary.addAll(Predefine.POSTFIX_MUTIPLE);
    }

    [TestMethod]
    public void testEndsWith() 
    {
        assertEquals(true, dictionary.endsWith("黄冈市"));
        assertEquals(false, dictionary.endsWith("黄冈一二三"));
    }
    [TestMethod]
    public void testLongest() 
    {
        assertEquals(2, dictionary.getLongestSuffixLength("巴尔干半岛"));
    }

//    public void testDump() 
//    {
//        DictionaryMaker dictionaryMaker = new DictionaryMaker();
//        for (Map.Entry<String, int> entry : PlaceSuffixDictionary.dictionary.entrySet())
//        {
//            dictionaryMaker.add(entry.getKey(), NS.H.ToString());
//        }
//        dictionaryMaker.saveTxtTo("data/dictionary/place/suffix.txt");
//    }
}