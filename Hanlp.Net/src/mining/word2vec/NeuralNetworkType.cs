namespace com.hankcs.hanlp.mining.word2vec;


/**
 * 神经网络类型
 */
public class NeuralNetworkType
{
    public virtual float GetDefaultInitialLearningRate() => 0.0f;

    /**
     * 更快，对高频词的准确率更高
     * Faster, slightly better accuracy for frequent words
     */
    protected class NeuralNetworkType_005 : NeuralNetworkType
    {
        public override float GetDefaultInitialLearningRate() => 0.05f;
    }
    public NeuralNetworkType CBOW() => new NeuralNetworkType_005();
    protected class NeuralNetworkType_0025 : NeuralNetworkType
    {
        public override float GetDefaultInitialLearningRate() => 0.025f;
    }
    /**
     * 较慢，对低频词的准确率更高
     */
    public NeuralNetworkType SKIP_GRAM()=>new NeuralNetworkType_0025();

    /**
     * @return 默认学习率
     */
}