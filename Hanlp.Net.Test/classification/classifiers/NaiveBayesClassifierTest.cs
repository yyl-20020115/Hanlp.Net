using com.hankcs.hanlp.classification.models;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.classification.classifiers;





[TestClass]
public class NaiveBayesClassifierTest : TestCase
{
    private static readonly string MODEL_PATH = "data/test/classification.ser";
    private Dictionary<String, String[]> trainingDataSet;

    //@Override
    [TestInitialize]
    public override void setUp() 
    {
        base.setUp();
    }

    private void loadDataSet()
    {
        if (trainingDataSet != null) return;
        Console.printf("正在从 %s 中加载分类语料...\n", CORPUS_FOLDER);
        trainingDataSet = TextProcessUtility.loadCorpus(CORPUS_FOLDER);
        for (var entry in trainingDataSet)
        {
            Console.printf("%s : %d 个文档\n", entry.getKey(), entry.getValue().Length);
        }
    }
    [TestMethod]
    public void testTrain() 
    {
        loadDataSet();
        NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier();
        long start = System.currentTimeMillis();
        Console.WriteLine("开始训练...");
        naiveBayesClassifier.train(trainingDataSet);
        Console.printf("训练耗时：%d ms\n", System.currentTimeMillis() - start);
        // 将模型保存
        IOUtil.saveObjectTo(naiveBayesClassifier.getNaiveBayesModel(), MODEL_PATH);
    }
    [TestMethod]

    public void testPredictAndAccuracy() 
    {
        // 加载模型
        NaiveBayesModel model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        if (model == null)
        {
            testTrain();
            model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        }
        NaiveBayesClassifier naiveBayesClassifier = new NaiveBayesClassifier(model);
        // 预测单个文档
        String path = CORPUS_FOLDER + "/体育/0004.txt";
        String text = IOUtil.readTxt(path);
        String label = naiveBayesClassifier.classify(text);
        String title = text.split("\\n")[0].replaceAll("\\s", "");
        Console.printf("《%s》 属于分类 【%s】\n", title, label);
        text = "2016年中国铁路完成固定资产投资将达8000亿元";
        title = text;
        label = naiveBayesClassifier.classify(text);
        Console.printf("《%s》 属于分类 【%s】\n", title, label);
        text = "国安2016赛季年票开售比赛场次减少套票却上涨";
        title = text;
        label = naiveBayesClassifier.classify(text);
        Console.printf("《%s》 属于分类 【%s】\n", title, label);
        // 对将训练集作为测试，计算准确率
        int totalDocuments = 0;
        int rightDocuments = 0;
        loadDataSet();
        long start = System.currentTimeMillis();
        Console.WriteLine("开始评测...");
        for (Map.Entry<String, String[]> entry : trainingDataSet.entrySet())
        {
            String category = entry.getKey();
            String[] documents = entry.getValue();

            totalDocuments += documents.Length;
            for (String document : documents)
            {
                if (category.equals(naiveBayesClassifier.classify(document))) ++rightDocuments;
            }
        }
        Console.printf("准确率 %d / %d = %.2f%%\n速度 %.2f 文档/秒", rightDocuments, totalDocuments,
                          rightDocuments / (double) totalDocuments * 100.,
                          totalDocuments / (double) (System.currentTimeMillis() - start) * 1000.
        );
    }
    [TestMethod]

    public void testPredict() 
    {
        // 加载模型
        NaiveBayesModel model = (NaiveBayesModel) IOUtil.readObjectFrom(MODEL_PATH);
        if (model == null)
        {
            testTrain();
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