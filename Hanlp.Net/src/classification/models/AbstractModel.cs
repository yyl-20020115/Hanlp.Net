/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/13 PM8:43</create-date>
 *
 * <copyright file="IModel.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.tokenizers;
using com.hankcs.hanlp.collection.trie.bintrie;

//namespace com.hankcs.hanlp.classification.models;



/**
 * 所有文本分类模型的基类,包含基本的需要序列化的数据
 * @author hankcs
 */
public class AbstractModel //: ISerializable
{
    /**
     * 类目表
     */
    public string[] catalog;
    /**
     * 分词器
     */
    public ITokenizer tokenizer;
    /**
     * 词语到的映射
     */
    public BinTrie<int> wordIdTrie;
}