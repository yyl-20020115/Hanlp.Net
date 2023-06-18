using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.summary;




/**
 * 基于TextRank算法的关键字提取，适用于单文档
 *
 * @author hankcs
 */
public class TextRankKeyword : KeywordExtractor
{
    /**
     * 阻尼系数（ＤａｍｐｉｎｇＦａｃｔｏｒ），一般取值为0.85
     */
    static float d = 0.85f;
    /**
     * 最大迭代次数
     */
    public static int max_iter = 200;
    static float min_diff = 0.001f;

    public TextRankKeyword(Segment defaultSegment)
    {
        base(defaultSegment);
    }

    public TextRankKeyword()
    {
    }

    /**
     * 提取关键词
     *
     * @param document 文档内容
     * @param size     希望提取几个关键词
     * @return 一个列表
     */
    public static List<string> getKeywordList(string document, int size)
    {
        TextRankKeyword textRankKeyword = new TextRankKeyword();

        return textRankKeyword.getKeywords(document, size);
    }

    /**
     * 提取关键词
     *
     * @param content
     * @return
     * @deprecated 请使用 {@link KeywordExtractor#getKeywords(java.lang.string)}
     */
    public List<string> getKeyword(string content)
    {
        return getKeywords(content);
    }

    /**
     * 返回全部分词结果和对应的rank
     *
     * @param content
     * @return
     */
    public Dictionary<string, float> getTermAndRank(string content)
    {
        assert content != null;
        List<Term> termList = defaultSegment.seg(content);
        return getTermAndRank(termList);
    }

    /**
     * 返回分数最高的前size个分词结果和对应的rank
     *
     * @param content
     * @param size
     * @return
     */
    public Dictionary<string, float> getTermAndRank(string content, int size)
    {
        Dictionary<string, float> map = getTermAndRank(content);
        Dictionary<string, float> result = top(size, map);

        return result;
    }

    private Dictionary<string, float> top(int size, Dictionary<string, float> map)
    {
        Dictionary<string, float> result = new ();
        for (KeyValuePair<string, float> entry in new MaxHeap<KeyValuePair<string, float>>(size, new CT()).addAll(map.entrySet()).ToList())
        {
            result.Add(entry.getKey(), entry.getValue());
        }
        return result;
    }
    public class CT: IComparer<KeyValuePair<string, float>>
    {
        //@Override
        public int Compare(KeyValuePair<string, float> o1, KeyValuePair<string, float> o2)
        {
            return o1.getValue().compareTo(o2.getValue());
        }
    }

    /**
     * 使用已经分好的词来计算rank
     *
     * @param termList
     * @return
     */
    public Dictionary<string, float> getTermAndRank(List<Term> termList)
    {
        List<string> wordList = new (termList.size());
        for (Term t : termList)
        {
            if (shouldInclude(t))
            {
                wordList.Add(t.word);
            }
        }
//        Console.WriteLine(wordList);
        Dictionary<string, HashSet<string>> words = new Dictionary<string, HashSet<string>>();
        Queue<string> que = new LinkedList<string>();
        for (string w : wordList)
        {
            if (!words.ContainsKey(w))
            {
                words.put(w, new TreeSet<string>());
            }
            // 复杂度O(n-1)
            if (que.size() >= 5)
            {
                que.poll();
            }
            for (string qWord : que)
            {
                if (w.Equals(qWord))
                {
                    continue;
                }
                //既然是邻居,那么关系是相互的,遍历一遍即可
                words.get(w).Add(qWord);
                words.get(qWord).Add(w);
            }
            que.offer(w);
        }
//        Console.WriteLine(words);
        Dictionary<string, float> score = new HashMap<string, float>();
        //依据TF来设置初值
        for (KeyValuePair<string, HashSet<string>> entry : words.entrySet())
        {
            score.put(entry.getKey(), sigMoid(entry.getValue().size()));
        }
        for (int i = 0; i < max_iter; ++i)
        {
            Dictionary<string, float> m = new HashMap<string, float>();
            float max_diff = 0;
            for (KeyValuePair<string, HashSet<string>> entry : words.entrySet())
            {
                string key = entry.getKey();
                HashSet<string> value = entry.getValue();
                m.put(key, 1 - d);
                for (string element : value)
                {
                    int size = words.get(element).size();
                    if (key.Equals(element) || size == 0) continue;
                    m.put(key, m.get(key) + d / size * (score.get(element) == null ? 0 : score.get(element)));
                }
                max_diff = Math.Max(max_diff, Math.abs(m.get(key) - (score.get(key) == null ? 0 : score.get(key))));
            }
            score = m;
            if (max_diff <= min_diff) break;
        }

        return score;
    }

    /**
     * sigmoid函数
     *
     * @param value
     * @return
     */
    public static float sigMoid(float value)
    {
        return (float) (1d / (1d + Math.exp(-value)));
    }

    //@Override
    public List<string> getKeywords(List<Term> termList, int size)
    {
        HashSet<KeyValuePair<string, float>> entrySet = top(size, getTermAndRank(termList)).entrySet();
        List<string> result = new (entrySet.size());
        for (KeyValuePair<string, float> entry : entrySet)
        {
            result.Add(entry.getKey());
        }
        return result;
    }
}
