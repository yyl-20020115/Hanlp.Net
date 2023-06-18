/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2015/4/23 22:56</create-date>
 *
 * <copyright file="CharTable.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2015, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.other;



/**
 * 字符正规化表
 * @author hankcs
 */
public class CharTable
{
    /**
     * 正规化使用的对应表
     */
    public static char[] CONVERT;

    static CharTable()
    {
        long start = DateTime.Now.Microsecond;
        if (!load(HanLP.Config.CharTablePath))
        {
            throw new ArgumentException("字符正规化表加载失败");
        }
        logger.info("字符正规化表加载成功：" + (DateTime.Now.Microsecond - start) + " ms");
    }

    private static bool load(string path)
    {
        string binPath = path + Predefine.BIN_EXT;
        if (loadBin(binPath)) return true;
        CONVERT = new char[char.MaxValue + 1];
        for (int i = 0; i < CONVERT.Length; i++)
        {
            CONVERT[i] = (char) i;
        }
        IOUtil.LineIterator iterator = new IOUtil.LineIterator(path);
        while (iterator.hasNext())
        {
            string line = iterator.next();
            if (line == null) return false;
            if (line.Length != 3) continue;
            CONVERT[line[0]] = CONVERT[line.charAt(2)];
        }
        loadSpace();
        logger.info("正在缓存字符正规化表到" + binPath);
        IOUtil.saveObjectTo(CONVERT, binPath);

        return true;
    }
    
    private static void loadSpace() {
        for (int i = char.MIN_CODE_POINT; i <= char.MAX_CODE_POINT; i++) {
            if (char.isWhitespace(i) || char.isSpaceChar(i)) {
                CONVERT[i] = ' ';
            }
        }
    }

    private static bool loadBin(string path)
    {
        try
        {
            ObjectInputStream _in = new ObjectInputStream(IOUtil.newInputStream(path));
            CONVERT = (char[]) _in.readObject();
            in.close();
        }
        catch (Exception e)
        {
            logger.warning("字符正规化表缓存加载失败，原因如下：" + e);
            return false;
        }

        return true;
    }

    /**
     * 将一个字符正规化
     * @param c 字符
     * @return 正规化后的字符
     */
    public static char convert(char c)
    {
        return CONVERT[c];
    }

    public static char[] convert(char[] charArray)
    {
        char[] result = new char[charArray.Length];
        for (int i = 0; i < charArray.Length; i++)
        {
            result[i] = CONVERT[charArray[i]];
        }

        return result;
    }

    public static string convert(string sentence)
    {
        //assert sentence != null;
        char[] result = new char[sentence.Length];
        convert(sentence, result);

        return new string(result);
    }

    public static void convert(string charArray, char[] result)
    {
        for (int i = 0; i < charArray.Length; i++)
        {
            result[i] = CONVERT[charArray.charAt(i)];
        }
    }

    /**
     * 正规化一些字符（原地正规化）
     * @param charArray 字符
     */
    public static void normalization(char[] charArray)
    {
        //assert charArray != null;
        for (int i = 0; i < charArray.Length; i++)
        {
            charArray[i] = CONVERT[charArray[i]];
        }
    }

    public static void normalize(Sentence sentence)
    {
        foreach (IWord word in sentence)
        {
            if (word is CompoundWord)
            {
                foreach (Word w in ((CompoundWord) word).innerList)
                {
                    w.value = convert(w.value);
                }
            }
            else
                word.setValue(word.getValue());
        }
    }
}
