/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-07-23 PM4:24</create-date>
 *
 * <copyright file="WordVectorMap.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.mining.word2vec;


/**
 * 词向量模型
 *
 * @author hankcs
 */
public class WordVectorModel : AbstractVectorModel<string>
{
    /**
     * 加载模型<br>
     *
     * @param modelFileName 模型路径
     * @ 加载错误
     */
    public WordVectorModel(string modelFileName) 
        : base(loadVectorMap(modelFileName))
    {
        ;
    }

    private static Dictionary<string, Vector> loadVectorMap(string modelFileName) 
    {
        VectorsReader reader = new VectorsReader(modelFileName);
        reader.readVectorFile();
        var map = new Dictionary<string, Vector>();
        for (int i = 0; i < reader.vocab.Length; i++)
        {
            map.Add(reader.vocab[i], new Vector(reader.matrix[i]));
        }
        return map;
    }

    /**
     * 返回跟 A - B + C 最相似的词语,比如 中国 - 北京 + 东京 = 日本。输入顺序按照 中国 北京 东京
     *
     * @param A 做加法的词语
     * @param B 做减法的词语
     * @param C 做加法的词语
     * @return 与(A - B + C)语义距离最近的词语及其相似度列表
     */
    public List<KeyValuePair<string,float>> analogy(string A, string B, string C)
    {
        return analogy(A, B, C, 10);
    }

    /**
     * 返回跟 A - B + C 最相似的词语,比如 中国 - 北京 + 东京 = 日本。输入顺序按照 中国 北京 东京
     *
     * @param A    做加法的词语
     * @param B    做减法的词语
     * @param C    做加法的词语
     * @param size topN个
     * @return 与(A - B + C)语义距离最近的词语及其相似度列表
     */
    public List<KeyValuePair<string, float>> analogy(string A, string B, string C, int size)
    {
        Vector a = storage.get(A);
        Vector b = storage.get(B);
        Vector c = storage.get(C);
        if (a == null || b == null || c == null)
        {
            return new();
        }

        List<KeyValuePair<string, float>> resultList = nearest(a.minus(b).Add(c), size + 3);
        ListIterator<KeyValuePair<string, float>> listIterator = resultList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            string key = listIterator.next().Key;
            if (key.Equals(A) || key.Equals(B) || key.Equals(C))
            {
                listIterator.Remove();
            }
        }

        if (resultList.size() > size)
        {
            resultList = resultList.subList(0, size);
        }

        return resultList;
    }

    //@Override
    public Vector query(string query)
    {
        return vector(query);
    }
}