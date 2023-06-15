/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-03 上午10:23</create-date>
 *
 * <copyright file="CWSEvaluator.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.seg.common;



/**
 * 中文分词评测工具
 *
 * @author hankcs
 */
public class CWSEvaluator
{
    private int A_size, B_size, A_cap_B_size, OOV, OOV_R, IV, IV_R;
    private Set<string> dic;

    public CWSEvaluator()
    {
    }

    public CWSEvaluator(Set<string> dic)
    {
        this.dic = dic;
    }

    public CWSEvaluator(string dictPath) 
    {
        this(new TreeSet<string>());
        if (dictPath == null) return;
        try
        {
            IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(dictPath);
            for (string word : lineIterator)
            {
                word = word.trim();
                if (word.isEmpty()) continue;
                dic.add(word);
            }
        }
        catch (Exception e)
        {
            throw new IOException(e);
        }
    }

    /**
     * 获取PRF
     *
     * @param percentage 百分制
     * @return
     */
    public Result getResult(bool percentage)
    {
        float p = A_cap_B_size / (float) B_size;
        float r = A_cap_B_size / (float) A_size;
        if (percentage)
        {
            p *= 100;
            r *= 100;
        }
        float oov_r = Float.NaN;
        if (OOV > 0)
        {
            oov_r = OOV_R / (float) OOV;
            if (percentage)
                oov_r *= 100;
        }
        float iv_r = Float.NaN;
        if (IV > 0)
        {
            iv_r = IV_R / (float) IV;
            if (percentage)
                iv_r *= 100;
        }
        return new Result(p, r, 2 * p * r / (p + r), oov_r, iv_r);
    }


    /**
     * 获取PRF
     *
     * @return
     */
    public Result getResult()
    {
        return getResult(true);
    }

    /**
     * 比较标准答案与分词结果
     *
     * @param gold
     * @param pred
     */
    public void compare(string gold, string pred)
    {
        string[] wordArray = gold.Split("\\s+");
        A_size += wordArray.length;
        string[] predArray = pred.Split("\\s+");
        B_size += predArray.length;

        int goldIndex = 0, predIndex = 0;
        int goldLen = 0, predLen = 0;

        while (goldIndex < wordArray.length && predIndex < predArray.length)
        {
            if (goldLen == predLen)
            {
                if (wordArray[goldIndex].equals(predArray[predIndex]))
                {
                    if (dic != null)
                    {
                        if (dic.contains(wordArray[goldIndex]))
                            IV_R += 1;
                        else
                            OOV_R += 1;
                    }
                    A_cap_B_size++;
                    goldLen += wordArray[goldIndex].length();
                    predLen += wordArray[goldIndex].length();
                    goldIndex++;
                    predIndex++;
                }
                else
                {
                    goldLen += wordArray[goldIndex].length();
                    predLen += predArray[predIndex].length();
                    goldIndex++;
                    predIndex++;
                }
            }
            else if (goldLen < predLen)
            {
                goldLen += wordArray[goldIndex].length();
                goldIndex++;
            }
            else
            {
                predLen += predArray[predIndex].length();
                predIndex++;
            }
        }

        if (dic != null)
        {
            for (string word : wordArray)
            {
                if (dic.contains(word))
                    IV += 1;
                else
                    OOV += 1;
            }
        }
    }

    /**
     * 在标准答案与分词结果上执行评测
     *
     * @param goldFile
     * @param predFile
     * @return
     */
    public static Result evaluate(string goldFile, string predFile) 
    {
        return evaluate(goldFile, predFile, null);
    }

    /**
     * 标准化评测分词器
     *
     * @param segment    分词器
     * @param outputPath 分词预测输出文件
     * @param goldFile   测试集segmented file
     * @param dictPath   训练集单词列表
     * @return 一个储存准确率的结构
     * @
     */
    public static CWSEvaluator.Result evaluate(Segment segment, string outputPath, string goldFile, string dictPath) 
    {
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(goldFile);
        BufferedWriter bw = IOUtil.newBufferedWriter(outputPath);
        for (string line : lineIterator)
        {
            List<Term> termList = segment.seg(line.replaceAll("\\s+", "")); // 一些testFile与goldFile根本不匹配，比如MSR的testFile有些行缺少单词，所以用goldFile去掉空格代替
            int i = 0;
            for (Term term : termList)
            {
                bw.write(term.word);
                if (++i != termList.size())
                    bw.write("  ");
            }
            bw.newLine();
        }
        bw.close();
        CWSEvaluator.Result result = CWSEvaluator.evaluate(goldFile, outputPath, dictPath);
        return result;
    }

    /**
     * 标准化评测分词器
     *
     * @param segment    分词器
     * @param testFile   测试集raw text
     * @param outputPath 分词预测输出文件
     * @param goldFile   测试集segmented file
     * @param dictPath   训练集单词列表
     * @return 一个储存准确率的结构
     * @
     */
    public static CWSEvaluator.Result evaluate(Segment segment, string testFile, string outputPath, string goldFile, string dictPath) 
    {
        return evaluate(segment, outputPath, goldFile, dictPath);
    }

    /**
     * 在标准答案与分词结果上执行评测
     *
     * @param goldFile
     * @param predFile
     * @return
     */
    public static Result evaluate(string goldFile, string predFile, string dictPath) 
    {
        IOUtil.LineIterator goldIter = new IOUtil.LineIterator(goldFile);
        IOUtil.LineIterator predIter = new IOUtil.LineIterator(predFile);
        CWSEvaluator evaluator = new CWSEvaluator(dictPath);
        while (goldIter.hasNext() && predIter.hasNext())
        {
            evaluator.compare(goldIter.next(), predIter.next());
        }
        return evaluator.getResult();
    }

    public class Result
    {
        public float P, R, F1, OOV_R, IV_R;

        public Result(float p, float r, float f1, float OOV_R, float IV_R)
        {
            P = p;
            R = r;
            F1 = f1;
            this.OOV_R = OOV_R;
            this.IV_R = IV_R;
        }

        //@Override
        public string ToString()
        {
            return string.format("P:%.2f R:%.2f F1:%.2f OOV-R:%.2f IV-R:%.2f", P, R, F1, OOV_R, IV_R);
        }
    }
}
