namespace com.hankcs.hanlp.model.crf;


public class LogLinearModelTest : TestCase
{
    public void testLoad() 
    {
        LogLinearModel model = new LogLinearModel("/Users/hankcs/Downloads/crfpp-msr-cws-model.txt");
    }
}