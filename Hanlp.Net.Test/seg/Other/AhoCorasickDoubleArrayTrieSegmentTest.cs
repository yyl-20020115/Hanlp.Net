namespace com.hankcs.hanlp.seg.Other;


public class AhoCorasickDoubleArrayTrieSegmentTest : TestCase
{
    public void testLoadMyDictionary() 
    {
        AhoCorasickDoubleArrayTrieSegment segment
            = new AhoCorasickDoubleArrayTrieSegment("data/dictionary/CoreNatureDictionary.mini.txt");
        HanLP.Config.ShowTermNature = false;
        assertEquals("[江西, 鄱阳湖, 干枯]", segment.seg("江西鄱阳湖干枯").toString());
    }
}