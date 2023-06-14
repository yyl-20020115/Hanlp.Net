namespace com.hankcs.hanlp.corpus.dictionary;


public class TMDictionaryMakerTest : TestCase
{
    public void testCreate() 
    {
        TMDictionaryMaker tmDictionaryMaker = new TMDictionaryMaker();
        tmDictionaryMaker.addPair("ab", "cd");
        tmDictionaryMaker.addPair("ab", "cd");
        tmDictionaryMaker.addPair("ab", "Y");
        tmDictionaryMaker.addPair("ef", "gh");
        tmDictionaryMaker.addPair("ij", "kl");
        tmDictionaryMaker.addPair("ij", "kl");
        tmDictionaryMaker.addPair("ij", "kl");
        tmDictionaryMaker.addPair("X", "Y");
//        Console.WriteLine(tmDictionaryMaker);
    }
}