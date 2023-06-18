namespace com.hankcs.hanlp.mining.word;

[TestClass]

public class TfIdfCounterTest : TestCase
{
    [TestMethod]
    public void TestGetKeywords() 
    {
        TfIdfCounter counter = new TfIdfCounter();
        counter.Add("《女排夺冠》", "女排北京奥运会夺冠");
        counter.Add("《羽毛球男单》", "北京奥运会的羽毛球男单决赛");
        counter.Add("《女排》", "中国队女排夺北京奥运会金牌重返巅峰，观众欢呼女排女排女排！");
        counter.compute();

        foreach (Object id in counter.documents())
        {
            Console.WriteLine(id + " : " + counter.getKeywordsOf(id, 3));
        }

        Console.WriteLine(counter.getKeywords("奥运会反兴奋剂", 2));
    }
}