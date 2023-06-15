namespace com.hankcs.hanlp.model.perceptron;

[TestClass]

public class PerceptronNERecognizerTest : TestCase
{
    [TestMethod]
    public void TestEmptyInput() 
    {
        PerceptronNERecognizer recognizer = new PerceptronNERecognizer();
        recognizer.recognize(new String[0], new String[0]);
    }
}