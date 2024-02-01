using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.model.perceptron;


[TestClass]

public class PerceptronLexicalAnalyzerTest : TestCase
{
    PerceptronLexicalAnalyzer analyzer;

    [TestInitialize]
    public override void SetUp() 
    {
        analyzer = new PerceptronLexicalAnalyzer(Config.CWS_MODEL_FILE, Config.POS_MODEL_FILE, Config.NER_MODEL_FILE);
    }
    [TestMethod]
    public void TestIssue() 
    {
//        Console.WriteLine(analyzer.seg(""));
        foreach (Term term in analyzer.seg("张三丰，刘五郎，黄三元，张一楠，王三强，丁一楠，李四光，闻一多，赵一楠，李四"))
        {
            if (term.nature == Nature.w) continue;
            AssertEquals(Nature.nr, term.nature);
        }
    }
    [TestMethod]
    public void TestLearn() 
    {
        analyzer.learn("我/r 在/p 浙江/ns 金华/ns 出生/v");
        AssertTrue(analyzer.analyze("我在浙江金华出生").ToString().Contains("金华/ns"));
        AssertTrue(analyzer.analyze("我的名字叫金华").ToString().Contains("金华/nr"));
    }
    [TestMethod]
    public void TestEmptyInput() 
    {
        analyzer.segment("");
        analyzer.seg("");
    }
    [TestMethod]
    public void testCustomDictionary() 
    {
        analyzer.enableCustomDictionary(true);
        AssertTrue(CustomDictionary.Contains("一字长蛇阵"));
        String text = "张飞摆出一字长蛇阵如入无人之境，孙权惊呆了";
//        Console.WriteLine(analyzer.analyze(text));
        AssertTrue(analyzer.analyze(text).ToString().Contains(" 一字长蛇阵/"));
    }
    [TestMethod]
    public void testCustomNature() 
    {
        AssertTrue(CustomDictionary.insert("饿了么", "ntc 1"));
        analyzer.enableCustomDictionaryForcing(true);
        AssertEquals("美团/n 与/p 饿了么/ntc 争夺/v 外卖/v 市场/n", analyzer.analyze("美团与饿了么争夺外卖市场").ToString());
    }
    [TestMethod]
    public void testIndexMode() 
    {
        analyzer.enableIndexMode(true);
        String text = "来到美国纽约现代艺术博物馆参观";
        List<Term> termList = analyzer.seg(text);
        AssertEquals("[来到/v, 美国纽约现代艺术博物馆/ns, 美国/ns, 纽约/ns, 现代/t, 艺术/n, 博物馆/n, 参观/v]", termList.ToString());
        foreach (Term term in termList)
        {
            AssertEquals(term.word, text.Substring(term.offset, term.Length));
        }
        analyzer.enableIndexMode(false);
    }
    [TestMethod]
    public void testOffset() 
    {
        analyzer.enableIndexMode(false);
        String text = "来到美国纽约现代艺术博物馆参观";
        List<Term> termList = analyzer.seg(text);
        foreach (Term term in termList)
        {
            AssertEquals(term.word, text.Substring(term.offset, term.Length));
        }
    }
    [TestMethod]
    public void testNormalization() 
    {
        analyzer.enableCustomDictionary(false);
        String text = "來到美國紐約現代藝術博物館參觀?";
        Sentence sentence = analyzer.analyze(text);
//        Console.WriteLine(sentence);
        AssertEquals("來到/v [美國/ns 紐約/ns 現代/t 藝術/n 博物館/n]/ns 參觀/v ?/w", sentence.ToString());
        List<Term> termList = analyzer.seg(text);
//        Console.WriteLine(termList);
        AssertEquals("[來到/v, 美國紐約現代藝術博物館/ns, 參觀/v, ?/w]", termList.ToString());
    }
    [TestMethod]
    public void testWhiteSpace() 
    {
        CharTable.CONVERT[' '] = '!';
        CharTable.CONVERT['\t'] = '!';
        Sentence sentence = analyzer.analyze("\"你好， 我想知道： 风是从哪里来; \t雷是从哪里来； 雨是从哪里来？\"");
        foreach (IWord word in sentence)
        {
            if (!word.Label.Equals("w"))
            {
                AssertFalse(word.Value.Contains(" "));
                AssertFalse(word.Value.Contains("\t"));
            }
        }
    }
    [TestMethod]
    public void testCustomDictionaryForcing() 
    {
        String text = "银川普通人与川普通电话讲四川普通话";
        CustomDictionary.insert("川普", "NRF 1");

        analyzer.enableCustomDictionaryForcing(false);
        Console.WriteLine(analyzer.analyze(text));

        analyzer.enableCustomDictionaryForcing(true);
        Console.WriteLine(analyzer.analyze(text));
    }
    [TestMethod]
    public void testRules() 
    {
        analyzer.enableRuleBasedSegment(true);
        Console.WriteLine(analyzer.analyze("これは微软公司於1975年由比爾·蓋茲和保羅·艾倫創立，18年啟動以智慧雲端、前端為導向的大改組。"));
    }
}