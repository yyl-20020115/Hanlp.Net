namespace com.hankcs.hanlp.model.perceptron;


public class PerceptronNameGenderClassifierTest : TestCase
{
    public static String TRAINING_SET = "data/test/cnname/train.csv";
    public static String TESTING_SET = "data/test/cnname/test.csv";
    public static String MODEL = "data/test/cnname.bin";

    //@Override
    public void setUp() 
    {
        super.setUp();
        TestUtility.ensureTestData("cnname", "http://file.hankcs.com/corpus/cnname.zip");
    }

    public void testTrain() 
    {
        PerceptronNameGenderClassifier classifier = new PerceptronNameGenderClassifier();
        Console.WriteLine(classifier.train(TRAINING_SET, 10, false));
        classifier.model.save(MODEL, classifier.model.featureMap.entrySet(), 0, true);
        predictNames(classifier);
    }

    public static void predictNames(PerceptronNameGenderClassifier classifier)
    {
        String[] names = new String[]{"赵建军", "沈雁冰", "陆雪琪", "李冰冰"};
        for (String name : names)
        {
            System.out.printf("%s=%s\n", name, classifier.predict(name));
        }
    }


    public void testEvaluate() 
    {
        PerceptronNameGenderClassifier classifier = new PerceptronNameGenderClassifier(MODEL);
        Console.WriteLine(classifier.evaluate(TESTING_SET));
    }

    public void testPrediction() 
    {
        PerceptronNameGenderClassifier classifier = new PerceptronNameGenderClassifier(MODEL);
        predictNames(classifier);
    }
}