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
//            Console.printf("%d / %d...", id++, sentenceList.Count);
//            long start = DateTime.Now.Microsecond;
//            List<Term> termList = new ();
//            for (CoNLLWord word : sentence.word)
//            {
//                termList.Add(new Term(word.LEMMA, Nature.valueOf(word.POSTAG)));
//            }
//            CoNLLSentence Out = CRFDependencyParser.compute(termList);
//            evaluator.e(sentence, Out);
//            Console.WriteLine("done in " + (DateTime.Now.Microsecond - start) + " ms.");
//        }
//        Console.WriteLine(evaluator);
//    }
}