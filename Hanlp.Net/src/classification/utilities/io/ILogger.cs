/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/16 AM11:15</create-date>
 *
 * <copyright file="ILogger.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.classification.utilities.io;

/**
 * 一个简单的日志接口
 * @author hankcs
 */
public interface ILogger
{
    void Out(string Format, params Object[] args);
    void Err(string Format, params Object[] args);
    void Start(string Format, params Object[] args);
    void Finish(string Format, params Object[] args);
}
