/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 20:00</create-date>
 *
 * <copyright file="NGramDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * 转移矩阵词典制作工具
 * @author hankcs
 */
public class TMDictionaryMaker : ISaveAble
{
    Dictionary<string, Dictionary<string, int>> transferMatrix;

    public TMDictionaryMaker()
    {
        transferMatrix = new TreeMap<string, Dictionary<string, int>>();
    }

    /**
     * 添加一个转移例子，会在内部完成统计
     * @param first
     * @param second
     */
    public void addPair(string first, string second)
    {
        Dictionary<string, int> firstMatrix = transferMatrix.get(first);
        if (firstMatrix == null)
        {
            firstMatrix = new TreeMap<string, int>();
            transferMatrix.put(first, firstMatrix);
        }
        int frequency = firstMatrix.get(second);
        if (frequency == null) frequency = 0;
        firstMatrix.put(second, frequency + 1);
    }

    //@Override
    public string toString()
    {
        Set<string> labelSet = new TreeSet<string>();
        for (KeyValuePair<string, Dictionary<string, int>> first : transferMatrix.entrySet())
        {
            labelSet.add(first.getKey());
            labelSet.addAll(first.getValue().keySet());
        }
        final StringBuilder sb = new StringBuilder();
        sb.Append(' ');
        for (string key : labelSet)
        {
            sb.Append(',');
            sb.Append(key);
        }
        sb.Append('\n');
        for (string first : labelSet)
        {
            Dictionary<string, int> firstMatrix = transferMatrix.get(first);
            if (firstMatrix == null) firstMatrix = new TreeMap<string, int>();
            sb.Append(first);
            for (string second : labelSet)
            {
                sb.Append(',');
                int frequency = firstMatrix.get(second);
                if (frequency == null) frequency = 0;
                sb.Append(frequency);
            }
            sb.Append('\n');
        }
        return sb.toString();
    }

    //@Override
    public bool saveTxtTo(string path)
    {
        try
        {
            BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(IOUtil.newOutputStream(path)));
            bw.write(toString());
            bw.close();
        }
        catch (Exception e)
        {
            logger.warning("在保存转移矩阵词典到" + path + "时发生异常" + e);
            return false;
        }
        return true;
    }
}
