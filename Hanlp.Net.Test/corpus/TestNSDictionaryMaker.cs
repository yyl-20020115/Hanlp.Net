namespace com.hankcs.hanlp.corpus;


public class TestNSDictionaryMaker {

    public static void main(String[] args)
    {
        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/2014_dictionary.txt");
        final NSDictionaryMaker nsDictionaryMaker = new NSDictionaryMaker(dictionary);
        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\", new CorpusLoader.Handler()
        {
            //@Override
            public void handle(Document document)
            {
                nsDictionaryMaker.compute(document.getComplexSentenceList());
            }
        });
        nsDictionaryMaker.saveTxtTo("D:\\JavaProjects\\HanLP\\data\\test\\place\\ns");
    }
}
