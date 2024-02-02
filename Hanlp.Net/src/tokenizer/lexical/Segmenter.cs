/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 下午7:30</create-date>
 *
 * <copyright file="Segmenter.java">
 * Copyright (c) 2018, Han He. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.tokenizer.lexical;


/**
 * 分词器接口
 *
 * @author hankcs
 */
public interface Segmenter
{
    /**
     * 中文分词
     *
     * @param text 文本
     * @return 词语
     */
    List<string> Segment(string text);
    void Segment(string text, string normalized, List<string> output);
}
