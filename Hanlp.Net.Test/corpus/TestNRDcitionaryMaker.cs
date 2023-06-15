using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.corpus.document;
using com.hankcs.hanlp.corpus.document.sentence.word;

namespace com.hankcs.hanlp.corpus;



public class TestNRDcitionaryMaker
{

    public static void Main(String[] args)
    {
        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/2014_dictionary.txt");
        NRDictionaryMaker nrDictionaryMaker = new NRDictionaryMaker(dictionary);
        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\", new NRD());
        nrDictionaryMaker.saveTxtTo("D:\\JavaProjects\\HanLP\\data\\test\\person\\nr1");
    }
    public class NRD : CorpusLoader.Handler
    {
        NRDictionaryMaker nrDictionaryMaker;
        //@Override
        public void handle(Document document)
        {
            List<List<Word>> simpleSentenceList = document.getSimpleSentenceList();
            List<List<IWord>> compatibleList = new List<List<IWord>>();
            foreach (List<Word> wordList in simpleSentenceList)
            {
                compatibleList.Add(new (wordList));
            }
            nrDictionaryMaker.compute(compatibleList);
        }
    }
}
