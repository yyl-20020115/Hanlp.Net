namespace com.hankcs.hanlp.algorithm;

[TestClass]
public class LongestCommonSubstringTest : TestCase
{
    readonly string a = "www.hankcs.com";
    readonly string b = "hankcs";
    [TestMethod]
    public void TestCompute()
    {
//        Console.WriteLine(LongestCommonSubstring.compute(a.ToCharArray(), b.ToCharArray()));
        assertEquals(6, LongestCommonSubstring.compute(a.ToCharArray(), b.ToCharArray()));
    }
    [TestMethod]
    public void TestLongestCommonSubstring()
    {
//        Console.WriteLine(LongestCommonSubstring.compute(a, b));
        assertEquals(6, LongestCommonSubstring.compute(a, b));
    }
}