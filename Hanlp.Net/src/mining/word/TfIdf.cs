using com.hankcs.hanlp.seg.common;
using System.Collections.ObjectModel;

namespace com.hankcs.hanlp.mining.word;



/**
 * 词频-倒排文档词频统计
 */
public class TfIdf
{

    /**
     * 词频统计方式
     */
    public enum TfType
    {
        /**
         * 普通词频
         */
        NATURAL,
        /**
         * 词频的对数并加1
         */
        LOGARITHM,
        /**
         * 01词频
         */
        BOOLEAN
    }

    /**
     * tf-idf 向量的正规化算法
     */
    public enum Normalization
    {
        /**
         * 不正规化
         */
        NONE,
        /**
         * cosine正规化
         */
        COSINE
    }

    /**
     * 单文档词频
     *
     * @param document 词袋
     * @param type     词频计算方式
     * @param <Term>   词语类型
     * @return 一个包含词频的Map
     */
    public static  Dictionary<Term, Double> tf<Term>(Collection<Term> document, TfType type)
    {
        Dictionary<Term, Double> tf = new ();
        foreach (Term term in document)
        {
            Double f = tf.get(term);
            if (f == null) f = 0.0;
            tf.Add(term, f + 1);
        }
        if (type != TfType.NATURAL)
        {
            foreach (Term term in tf.keySet())
            {
                switch (type)
                {
                    case LOGARITHM:
                        tf.Add(term, 1 + Math.Log(tf.get(term)));
                        break;
                    case BOOLEAN:
                        tf.Add(term, tf.get(term) == 0.0 ? 0.0 : 1.0);
                        break;
                }
            }
        }
        return tf;
    }

    /**
     * 单文档词频
     *
     * @param document 词袋
     * @param <Term>   词语类型
     * @return 一个包含词频的Map
     */
    public static  Dictionary<Term, Double> tf<Term>(Collection<Term> document)
    {
        return tf(document, TfType.NATURAL);
    }

    /**
     * 多文档词频
     *
     * @param documents 多个文档，每个文档都是一个词袋
     * @param type      词频计算方式
     * @param <Term>    词语类型
     * @return 一个包含词频的Map的列表
     */
    public static  Iterable<Dictionary<Term, Double>> tfs<Term>(Iterable<Collection<Term>> documents, TfType type)
    {
        List<Dictionary<Term, Double>> tfs = new ();
        foreach (Collection<Term> document in documents)
        {
            tfs.Add(tf(document, type));
        }
        return tfs;
    }

    /**
     * 多文档词频
     *
     * @param documents 多个文档，每个文档都是一个词袋
     * @param <Term>    词语类型
     * @return 一个包含词频的Map的列表
     */
    public static  Iterable<Dictionary<Term, Double>> tfs<Term>(Iterable<Collection<Term>> documents)
    {
        return tfs(documents, TfType.NATURAL);
    }

    /**
     * 一系列文档的倒排词频
     *
     * @param documentVocabularies 词表
     * @param smooth               平滑参数，视作额外有一个文档，该文档含有smooth个每个词语
     * @param addOne               tf-idf加一平滑
     * @param <Term>               词语类型
     * @return 一个词语->倒排文档的Map
     */
    public static  Dictionary<Term, Double> idf<Term>(Iterable<Iterable<Term>> documentVocabularies,
                                               bool smooth, bool addOne)
    {
        Dictionary<Term, int> df = new HashMap<Term, int>();
        int d = smooth ? 1 : 0;
        int a = addOne ? 1 : 0;
        int n = d;
        foreach (Iterable<Term> documentVocabulary in documentVocabularies)
        {
            n += 1;
            foreach (Term term in documentVocabulary)
            {
                int t = df.get(term);
                if (t == null) t = d;
                df.Add(term, t + 1);
            }
        }
        Dictionary<Term, Double> idf = new HashMap<Term, Double>();
        foreach (KeyValuePair<Term, int> e in df.entrySet())
        {
            Term term = e.Key;
            double f = e.Value;
            idf.Add(term, Math.Log(n / f) + a);
        }
        return idf;
    }

