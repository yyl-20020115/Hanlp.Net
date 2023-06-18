using com.hankcs.hanlp.collection.trie.datrie;

namespace com.hankcs.hanlp.classification.features;


/**
 * 词权重计算
 */
public interface IFeatureWeighter : Serializable
{
    /**
     * 计算权重
     *
     * @param feature 词的id
     * @return 权重
     */
    double weight(int feature, int tf);
}