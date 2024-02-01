namespace com.hankcs.hanlp.model.hmm;


[TestClass]
public class SecondOrderHiddenMarkovModelTest : FirstOrderHiddenMarkovModelTest
{
    static float[][][] transition_probability2 = new float[][][]{
        {{0.7f, 0.3f}, {0.4f, 0.6f}},
        {{0.7f, 0.3f}, {0.4f, 0.6f}},
    };
    [TestMethod] 
    public void TestPredict() 
    {
        SecondOrderHiddenMarkovModel hmm2 = new SecondOrderHiddenMarkovModel(
            start_probability,
            transition_probability,
            emission_probability,
            transition_probability2);

        SecondOrderHiddenMarkovModel trainedModel = new SecondOrderHiddenMarkovModel();
        trainedModel.train(hmm2.generate(3, 10, 100000));
        hmm2.unLog();
        trainedModel.unLog();
        AssertTrue(hmm2.similar(trainedModel));
    }
}