    /**
     * 平滑处理后的一系列文档的倒排词频
     *
     * @param documentVocabularies 词表
     * @param <Term>               词语类型
     * @return 一个词语->倒排文档的Map
     */
    public static  Dictionary<Term, Double> idf<Term>(Iterable<Iterable<Term>> documentVocabularies)
    {
        return idf(documentVocabularies, true, true);
    }

    /**
     * 计算文档的tf-idf
     *
     * @param tf            词频
     * @param idf           倒排频率
     * @param normalization 正规化
     * @param <Term>        词语类型
     * @return 一个词语->tf-idf的Map
     */
    public static <Term> Dictionary<Term, Double> tfIdf(Dictionary<Term, Double> tf, Dictionary<Term, Double> idf,
                                                 Normalization normalization)
    {
        Dictionary<Term, Double> tfIdf = new HashMap<Term, Double>();
        foreach (Term term in tf.keySet())
        {
            Double TF = tf.get(term);
            if (TF == null) TF = 1.;
            Double IDF = idf.get(term);
            if (IDF == null) IDF = 1.;
            tfIdf.Add(term, TF * IDF);
        }
        if (normalization == Normalization.COSINE)
        {
            double n = 0.0;
            foreach (double x in tfIdf.values())
            {
                n += x * x;
            }
            n = Math.Sqrt(n);

            foreach (Term term in tfIdf.keySet())
            {
                tfIdf.Add(term, tfIdf.get(term) / n);
            }
        }
        return tfIdf;
    }

    /**
     * 计算文档的tf-idf（不正规化）
     *
     * @param tf     词频
     * @param idf    倒排频率
     * @param <Term> 词语类型
     * @return 一个词语->tf-idf的Map
     */
    public static <Term> Dictionary<Term, Double> tfIdf(Dictionary<Term, Double> tf, Dictionary<Term, Double> idf)
    {
        return tfIdf(tf, idf, Normalization.NONE);
    }

    /**
     * 从词频集合建立倒排频率
     *
     * @param tfs    次品集合
     * @param smooth 平滑参数，视作额外有一个文档，该文档含有smooth个每个词语
     * @param addOne tf-idf加一平滑
     * @param <Term> 词语类型
     * @return 一个词语->倒排文档的Map
     */
    public static <Term> Dictionary<Term, Double> idfFromTfs(Iterable<Dictionary<Term, Double>> tfs, bool smooth, bool addOne)
    {
        return idf(new KeySetIterable<Term, Double>(tfs), smooth, addOne);
    }

    /**
     * 从词频集合建立倒排频率（默认平滑词频，且加一平滑tf-idf）
     *
     * @param tfs    次品集合
     * @param <Term> 词语类型
     * @return 一个词语->倒排文档的Map
     */
    public static <Term> Dictionary<Term, Double> idfFromTfs(Iterable<Dictionary<Term, Double>> tfs)
    {
        return idfFromTfs(tfs, true, true);
    }

    /**
     * Map的迭代器
     *
     * @param <KEY>   map 键类型
     * @param <VALUE> map 值类型
     */
    private class KeySetIterable<KEY, VALUE> : Iterable<Iterable<KEY>>
    {
        private Iterator<Dictionary<KEY, VALUE>> maps;

        public KeySetIterable(Iterable<Dictionary<KEY, VALUE>> maps)
        {
            this.maps = maps.iterator();
        }

        //@Override
        public Iterator<Iterable<KEY>> iterator()
        {
            return new Iterator<Iterable<KEY>>()
            {
                //@Override
                public bool MoveNext()
                {
                    return maps.MoveNext();
                }

                //@Override
                public Iterable<KEY> next()
                {
                    return maps.next().keySet();
                }

                //@Override
                public void Remove()
                {

                }
            };
        }
    }
}
