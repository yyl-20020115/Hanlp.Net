using com.hankcs.hanlp.corpus;
using com.hankcs.hanlp.model.hmm;
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron.utility;


[TestClass]
public class UtilityTest : TestCase
{
    [TestMethod]
    public void TestCombineNER() 
    {
        NERTagSet nerTagSet = new HMMNERecognizer().getNERTagSet();
        String[] nerArray = Utility.reshapeNER(Utility.convertSentenceToNER(Sentence.create("萨哈夫/nr 说/v ，/w 伊拉克/ns 将/d 同/p [联合国/nt 销毁/v 伊拉克/ns 大规模/b 杀伤性/n 武器/n 特别/a 委员会/n]/nt 继续/v 保持/v 合作/v 。/w"), nerTagSet))[2];
        Console.WriteLine(Arrays.ToString(nerArray));
        Console.WriteLine(Utility.combineNER(nerArray, nerTagSet));
    }

    [TestMethod]
    public void TestEvaluateNER() 
    {
        var scores = Utility.evaluateNER(new PerceptronNERecognizer(), PKU.PKU199801_TEST);
        Utility.printNERScore(scores);
    }
}