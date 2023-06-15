namespace com.hankcs.hanlp.model.perceptron;


[TestClass]

public class POSTrainerTest : TestCase
{
    [TestMethod]
    public void testTrain() 
    {
        PerceptronTrainer trainer = new POSTrainer();
        trainer.train("data/test/pku98/199801.txt", Config.POS_MODEL_FILE);
    }
    [TestMethod]
    public void testLoad() 
    {
        PerceptronPOSTagger tagger = new PerceptronPOSTagger(Config.POS_MODEL_FILE);
        Console.WriteLine(Arrays.ToString(tagger.tag("中国 交响乐团 谭利华 在 布达拉宫 广场 演出".split(" "))));
    }
}