namespace com.hankcs.hanlp.dictionary;

[TestClass]

public class CoreBiGramTableDictionaryTest : TestCase
{
    [TestMethod]
    public void testReload() 
    {
        int biFrequency = CoreBiGramTableDictionary.getBiFrequency("高性能", "计算");
        CoreBiGramTableDictionary.reload();
        assertEquals(biFrequency, CoreBiGramTableDictionary.getBiFrequency("高性能", "计算"));
    }
}