/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/7/29 16:37</create-date>
 *
 * <copyright file="DumpHander.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.io;


/**
 * @author hankcs
 */
public abstract class LineHandler
{
    string delimiter = "\t";

    public LineHandler(string delimiter)
    {
        this.delimiter = delimiter;
    }

    public LineHandler()
    {
    }

    public void handle(string line)
    {
        List<string> tokenList = new ();
        int start = 0;
        int end;
        while ((end = line.IndexOf(delimiter, start)) != -1)
        {
            tokenList.Add(line.substring(start, end));
            start = end + 1;
        }
        tokenList.Add(line.substring(start, line.Length));
        handle(tokenList.ToArray());
    }

    public void done() 
    {
        // do noting
    }

    public abstract void handle(string[] _params) ;
}
