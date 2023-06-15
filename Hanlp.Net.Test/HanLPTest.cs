using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.seg.Viterbi;

namespace com.hankcs.hanlp;

[TestClass]
public class HanLPTest : TestCase
{
    [TestMethod]
    public void TestNewSegment() 
    {
        assertTrue(HanLP.newSegment("维特比") is ViterbiSegment);
        assertTrue(HanLP.newSegment("感知机") is PerceptronLexicalAnalyzer);
    }
    [TestMethod]

    public void TestDicUpdate()
    {
        Console.WriteLine(HanLP.segment("大数据是一个新词汇！"));
    }
}