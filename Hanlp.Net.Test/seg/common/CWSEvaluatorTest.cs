namespace com.hankcs.hanlp.seg.common;

[TestClass]

public class CWSEvaluatorTest : TestCase
{
    [TestMethod]
    public void TestGetPRF() 
    {
        CWSEvaluator evaluator = new CWSEvaluator();
        evaluator.compare("结婚 的 和 尚未 结婚 的", "结婚 的 和尚 未结婚 的");
        CWSEvaluator.Result prf = evaluator.getResult(false);
        AssertEquals(0.6f, prf.P);
        AssertEquals(0.5f, prf.R);
    }
}