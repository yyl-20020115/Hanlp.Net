namespace com.hankcs.hanlp;

[TestClass]
public class HanLPTest : TestCase
{
    [TestMethod]

    public void testNewSegment() 
    {
        assertTrue(HanLP.newSegment("维特比") instanceof ViterbiSegment);
        assertTrue(HanLP.newSegment("感知机") instanceof PerceptronLexicalAnalyzer);
    }
    [TestMethod]

    public void testDicUpdate()
    {
        Console.WriteLine(HanLP.segment("大数据是一个新词汇！"));
    }
}