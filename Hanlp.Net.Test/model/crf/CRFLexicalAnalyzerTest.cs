namespace com.hankcs.hanlp.model.crf;


[TestClass]
public class CRFLexicalAnalyzerTest : TestCase
{
    [TestMethod]

    public void TestLoad() 
    {
        CRFLexicalAnalyzer analyzer = new CRFLexicalAnalyzer();
        String[] tests = new String[]{
            "商品和服务",
            "总统普京与特朗普通电话讨论太空探索技术公司",
            "微软公司於1975年由比爾·蓋茲和保羅·艾倫創立，18年啟動以智慧雲端、前端為導向的大改組。"
        };
//        for (String sentence : tests)
//        {
//            Console.WriteLine(analyzer.analyze(sentence));
//            Console.WriteLine(analyzer.seg(sentence));
//        }
    }
}