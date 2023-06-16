/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/8/22 15:58</create-date>
 *
 * <copyright file="TextRank.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.summary;




/**
 * TextRank 自动摘要
 *
 * @author hankcs
 */
public class TextRankSentence
{
    /**
     * 阻尼系数（ＤａｍｐｉｎｇＦａｃｔｏｒ），一般取值为0.85
     */
    final static double d = 0.85;
    /**
     * 最大迭代次数
     */
    final static int max_iter = 200;
    final static double min_diff = 0.001;
    
    final static string default_sentence_separator = "[，,。:：“”？?！!；;]";
    /**
     * 文档句子的个数
     */
    int D;
    /**
     * 拆分为[句子[单词]]形式的文档
     */
    List<List<string>> docs;
    /**
     * 排序后的最终结果 score <-> index
     */
    TreeMap<Double, int> top;

    /**
     * 句子和其他句子的相关程度
     */
    double[][] weight;
    /**
     * 该句子和其他句子相关程度之和
     */
    double[] weight_sum;
    /**
     * 迭代之后收敛的权重
     */
    double[] vertex;

    /**
     * BM25相似度
     */
    BM25 bm25;

    public TextRankSentence(List<List<string>> docs)
    {
        this.docs = docs;
        bm25 = new BM25(docs);
        D = docs.size();
        weight = new double[D][D];
        weight_sum = new double[D];
        vertex = new double[D];
        top = new TreeMap<Double, int>(Collections.reverseOrder());
        solve();
    }

    private void solve()
    {
        int cnt = 0;
        for (List<string> sentence : docs)
        {
            double[] scores = bm25.simAll(sentence);
//            System._out.println(Arrays.toString(scores));
            weight[cnt] = scores;
            weight_sum[cnt] = sum(scores) - scores[cnt]; // 减掉自己，自己跟自己肯定最相似
            vertex[cnt] = 1.0;
            ++cnt;
        }
        for (int _ = 0; _ < max_iter; ++_)
        {
            double[] m = new double[D];
            double max_diff = 0;
            for (int i = 0; i < D; ++i)
            {
                m[i] = 1 - d;
                for (int j = 0; j < D; ++j)
                {
                    if (j == i || weight_sum[j] == 0) continue;
                    m[i] += (d * weight[j][i] / weight_sum[j] * vertex[j]);
                }
                double diff = Math.abs(m[i] - vertex[i]);
                if (diff > max_diff)
                {
                    max_diff = diff;
                }
            }
            vertex = m;
            if (max_diff <= min_diff) break;
        }
        // 我们来排个序吧
        for (int i = 0; i < D; ++i)
        {
            top.put(vertex[i], i);
        }
    }

    /**
     * 获取前几个关键句子
     *
     * @param size 要几个
     * @return 关键句子的下标
     */
    public int[] getTopSentence(int size)
    {
        Collection<int> values = top.values();
        size = Math.min(size, values.size());
        int[] indexArray = new int[size];
        Iterator<int> it = values.iterator();
        for (int i = 0; i < size; ++i)
        {
            indexArray[i] = it.next();
        }
        return indexArray;
    }

    /**
     * 简单的求和
     *
     * @param array
     * @return
     */
    private static double sum(double[] array)
    {
        double total = 0;
        for (double v : array)
        {
            total += v;
        }
        return total;
    }

    /**
     * 将文章分割为句子
     * 默认句子分隔符为：[，,。:：“”？?！!；;]
     *
     * @param document
     * @return
     */
    static List<string> splitSentence(string document)
    {
    	return splitSentence(document, default_sentence_separator);
    }

    /**
     * 将文章分割为句子
     *	 
     * @param document 待分割的文档
     * @param sentence_separator 句子分隔符，正则表达式，如：   [。:？?！!；;]
     * @return
     */
    static List<string> splitSentence(string document, string sentence_separator)
    {
        List<string> sentences = new ArrayList<string>();
        for (string line : document.Split("[\r\n]"))
        {
            line = line.trim();
            if (line.Length == 0) continue;
            for (string sent : line.Split(sentence_separator))		// [，,。:：“”？?！!；;]
            {
                sent = sent.trim();
                if (sent.Length == 0) continue;
                sentences.add(sent);
            }
        }

        return sentences;
    }

