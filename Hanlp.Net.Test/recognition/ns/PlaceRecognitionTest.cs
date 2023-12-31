using com.hankcs.hanlp.seg.Dijkstra;

namespace com.hankcs.hanlp.recognition.ns;

[TestClass]

public class PlaceRecognitionTest : TestCase
{
    [TestMethod]
    public void TestSeg() 
    {
//        HanLP.Config.enableDebug();
        DijkstraSegment segment = new DijkstraSegment();
        segment.enableJapaneseNameRecognize(false);
        segment.enableTranslatedNameRecognize(false);
        segment.enableNameRecognize(false);
        segment.enableCustomDictionary(false);

        segment.enablePlaceRecognize(true);
//        Console.WriteLine(segment.seg("南翔向宁夏固原市彭阳县红河镇黑牛沟村捐赠了挖掘机"));
    }
}