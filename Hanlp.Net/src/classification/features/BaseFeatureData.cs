using com.hankcs.hanlp.classification.corpus;
using com.hankcs.hanlp.collection.trie.bintrie;

namespace com.hankcs.hanlp.classification.features;



/**
 * 储存所有必需的统计数据,尽量不要存太多东西在这里,因为多个分类器都用这个结构,所以里面的数据仅保留必需的数据
 */
public class BaseFeatureData
{
    /**
     * 样本数量
     */
    public int n;

    /**
     * 一个特征在类目中分别出现几次(键是特征,值的键是类目)
     */
    public int[][] featureCategoryJointCount;

    /**
     * 每个类目中的文档数量
     */
    public int[] categoryCounts;

    /**
     * 新的特征映射表
     */
    public BinTrie<int> wordIdTrie;

    /**
     * 构造一个空白的统计对象
     */
    public BaseFeatureData(IDataSet dataSet)
    {
        Catalog catalog = dataSet.getCatalog();
        Lexicon lexicon = dataSet.getLexicon();
        n = dataSet.size();
        featureCategoryJointCount = new int[lexicon.size()][];
        for(int i = 0; i < lexicon.size(); i++)
        {
            featureCategoryJointCount[i]=new int[ catalog.size()];
        }
        categoryCounts = new int[catalog.size()];

        // 执行统计
        foreach (Document document in dataSet)
        {
            ++categoryCounts[document.category];

            foreach (KeyValuePair<int, int[]> entry in document.tfMap)
            {
                featureCategoryJointCount[entry.Key][document.category] += 1;
            }
        }
    }
}
