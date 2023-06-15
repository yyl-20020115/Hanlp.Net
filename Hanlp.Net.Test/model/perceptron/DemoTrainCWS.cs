namespace com.hankcs.hanlp.model.perceptron;

public class DemoTrainCWS
{
    public static void Main(String[] args) 
    {
        PerceptronTrainer trainer = new CWSTrainer();
        PerceptronTrainer.Result result = trainer.train(
                "data/test/pku98/199801.txt",
                Config.CWS_MODEL_FILE
        );
        Console.WriteLine("准确率F1:{0}\n", result.getAccuracy());
        PerceptronSegmenter segment = new PerceptronSegmenter(result.getModel());
        // 也可以用
//        Segment segment = new AveragedPerceptronSegment(POS_MODEL_FILE);
        Console.WriteLine(segment.segment("商品与服务"));
    }
}