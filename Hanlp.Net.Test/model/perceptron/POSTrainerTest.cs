namespace com.hankcs.hanlp.model.perceptron;



public class POSTrainerTest : TestCase
{

    public void testTrain() 
    {
        PerceptronTrainer trainer = new POSTrainer();
        trainer.train("data/test/pku98/199801.txt", Config.POS_MODEL_FILE);
    }

    public void testLoad() 
    {
        PerceptronPOSTagger tagger = new PerceptronPOSTagger(Config.POS_MODEL_FILE);
        Console.WriteLine(Arrays.toString(tagger.tag("中国 交响乐团 谭利华 在 布达拉宫 广场 演出".split(" "))));
    }
}