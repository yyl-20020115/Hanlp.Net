namespace com.hankcs.hanlp.model.perceptron;

public class DemoTrainCWS
{
    public static void Main(String[] args) 
    {
        PerceptronTrainer trainer = new CWSTrainer();
        PerceptronTrainer.Result result = trainer.Train(
                "data/test/pku98/199801.txt",
                Config.CWS_MODEL_FILE
        );
        Console.WriteLine("准确率F1:{0}\n", result.GetAccuracy());
        PerceptronSegmenter segment = new PerceptronSegmenter(result.Model);
        // 也可以用
//        Segment segment = new AveragedPerceptronSegment(POS_MODEL_FILE);
        Console.WriteLine(segment.Segment("商品与服务"));
    }
}