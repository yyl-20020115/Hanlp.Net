namespace com.hankcs.hanlp.mining.word;


/**
 * 提取出来的词语
 * @author hankcs
 */
public class WordInfo
{
    /**
     * 左邻接字集合
     */
    public Dictionary<char, int[]> left;
    /**
     * 右邻接字集合
     */
    public Dictionary<char, int[]> right;
    /**
     * 词语
     */
    public string text;
    /**
     * 词频
     */
    public int frequency;
    public float p;
    public float leftEntropy;
    public float rightEntropy;
    /**
     * 互信息
     */
    public float aggregation;
    /**
     * 信息熵
     */
    public float entropy;

    public WordInfo(string text)
    {
        this.text = text;
        left = new Dictionary<char, int[]>();
        right = new Dictionary<char, int[]>();
        aggregation = float.MaxValue;
    }

    private static void increaseFrequency(char c, Dictionary<char, int[]> storage)
    {
        if (!storage.TryGetValue(c,out var freq))
        {
            freq = new int[]{1};
            storage.Add(c, freq);
        }
        else
        {
            ++freq[0];
        }
    }

    private float computeEntropy(Dictionary<char, int[]> storage)
    {
        float sum = 0;
        foreach (KeyValuePair<char, int[]> entry in storage)
        {
            float p = entry.Value[0] / (float) frequency;
            sum -= (float)(p * Math.Log(p));
        }
        return sum;
    }

    public void update(char left, char right)
    {
        ++frequency;
        increaseFrequency(left, this.left);
        increaseFrequency(right, this.right);
    }

    public void computeProbabilityEntropy(int Length)
    {
        p = frequency / (float) Length;
        leftEntropy = computeEntropy(left);
        rightEntropy = computeEntropy(right);
        entropy = Math.Min(leftEntropy, rightEntropy);
    }

    public void computeAggregation(Dictionary<string, WordInfo> word_cands)
    {
        if (text.Length == 1)
        {
            aggregation = (float) Math.Sqrt(p);
            return;
        }
        for (int i = 1; i < text.Length; ++i)
        {
            aggregation = Math.Min(aggregation,
                                   p / word_cands[(text[0..i])].p / word_cands[text[i..]].p);
        }
    }

    //@Override
    public override string ToString()
    {
        return text;
    }
}
