using com.hankcs.hanlp.collection.trie.datrie;
using System.Runtime.Serialization;

namespace com.hankcs.hanlp.classification.features;


/**
 * 词权重计算
 */
public interface IFeatureWeighter
{
    /**
     * 计算权重
     *
     * @param feature 词的id
     * @return 权重
     */
    double Weight(int feature, int tf);
}