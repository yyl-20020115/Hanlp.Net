using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.seg.NShort;


[TestClass]

public class NShortSegmentTest : TestCase
{
    [TestMethod]
    public void TestParse() 
    {
        List<List<Term>> wordResults = new ();
        wordResults.Add(NShortSegment.parse("3-4月"));
        wordResults.Add(NShortSegment.parse("3-4月份"));
        wordResults.Add(NShortSegment.parse("3-4季"));
        wordResults.Add(NShortSegment.parse("3-4年"));
        wordResults.Add(NShortSegment.parse("3-4人"));
        wordResults.Add(NShortSegment.parse("2014年"));
        wordResults.Add(NShortSegment.parse("04年"));
        wordResults.Add(NShortSegment.parse("12点半"));
        wordResults.Add(NShortSegment.parse("1.abc"));

//        for (List<Term> result : wordResults)
//        {
//            Console.WriteLine(result);
//        }
    }
    [TestMethod]
    public void TestIssue691() 
    {
//        HanLP.Config.enableDebug();
        StandardTokenizer.SEGMENT.enableCustomDictionary(false);
        Segment nShortSegment = new NShortSegment().enableCustomDictionary(false).enablePlaceRecognize(true).enableOrganizationRecognize(true);
//        Console.WriteLine(nShortSegment.seg("今天，刘志军案的关键人物,山西女商人丁书苗在市二中院出庭受审。"));
//        Console.WriteLine(nShortSegment.seg("今日消费5,513.58元"));
    }
}