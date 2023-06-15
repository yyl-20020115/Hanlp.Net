namespace com.hankcs.hanlp.algorithm;

using com.hankcs.hanlp;

[TestClass]
public class LongestCommonSubsequenceTest : TestCase
{
    readonly string a = "Tom Hanks";
    readonly string b = "Hankcs";
    [TestMethod]
    public void TestCompute()
    {
        AssertEquals(5, LongestCommonSubsequence.compute(a, b));
    }

}