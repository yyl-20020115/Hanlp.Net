namespace com.hankcs.hanlp.classification.features;

/**
 * 仅仅使用TF的权重计算方式
 */
public class TfOnlyFeatureWeighter : IFeatureWeighter
{
    public double Weight(int feature, int tf) => tf;
}