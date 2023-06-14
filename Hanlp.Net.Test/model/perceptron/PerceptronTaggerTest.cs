namespace com.hankcs.hanlp.model.perceptron;



public class PerceptronTaggerTest : TestCase
{
    public void testEmptyInput() 
    {
        PerceptronPOSTagger tagger = new PerceptronPOSTagger();
        tagger.tag(new ArrayList<String>());
    }
}