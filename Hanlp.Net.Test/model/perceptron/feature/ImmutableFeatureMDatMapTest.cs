using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.model.perceptron.model;

namespace com.hankcs.hanlp.model.perceptron.feature;


[TestClass]

public class ImmutableFeatureMDatMapTest : TestCase
{
    [TestMethod]
    public void TestCompress() 
    {
        LinearModel model = new LinearModel(HanLP.Config.PerceptronCWSModelPath);
        model.compress(0.1);
    }
    [TestMethod]
    public void TestFeatureMap() 
    {
        LinearModel model = new LinearModel(HanLP.Config.PerceptronCWSModelPath);
        ImmutableFeatureMDatMap featureMap = (ImmutableFeatureMDatMap) model.featureMap;
        MutableDoubleArrayTrieInteger dat = featureMap.dat;
        Console.WriteLine(featureMap.        Count);
        Console.WriteLine(featureMap.entrySet().Count);
        Console.WriteLine(featureMap.idOf("\u0001/\u00014"));
        Dictionary<String, int> map = new Dictionary<String, int>();
        foreach (var entry in dat.entrySet())
        {
            map.Add(entry.Key, entry.Value);
            AssertEquals(entry.Value, dat.get(entry.Key));
        }
        Console.WriteLine(map.Count);
        AssertEquals(dat.Count, map.Count);
    }
}