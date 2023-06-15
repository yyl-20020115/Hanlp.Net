namespace com.hankcs.hanlp.classification.models;


/**
 * 储存学习过程中的数据
 */
public class NaiveBayesModel : AbstractModel
{

    /**
     * 先验概率的对数值 log( P(c) )
     */
    public Dictionary<int, Double> logPriors = new HashMap<int, Double>();

    /**
     * 似然对数值 log( P(x|c) )
     */
    public Dictionary<int, Dictionary<int, Double>> logLikelihoods = new HashMap<int, Dictionary<int, Double>>();

    /**
     * 训练样本数
     */
    public int n = 0;
    /**
     * 类别数
     */
    public int c = 0;
    /**
     * 特征数
     */
    public int d = 0;
}