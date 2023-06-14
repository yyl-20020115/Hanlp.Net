namespace com.hankcs.hanlp.dependency;



public class MaxEntDependencyParserTest : TestCase
{
    public void testMaxEntParser() 
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
//            System.out.printf("%d / %d...", id++, sentenceList.size());
//            long start = System.currentTimeMillis();
//            List<Term> termList = new LinkedList<Term>();
//            for (CoNLLWord word : sentence.word)
//            {
//                termList.add(new Term(word.LEMMA, Nature.valueOf(word.POSTAG)));
//            }
//            CoNLLSentence out = CRFDependencyParser.compute(termList);
//            evaluator.e(sentence, out);
//            Console.WriteLine("done in " + (System.currentTimeMillis() - start) + " ms.");
//        }
//        Console.WriteLine(evaluator);
//    }
}