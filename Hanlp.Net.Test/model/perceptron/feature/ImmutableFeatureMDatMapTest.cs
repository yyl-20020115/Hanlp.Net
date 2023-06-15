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
        Console.WriteLine(featureMap.size());
        Console.WriteLine(featureMap.entrySet().size());
        Console.WriteLine(featureMap.idOf("\u0001/\u00014"));
        TreeMap<String, int> map = new TreeMap<String, int>();
        for (Map.Entry<String, int> entry : dat.entrySet())
        {
            map.put(entry.getKey(), entry.getValue());
            assertEquals(entry.getValue().intValue(), dat.get(entry.getKey()));
        }
        Console.WriteLine(map.size());
        assertEquals(dat.size(), map.size());
    }
}