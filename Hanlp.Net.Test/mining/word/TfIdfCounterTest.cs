namespace com.hankcs.hanlp.mining.word;


public class TfIdfCounterTest : TestCase
{
    public void testGetKeywords() 
    {
        TfIdfCounter counter = new TfIdfCounter();
        counter.add("《女排夺冠》", "女排北京奥运会夺冠");
        counter.add("《羽毛球男单》", "北京奥运会的羽毛球男单决赛");
        counter.add("《女排》", "中国队女排夺北京奥运会金牌重返巅峰，观众欢呼女排女排女排！");
        counter.compute();

        for (Object id : counter.documents())
        {
            Console.WriteLine(id + " : " + counter.getKeywordsOf(id, 3));
        }

        Console.WriteLine(counter.getKeywords("奥运会反兴奋剂", 2));
    }
}