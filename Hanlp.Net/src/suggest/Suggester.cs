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
        scorerList.add(new IdVectorScorer());
        scorerList.add(new EditDistanceScorer());
        scorerList.add(new PinyinScorer());
    }

    public Suggester(List<BaseScorer> scorerList)
    {
        this.scorerList = scorerList;
    }

    /**
     * 构造一个推荐器
     * @param scorers 打分器
     */
    public Suggester(BaseScorer... scorers)
    {
        scorerList = new ArrayList<BaseScorer>(scorers.length);
        for (BaseScorer scorer : scorers)
        {
            scorerList.add(scorer);
        }
    }

    //@Override
    public void addSentence(string sentence)
    {
        for (IScorer scorer : scorerList)
        {
            scorer.addSentence(sentence);
        }
    }

    //@Override
    public void removeAllSentences()
    {
        for (IScorer scorer : scorerList)
        {
            scorer.removeAllSentences();
        }
    }

    //@Override
    public List<string> suggest(string key, int size)
    {
        List<string> resultList = new ArrayList<string>(size);
        TreeMap<string, Double> scoreMap = new TreeMap<string, Double>();
        for (BaseScorer scorer : scorerList)
        {
            Dictionary<string, Double> map = scorer.computeScore(key);
            Double max = max(map);  // 用于正规化一个map
            for (KeyValuePair<string, Double> entry : map.entrySet())
            {
                Double score = scoreMap.get(entry.getKey());
                if (score == null) score = 0.0;
                scoreMap.put(entry.getKey(), score / max + entry.getValue() * scorer.boost);
            }
        }
        for (KeyValuePair<Double, Set<string>> entry : sortScoreMap(scoreMap).entrySet())
        {
            for (string sentence : entry.getValue())
            {
                if (resultList.size() >= size) return resultList;
                resultList.add(sentence);
            }
        }

        return resultList;
    }

    /**
     * 将分数map排序折叠
     * @param scoreMap
     * @return
     */
    private static TreeMap<Double ,Set<string>> sortScoreMap(TreeMap<string, Double> scoreMap)
    {
        TreeMap<Double, Set<string>> result = new TreeMap<Double, Set<string>>(Collections.reverseOrder());
        for (KeyValuePair<string, Double> entry : scoreMap.entrySet())
        {
            Set<string> sentenceSet = result.get(entry.getValue());
            if (sentenceSet == null)
            {
                sentenceSet = new HashSet<string>();
                result.put(entry.getValue(), sentenceSet);
            }
            sentenceSet.add(entry.getKey());
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
        for (Double v : map.values())
        {
            theMax = Math.max(theMax, v);
        }

        return theMax;
    }
}
