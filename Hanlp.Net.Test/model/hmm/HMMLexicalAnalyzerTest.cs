namespace com.hankcs.hanlp.model.hmm;


[TestClass]
public class HMMLexicalAnalyzerTest : TestCase
{

    public static readonly string CORPUS_PATH = "data/test/pku98/199801.txt";
    [TestMethod]
    public void testTrain() 
    {
        HMMSegmenter segmenter = new HMMSegmenter();
        segmenter.train(CORPUS_PATH);
        HMMPOSTagger tagger = new HMMPOSTagger();
        tagger.train(CORPUS_PATH);
        HMMNERecognizer recognizer = new HMMNERecognizer();
        recognizer.train(CORPUS_PATH);
        HMMLexicalAnalyzer analyzer = new HMMLexicalAnalyzer(segmenter, tagger, recognizer);
        Console.WriteLine(analyzer.analyze("我的希望是希望人们幸福"));
    }
}