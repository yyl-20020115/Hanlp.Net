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
using com.hankcs.hanlp.corpus.io;
using System.Text;

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
        transferMatrix = new ();
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
            firstMatrix = new ();
            transferMatrix.Add(first, firstMatrix);
        }
        int frequency = firstMatrix.get(second);
        if (frequency == null) frequency = 0;
        firstMatrix.Add(second, frequency + 1);
    }

    //@Override
    public override string ToString()
    {
        HashSet<string> labelSet = new ();
        foreach (KeyValuePair<string, Dictionary<string, int>> first in transferMatrix)
        {
            labelSet.Add(first.Key);
            labelSet.UnionWith(first.Value.Keys);
        }
        StringBuilder sb = new StringBuilder();
        sb.Append(' ');
        foreach (string key in labelSet)
        {
            sb.Append(',');
            sb.Append(key);
        }
        sb.Append('\n');
        foreach (string first in labelSet)
        {
            Dictionary<string, int> firstMatrix = transferMatrix.get(first);
            if (firstMatrix == null) firstMatrix = new ();
            sb.Append(first);
            foreach (string second in labelSet)
            {
                sb.Append(',');
                int frequency = firstMatrix.get(second);
                if (frequency == null) frequency = 0;
                sb.Append(frequency);
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    //@Override
    public bool saveTxtTo(string path)
    {
        try
        {
            TextWriter bw = new TextWriter(new StreamWriter(IOUtil.newOutputStream(path)));
            bw.Write(ToString());
            bw.Close();
        }
        catch (Exception e)
        {
            logger.warning("在保存转移矩阵词典到" + path + "时发生异常" + e);
            return false;
        }
        return true;
    }
}
