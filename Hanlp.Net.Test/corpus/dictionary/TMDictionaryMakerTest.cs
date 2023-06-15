namespace com.hankcs.hanlp.corpus.dictionary;


[TestClass]
public class TMDictionaryMakerTest : TestCase
{
    [TestMethod]
    public void TestCreate()
    {
        TMDictionaryMaker tmDictionaryMaker = new ();
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