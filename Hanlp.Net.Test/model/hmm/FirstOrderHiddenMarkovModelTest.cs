namespace com.hankcs.hanlp.model.hmm;




[TestClass]
public class FirstOrderHiddenMarkovModelTest : TestCase
{

    /**
     * 隐状态
     */
    public enum Status:int
    {
        Healthy,
        Fever,
    }

    /**
     * 显状态
     */
    public enum Feel:int
    {
        normal,
        cold,
        dizzy,
    }
    /**
     * 初始状态概率矩阵
     */
    public static float[] start_probability = new float[]{0.6f, 0.4f};
    /**
     * 状态转移概率矩阵
     */
    public static float[][] transition_probability = new float[][]{
        new float[]{0.7f, 0.3f},
        new float[] { 0.4f, 0.6f },
    };
    /**
     * 发射概率矩阵
     */
    public static float[][] emission_probability = new float[][]{
        new float[] { 0.5f, 0.4f, 0.1f },
        new float[] { 0.1f, 0.3f, 0.6f },
    };
    /**
     * 某个病人的观测序列
     */
    static int[] observations = new int[]{
        (int) Feel.normal,
        (int) Feel.cold, 
        (int) Feel.dizzy};
    static int[] status_set = new int[]
    {
        (int) Status.Healthy,
        (int) Status.Fever
    };

    [TestMethod]
    public void testGenerate() 
    {
        FirstOrderHiddenMarkovModel givenModel = new FirstOrderHiddenMarkovModel(start_probability, transition_probability, emission_probability);
        foreach (int[][] sample in givenModel.generate(3, 5, 2))
        {
            for (int t = 0; t < sample[0].Length; t++)
                Console.WriteLine("%s/%s ",
                    observations[sample[0][t]],
                    status_set[sample[1][t]]);
            Console.WriteLine(); 
        }
    }
    [TestMethod]
    public void testTrain() 
    {
        FirstOrderHiddenMarkovModel givenModel = new FirstOrderHiddenMarkovModel(start_probability, transition_probability, emission_probability);
        FirstOrderHiddenMarkovModel trainedModel = new FirstOrderHiddenMarkovModel();
        trainedModel.train(givenModel.generate(3, 10, 100000));
        AssertTrue(trainedModel.similar(givenModel));
    }
    [TestMethod]
    public void testPredict() 
    {
        FirstOrderHiddenMarkovModel model = new FirstOrderHiddenMarkovModel(start_probability, transition_probability, emission_probability);
        EvaluateModel(model);
    }
    [TestMethod]
    public void EvaluateModel(FirstOrderHiddenMarkovModel model)
    {
        int[] pred = new int[observations.Length];
        float prob = (float) Math.Exp(model.predict(observations, pred));
        int[] answer = { (int)Status.Healthy, (int)Status.Healthy, (int)Status.Fever };
        AssertEquals(String.Join(",", answer), String.Join(",",pred));
        //        assertEquals("0.01512", String.Format("%.5f", prob));
        AssertEquals("0.015", String.Format("%.3f", prob));

        pred = new int[]{pred[0], pred[1]};
        answer = new int[]{answer[0], answer[1]};
        AssertEquals(String.Join(",",answer), String.Join(",", pred));

        pred = new int[]{pred[0]};
        answer = new int[]{answer[0]};
        AssertEquals(String.Join(",", answer), String.Join(",", pred));
//        for (int s : pred)
//        {
//            Console.print(Status.values()[s] + " ");
//        }
//        Console.printf(" with highest probability of %.5f\n", prob);
    }
}