namespace com.hankcs.hanlp.dictionary;


public class CoreBiGramTableDictionaryTest : TestCase
{
    public void testReload() 
    {
        int biFrequency = CoreBiGramTableDictionary.getBiFrequency("高性能", "计算");
        CoreBiGramTableDictionary.reload();
        assertEquals(biFrequency, CoreBiGramTableDictionary.getBiFrequency("高性能", "计算"));
    }
}