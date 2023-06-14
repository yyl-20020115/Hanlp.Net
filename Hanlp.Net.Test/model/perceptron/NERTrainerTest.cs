namespace com.hankcs.hanlp.model.perceptron;



public class NERTrainerTest : TestCase
{
    public void testTrain() 
    {
        PerceptronTrainer trainer = new NERTrainer();
        trainer.train("data/test/pku98/199801.txt", Config.NER_MODEL_FILE);
    }

    public void testTag() 
    {
        PerceptronNERecognizer recognizer = new PerceptronNERecognizer(Config.NER_MODEL_FILE);
        Console.WriteLine(Arrays.toString(recognizer.recognize("吴忠市 乳制品 公司 谭利华 来到 布达拉宫 广场".split(" "), "ns n n nr p ns n".split(" "))));
    }
}