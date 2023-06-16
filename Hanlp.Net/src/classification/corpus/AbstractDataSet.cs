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

    public IDataSet setTokenizer(ITokenizer tokenizer)
    {
        this.tokenizer = tokenizer;
        return this;
    }

    public Document convert(string category, string text)
    {
        string[] tokenArray = tokenizer.segment(text);
        return testingDataSet ?
                new Document(catalog.categoryId, lexicon.wordId, category, tokenArray) :
                new Document(catalog, lexicon, category, tokenArray);
    }

    public ITokenizer getTokenizer()
    {
        return tokenizer;
    }

    public Catalog getCatalog()
    {
        return catalog;
    }

    public Lexicon getLexicon()
    {
        return lexicon;
    }

    //@Override
    public IDataSet load(string folderPath, string charsetName) 
    {
        return load(folderPath, charsetName, 1.);
    }

    //@Override
    public IDataSet load(string folderPath) 
    {
        return load(folderPath, "UTF-8");
    }

    //@Override
    public bool isTestingDataSet()
    {
        return testingDataSet;
    }

    //@Override
    public IDataSet load(string folderPath, string charsetName, double percentage)
    {
        if (folderPath == null) throw new IllegalArgumentException("参数 folderPath == null");
        File root = new File(folderPath);
        if (!root.exists()) throw new IllegalArgumentException(string.format("目录 %s 不存在", root.getAbsolutePath()));
        if (!root.isDirectory())
            throw new IllegalArgumentException(string.format("目录 %s 不是一个目录", root.getAbsolutePath()));
        if (percentage > 1.0 || percentage < -1.0) throw new IllegalArgumentException("percentage 的绝对值必须介于[0, 1]之间");

        File[] folders = root.listFiles();
        if (folders == null) return null;
        logger.start("模式:%s\n文本编码:%s\n根目录:%s\n加载中...\n", testingDataSet ? "测试集" : "训练集", charsetName, folderPath);
        for (File folder : folders)
        {
            if (folder.isFile()) continue;
            File[] files = folder.listFiles();
            if (files == null) continue;
            string category = folder.getName();
            logger._out("[%s]...", category);
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

            int logEvery = (int) Math.ceil((e - b) / 10000f);
            for (int i = b; i < e; i++)
            {
                add(folder.getName(), TextProcessUtility.readTxt(files[i], charsetName));
                if (i % logEvery == 0)
                {
                    logger._out("%c[%s]...%.2f%%", 13, category, MathUtility.percentage(i - b + 1, e - b));
                }
            }
            logger._out(" %d 篇文档\n", e - b);
        }
        logger.finish(" 加载了 %d 个类目,共 %d 篇文档\n", getCatalog().size(), size());
        return this;
    }

    //@Override
    public IDataSet load(string folderPath, double rate)
    {
        return null;
    }

    //@Override
    public IDataSet add(Dictionary<string, string[]> testingDataSet)
    {
        for (KeyValuePair<string, string[]> entry : testingDataSet.entrySet())
        {
            for (string document : entry.getValue())
            {
                add(entry.getKey(), document);
            }
        }
        return this;
    }
}
