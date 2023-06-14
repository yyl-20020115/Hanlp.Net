namespace com.hankcs.hanlp.algorithm;


public class LongestCommonSubstringTest : TestCase
{
    String a = "www.hankcs.com";
    String b = "hankcs";
    public void testCompute() 
    {
//        Console.WriteLine(LongestCommonSubstring.compute(a.toCharArray(), b.toCharArray()));
        assertEquals(6, LongestCommonSubstring.compute(a.toCharArray(), b.toCharArray()));
    }

    public void testLongestCommonSubstring() 
    {
//        Console.WriteLine(LongestCommonSubstring.compute(a, b));
        assertEquals(6, LongestCommonSubstring.compute(a, b));
    }
}