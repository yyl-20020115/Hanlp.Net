namespace com.hankcs.hanlp.model.perceptron.model;


[TestClass]
public class LinearModelTest : TestCase
{
    public static readonly string MODEL_FILE = "data/pku_mini.bin";

//    public void testLoad() 
//    {
//        LinearModel model = new LinearModel(MODEL_FILE);
//        PerceptronTra.vsiner trainer = new CWSTrainer();
//        double[] prf = trainer.evaluate("icwb2-data/mini/pku_development.txt",
//                                                              model
//        );
//        Out.printf("Performance - P:%.2f R:%.2f F:%.2f\n", prf[0], prf[1], prf[2]);
//    }
}