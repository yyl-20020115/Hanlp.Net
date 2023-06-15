using com.hankcs.hanlp.corpus.dictionary;

namespace com.hankcs.hanlp.corpus;


public class TestNTDcitionaryMaker
{

    public static void main(String[] args)
    {
        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/2014_dictionary.txt");
         NTDictionaryMaker ntDictionaryMaker = new NTDictionaryMaker(dictionary);
        // CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\", new CorpusLoader.Handler()
        CorpusLoader.walk("data/test/nt/test/", new CorpusLoader.Handler()
        {
            //@Override
            public void handle(Document document)
            {
                ntDictionaryMaker.compute(document.getComplexSentenceList());
            }
        });
        ntDictionaryMaker.saveTxtTo("D:\\JavaProjects\\HanLP\\data\\test\\organization\\nt");
    }

}
