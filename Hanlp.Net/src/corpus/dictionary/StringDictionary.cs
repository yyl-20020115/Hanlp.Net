/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/1 19:53</create-date>
 *
 * <copyright file="StringDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * 满足 key=value 格式的词典，其中“=”可以自定义
 * @author hankcs
 */
public class StringDictionary : SimpleDictionary<string>
{
    /**
     * key value之间的分隔符
     */
    protected string separator;

    public StringDictionary(string separator)
    {
        this.separator = separator;
    }

    public StringDictionary()
        : this("=")
    {
        ;
    }

    //@Override
    protected override KeyValuePair<string, string> onGenerateEntry(string line)
    {
        string[] paramArray = line.Split(separator, 2);
        if (paramArray.Length != 2)
        {
            logger.warning("词典有一行读取错误： " + line);
            return null;
        }
        return new AbstractMap.SimpleEntry<string, string>(paramArray[0], paramArray[1]);
    }

    /**
     * 保存词典
     * @param path
     * @return 是否成功
     */
    public bool save(string path)
    {
        try
        {
            TextWriter bw = new StreamWriter(IOUtil.newOutputStream(path));
            foreach (KeyValuePair<string, string> entry in trie.entrySet())
            {
                bw.Write(entry.Key);
                bw.Write(separator);
                bw.Write(entry.Value);
                bw.WriteLine();
            }
            bw.Close();
        }
        catch (Exception e)
        {
            logger.warning("保存词典到" + path + "失败");
            return true;
        }
        return false;
    }

    /**
     * 将自己逆转过来返回
     * @return
     */
    public StringDictionary reverse()
    {
        StringDictionary dictionary = new StringDictionary(separator);
        foreach (KeyValuePair<string, string> entry in entrySet())
        {
            dictionary.trie.Add(entry.Value, entry.Key);
        }

        return dictionary;
    }
}
