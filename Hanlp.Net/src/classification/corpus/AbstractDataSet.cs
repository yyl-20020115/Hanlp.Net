/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/10 PM5:43</create-date>
 *
 * <copyright file="AbstractDataSet.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.models;
using com.hankcs.hanlp.classification.tokenizers;
using com.hankcs.hanlp.classification.utilities;
using com.hankcs.hanlp.utility;
using System.Collections;

namespace com.hankcs.hanlp.classification.corpus;




/**
 * @author hankcs
 */
public abstract class AbstractDataSet : IDataSet
{
    protected ITokenizer tokenizer;
    protected Catalog catalog;
    protected Lexicon lexicon;
    /**
     * 是否属于测试集
     */
    protected bool testingDataSet;

    /**
     * 构造测试集
     * @param model 待测试的模型
     */
    public AbstractDataSet(AbstractModel model)
    {
        lexicon = new Lexicon(model.wordIdTrie);
        tokenizer = model.tokenizer;
        catalog = new Catalog(model.catalog);
        testingDataSet = true;
    }

    public AbstractDataSet()
    {
        tokenizer = new HanLPTokenizer();
        catalog = new Catalog();
        lexicon = new Lexicon();
    }


    public Document Convert(string category, string text)
    {
        string[] tokenArray = tokenizer.Segment(text);
        return testingDataSet ?
                new Document(catalog.categoryId, lexicon.wordId, category, tokenArray) :
                new Document(catalog, lexicon, category, tokenArray);
    }

    public ITokenizer GetTokenizer()
    {
        return tokenizer;
    }
    public IDataSet SetTokenizer(ITokenizer tokenizer)
    {
        this.tokenizer = tokenizer;
        return this;
    }

    public Catalog Catalog => catalog;

    public Lexicon Lexicon => lexicon;

    //@Override
    public IDataSet Load(string folderPath, string charsetName) 
    {
        return Load(folderPath, charsetName, 1.0);
    }

    //@Override
    public IDataSet Load(string folderPath) 
    {
        return Load(folderPath, "UTF-8");
    }

    //@Override
    public bool IsTestingDataSet => testingDataSet;

    public abstract int Count { get; }

    //@Override
    public IDataSet Load(string folderPath, string charsetName, double percentage)
    {
        if (folderPath == null) throw new ArgumentException("参数 folderPath == null");
        string root = (folderPath);
        if (!root.exists()) throw new ArgumentException(string.Format("目录 %s 不存在", root));
        if (!root.isDirectory())
            throw new ArgumentException(string.Format("目录 %s 不是一个目录", root));
        if (percentage > 1.0 || percentage < -1.0) throw new ArgumentException("percentage 的绝对值必须介于[0, 1]之间");

        string[] folders = root.listFiles();
        if (folders == null) return null;
        logger.start("模式:%s\n文本编码:%s\n根目录:%s\n加载中...\n", testingDataSet ? "测试集" : "训练集", charsetName, folderPath);
        foreach (string folder in folders)
        {
            if (folder.isFile()) continue;
            string[] files = folder.listFiles();
            if (files == null) continue;
            string category = folder.Name;
            logger.Out("[%s]...", category);
            int b, e;
            if (percentage > 0)
            {
                b = 0;
                e = (int) (files.Length * percentage);
            }
            else
            {
                b = (int) (files.Length * (1 + percentage));
                e = files.Length;
            }

            int logEvery = (int) Math.Ceiling((e - b) / 10000f);
            for (int i = b; i < e; i++)
            {
                Add(folder.Name, TextProcessUtility.ReadTxt(files[i], charsetName));
                if (i % logEvery == 0)
                {
                    logger.Out("%c[%s]...%.2f%%", 13, category, MathUtility.percentage(i - b + 1, e - b));
                }
            }
            logger.Out(" %d 篇文档\n", e - b);
        }
        logger.finish(" 加载了 %d 个类目,共 %d 篇文档\n", Catalog.Count, Count);
        return this;
    }

    //@Override
    public IDataSet Load(string folderPath, double rate)
    {
        return null;
    }

    //@Override
    public IDataSet Add(Dictionary<string, string[]> testingDataSet)
    {
        foreach (KeyValuePair<string, string[]> entry in testingDataSet)
        {
            foreach (string document in entry.Value)
            {
                Add(entry.Key, document);
            }
        }
        return this;
    }
    public abstract Document Add(string category, string text);
    public abstract void Clear();
    public abstract IDataSet Shrink(int[] idMap);
    public abstract IEnumerator<Document> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
