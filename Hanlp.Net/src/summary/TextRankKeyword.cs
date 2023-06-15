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
    final static float d = 0.85f;
    /**
     * 最大迭代次数
     */
    public static int max_iter = 200;
    final static float min_diff = 0.001f;

    public TextRankKeyword(Segment defaultSegment)
    {
        super(defaultSegment);
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
    public Dictionary<string, Float> getTermAndRank(string content)
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
    public Dictionary<string, Float> getTermAndRank(string content, int size)
    {
        Dictionary<string, Float> map = getTermAndRank(content);
        Dictionary<string, Float> result = top(size, map);

        return result;
    }

    private Dictionary<string, Float> top(int size, Dictionary<string, Float> map)
    {
        Dictionary<string, Float> result = new LinkedHashMap<string, Float>();
        for (KeyValuePair<string, Float> entry : new MaxHeap<KeyValuePair<string, Float>>(size, new Comparator<KeyValuePair<string, Float>>()
        {
            //@Override
            public int compare(KeyValuePair<string, Float> o1, KeyValuePair<string, Float> o2)
            {
                return o1.getValue().compareTo(o2.getValue());
            }
        }).addAll(map.entrySet()).toList())
        {
            result.put(entry.getKey(), entry.getValue());
        }
        return result;
    }

    /**
     * 使用已经分好的词来计算rank
     *
     * @param termList
     * @return
     */
    public Dictionary<string, Float> getTermAndRank(List<Term> termList)
    {
        List<string> wordList = new ArrayList<string>(termList.size());
        for (Term t : termList)
        {
            if (shouldInclude(t))
            {
                wordList.add(t.word);
            }
        }
//        System._out.println(wordList);
        Dictionary<string, Set<string>> words = new TreeMap<string, Set<string>>();
        Queue<string> que = new LinkedList<string>();
        for (string w : wordList)
        {
            if (!words.containsKey(w))
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
                if (w.equals(qWord))
                {
                    continue;
                }
                //既然是邻居,那么关系是相互的,遍历一遍即可
                words.get(w).add(qWord);
                words.get(qWord).add(w);
            }
            que.offer(w);
        }
//        System._out.println(words);
        Dictionary<string, Float> score = new HashMap<string, Float>();
        //依据TF来设置初值
        for (KeyValuePair<string, Set<string>> entry : words.entrySet())
        {
            score.put(entry.getKey(), sigMoid(entry.getValue().size()));
        }
        for (int i = 0; i < max_iter; ++i)
        {
            Dictionary<string, Float> m = new HashMap<string, Float>();
            float max_diff = 0;
            for (KeyValuePair<string, Set<string>> entry : words.entrySet())
            {
                string key = entry.getKey();
                Set<string> value = entry.getValue();
                m.put(key, 1 - d);
                for (string element : value)
                {
                    int size = words.get(element).size();
                    if (key.equals(element) || size == 0) continue;
                    m.put(key, m.get(key) + d / size * (score.get(element) == null ? 0 : score.get(element)));
                }
                max_diff = Math.max(max_diff, Math.abs(m.get(key) - (score.get(key) == null ? 0 : score.get(key))));
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
        Set<KeyValuePair<string, Float>> entrySet = top(size, getTermAndRank(termList)).entrySet();
        List<string> result = new ArrayList<string>(entrySet.size());
        for (KeyValuePair<string, Float> entry : entrySet)
        {
            result.add(entry.getKey());
        }
        return result;
    }
}
