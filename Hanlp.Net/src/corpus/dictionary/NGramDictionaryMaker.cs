/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/9 20:19</create-date>
 *
 * <copyright file="NGramDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.document.sentence.word;

namespace com.hankcs.hanlp.corpus.dictionary;




/**
 * 2-gram词典制作工具
 *
 * @author hankcs
 */
public class NGramDictionaryMaker
{
    BinTrie<int> trie;
    /**
     * 转移矩阵
     */
    TMDictionaryMaker tmDictionaryMaker;

    public NGramDictionaryMaker()
    {
        trie = new BinTrie<int>();
        tmDictionaryMaker = new TMDictionaryMaker();
    }

    public void addPair(IWord first, IWord second)
    {
        string combine = first.getValue() + "@" + second.getValue();
        int frequency = trie.get(combine);
        if (frequency == null) frequency = 0;
        trie.put(combine, frequency + 1);
        // 同时还要统计标签的转移情况
        tmDictionaryMaker.addPair(first.getLabel(), second.getLabel());
    }

    /**
     * 保存NGram词典和转移矩阵
     *
     * @param path
     * @return
     */
    public bool saveTxtTo(string path)
    {
        saveNGramToTxt(path + ".ngram.txt");
        saveTransformMatrixToTxt(path + ".tr.txt");
        return true;
    }

    /**
     * 保存NGram词典
     *
     * @param path
     * @return
     */
    public bool saveNGramToTxt(string path)
    {
        try
        {
            BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(IOUtil.newOutputStream(path)));
            foreach (KeyValuePair<string, int> entry in trie)
            {
                bw.write(entry.Key + " " + entry.Value);
                bw.newLine();
            }
            bw.close();
        }
        catch (Exception e)
        {
            logger.warning("在保存NGram词典到" + path + "时发生异常" + e);
            return false;
        }

        return true;
    }

    /**
     * 保存转移矩阵
     *
     * @param path
     * @return
     */
    public bool saveTransformMatrixToTxt(string path)
    {
        return tmDictionaryMaker.saveTxtTo(path);
    }
}
