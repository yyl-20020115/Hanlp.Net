using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.corpus.document;

namespace com.hankcs.hanlp.corpus;


public class TestNTDcitionaryMaker
{

    public static void Main(String[] args)
    {
        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/2014_dictionary.txt");
         NTDictionaryMaker ntDictionaryMaker = new NTDictionaryMaker(dictionary);
        // CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\", new CorpusLoader.Handler()
        CorpusLoader.walk("data/test/nt/test/", new CT(ntDictionaryMaker));
        ntDictionaryMaker.saveTxtTo("D:\\JavaProjects\\HanLP\\data\\test\\organization\\nt");
    }
    public class CT: CorpusLoader.Handler
    {
        private NTDictionaryMaker ntDictionaryMaker;
        public CT(NTDictionaryMaker ntDictionaryMaker)
        {
            this.ntDictionaryMaker = ntDictionaryMaker;
        }
        //@Override
        public void handle(Document document)
        {
            ntDictionaryMaker.compute(document.getComplexSentenceList());
        }
    }
}
