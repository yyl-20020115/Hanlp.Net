using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.model.perceptron;

[TestClass]

public class CWSTrainerTest : TestCase
{

    public static readonly string SENTENCE = "香港特别行政区的张朝阳说商品和服务是三原县鲁桥食品厂的主营业务";
    [TestMethod]
    public void TestTrain() 
    {
        HanLP.Config.enableDebug();
        PerceptronTrainer trainer = new CWSTrainer();
        PerceptronTrainer.Result result = trainer.train(
                "data/test/pku98/199801.txt",
                Config.CWS_MODEL_FILE
        );
//        Console.printf("准确率F1:%.2f\n", result.prf[2]);
        PerceptronSegmenter segmenter = new PerceptronSegmenter(result.model);
        // 也可以用
//        Segment segmenter = new AveragedPerceptronSegment(POS_MODEL_FILE);
        Console.WriteLine(segmenter.segment("商品和服务?"));
    }

    [TestMethod]
    public void TestCWS() 
    {
        PerceptronSegmenter segmenter = new PerceptronSegmenter(Config.CWS_MODEL_FILE);
        segmenter.learn("下雨天 地面 积水");
        Console.WriteLine(segmenter.segment("下雨天地面积水分外严重"));
    }

    [TestMethod]
    public void TestCWSandPOS() 
    {
        Segment segmenter = new PerceptronLexicalAnalyzer(Config.CWS_MODEL_FILE, Config.POS_MODEL_FILE);
        Console.WriteLine(segmenter.seg(SENTENCE));
    }
    [TestMethod]
    public void TestCWSandPOSandNER() 
    {
        PerceptronLexicalAnalyzer segmenter = new PerceptronLexicalAnalyzer(Config.CWS_MODEL_FILE, Config.POS_MODEL_FILE, Config.NER_MODEL_FILE);
        Sentence sentence = segmenter.analyze(SENTENCE);
        Console.WriteLine(sentence);
        Console.WriteLine(segmenter.seg(SENTENCE));
        foreach(IWord word in sentence)
        {
            if (word is CompoundWord)
                Console.WriteLine(((CompoundWord) word).innerList);
        }
    }
    [TestMethod]
    public void TestCompareWithHanLP() 
    {
        Console.WriteLine(NLPTokenizer.segment(SENTENCE));
    }
}