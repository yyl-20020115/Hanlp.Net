namespace com.hankcs.hanlp.algorithm;

[TestClass]
public class LongestCommonSubstringTest : TestCase
{
    String a = "www.hankcs.com";
    String b = "hankcs";
    [TestMethod]
    public void testCompute() 
    {
//        Console.WriteLine(LongestCommonSubstring.compute(a.ToCharArray(), b.ToCharArray()));
        assertEquals(6, LongestCommonSubstring.compute(a.ToCharArray(), b.ToCharArray()));
    }
    [TestMethod]
    public void testLongestCommonSubstring() 
    {
//        Console.WriteLine(LongestCommonSubstring.compute(a, b));
        assertEquals(6, LongestCommonSubstring.compute(a, b));
    }
}