namespace com.hankcs.hanlp.utility;

[TestClass]
public class TextUtilityTest : TestCase
{
    [TestMethod]
    public void testIsAllSingleByte() 
    {
        assertEquals(false, TextUtility.isAllSingleByte("中文a"));
        assertEquals(true, TextUtility.isAllSingleByte("abcABC!@#"));
    }

    [TestMethod]
    public void testChineseNum()
    {
        assertEquals(true, TextUtility.isAllChineseNum("两千五百万"));
        assertEquals(true, TextUtility.isAllChineseNum("两千分之一"));
        assertEquals(true, TextUtility.isAllChineseNum("几十"));
        assertEquals(true, TextUtility.isAllChineseNum("十几"));
        assertEquals(false,TextUtility.isAllChineseNum("上来"));
    }

    [TestMethod]
    public void testArabicNum()
    {
        assertEquals(true, TextUtility.isAllNum("2.5"));
        assertEquals(true, TextUtility.isAllNum("3600"));
        assertEquals(true, TextUtility.isAllNum("500万"));
        assertEquals(true, TextUtility.isAllNum("87.53%"));
        assertEquals(true, TextUtility.isAllNum("５５０"));
        assertEquals(true, TextUtility.isAllNum("１０％"));
        assertEquals(true, TextUtility.isAllNum("98．1％"));
        assertEquals(false, TextUtility.isAllNum("，"));
    }
}