namespace com.hankcs.hanlp.model.crf;


public class CRFNERecognizerTest : TestCase
{
    public static final String CORPUS = "data/test/pku98/199801.txt";
    public static String NER_MODEL_PATH = "data/model/crf/pku199801/ner.txt";
    public void testTrain() 
    {
        CRFTagger tagger = new CRFNERecognizer(null);
        tagger.train(CORPUS, NER_MODEL_PATH);
    }

    public void testLoad() 
    {
        CRFTagger tagger = new CRFNERecognizer(NER_MODEL_PATH);
    }

    public void testConvert() 
    {
        CRFTagger tagger = new CRFNERecognizer(null);
        tagger.convertCorpus(CORPUS, "data/test/crf/ner-corpus.tsv");
    }

    public void testDumpTemplate() 
    {
        CRFTagger tagger = new CRFNERecognizer(null);
        tagger.dumpTemplate("data/test/crf/ner-template.txt");
    }
}