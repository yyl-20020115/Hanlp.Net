using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.seg.Viterbi;

namespace com.hankcs.hanlp;

[TestClass]
public class HanLPTest : TestCase
{
    [TestMethod]
    public void TestNewSegment() 
    {
        AssertTrue(HanLP.newSegment("维特比") is ViterbiSegment);
        AssertTrue(HanLP.newSegment("感知机") is PerceptronLexicalAnalyzer);
    }
    [TestMethod]

    public void TestDicUpdate()
    {
        Console.WriteLine(HanLP.segment("大数据是一个新词汇！"));
    }
}