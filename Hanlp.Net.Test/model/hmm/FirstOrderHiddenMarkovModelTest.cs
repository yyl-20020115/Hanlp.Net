namespace com.hankcs.hanlp.model.hmm;




[TestClass]
public class FirstOrderHiddenMarkovModelTest : TestCase
{

    /**
     * 隐状态
     */
    enum Status
    {
        Healthy,
        Fever,
    }

    /**
     * 显状态
     */
    enum Feel
    {
        normal,
        cold,
        dizzy,
    }
    /**
     * 初始状态概率矩阵
     */
    static float[] start_probability = new float[]{0.6f, 0.4f};
    /**
     * 状态转移概率矩阵
     */
    static float[][] transition_probability = new float[][]{
        {0.7f, 0.3f},
        {0.4f, 0.6f},
    };
    /**
     * 发射概率矩阵
     */
    static float[][] emission_probability = new float[][]{
        {0.5f, 0.4f, 0.1f},
        {0.1f, 0.3f, 0.6f},
    };
    /**
     * 某个病人的观测序列
     */
    static int[] observations = new int[]{normal.ordinal(), cold.ordinal(), dizzy.ordinal()};
    [TestMethod]
    public void testGenerate() 
    {
        FirstOrderHiddenMarkovModel givenModel = new FirstOrderHiddenMarkovModel(start_probability, transition_probability, emission_probability);
        for (int[][] sample : givenModel.generate(3, 5, 2))
        {
            for (int t = 0; t < sample[0].Length; t++)
                Console.printf("%s/%s ", Feel.values()[sample[0][t]], Status.values()[sample[1][t]]);
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
        evaluateModel(model);
    }
    [TestMethod]
    public void evaluateModel(FirstOrderHiddenMarkovModel model)
    {
        int[] pred = new int[observations.Length];
        float prob = (float) Math.exp(model.predict(observations, pred));
        int[] answer = {Healthy.ordinal(), Healthy.ordinal(), Fever.ordinal()};
        assertEquals(Arrays.ToString(answer), Arrays.ToString(pred));
//        assertEquals("0.01512", String.format("%.5f", prob));
        assertEquals("0.015", String.format("%.3f", prob));

        pred = new int[]{pred[0], pred[1]};
        answer = new int[]{answer[0], answer[1]};
        assertEquals(Arrays.ToString(answer), Arrays.ToString(pred));

        pred = new int[]{pred[0]};
        answer = new int[]{answer[0]};
        assertEquals(Arrays.ToString(answer), Arrays.ToString(pred));
//        for (int s : pred)
//        {
//            Console.print(Status.values()[s] + " ");
//        }
//        Console.printf(" with highest probability of %.5f\n", prob);
    }
}