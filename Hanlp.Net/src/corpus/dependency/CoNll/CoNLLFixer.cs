/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/19 18:55</create-date>
 *
 * <copyright file="CoNLLFixer.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.corpus.dependency.CoNll;


/**
 * 修正一些非10行的依存语料
 * @author hankcs
 */
public class CoNLLFixer
{
    public static bool fix(string path)
    {
        StringBuilder sbOut = new StringBuilder();
        foreach (string line in IOUtil.readLineListWithLessMemory(path))
        {
            if (line.Trim().Length == 0)
            {
                sbOut.Append(line);
                sbOut.Append('\n');
                continue;
            }
            string[] args = line.Split("\t");
            string ln = line;
            for (int i = 10 - args.Length; i > 0; --i)
            {
                ln += "\t_";
            }
            sbOut.Append(ln);
            sbOut.Append('\n');
        }
        return IOUtil.saveTxt(path + ".fixed.txt", sbOut.ToString());
    }
}
