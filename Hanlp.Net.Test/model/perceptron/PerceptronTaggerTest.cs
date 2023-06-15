namespace com.hankcs.hanlp.model.perceptron;


[TestClass]

public class PerceptronTaggerTest : TestCase
{
    [TestMethod]
    public void testEmptyInput() 
    {
        PerceptronPOSTagger tagger = new PerceptronPOSTagger();
        tagger.tag(new List<String>());
    }
}