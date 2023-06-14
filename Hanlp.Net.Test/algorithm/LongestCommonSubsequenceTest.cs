namespace com.hankcs.hanlp.algorithm;


public class LongestCommonSubsequenceTest : TestCase
{
    String a = "Tom Hanks";
    String b = "Hankcs";
    public void testCompute() 
    {
        assertEquals(5, LongestCommonSubsequence.compute(a, b));
    }

}