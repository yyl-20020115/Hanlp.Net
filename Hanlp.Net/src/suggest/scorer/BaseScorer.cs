/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/5 16:34</create-date>
 *
 * <copyright file="BaseScorer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.suggest.scorer;



/**
 * 基本打分器
 * @param <T> 这是储存器map中key的类型，具有相同key的句子会存入同一个entry
 * @author hankcs
 */
public abstract class BaseScorer<T : ISentenceKey> : IScorer
{
    public BaseScorer()
    {
        storage = new TreeMap<T, Set<string>>();
    }

    /**
     * 储存
     */
    protected Dictionary<T, Set<string>> storage;
    /**
     * 权重
     */
    public double boost = 1.0;

    /**
     * 设置权重
     * @param boost
     * @return
     */
    public BaseScorer setBoost(double boost)
    {
        this.boost = boost;
        return this;
    }

    //@Override
    public void addSentence(string sentence)
    {
        T key = generateKey(sentence);
        if (key == null) return;
        Set<string> set = storage.get(key);
        if (set == null)
        {
            set = new TreeSet<string>();
            storage.put(key, set);
        }
        set.add(sentence);
    }

    /**
     * 生成能够代表这个句子的键
     * @param sentence
     * @return
     */
    protected abstract T generateKey(string sentence);

    //@Override
    public Dictionary<string, Double> computeScore(string outerSentence)
    {
        TreeMap<string, Double> result = new TreeMap<string, Double>(Collections.reverseOrder());
        T keyOuter = generateKey(outerSentence);
        if (keyOuter == null) return result;
        for (KeyValuePair<T, Set<string>> entry : storage.entrySet())
        {
            T key = entry.getKey();
            Double score = keyOuter.similarity(key);
            for (string sentence : entry.getValue())
            {
                result.put(sentence, score);
            }
        }
        return result;
    }

    //@Override
    public void removeAllSentences()
    {
        storage.clear();
    }
}
