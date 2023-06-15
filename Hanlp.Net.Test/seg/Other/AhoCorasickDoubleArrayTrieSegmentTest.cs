namespace com.hankcs.hanlp.seg.Other;

[TestClass]

public class AhoCorasickDoubleArrayTrieSegmentTest : TestCase
{
    [TestMethod]
    public void TestLoadMyDictionary() 
    {
        AhoCorasickDoubleArrayTrieSegment segment
            = new AhoCorasickDoubleArrayTrieSegment("data/dictionary/CoreNatureDictionary.mini.txt");
        HanLP.Config.ShowTermNature = false;
        AssertEquals("[江西, 鄱阳湖, 干枯]", segment.seg("江西鄱阳湖干枯").ToString());
    }
}