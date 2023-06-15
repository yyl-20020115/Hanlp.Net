namespace com.hankcs.hanlp.model.crf;


[TestClass]
public class LogLinearModelTest : TestCase
{
    [TestMethod]
    public void testLoad() 
    {
        LogLinearModel model = new LogLinearModel("/Users/hankcs/Downloads/crfpp-msr-cws-model.txt");
    }
}