namespace com.hankcs.hanlp.classification.features;

/**
 * TF-IDF权重计算
 */
public class TfIdfFeatureWeighter : IFeatureWeighter
{
    int numDocs;
    int[] df;

    public TfIdfFeatureWeighter(int numDocs, int[] df)
    {
        this.numDocs = numDocs;
        this.df = df;
    }

    public double weight(int feature, int tf)
    {
        if (feature >= df.Length) Console.Error.WriteLine(feature);
        return Math.Log10(tf + 1) * (Math.Log10((double) numDocs / df[feature] + 1));    // 一种改进的tf*idf计算方式;
    }
}