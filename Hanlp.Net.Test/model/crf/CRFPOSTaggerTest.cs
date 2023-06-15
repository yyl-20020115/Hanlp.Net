namespace com.hankcs.hanlp.model.crf;

[TestClass]
public class CRFPOSTaggerTest : TestCase
{
    public static readonly string CORPUS = "data/test/pku98/199801.txt";
    public static String POS_MODEL_PATH = HanLP.Config.CRFPOSModelPath;
    [TestMethod]
    public void testTrain() 
    {
        CRFPOSTagger tagger = new CRFPOSTagger(null); // 创建空白标注器
        tagger.train(PKU.PKU199801_TRAIN, PKU.POS_MODEL); // 训练
        tagger = new CRFPOSTagger(PKU.POS_MODEL); // 加载
        Console.WriteLine(Arrays.ToString(tagger.tag("他", "的", "希望", "是", "希望", "上学"))); // 预测
        AbstractLexicalAnalyzer analyzer = new AbstractLexicalAnalyzer(new PerceptronSegmenter(), tagger); // 构造词法分析器
        Console.WriteLine(analyzer.analyze("李狗蛋的希望是希望上学")); // 分词+词性标注
    }
    [TestMethod]
    public void testLoad() 
    {
        CRFPOSTagger tagger = new CRFPOSTagger("data/model/crf/pku199801/pos.txt");
        Console.WriteLine(Arrays.ToString(tagger.tag("我", "的", "希望", "是", "希望", "和平")));
    }
    [TestMethod]
    public void testConvert() 
    {
        CRFTagger tagger = new CRFPOSTagger(null);
        tagger.convertCorpus(CORPUS, "data/test/crf/pos-corpus.tsv");
    }
    [TestMethod]
    public void testDumpTemplate() 
    {
        CRFTagger tagger = new CRFPOSTagger(null);
        tagger.dumpTemplate("data/test/crf/pos-template.txt");
    }
}