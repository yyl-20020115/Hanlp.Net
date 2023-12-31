using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.perceptron;

[TestClass]

public class PerceptronNameGenderClassifierTest : TestCase
{
    public static String TRAINING_SET = "data/test/cnname/train.csv";
    public static String TESTING_SET = "data/test/cnname/test.csv";
    public static String MODEL = "data/test/cnname.bin";

    [TestInitialize]
    public override void SetUp() 
    {
        base.SetUp();
        TestUtility.EnsureTestData("cnname", "http://file.hankcs.com/corpus/cnname.zip");
    }
    [TestMethod]
    public void TestTrain() 
    {
        PerceptronNameGenderClassifier classifier = new PerceptronNameGenderClassifier();
        Console.WriteLine(classifier.train(TRAINING_SET, 10, false));
        classifier.model.save(MODEL, classifier.model.featureMap.entrySet(), 0, true);
        PredictNames(classifier);
    }

    public static void PredictNames(PerceptronNameGenderClassifier classifier)
    {
        String[] names = new String[]{"赵建军", "沈雁冰", "陆雪琪", "李冰冰"};
        foreach (String name in names)
        {
            Console.Write("%s=%s\n", name, classifier.predict(name));
        }
    }

    [TestMethod]
    public void TestEvaluate()
    {
        PerceptronNameGenderClassifier classifier = new PerceptronNameGenderClassifier(MODEL);
        Console.WriteLine(classifier.evaluate(TESTING_SET));
    }

    [TestMethod]
    public void TestPrediction()
    {
        PerceptronNameGenderClassifier classifier = new PerceptronNameGenderClassifier(MODEL);
        PredictNames(classifier);
    }
}