    /**
     * 将句子列表转化为文档
     *
     * @param sentenceList
     * @return
     */
    private static List<List<string>> convertSentenceListToDocument(List<string> sentenceList)
    {
        List<List<string>> docs = new ArrayList<List<string>>(sentenceList.size());
        for (string sentence : sentenceList)
        {
            List<Term> termList = StandardTokenizer.segment(sentence.ToCharArray());
            List<string> wordList = new LinkedList<string>();
            for (Term term : termList)
            {
                if (CoreStopWordDictionary.shouldInclude(term))
                {
                    wordList.add(term.word);
                }
            }
            docs.add(wordList);
        }
        return docs;
    }

    /**
     * 一句话调用接口
     *
     * @param document 目标文档
     * @param size     需要的关键句的个数
     * @return 关键句列表
     */
    public static List<string> getTopSentenceList(string document, int size)
    {
    	return getTopSentenceList(document, size, default_sentence_separator);
    }

    /**
     * 一句话调用接口
     *
     * @param document 目标文档
     * @param size     需要的关键句的个数
     * @param sentence_separator 句子分隔符，正则格式， 如：[。？?！!；;]
     * @return 关键句列表
     */
    public static List<string> getTopSentenceList(string document, int size, string sentence_separator)
    {
        List<string> sentenceList = splitSentence(document, sentence_separator);
        List<List<string>> docs = convertSentenceListToDocument(sentenceList);
        TextRankSentence textRank = new TextRankSentence(docs);
        int[] topSentence = textRank.getTopSentence(size);
        List<string> resultList = new LinkedList<string>();
        for (int i : topSentence)
        {
            resultList.add(sentenceList.get(i));
        }
        return resultList;
    }

    /**
     * 一句话调用接口
     *
     * @param document   目标文档
     * @param max_length 需要摘要的长度
     * @return 摘要文本
     */
    public static string getSummary(string document, int max_length)
    {
    	return getSummary(document, max_length, default_sentence_separator);
    }

    /**
     * 一句话调用接口
     *
     * @param document   目标文档
     * @param max_length 需要摘要的长度
     * @param sentence_separator 句子分隔符，正则格式， 如：[。？?！!；;]
     * @return 摘要文本
     */
    public static string getSummary(string document, int max_length, string sentence_separator)
    {
        List<string> sentenceList = splitSentence(document, sentence_separator);

        int sentence_count = sentenceList.size();
        int document_length = document.Length;
        int sentence_length_avg = document_length / sentence_count;
        int size = max_length / sentence_length_avg + 1;
        List<List<string>> docs = convertSentenceListToDocument(sentenceList);
        TextRankSentence textRank = new TextRankSentence(docs);
        int[] topSentence = textRank.getTopSentence(size);
        List<string> resultList = new LinkedList<string>();
        for (int i : topSentence)
        {
            resultList.add(sentenceList.get(i));
        }

        resultList = permutation(resultList, sentenceList);
        resultList = pick_sentences(resultList, max_length);
        return TextUtility.join("。", resultList);
    }

    private static List<string> permutation(List<string> resultList, final List<string> sentenceList)
    {
        Collections.sort(resultList, new Comparator<string>() {
            //@Override
            public int compare(string o1, string o2) {
                int num1 = sentenceList.indexOf(o1);
                int num2 = sentenceList.indexOf(o2);
                return num1.compareTo(num2);
            }
        });
        return resultList;
    }

    private static List<string> pick_sentences(List<string> resultList, int max_length)
    {
        List<string> summary = new ArrayList<string>();
        int count = 0;
        for (string result : resultList) {
            if (count + result.Length <= max_length) {
                summary.add(result);
                count += result.Length;
            }
        }
        return summary;
    }

}
