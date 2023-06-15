namespace com.hankcs.hanlp.model.perceptron.utility;


[TestClass]

public class IOUtilityTest : TestCase
{
    [TestMethod]
    public void testReadLineToArray() 
    {
        String line = " 你好   世界 ! ";
        String[] array = IOUtility.readLineToArray(line);
        Console.WriteLine(string.Join(',',array));
    }
}