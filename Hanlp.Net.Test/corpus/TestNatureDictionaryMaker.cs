using com.hankcs.hanlp.corpus.dictionary;
using com.hankcs.hanlp.corpus.document;
using com.hankcs.hanlp.corpus.util;

namespace com.hankcs.hanlp.corpus;


public class TestNatureDictionaryMaker
{

    public class NDM : CorpusLoader.Handler
    {
        public NatureDictionaryMaker dictionaryMaker;
        //@Override
        public void handle(Document document)
        {
            dictionaryMaker.compute(CorpusUtil.convert2CompatibleList(document.getSimpleSentenceList(false))); // 再打一遍不拆分的
            dictionaryMaker.compute(CorpusUtil.convert2CompatibleList(document.getSimpleSentenceList(true)));  // 先打一遍拆分的
        }
    }
    public static void main(String[] args)
    {
        //        makeCoreDictionary("D:\\JavaProjects\\CorpusToolBox\\data\\2014", "data/dictionary/CoreNatureDictionary.txt");
        //        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/CoreNatureDictionary.txt");
        NatureDictionaryMaker dictionaryMaker = new NatureDictionaryMaker();
        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new NDM() { dictionaryMaker = dictionaryMaker }); ;
        dictionaryMaker.saveTxtTo("data/test/CoreNatureDictionary");
    }

}
