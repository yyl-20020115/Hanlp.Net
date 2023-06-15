namespace com.hankcs.hanlp.model.crf;


[TestClass]
public class CRFNERecognizerTest : TestCase
{
    public static readonly string CORPUS = "data/test/pku98/199801.txt";
    public static String NER_MODEL_PATH = "data/model/crf/pku199801/ner.txt";
    [TestMethod]
    public void testTrain() 
    {
        CRFTagger tagger = new CRFNERecognizer(null);
        tagger.train(CORPUS, NER_MODEL_PATH);
    }
    [TestMethod]
    public void testLoad() 
    {
        CRFTagger tagger = new CRFNERecognizer(NER_MODEL_PATH);
    }
    [TestMethod]
    public void testConvert() 
    {
        CRFTagger tagger = new CRFNERecognizer(null);
        tagger.convertCorpus(CORPUS, "data/test/crf/ner-corpus.tsv");
    }
    [TestMethod]
    public void testDumpTemplate() 
    {
        CRFTagger tagger = new CRFNERecognizer(null);
        tagger.dumpTemplate("data/test/crf/ner-template.txt");
    }
}