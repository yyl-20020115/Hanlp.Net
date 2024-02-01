using com.hankcs.hanlp.classification.corpus;
using com.hankcs.hanlp.classification.features;
using com.hankcs.hanlp.classification.models;
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.classification.classifiers;




/**
 * 实现一个基于多项式贝叶斯模型的文本分类器
 */
public class NaiveBayesClassifier : AbstractClassifier
{

    private NaiveBayesModel model;

    /**
     * 由训练结果构造一个贝叶斯分类器，通常用来加载磁盘中的分类器
     *
     * @param naiveBayesModel
     */
    public NaiveBayesClassifier(NaiveBayesModel naiveBayesModel)
    {
        this.model = naiveBayesModel;
    }

    /**
     * 构造一个空白的贝叶斯分类器，通常准备用来进行训练
     */
    public NaiveBayesClassifier()
        : this(null)
    {
        ;
    }

    /**
     * 获取训练结果
     *
     * @return
     */
    public NaiveBayesModel getNaiveBayesModel()
    {
        return model;
    }

    public override void Train(IDataSet dataSet)
    {
        logger._out("原始数据集大小:%d\n", dataSet.Count);
        //选择最佳特征
        BaseFeatureData featureData = SelectFeatures(dataSet);

        //初始化分类器所用的数据
        model = new NaiveBayesModel();
        model.n = featureData.n; //样本数量
        model.d = featureData.featureCategoryJointCount.Length; //特征数量

        model.c = featureData.categoryCounts.Length; //类目数量
        model.logPriors = new ();

        int sumCategory;
        for (int category = 0; category < featureData.categoryCounts.Length; category++)
        {
            sumCategory = featureData.categoryCounts[category];
            model.logPriors.Add(category, Math.Log((double) sumCategory / model.n));
        }

        //拉普拉斯平滑处理（又称加一平滑），这时需要估计每个类目下的实例
        Dictionary<int, Double> featureOccurrencesInCategory = new ();

        Double featureOccSum;
        foreach (int category in model.logPriors.Keys)
        {
            featureOccSum = 0.0;
            for (int feature = 0; feature < featureData.featureCategoryJointCount.Length; feature++)
            {

                featureOccSum += featureData.featureCategoryJointCount[feature][category];
            }
            featureOccurrencesInCategory.Add(category, featureOccSum);
        }

        //对数似然估计
        int count;
        int[] featureCategoryCounts;
        double logLikelihood;
        foreach (int category in model.logPriors.Keys)
        {
            for (int feature = 0; feature < featureData.featureCategoryJointCount.Length; feature++)
            {

                featureCategoryCounts = featureData.featureCategoryJointCount[feature];

                count = featureCategoryCounts[category];

                logLikelihood = Math.Log((count + 1.0) / (featureOccurrencesInCategory[(category)] + model.d));
                if (!model.logLikelihoods.ContainsKey(feature))
                {
                    model.logLikelihoods.Add(feature, new ());
                }
                model.logLikelihoods[(feature)].Add(category, logLikelihood);
            }
        }
        logger._out("贝叶斯统计结束\n");
        model.catalog = dataSet.Catalog.ToArray();
        model.tokenizer = dataSet.GetTokenizer();
        model.wordIdTrie = featureData.wordIdTrie;
    }

    public override AbstractModel GetModel()
    {
        return model;
    }

    public override Dictionary<string, Double> Predict(string text) 
    {
        if (model == null)
        {
            throw new InvalidOperationException("未训练模型！无法执行预测！");
        }
        if (text == null)
        {
            throw new InvalidOperationException("参数 text == null");
        }

        //分词，创建文档
        Document doc = new Document(model.wordIdTrie, model.tokenizer.Segment(text));

        return Predict(doc);
    }

    //@Override
    public override double[] Categorize(Document document) 
    {
        int category;
        int feature;
        int occurrences;
        Double logprob;

        double[] predictionScores = new double[model.catalog.Length];
        foreach (KeyValuePair<int, Double> entry1 in model.logPriors)
        {
            category = entry1.Key;
            logprob = entry1.Value; //用类目的对数似然初始化概率

            //对文档中的每个特征
            foreach (KeyValuePair<int, int[]> entry2 in document.tfMap)
            {
                feature = entry2.Key;

                if (!model.logLikelihoods.ContainsKey(feature))
                {
                    continue; //如果在模型中找不到就跳过了
                }

                occurrences = entry2.Value[0]; //获取其在文档中的频次

                logprob += occurrences * model.logLikelihoods[(feature)][(category)]; //将对数似然乘上频次
            }
            predictionScores[category] = logprob;
        }

        if (configProbabilityEnabled) MathUtility.normalizeExp(predictionScores);
        return predictionScores;
    }

    /**
     * 统计特征并且执行特征选择，返回一个FeatureStats对象，用于计算模型中的概率
     *
     * @param dataSet
     * @return
     */
    protected BaseFeatureData SelectFeatures(IDataSet dataSet)
    {
        ChiSquareFeatureExtractor chiSquareFeatureExtractor = new ChiSquareFeatureExtractor();

        logger.start("使用卡方检测选择特征中...");
        //FeatureStats对象包含文档中所有特征及其统计信息
        BaseFeatureData featureData = ChiSquareFeatureExtractor.ExtractBasicFeatureData(dataSet); //执行统计

        //我们传入这些统计信息到特征选择算法中，得到特征与其分值
        Dictionary<int, Double> selectedFeatures = chiSquareFeatureExtractor.ChiSquare(featureData);

        //从统计数据中删掉无用的特征并重建特征映射表
        int[][] featureCategoryJointCount = new int[selectedFeatures.Count][];
        featureData.wordIdTrie = new BinTrie<int>();
        string[] wordIdArray = dataSet.Lexicon.GetWordIdArray();
        int p = -1;
        foreach (int feature in selectedFeatures.Keys)
        {
            featureCategoryJointCount[++p] = featureData.featureCategoryJointCount[feature];
            featureData.wordIdTrie.Add(wordIdArray[feature], p);
        }
        logger.finish(",选中特征数:%d / %d = %.2f%%\n", featureCategoryJointCount.Length,
                      featureData.featureCategoryJointCount.Length,
                      featureCategoryJointCount.Length / (double)featureData.featureCategoryJointCount.Length * 100.0);
        featureData.featureCategoryJointCount = featureCategoryJointCount;

        return featureData;
    }
}
