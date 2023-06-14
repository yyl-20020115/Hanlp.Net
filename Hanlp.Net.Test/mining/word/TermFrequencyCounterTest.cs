namespace com.hankcs.hanlp.mining.word;


public class TermFrequencyCounterTest : TestCase
{
    public void testGetKeywords() 
    {
        TermFrequencyCounter counter = new TermFrequencyCounter();
        counter.add("加油加油中国队！");
        Console.WriteLine(counter);
        Console.WriteLine(counter.getKeywords("女排夺冠，观众欢呼女排女排女排！"));
    }
}