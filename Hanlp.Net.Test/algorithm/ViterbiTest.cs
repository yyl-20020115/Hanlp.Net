namespace com.hankcs.hanlp.algorithm;


[TestClass]
public class ViterbiTest : TestCase
{
    public enum Weather
    {
        Rainy,
        Sunny,
    }
    public enum Activity
    {
        walk,
        shop,
        clean,
    }
    static int[] states = new int[]{ (int)Weather.Rainy, (int)Weather.Sunny};
    static int[] observations = new int[]{
        (int)Activity.walk,
        (int)Activity.shop,
        (int)Activity.clean};
    double[] start_probability = new double[]{0.6, 0.4};
    double[][] transititon_probability = new double[][]{
        new double[]{0.7, 0.3},
        new double[]{0.4, 0.6},
    };
    double[][] emission_probability = new double[][]{
        new double[]{0.1, 0.4, 0.5},
        new double[]{0.6, 0.3, 0.1},
    };
    [TestMethod]
    public void TestCompute() 
    {
        for (int i = 0; i < start_probability.Length; ++i)
        {
            start_probability[i] = -Math.Log(start_probability[i]);
        }
        for (int i = 0; i < transititon_probability.Length; ++i)
        {
            for (int j = 0; j < transititon_probability[i].Length; ++j)
            {
                transititon_probability[i][j] = -Math.Log(transititon_probability[i][j]);
            }
        }
        for (int i = 0; i < emission_probability.Length; ++i)
        {
            for (int j = 0; j < emission_probability[i].Length; ++j)
            {
                emission_probability[i][j] = -Math.Log(emission_probability[i][j]);
            }
        }
        int[] result = Viterbi.Compute(observations, states, start_probability, transititon_probability, emission_probability);
        foreach (int r in result)
        {
//            Console.print(Weather.values()[r] + " ");
        }
//        Console.WriteLine();
    }
}