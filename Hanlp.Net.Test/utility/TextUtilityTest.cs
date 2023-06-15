namespace com.hankcs.hanlp.utility;

[TestClass]
public class TextUtilityTest : TestCase
{
    [TestMethod]
    public void TestIsAllSingleByte() 
    {
        AssertEquals(false, TextUtility.isAllSingleByte("中文a"));
        AssertEquals(true, TextUtility.isAllSingleByte("abcABC!@#"));
    }

    [TestMethod]
    public void TestChineseNum()
    {
        AssertEquals(true, TextUtility.isAllChineseNum("两千五百万"));
        AssertEquals(true, TextUtility.isAllChineseNum("两千分之一"));
        AssertEquals(true, TextUtility.isAllChineseNum("几十"));
        AssertEquals(true, TextUtility.isAllChineseNum("十几"));
        AssertEquals(false,TextUtility.isAllChineseNum("上来"));
    }

    [TestMethod]
    public void TestArabicNum()
    {
        AssertEquals(true, TextUtility.isAllNum("2.5"));
        AssertEquals(true, TextUtility.isAllNum("3600"));
        AssertEquals(true, TextUtility.isAllNum("500万"));
        AssertEquals(true, TextUtility.isAllNum("87.53%"));
        AssertEquals(true, TextUtility.isAllNum("５５０"));
        AssertEquals(true, TextUtility.isAllNum("１０％"));
        AssertEquals(true, TextUtility.isAllNum("98．1％"));
        AssertEquals(false, TextUtility.isAllNum("，"));
    }
}