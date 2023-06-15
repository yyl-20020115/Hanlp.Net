namespace com.hankcs.hanlp.model.perceptron;

[TestClass]


public class NERTrainerTest : TestCase
{
    [TestMethod]
    public void testTrain() 
    {
        PerceptronTrainer trainer = new NERTrainer();
        trainer.train("data/test/pku98/199801.txt", Config.NER_MODEL_FILE);
    }

    [TestMethod]
    public void testTag() 
    {
        PerceptronNERecognizer recognizer = new PerceptronNERecognizer(Config.NER_MODEL_FILE);
        Console.WriteLine(string.Join(' ',recognizer.recognize("吴忠市 乳制品 公司 谭利华 来到 布达拉宫 广场".Split(" "), "ns n n nr p ns n".Split(" "))));
    }
}