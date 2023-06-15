namespace com.hankcs.hanlp.dictionary.other;

[TestClass]
public class PartOfSpeechTagDictionaryTest : TestCase
{
    [TestMethod]
    public void testTranslate() 
    {
        assertEquals("名词", PartOfSpeechTagDictionary.translate("n"));
    }
}