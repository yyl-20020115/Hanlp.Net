/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/8 17:43</create-date>
 *
 * <copyright file="IWord.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.document.sentence.word;


/**
 * 词语接口
 * @author hankcs
 */
public interface IWord : Serializable
{
    /**
     * 获取单词
     * @return
     */
    string Value;

    /**
     * 获取标签
     * @return
     */
    string getLabel();

    /**
     * 设置标签
     * @param label
     */
    void setLabel(string label);

    /**
     * 设置单词
     * @param value
     */
    void setValue(string value);

    /**
     * 单词长度
     * @return
     */
    int Length();
}
