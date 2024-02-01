using com.hankcs.hanlp.classification.models;
using com.hankcs.hanlp.classification.utilities;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.classification.classifiers;



[TestClass]
public class NaiveBayesClassifierTest : TestCase
{
    public static readonly string CORPUS_FOLDER = TestUtility.EnsureTestData("ChnSentiCorp情感分析酒店评论", "http://hanlp.linrunsoft.com/release/corpus/ChnSentiCorp.zip");

    private static readonly string MODEL_PATH = "data/test/classification.ser";
    private Dictionary<String, String[]> trainingDataSet;

    [TestInitialize]
    public override void SetUp() 
    {
        base.SetUp();
    }

    private void LoadDataSet()
    {
        if (trainingDataSet != null) return;
        Console.WriteLine("正在从 {0} 中加载分类语料...\n", CORPUS_FOLDER);
        trainingDataSet = TextProcessUtility.loadCorpus(CORPUS_FOLDER);
        foreach (var entry in trainingDataSet)
        {
            Console.WriteLine("{0} : {1} 个文档\n", entry.Key, entry.Value.Length);
        }
    }
    [TestMethod]
    public void TestTrain() 
    {
        LoadDataSet();
        NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier();
        long start = DateTime.Now.Microsecond;
        Console.WriteLine("开始训练...");
        naiveBayesClassifier.Train(trainingDataSet);
        Console.WriteLine("训练耗时：{0} ms\n", DateTime.Now.Microsecond - start);
        // 将模型保存
        IOUtil.saveObjectTo(naiveBayesClassifier.getNaiveBayesModel(), MODEL_PATH);
    }
    [TestMethod]
    public void TestPredictAndAccuracy()
    {
        // 加载模型
        NaiveBayesModel model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        if (model == null)
        {
            TestTrain();
            model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        }
        NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier(model);
        // 预测单个文档
        String path = CORPUS_FOLDER + "/体育/0004.txt";
        String text = IOUtil.readTxt(path);
        String label = naiveBayesClassifier.Classify(text);
        String title = text.Split("\\n")[0].Replace("\\s", "");
        Console.WriteLine("《{0}》 属于分类 【{1}】\n", title, label);
        text = "2016年中国铁路完成固定资产投资将达8000亿元";
        title = text;
        label = naiveBayesClassifier.Classify(text);
        Console.WriteLine("《{0}》 属于分类 【{1}】\n", title, label);
        text = "国安2016赛季年票开售比赛场次减少套票却上涨";
        title = text;
        label = naiveBayesClassifier.Classify(text);
        Console.WriteLine("《{0}》 属于分类 【{1}】\n", title, label);
        // 对将训练集作为测试，计算准确率
        int totalDocuments = 0;
        int rightDocuments = 0;
        LoadDataSet();
        long start = DateTime.Now.Microsecond;
        Console.WriteLine("开始评测...");
        foreach (var entry in trainingDataSet)
        {
            String category = entry.Key;
            String[] documents = entry.Value;

            totalDocuments += documents.Length;
            foreach (String document in documents)
            {
                if (category.Equals(naiveBayesClassifier.Classify(document))) ++rightDocuments;
            }
        }
        Console.WriteLine("准确率 {0} / {1} = {2}%\n速度 {3} 文档/秒", 
                            rightDocuments, totalDocuments,
                            rightDocuments / (double) totalDocuments * 100.0,
                            totalDocuments / (double) (DateTime.Now.Microsecond - start) * 1000.0
        );
    }
    [TestMethod]
    public void TestPredict()
    {
        // 加载模型
        NaiveBayesModel model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        if (model == null)
        {
            TestTrain();
            model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        }
        NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier(model);
        var pMap = naiveBayesClassifier.predict("国安2016赛季年票开售比赛场次减少套票却上涨");
        foreach (var entry in pMap)
        {
            Console.WriteLine(entry);
        }
    }
}