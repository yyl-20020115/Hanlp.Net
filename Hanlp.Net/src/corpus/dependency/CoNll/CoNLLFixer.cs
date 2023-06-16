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
        for (string line : IOUtil.readLineListWithLessMemory(path))
        {
            if (line.trim().Length == 0)
            {
                sbOut.Append(line);
                sbOut.Append('\n');
                continue;
            }
            string[] args = line.Split("\t");
            for (int i = 10 - args.Length; i > 0; --i)
            {
                line += "\t_";
            }
            sbOut.Append(line);
            sbOut.Append('\n');
        }
        return IOUtil.saveTxt(path + ".fixed.txt", sbOut.toString());
    }
}
