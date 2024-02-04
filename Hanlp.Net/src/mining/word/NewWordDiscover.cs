using com.hankcs.hanlp.algorithm;
using com.hankcs.hanlp.utility;
using System.Text.RegularExpressions;

namespace com.hankcs.hanlp.mining.word;



/**
 * 新词发现工具<br>
 * 在实现上参考了：https://github.com/Moonshile/ChineseWordSegmentation
 *
 * @author hankcs
 */
public class NewWordDiscover
{
    private int max_word_len;
    private float min_freq;
    private float min_entropy;
    private float min_aggregation;
    private bool filter;

    public NewWordDiscover()
        : this(4, 0.00005f, .4f, 1.2f, false)
    {
        ;
    }

    /**
     * 构造一个新词识别工具
     *
     * @param max_word_len    词语最长长度
     * @param min_freq        词语最低频率
     * @param min_entropy     词语最低熵
     * @param min_aggregation 词语最低互信息
     * @param filter          是否过滤掉HanLP中的词库中已存在的词语
     */
    public NewWordDiscover(int max_word_len, float min_freq, float min_entropy, float min_aggregation, bool filter)
    {
        this.max_word_len = max_word_len;
        this.min_freq = min_freq;
        this.min_entropy = min_entropy;
        this.min_aggregation = min_aggregation;
        this.filter = filter;
    }

    /**
     * 提取词语
     *
     * @param reader 大文本
     * @param size   需要提取词语的数量
     * @return 一个词语列表
     */
    public List<WordInfo> discover(TextReader reader, int size) 
    {
        string doc;
        Dictionary<string, WordInfo> word_cands = new Dictionary<string, WordInfo>();
        int totalLength = 0;
        Regex delimiter = new Regex("[\\s\\d,.<>/?:;'\"\\[\\]{}()\\|~!@#$%^&*\\-_=+，。《》、？：；“”‘’｛｝【】（）…￥！—┄－]+");
        while ((doc = reader.ReadLine()) != null)
        {
            doc = delimiter.matcher(doc).replaceAll("\0");
            int docLength = doc.Length;
            for (int i = 0; i < docLength; ++i)
            {
                int end = Math.Min(i + 1 + max_word_len, docLength + 1);
                for (int j = i + 1; j < end; ++j)
                {
                    string word = doc[i .. j];
                    if (word.IndexOf('\0') >= 0)
                        continue; // 含有分隔符的不认为是词语
                    WordInfo info = word_cands.get(word);
                    if (info == null)
                    {
                        info = new WordInfo(word);
                        word_cands.Add(word, info);
                    }
                    info.update(i == 0 ? '\0' : doc[(i - 1)], j < docLength ? doc[(j)] : '\0');
                }
            }
            totalLength += docLength;
        }

        foreach (WordInfo info in word_cands.Values)
        {
            info.computeProbabilityEntropy(totalLength);
        }
        foreach (WordInfo info in word_cands.Values)
        {
            info.computeAggregation(word_cands);
        }
        // 过滤
        List<WordInfo> wordInfoList = new (word_cands.Values);
        var listIterator = wordInfoList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            WordInfo info = listIterator.next();
            if (info.text.Trim().Length < 2 || info.p < min_freq || info.entropy < min_entropy || info.aggregation < min_aggregation
                || (filter && LexiconUtility.GetFrequency(info.text) > 0)
                )
            {
                listIterator.Remove();
            }
        }
        // 按照频率排序
        MaxHeap<WordInfo> topN = new MaxHeap<WordInfo>(size, new COMP());
        topN.AddRange(wordInfoList);

        return topN.ToList();
    }
    public class COMP: IComparer<WordInfo>
    {
        public int Compare(WordInfo o1, WordInfo o2)
        {
            return (int)Math.Ceiling(o1.p - o2.p);
        }
    }
    /**
     * 提取词语
     *
     * @param doc  大文本
     * @param size 需要提取词语的数量
     * @return 一个词语列表
     */
    public List<WordInfo> discover(string doc, int size)
    {
        try
        {
            return discover(new StringReader(doc), size);
        }
        catch (IOException e)
        {
            throw new InvalidOperationException(e);
        }
    }
}