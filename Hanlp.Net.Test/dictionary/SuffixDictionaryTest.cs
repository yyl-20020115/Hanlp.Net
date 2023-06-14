namespace com.hankcs.hanlp.dictionary;



public class SuffixDictionaryTest : TestCase
{
    SuffixDictionary dictionary = new SuffixDictionary();
    public void setUp() 
    {
        super.setUp();
        dictionary.addAll(Predefine.POSTFIX_SINGLE);
        dictionary.addAll(Predefine.POSTFIX_MUTIPLE);
    }

    public void testGet() 
    {
        String total = Predefine.POSTFIX_SINGLE;
        for (int i = 0; i < total.length(); ++i)
        {
            String single = String.valueOf(total.charAt(i));
            assertEquals(1, dictionary.get(single));
        }
        for (String single : Predefine.POSTFIX_MUTIPLE)
        {
            assertEquals(single.length(), dictionary.get(single));
        }
    }

    public void testEndsWith() 
    {
        assertEquals(true, dictionary.endsWith("黄冈市"));
        assertEquals(false, dictionary.endsWith("黄冈一二三"));
    }

    public void testLongest() 
    {
        assertEquals(2, dictionary.getLongestSuffixLength("巴尔干半岛"));
    }

//    public void testDump() 
//    {
//        DictionaryMaker dictionaryMaker = new DictionaryMaker();
//        for (Map.Entry<String, Integer> entry : PlaceSuffixDictionary.dictionary.entrySet())
//        {
//            dictionaryMaker.add(entry.getKey(), NS.H.toString());
//        }
//        dictionaryMaker.saveTxtTo("data/dictionary/place/suffix.txt");
//    }
}