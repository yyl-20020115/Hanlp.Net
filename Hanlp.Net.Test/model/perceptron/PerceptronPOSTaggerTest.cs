using com.hankcs.hanlp.corpus;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.perceptron;


[TestClass]

public class PerceptronPOSTaggerTest : TestCase
{
    [TestMethod]
    public void TestTrain()
    {
        PerceptronTrainer trainer = new POSTrainer();
        trainer.Train(PKU.PKU199801_TRAIN, PKU.POS_MODEL); // 训练
        PerceptronPOSTagger tagger = new PerceptronPOSTagger(PKU.POS_MODEL); // 加载
        Console.WriteLine(string.Join(' ',tagger.Tag("他", "的", "希望", "是", "希望", "上学"))); // 预测
        AbstractLexicalAnalyzer analyzer = new AbstractLexicalAnalyzer(new PerceptronSegmenter(), tagger); // 构造词法分析器
        Console.WriteLine(analyzer.Analyze("李狗蛋的希望是希望上学")); // 分词+词性标注
    }
    [TestMethod]
    public void TestCompress()
    {
        PerceptronPOSTagger tagger = new PerceptronPOSTagger();
        tagger.getModel().compress(0.01);
        double[] scores = tagger.evaluate("data/test/pku98/199801.txt");
        Console.WriteLine(scores[0]);
        tagger.getModel().save(HanLP.Config.PerceptronPOSModelPath + ".small");
    }
}