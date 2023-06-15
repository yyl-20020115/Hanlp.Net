namespace com.hankcs.hanlp.algorithm;

using com.hankcs.hanlp;

[TestClass]
public class LongestCommonSubsequenceTest : TestCase
{
    String a = "Tom Hanks";
    String b = "Hankcs";
    [TestMethod]
    public void testCompute() 
    {
        assertEquals(5, LongestCommonSubsequence.compute(a, b));
    }

}