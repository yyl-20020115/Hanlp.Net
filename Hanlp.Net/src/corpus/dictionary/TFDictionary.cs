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
namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * 词频词典
 * @author hankcs
 */
public class TFDictionary : SimpleDictionary<TermFrequency> : ISaveAble
{
    string delimeter;

    public TFDictionary(string delimeter)
    {
        this.delimeter = delimeter;
    }

    public TFDictionary()
    {
        this("=");
    }

    //@Override
    protected KeyValuePair<string, TermFrequency> onGenerateEntry(string line)
    {
        string[] param = line.split(delimeter);
        return new AbstractMap.SimpleEntry<string, TermFrequency>(param[0], new TermFrequency(param[0], int.valueOf(param[1])));
    }

    /**
     * 合并自己（主词典）和某个词频词典
     * @param dictionary 某个词频词典
     * @param limit 如果该词频词典试图引入一个词语，其词频不得超过此limit（如果不需要使用limit功能，可以传入int.MAX_VALUE）
     * @param add 设为true则是词频叠加模式，否则是词频覆盖模式
     * @return 词条的增量
     */
    public int combine(TFDictionary dictionary, int limit, bool add)
    {
        int preSize = trie.size();
        for (KeyValuePair<string, TermFrequency> entry : dictionary.trie.entrySet())
        {
            TermFrequency termFrequency = trie.get(entry.getKey());
            if (termFrequency == null)
            {
                trie.put(entry.getKey(), new TermFrequency(entry.getKey(), Math.min(limit, entry.getValue().getValue())));
            }
            else
            {
                if (add)
                {
                    termFrequency.setValue(termFrequency.getValue() + Math.min(limit, entry.getValue().getValue()));
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
    public static int combine(string... path)
    {
        TFDictionary dictionaryMain = new TFDictionary();
        dictionaryMain.load(path[0]);
        int preSize = dictionaryMain.trie.size();
        for (int i = 1; i < path.length; ++i)
        {
            TFDictionary dictionary = new TFDictionary();
            dictionary.load(path[i]);
            dictionaryMain.combine(dictionary, 1, true);
        }
        try
        {
            BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(IOUtil.newOutputStream(path[0]), "UTF-8"));
            for (KeyValuePair<string, TermFrequency> entry : dictionaryMain.trie.entrySet())
            {
                bw.write(entry.getKey());
                bw.write(' ');
                bw.write(string.valueOf(entry.getValue().getValue()));
                bw.newLine();
            }
            bw.close();
        }
        catch (Exception e)
        {
            e.printStackTrace();
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

    public void add(string key)
    {
        TermFrequency termFrequency = trie.get(key);
        if (termFrequency == null)
        {
            termFrequency = new TermFrequency(key);
            trie.put(key, termFrequency);
        }
        else
        {
            termFrequency.increase();
        }
    }

    //@Override
    public bool saveTxtTo(string path)
    {
        if ("=".equals(delimeter))
        {
            LinkedList<TermFrequency> termFrequencyLinkedList = new LinkedList<TermFrequency>();
            for (KeyValuePair<string, TermFrequency> entry : trie.entrySet())
            {
                termFrequencyLinkedList.add(entry.getValue());
            }
            return IOUtil.saveCollectionToTxt(termFrequencyLinkedList, path);
        }
        else
        {
            ArrayList<string> outList = new ArrayList<string>(size());
            for (KeyValuePair<string, TermFrequency> entry : trie.entrySet())
            {
                outList.add(entry.getKey() + delimeter + entry.getValue().getFrequency());
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
        for (KeyValuePair<string, TermFrequency> entry : trie.entrySet())
        {
            keyList.add(entry.getKey());
        }
        return IOUtil.saveCollectionToTxt(keyList, path);
    }

    /**
     * 按照频率从高到低排序的条目
     * @return
     */
    public TreeSet<TermFrequency> values()
    {
        TreeSet<TermFrequency> set = new TreeSet<TermFrequency>(Collections.reverseOrder());

        for (KeyValuePair<string, TermFrequency> entry : entrySet())
        {
            set.add(entry.getValue());
        }

        return set;
    }
}
