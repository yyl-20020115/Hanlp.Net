/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/19 18:53</create-date>
 *
 * <copyright file="CoNLLLoader.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.corpus.dependency.CoNll;



/**
 * CoNLL格式依存语料加载
 * @author hankcs
 */
public class CoNLLLoader
{
    public static List<CoNLLSentence> loadSentenceList(string path)
    {
        List<CoNLLSentence> result = new ();
        List<CoNllLine> lineList = new ();
        foreach (string line in IOUtil.readLineListWithLessMemory(path))
        {
            if (line.Trim().Length == 0)
            {
                result.Add(new CoNLLSentence(lineList));
                lineList = new ();
                continue;
            }
            lineList.Add(new CoNllLine(line.Split("\t")));
        }

        return result;
    }
}
