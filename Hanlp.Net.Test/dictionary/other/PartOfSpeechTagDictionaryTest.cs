namespace com.hankcs.hanlp.dictionary.other;

[TestClass]
public class PartOfSpeechTagDictionaryTest : TestCase
{
    [TestMethod]
    public void TestTranslate() 
    {
        AssertEquals("名词", PartOfSpeechTagDictionary.translate("n"));
    }
}