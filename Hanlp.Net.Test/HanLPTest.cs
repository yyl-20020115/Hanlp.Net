namespace com.hankcs.hanlp;


public class HanLPTest : TestCase
{
    public void testNewSegment() 
    {
        assertTrue(HanLP.newSegment("维特比") instanceof ViterbiSegment);
        assertTrue(HanLP.newSegment("感知机") instanceof PerceptronLexicalAnalyzer);
    }

    public void testDicUpdate()
    {
        Console.WriteLine(HanLP.segment("大数据是一个新词汇！"));
    }
}