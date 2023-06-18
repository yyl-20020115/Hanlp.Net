namespace com.hankcs.hanlp.model.crf.crfpp;


/**
 * @author zhifac
 */
public abstract class Tagger
{
    public bool open(string[] args)
    {
        return true;
    }

    public bool open(FeatureIndex featureIndex, int nbest, int vlevel, double costFactor)
    {
        return true;
    }

    public bool open(FeatureIndex featureIndex, int nbest, int vlevel)
    {
        return true;
    }

    public bool open(string arg)
    {
        return true;
    }

    public bool Add(string[] strArr)
    {
        return true;
    }

    public void close()
    {
    }

    public float[] weightVector()
    {
        return null;
    }

    public bool Add(string str)
    {
        return true;
    }

    public int size()
    {
        return 0;
    }

    public int xsize()
    {
        return 0;
    }

    public int dsize()
    {
        return 0;
    }

    public int result(int i)
    {
        return 0;
    }

    public int answer(int i)
    {
        return 0;
    }

    public int y(int i)
    {
        return result(i);
    }

    public string y2(int i)
    {
        return "";
    }

    public string yname(int i)
    {
        return "";
    }

    public string x(int i, int j)
    {
        return "";
    }

    public int ysize()
    {
        return 0;
    }

    public double prob(int i, int j)
    {
        return 0.0;
    }

    public double prob(int i)
    {
        return 0.0;
    }

    public double prob()
    {
        return 0.0;
    }

    public double alpha(int i, int j)
    {
        return 0.0;
    }

    public double beta(int i, int j)
    {
        return 0.0;
    }

    public double emissionCost(int i, int j)
    {
        return 0.0;
    }

    public double nextTransitionCost(int i, int j, int k)
    {
        return 0.0;
    }

    public double prevTransitionCost(int i, int j, int k)
    {
        return 0.0;
    }

    public double bestCost(int i, int j)
    {
        return 0.0;
    }

    public List<int> emissionVector(int i, int j)
    {
        return null;
    }

    public List<int> nextTransitionVector(int i, int j, int k)
    {
        return null;
    }

    public List<int> prevTransitionVector(int i, int j, int k)
    {
        return null;
    }

    public double Z()
    {
        return 0.0;
    }

    public bool parse()
    {
        return true;
    }

    public bool empty()
    {
        return true;
    }

    public bool clear()
    {
        return true;
    }

    public bool next()
    {
        return true;
    }

    public string parse(string str)
    {
        return "";
    }

    public string toString()
    {
        return "";
    }

    public string toString(string result, int size)
    {
        return "";
    }

    public string parse(string str, int size)
    {
        return "";
    }

    public string parse(string str, int size1, string result, int size2)
    {
        return "";
    }

    // set token-level penalty. It would be useful for implementing
    // Dual decompositon decoding.
    // e.g.
    // "Dual Decomposition for Parsing with Non-Projective Head Automata"
    // Terry Koo Alexander M. Rush Michael Collins Tommi Jaakkola David Sontag
    public void setPenalty(int i, int j, double penalty)
    {
    }

    public double penalty(int i, int j)
    {
        return 0.0;
    }
}
