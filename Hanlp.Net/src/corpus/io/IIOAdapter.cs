/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-07 PM4:40</create-date>
 *
 * <copyright file="IIOAdapter.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.io;


/**
 * IO适配器接口<br>
 * 实现该接口以移植HanLP到不同的平台
 *
 * @author hankcs
 */
public interface IIOAdapter
{
    /**
     * 打开一个文件以供读取
     * @param path 文件路径
     * @return 一个输入流
     * @ 任何可能的IO异常
     */
    Stream open(string path) ;

    /**
     * 创建一个新文件以供输出
     * @param path 文件路径
     * @return 一个输出流
     * @ 任何可能的IO异常
     */
    Stream create(string path) ;
}
