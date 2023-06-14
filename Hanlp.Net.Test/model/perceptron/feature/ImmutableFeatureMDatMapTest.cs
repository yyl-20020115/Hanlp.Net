namespace com.hankcs.hanlp.model.perceptron.feature;



public class ImmutableFeatureMDatMapTest : TestCase
{
    public void testCompress() 
    {
        LinearModel model = new LinearModel(HanLP.Config.PerceptronCWSModelPath);
        model.compress(0.1);
    }

    public void testFeatureMap() 
    {
        LinearModel model = new LinearModel(HanLP.Config.PerceptronCWSModelPath);
        ImmutableFeatureMDatMap featureMap = (ImmutableFeatureMDatMap) model.featureMap;
        MutableDoubleArrayTrieInteger dat = featureMap.dat;
        Console.WriteLine(featureMap.size());
        Console.WriteLine(featureMap.entrySet().size());
        Console.WriteLine(featureMap.idOf("\u0001/\u00014"));
        TreeMap<String, Integer> map = new TreeMap<String, Integer>();
        for (Map.Entry<String, Integer> entry : dat.entrySet())
        {
            map.put(entry.getKey(), entry.getValue());
            assertEquals(entry.getValue().intValue(), dat.get(entry.getKey()));
        }
        Console.WriteLine(map.size());
        assertEquals(dat.size(), map.size());
    }
}