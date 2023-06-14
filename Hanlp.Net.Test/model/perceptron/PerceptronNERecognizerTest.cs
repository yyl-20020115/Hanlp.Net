namespace com.hankcs.hanlp.model.perceptron;


public class PerceptronNERecognizerTest : TestCase
{
    public void testEmptyInput() 
    {
        PerceptronNERecognizer recognizer = new PerceptronNERecognizer();
        recognizer.recognize(new String[0], new String[0]);
    }
}