/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/10 0:23</create-date>
 *
 * <copyright file="ISaveAble.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.io;


/**
 * 可写入或读取二进制
 * @author hankcs
 */
public interface ICacheAble
{
    /**
     * 写入
     * @param Out
     * @throws Exception
     */
    void save(Stream Out);

    /**
     * 载入
     * @param byteArray
     * @return
     */
    bool load(ByteArray byteArray); // 目前的设计并不好，应该抛异常而不是返回布尔值
}
