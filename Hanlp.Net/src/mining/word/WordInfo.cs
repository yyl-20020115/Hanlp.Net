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
    Dictionary<char, int[]> left;
    /**
     * 右邻接字集合
     */
    Dictionary<char, int[]> right;
    /**
     * 词语
     */
    public string text;
    /**
     * 词频
     */
    public int frequency;
    float p;
    float leftEntropy;
    float rightEntropy;
    /**
     * 互信息
     */
    public float aggregation;
    /**
     * 信息熵
     */
    public float entropy;

    WordInfo(string text)
    {
        this.text = text;
        left = new Dictionary<char, int[]>();
        right = new Dictionary<char, int[]>();
        aggregation = Float.MAX_VALUE;
    }

    private static void increaseFrequency(char c, Dictionary<char, int[]> storage)
    {
        int[] freq = storage.get(c);
        if (freq == null)
        {
            freq = new int[]{1};
            storage.put(c, freq);
        }
        else
        {
            ++freq[0];
        }
    }

    private float computeEntropy(Dictionary<char, int[]> storage)
    {
        float sum = 0;
        for (KeyValuePair<char, int[]> entry : storage.entrySet())
        {
            float p = entry.getValue()[0] / (float) frequency;
            sum -= p * Math.log(p);
        }
        return sum;
    }

    void update(char left, char right)
    {
        ++frequency;
        increaseFrequency(left, this.left);
        increaseFrequency(right, this.right);
    }

    void computeProbabilityEntropy(int Length)
    {
        p = frequency / (float) Length;
        leftEntropy = computeEntropy(left);
        rightEntropy = computeEntropy(right);
        entropy = Math.min(leftEntropy, rightEntropy);
    }

    void computeAggregation(Dictionary<string, WordInfo> word_cands)
    {
        if (text.Length == 1)
        {
            aggregation = (float) Math.sqrt(p);
            return;
        }
        for (int i = 1; i < text.Length; ++i)
        {
            aggregation = Math.min(aggregation,
                                   p / word_cands.get(text.substring(0, i)).p / word_cands.get(text.substring(i)).p);
        }
    }

    //@Override
    public string toString()
    {
        return text;
    }
}
