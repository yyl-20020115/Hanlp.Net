namespace com.hankcs.hanlp.corpus;



public class TestNRDcitionaryMaker
{

    public static void main(String[] args)
    {
        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/2014_dictionary.txt");
        NRDictionaryMaker nrDictionaryMaker = new NRDictionaryMaker(dictionary);
        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\", new CorpusLoader.Handler()
        {
            //@Override
            public void handle(Document document)
            {
                List<List<Word>> simpleSentenceList = document.getSimpleSentenceList();
                List<List<IWord>> compatibleList = new LinkedList<List<IWord>>();
                for (List<Word> wordList : simpleSentenceList)
                {
                    compatibleList.add(new LinkedList<IWord>(wordList));
                }
                nrDictionaryMaker.compute(compatibleList);
            }
        });
        nrDictionaryMaker.saveTxtTo("D:\\JavaProjects\\HanLP\\data\\test\\person\\nr1");
    }

}
