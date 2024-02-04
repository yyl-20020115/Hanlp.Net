/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/16 AM11:17</create-date>
 *
 * <copyright file="ConsoleLogger.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.classification.utilities.io;

/**
 * 输出到stdout和stderr的日志系统
 *
 * @author hankcs
 */
public class ConsoleLogger :Logger, ILogger
{
    /**
     * 默认日志
     */
    public static ILogger logger = new ConsoleLogger();
    long _start;

    public void Out(string Format, params Object[] args)
    {
        Console.WriteLine(Format, args);
    }

    public void Err(string Format, params Object[] args)
    {
        Console.Error.WriteLine(Format, args);
    }

    public void Start(string Format, params Object[] args)
    {
        Out(Format, args);
        _start = DateTime.Now.Microsecond;
    }

    public void Finish(string Format, params Object[] args)
    {
        Out(string.Format("耗时 %d ms", (DateTime.Now.Microsecond - _start) + Format, args));
    }

    private Level level;
    public void SetLevel(Level level)
    {
       this.level = level;
    }
}