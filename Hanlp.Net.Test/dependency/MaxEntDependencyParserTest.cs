namespace com.hankcs.hanlp.dependency;


[TestClass]
public class MaxEntDependencyParserTest : TestCase
{
    [TestMethod]
    public void TestMaxEntParser() 
    {
//        HanLP.Config.enableDebug();
//        Console.WriteLine(MaxEntDependencyParser.compute("我每天骑车上学"));
    }

//    public void testEvaluate() 
//    {
//        LinkedList<CoNLLSentence> sentenceList = CoNLLLoader.loadSentenceList("D:\\Doc\\语料库\\依存分析训练数据\\THU\\dev.conll");
//        Evaluator evaluator = new Evaluator();
//        int id = 1;
//        for (CoNLLSentence sentence : sentenceList)
//        {
//            Console.printf("%d / %d...", id++, sentenceList.size());
//            long start = DateTime.Now.Microsecond;
//            List<Term> termList = new LinkedList<Term>();
//            for (CoNLLWord word : sentence.word)
//            {
//                termList.add(new Term(word.LEMMA, Nature.valueOf(word.POSTAG)));
//            }
//            CoNLLSentence _out = CRFDependencyParser.compute(termList);
//            evaluator.e(sentence, _out);
//            Console.WriteLine("done in " + (DateTime.Now.Microsecond - start) + " ms.");
//        }
//        Console.WriteLine(evaluator);
//    }
}