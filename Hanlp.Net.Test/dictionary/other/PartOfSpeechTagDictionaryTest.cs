namespace com.hankcs.hanlp.dictionary.other;


public class PartOfSpeechTagDictionaryTest : TestCase
{
    public void testTranslate() 
    {
        assertEquals("名词", PartOfSpeechTagDictionary.translate("n"));
    }
}