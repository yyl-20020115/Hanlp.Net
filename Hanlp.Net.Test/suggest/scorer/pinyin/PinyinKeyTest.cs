using com.hankcs.hanlp.algorithm;

namespace com.hankcs.hanlp.suggest.scorer.pinyin;


[TestClass]
public class PinyinKeyTest : TestCase
{
    [TestMethod]
    public void testConstruct() 
    {
        PinyinKey pinyinKeyA = new PinyinKey("专题分析");
        PinyinKey pinyinKeyB = new PinyinKey("教室资格");
//        Console.WriteLine(pinyinKeyA);
//        Console.WriteLine(pinyinKeyB);
        assertEquals(1, LongestCommonSubstring.compute(pinyinKeyA.getFirstCharArray(), pinyinKeyB.getFirstCharArray()));
    }
}