/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/30 17:28</create-date>
 *
 * <copyright file="DictionaryUtil.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.corpus.util;



/**
 * @author hankcs
 */
public class DictionaryUtil
{
    /**
     * 给某个字典排序
     * @param path
     * @return
     */
    public static bool sortDictionary(string path)
    {
        try
        {
            TextReader br = new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            Dictionary map = new ();
            string line;

            while ((line = br.ReadLine()) != null)
            {
                string[] param = line.Split("\\s");
                map.Add(param[0], line);
            }
            br.Close();

            TextWriter bw = new TextWriter(new StreamWriter(IOUtil.newOutputStream(path)));
            for (KeyValuePair<string, string> entry : map.entrySet())
            {
                bw.write(entry.Value);
                bw.newLine();
            }
            bw.Close();
        }
        catch (Exception e)
        {
            //e.printStackTrace();
            return false;
        }

        return true;
    }
}
