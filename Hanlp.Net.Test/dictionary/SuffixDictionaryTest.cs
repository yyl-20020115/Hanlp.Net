using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary;


[TestClass]

public class SuffixDictionaryTest : TestCase
{
    SuffixDictionary dictionary = new SuffixDictionary();
    [TestInitialize]
    public override void SetUp() 
    {
        base.SetUp();
        dictionary.addAll(Predefine.POSTFIX_SINGLE);
        dictionary.addAll(Predefine.POSTFIX_MUTIPLE);
    }

    [TestMethod]
    public void TestEndsWith() 
    {
        AssertEquals(true, dictionary.EndsWith("黄冈市"));
        AssertEquals(false, dictionary.EndsWith("黄冈一二三"));
    }
    [TestMethod]
    public void TestLongest()
    {
        AssertEquals(2, dictionary.getLongestSuffixLength("巴尔干半岛"));
    }
    [TestMethod]
    public void TestGet()
    {
        String total = Predefine.POSTFIX_SINGLE;
        for (int i = 0; i < total.Length; ++i)
        {
            String single = (total[i]).ToString();
            AssertEquals(1, dictionary.get(single));
        }
        foreach (String single in Predefine.POSTFIX_MUTIPLE)
        {
            AssertEquals(single.Length, dictionary.get(single));
        }
    }
    //    public void testDump() 
    //    {
    //        DictionaryMaker dictionaryMaker = new DictionaryMaker();
    //        for (Map.Entry<String, int> entry : PlaceSuffixDictionary.dictionary.entrySet())
    //        {
    //            dictionaryMaker.Add(entry.getKey(), NS.H.ToString());
    //        }
    //        dictionaryMaker.saveTxtTo("data/dictionary/place/suffix.txt");
    //    }
}