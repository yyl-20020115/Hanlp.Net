/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 下午7:36</create-date>
 *
 * <copyright file="POSTagger.java">
 * Copyright (c) 2018, Han He. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.tokenizer.lexical;


/**
 * 词性标注接口
 *
 * @author hankcs
 */
public interface POSTagger
{
    /**
     * 词性标注
     *
     * @param words 单词
     * @return 词性数组
     */
    string[] Tag(params string[] words);

    /**
     * 词性标注
     *
     * @param wordList 单词
     * @return 词性数组
     */
    string[] Tag(List<string> wordList);
}
