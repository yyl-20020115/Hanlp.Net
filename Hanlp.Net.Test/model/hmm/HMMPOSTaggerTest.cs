using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.hmm;



[TestClass]
public class HMMPOSTaggerTest : TestCase
{
    [TestMethod]
    public void TestTrain() 
    {
        HMMPOSTagger tagger = new HMMPOSTagger(); // 创建词性标注器
//        HMMPOSTagger tagger = new HMMPOSTagger(new SecondOrderHiddenMarkovModel()); // 或二阶隐马
        tagger.train(PKU.PKU199801); // 训练
        Console.WriteLine(Arrays.ToString(tagger.Tag("他", "的", "希望", "是", "希望", "上学"))); // 预测
        AbstractLexicalAnalyzer analyzer = new AbstractLexicalAnalyzer(new PerceptronSegmenter(), tagger); // 构造词法分析器
        Console.WriteLine(analyzer.Analyze("他的希望是希望上学").translateLabels()); // 分词+词性标注
    }
}