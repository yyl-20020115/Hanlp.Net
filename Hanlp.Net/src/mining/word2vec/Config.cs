namespace com.hankcs.hanlp.mining.word2vec;

public class Config
{

    static readonly int DEF_ITER = 5;
    static readonly int DEF_WINDOW = 5;
    static readonly int DEF_MIN_COUNT = 5;
    static readonly int DEF_NEGATIVE = 5;
    static readonly int DEF_LAYER1_SIZE = 100;
    static readonly int DEF_NUM_THREADS = Runtime.getRuntime().availableProcessors();
    static readonly float DEF_SAMPLE = 0.001f;

    protected string outputFile;
    protected int iter = DEF_ITER, window = DEF_WINDOW, minCount = DEF_MIN_COUNT, negative = DEF_NEGATIVE,
            layer1Size = DEF_LAYER1_SIZE, numThreads = DEF_NUM_THREADS;
    protected bool hs, cbow;
    protected float sample = DEF_SAMPLE, alpha = 0.025f;
    private string inputFile;
    private TrainingCallback callback;

    public Config setOutputFile(string outputFile)
    {
        this.outputFile = outputFile;
        return this;
    }

    public string getOutputFile()
    {
        return outputFile;
    }

    public Config setIter(int iter)
    {
        this.iter = iter;
        return this;
    }

    public int getIter()
    {
        return iter;
    }

    public Config setWindow(int window)
    {
        this.window = window;
        return this;
    }

    public int getWindow()
    {
        return window;
    }

    public Config setMinCount(int minCount)
    {
        this.minCount = minCount;
        return this;
    }

    public int getMinCount()
    {
        return minCount;
    }

    public Config setNegative(int negative)
    {
        this.negative = negative;
        return this;
    }

    public int getNegative()
    {
        return negative;
    }

    public Config setLayer1Size(int layer1Size)
    {
        this.layer1Size = layer1Size;
        return this;
    }

    public int getLayer1Size()
    {
        return layer1Size;
    }

    public Config setNumThreads(int numThreads)
    {
        this.numThreads = numThreads;
        return this;
    }

    public int getNumThreads()
    {
        return numThreads;
    }

    public Config setUseHierarchicalSoftmax(bool hs)
    {
        this.hs = hs;
        return this;
    }

    public bool useHierarchicalSoftmax()
    {
        return hs;
    }

    public Config setUseContinuousBagOfWords(bool cbow)
    {
        this.cbow = cbow;
        return this;
    }

    public bool useContinuousBagOfWords()
    {
        return cbow;
    }

    public Config setSample(float sample)
    {
        this.sample = sample;
        return this;
    }

    public float getSample()
    {
        return sample;
    }

    public Config setAlpha(float alpha)
    {
        this.alpha = alpha;
        return this;
    }

    public float getAlpha()
    {
        return alpha;
    }

    public Config setInputFile(string inputFile)
    {
        this.inputFile = inputFile;
        return this;
    }

    public string getInputFile()
    {
        return inputFile;
    }

    public TrainingCallback getCallback()
    {
        return callback;
    }

    public void setCallback(TrainingCallback callback)
    {
        this.callback = callback;
    }
}
