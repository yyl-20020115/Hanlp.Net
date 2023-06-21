/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/17 14:20</create-date>
 *
 * <copyright file="SuggesterEx.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.suggest.scorer;
using com.hankcs.hanlp.suggest.scorer.editdistance;
using com.hankcs.hanlp.suggest.scorer.lexeme;
using com.hankcs.hanlp.suggest.scorer.pinyin;

namespace com.hankcs.hanlp.suggest;




/**
 * 文本推荐器
 * @author hankcs
 */
public class Suggester : ISuggester
{
    List<BaseScorer> scorerList;

    public Suggester()
    {
        scorerList = new List<BaseScorer>();
        scorerList.Add(new IdVectorScorer());
        scorerList.Add(new EditDistanceScorer());
        scorerList.Add(new PinyinScorer());
    }

    public Suggester(List<BaseScorer> scorerList)
    {
        this.scorerList = scorerList;
    }

    /**
     * 构造一个推荐器
     * @param scorers 打分器
     */
    public Suggester(params BaseScorer[] scorers)
    {
        scorerList = new (scorers.Length);
        foreach (BaseScorer scorer in scorers)
        {
            scorerList.Add(scorer);
        }
    }

    //@Override
    public void addSentence(string sentence)
    {
        foreach (IScorer scorer in scorerList)
        {
            scorer.addSentence(sentence);
        }
    }

    //@Override
    public void removeAllSentences()
    {
        foreach (IScorer scorer in scorerList)
        {
            scorer.removeAllSentences();
        }
    }

    //@Override
    public List<string> suggest(string key, int size)
    {
        List<string> resultList = new (size);
        Dictionary<string, Double> scoreMap = new Dictionary<string, Double>();
        foreach (BaseScorer scorer in scorerList)
        {
            Dictionary<string, Double> map = scorer.computeScore(key);
            Double max = max(map);  // 用于正规化一个map
            foreach (KeyValuePair<string, Double> entry in map)
            {
                Double score = scoreMap.get(entry.Key);
                if (score == null) score = 0.0;
                scoreMap.Add(entry.Key, score / max + entry.Value * scorer.boost);
            }
        }
        foreach (KeyValuePair<Double, HashSet<string>> entry in sortScoreMap(scoreMap))
        {
            foreach (string sentence in entry.Value)
            {
                if (resultList.Count >= size) return resultList;
                resultList.Add(sentence);
            }
        }

        return resultList;
    }

    /**
     * 将分数map排序折叠
     * @param scoreMap
     * @return
     */
    private static Dictionary<Double ,HashSet<string>> sortScoreMap(Dictionary<string, Double> scoreMap)
    {
        Dictionary<Double, HashSet<string>> result = new Dictionary<Double, HashSet<string>>(Collections.reverseOrder());
        foreach (KeyValuePair<string, Double> entry in scoreMap)
        {
            HashSet<string> sentenceSet = result.get(entry.Value);
            if (sentenceSet == null)
            {
                sentenceSet = new HashSet<string>();
                result.Add(entry.Value, sentenceSet);
            }
            sentenceSet.Add(entry.Key);
        }

        return result;
    }

    /**
     * 从map的值中找出最大值，这个值是从0开始的
     * @param map
     * @return
     */
    private static Double max(Dictionary<string, Double> map)
    {
        Double theMax = 0.0;
        foreach (Double v in map.Values)
        {
            theMax = Math.Max(theMax, v);
        }

        return theMax;
    }
}
