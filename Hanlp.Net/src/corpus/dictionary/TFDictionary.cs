/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/8 14:07</create-date>
 *
 * <copyright file="TFDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.MDAG;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.corpus.occurrence;

namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * 词频词典
 * @author hankcs
 */
public class TFDictionary : SimpleDictionary<TermFrequency> , ISaveAble
{
    string delimeter;

    public TFDictionary(string delimeter)
    {
        this.delimeter = delimeter;
    }

    public TFDictionary()
        :this("=")
    {
    }

    //@Override
    protected KeyValuePair<string, TermFrequency> onGenerateEntry(string line)
    {
        string[] param = line.Split(delimeter);
        return new AbstractMap.SimpleEntry<string, TermFrequency>(param[0], new TermFrequency(param[0], int.valueOf(param[1])));
    }

    /**
     * 合并自己（主词典）和某个词频词典
     * @param dictionary 某个词频词典
     * @param limit 如果该词频词典试图引入一个词语，其词频不得超过此limit（如果不需要使用limit功能，可以传入int.MaxValue）
     * @param Add 设为true则是词频叠加模式，否则是词频覆盖模式
     * @return 词条的增量
     */
    public int combine(TFDictionary dictionary, int limit, bool Add)
    {
        int preSize = trie.size();
        foreach (KeyValuePair<string, TermFrequency> entry in dictionary.trie.entrySet())
        {
            TermFrequency termFrequency = trie.get(entry.Key);
            if (termFrequency == null)
            {
                trie.Add(entry.Key, new TermFrequency(entry.Key, Math.Min(limit, entry.Value.Value)));
            }
            else
            {
                if (Add)
                {
                    termFrequency.setValue(termFrequency.Value + Math.Min(limit, entry.Value.Value));
                }
            }
        }
        return trie.size() - preSize;
    }

    /**
     * 合并多个词典
     * @param path 多个词典的路径，第一个是主词典。主词典与其他词典的区别详见com.hankcs.hanlp.corpus.dictionary.TFDictionary#combine(com.hankcs.hanlp.corpus.dictionary.TFDictionary, int, bool)
     * @return 词条的增量
     */
    public static int combine(params string[] path)
    {
        TFDictionary dictionaryMain = new TFDictionary();
        dictionaryMain.load(path[0]);
        int preSize = dictionaryMain.trie.size();
        for (int i = 1; i < path.Length; ++i)
        {
            TFDictionary dictionary = new TFDictionary();
            dictionary.load(path[i]);
            dictionaryMain.combine(dictionary, 1, true);
        }
        try
        {
            TextWriter bw = new TextWriter(new StreamWriter(IOUtil.newOutputStream(path[0]), "UTF-8"));
            foreach (KeyValuePair<string, TermFrequency> entry in dictionaryMain.trie.entrySet())
            {
                bw.Write(entry.Key);
                bw.Write(' ');
                bw.Write(string.valueOf(entry.Value.Value));
                bw.AppendLine();
            }
            bw.Close();
        }
        catch (Exception e)
        {
            ////e.printStackTrace();
            return -1;
        }

        return dictionaryMain.trie.size() - preSize;
    }

    /**
     * 获取频次
     * @param key
     * @return
     */
    public int getFrequency(string key)
    {
        TermFrequency termFrequency = get(key);
        if (termFrequency == null) return 0;
        return termFrequency.getFrequency();
    }

    public void Add(string key)
    {
        TermFrequency termFrequency = trie.get(key);
        if (termFrequency == null)
        {
            termFrequency = new TermFrequency(key);
            trie.Add(key, termFrequency);
        }
        else
        {
            termFrequency.increase();
        }
    }

    //@Override
    public bool saveTxtTo(string path)
    {
        if ("=".Equals(delimeter))
        {
            LinkedList<TermFrequency> termFrequencyLinkedList = new LinkedList<TermFrequency>();
            foreach (KeyValuePair<string, TermFrequency> entry in trie.entrySet())
            {
                termFrequencyLinkedList.Add(entry.Value);
            }
            return IOUtil.saveCollectionToTxt(termFrequencyLinkedList, path);
        }
        else
        {
            var outList = new List<string>(size());
            foreach (KeyValuePair<string, TermFrequency> entry in trie.entrySet())
            {
                outList.Add(entry.Key + delimeter + entry.Value.getFrequency());
            }
            return IOUtil.saveCollectionToTxt(outList, path);
        }
    }

    /**
     * 仅仅将值保存到文件
     * @param path
     * @return
     */
    public bool saveKeyTo(string path)
    {
        LinkedList<string> keyList = new LinkedList<string>();
        foreach (KeyValuePair<string, TermFrequency> entry in trie.entrySet())
        {
            keyList.Add(entry.Key);
        }
        return IOUtil.saveCollectionToTxt(keyList, path);
    }

    /**
     * 按照频率从高到低排序的条目
     * @return
     */
    public HashSet<TermFrequency> values()
    {
        HashSet<TermFrequency> set = new HashSet<TermFrequency>(Collections.reverseOrder());

        foreach (KeyValuePair<string, TermFrequency> entry in entrySet())
        {
            set.Add(entry.Value);
        }

        return set;
    }
}
