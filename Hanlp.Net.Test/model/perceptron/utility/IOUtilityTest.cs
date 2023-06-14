namespace com.hankcs.hanlp.model.perceptron.utility;



public class IOUtilityTest : TestCase
{
    public void testReadLineToArray() 
    {
        String line = " 你好   世界 ! ";
        String[] array = IOUtility.readLineToArray(line);
        Console.WriteLine(Arrays.toString(array));
    }
}