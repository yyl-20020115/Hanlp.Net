using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.seg.NShort;


[TestClass]

public class NShortSegmentTest : TestCase
{
    [TestMethod]
    public void testParse() 
    {
        List<List<Term>> wordResults = new LinkedList<List<Term>>();
        wordResults.add(NShortSegment.parse("3-4月"));
        wordResults.add(NShortSegment.parse("3-4月份"));
        wordResults.add(NShortSegment.parse("3-4季"));
        wordResults.add(NShortSegment.parse("3-4年"));
        wordResults.add(NShortSegment.parse("3-4人"));
        wordResults.add(NShortSegment.parse("2014年"));
        wordResults.add(NShortSegment.parse("04年"));
        wordResults.add(NShortSegment.parse("12点半"));
        wordResults.add(NShortSegment.parse("1.abc"));

//        for (List<Term> result : wordResults)
//        {
//            Console.WriteLine(result);
//        }
    }
    [TestMethod]
    public void testIssue691() 
    {
//        HanLP.Config.enableDebug();
        StandardTokenizer.SEGMENT.enableCustomDictionary(false);
        Segment nShortSegment = new NShortSegment().enableCustomDictionary(false).enablePlaceRecognize(true).enableOrganizationRecognize(true);
//        Console.WriteLine(nShortSegment.seg("今天，刘志军案的关键人物,山西女商人丁书苗在市二中院出庭受审。"));
//        Console.WriteLine(nShortSegment.seg("今日消费5,513.58元"));
    }
}