namespace com.hankcs.hanlp.dictionary.ts;

[TestClass]

public class TraditionalChineseDictionaryTest : TestCase
{
    [TestMethod]
    public void TestF2J()
    {
        AssertEquals("草莓是红色的", TraditionalChineseDictionary.convertToSimplifiedChinese("士多啤梨是紅色的"));
    }

    [TestMethod]
    public void TestJ2F() 
    {
        AssertEquals("草莓是紅色的", SimplifiedChineseDictionary.convertToTraditionalChinese("草莓是红色的"));
    }

    [TestMethod]
    public void TestInterface() 
    {
        AssertEquals("“以后等你当上皇后，就能买草莓庆祝了”", HanLP.convertToSimplifiedChinese("「以後等妳當上皇后，就能買士多啤梨慶祝了」"));
        AssertEquals("「以後等你當上皇后，就能買草莓慶祝了」", HanLP.convertToTraditionalChinese("“以后等你当上皇后，就能买草莓庆祝了”"));
    }
}
