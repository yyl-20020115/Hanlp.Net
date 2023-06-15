using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.seg.Dijkstra;


[TestClass]

public class DijkstraSegmentTest : SegmentTestCase
{
    [TestMethod]
    public void TestWrongName()
    {
        Segment segment = new DijkstraSegment();
        List<Term> termList = segment.seg("好像向你借钱的人跑了");
        assertNoNature(termList, Nature.nr);
//        Console.WriteLine(termList);
    }
    [TestMethod]
    public void TestIssue770() 
    {
//        HanLP.Config.enableDebug();
        Segment segment = new DijkstraSegment();
        List<Term> termList = segment.seg("为什么我扔出的瓶子没有人回复？");
//        Console.WriteLine(termList);
        assertSegmentationHas(termList, "瓶子 没有");
    }
}