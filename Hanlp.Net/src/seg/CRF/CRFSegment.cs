/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/10 13:44</create-date>
 *
 * <copyright file="CRFSegment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.model;
using com.hankcs.hanlp.model.crf;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.seg.CRF;





/**
 * 基于CRF的分词器
 *
 * @author hankcs
 * @deprecated 已废弃，请使用{@link com.hankcs.hanlp.model.crf.CRFLexicalAnalyzer}
 */
public class CRFSegment : CharacterBasedSegment
{
    private CRFModel crfModel;

    public CRFSegment(CRFSegmentModel crfModel)
    {
        this.crfModel = crfModel;
    }

    public CRFSegment(string modelPath)
    {
        logger.warning("已废弃CRFSegment，请使用功能更丰富、设计更优雅的CRFLexicalAnalyzer");
        crfModel = GlobalObjectPool.get(modelPath);
        if (crfModel != null)
        {
            return;
        }
        logger.info("CRF分词模型正在加载 " + modelPath);
        long start = DateTime.Now.Microsecond;
        crfModel = CRFModel.loadTxt(modelPath, new CRFSegmentModel(new BinTrie<FeatureFunction>()));
        if (crfModel == null)
        {
            string error = "CRF分词模型加载 " + modelPath + " 失败，耗时 " + (DateTime.Now.Microsecond - start) + " ms";
            logger.severe(error);
            throw new ArgumentException(error);
        }
        else
            logger.info("CRF分词模型加载 " + modelPath + " 成功，耗时 " + (DateTime.Now.Microsecond - start) + " ms");
        GlobalObjectPool.Add(modelPath, crfModel);
    }

    // 已废弃，请使用功能更丰富、设计更优雅的{@link com.hankcs.hanlp.model.crf.CRFLexicalAnalyzer}。
    public CRFSegment()
    {
        this(HanLP.Config.CRFSegmentModelPath);
    }

    //@Override
    protected List<Term> roughSegSentence(char[] sentence)
    {
        if (sentence.Length == 0) return new();
        char[] sentenceConverted = CharTable.convert(sentence);
        Table table = new Table();
        table.v = atomSegmentToTable(sentenceConverted);
        crfModel.tag(table);
        List<Term> termList = new ();
        if (HanLP.Config.DEBUG)
        {
            Console.WriteLine("CRF标注结果");
            Console.WriteLine(table);
        }
        int offset = 0;
        OUTER:
        for (int i = 0; i < table.v.Length; offset += table.v[i][1].Length, ++i)
        {
            string[] line = table.v[i];
            switch (line[2][0])
            {
                case 'B':
                {
                    int begin = offset;
                    while (table.v[i][2][0] != 'E')
                    {
                        offset += table.v[i][1].Length;
                        ++i;
                        if (i == table.v.Length)
                        {
                            break;
                        }
                    }
                    if (i == table.v.Length)
                    {
                        termList.Add(new Term(new string(sentence, begin, offset - begin), toDefaultNature(table.v[i][0])));
                        break OUTER;
                    }
                    else
                        termList.Add(new Term(new string(sentence, begin, offset - begin + table.v[i][1].Length), toDefaultNature(table.v[i][0])));
                }
                break;
                default:
                {
                    termList.Add(new Term(new string(sentence, offset, table.v[i][1].Length), toDefaultNature(table.v[i][0])));
                }
                break;
            }
        }
        return termList;
    }

    protected static Nature toDefaultNature(string compiledChar)
    {
        if (compiledChar.Equals("M"))
            return Nature.m;
        if (compiledChar.Equals("W"))
            return Nature.nx;
        return null;
    }

    public static List<string> atomSegment(char[] sentence)
    {
        List<string> atomList = new (sentence.Length);
        int maxLen = sentence.Length - 1;
        StringBuilder sbAtom = new StringBuilder();
        _out:
        for (int i = 0; i < sentence.Length; i++)
        {
            if (sentence[i] >= '0' && sentence[i] <= '9')
            {
                sbAtom.Append(sentence[i]);
                if (i == maxLen)
                {
                    atomList.Add(sbAtom.ToString());
                    sbAtom.setLength(0);
                    break;
                }
                char c = sentence[++i];
                while (c == '.' || c == '%' || (c >= '0' && c <= '9'))
                {
                    sbAtom.Append(sentence[i]);
                    if (i == maxLen)
                    {
                        atomList.Add(sbAtom.ToString());
                        sbAtom.setLength(0);
                        break _out;
                    }
                    c = sentence[++i];
                }
                atomList.Add(sbAtom.ToString());
                sbAtom.setLength(0);
                --i;
            }
            else if (CharacterHelper.isEnglishLetter(sentence[i]))
            {
                sbAtom.Append(sentence[i]);
                if (i == maxLen)
                {
                    atomList.Add(sbAtom.ToString());
                    sbAtom.setLength(0);
                    break;
                }
                char c = sentence[++i];
                while (CharacterHelper.isEnglishLetter(c))
                {
                    sbAtom.Append(sentence[i]);
                    if (i == maxLen)
                    {
                        atomList.Add(sbAtom.ToString());
                        sbAtom.setLength(0);
                        break _out;
                    }
                    c = sentence[++i];
                }
                atomList.Add(sbAtom.ToString());
                sbAtom.setLength(0);
                --i;
            }
            else
            {
                atomList.Add(string.valueOf(sentence[i]));
            }
        }

        return atomList;
    }

    public static string[][] atomSegmentToTable(char[] sentence)
    {
        string[][] table = new string[sentence.Length][3];
        int size = 0;
        int maxLen = sentence.Length - 1;
        StringBuilder sbAtom = new StringBuilder();
        _out:
        for (int i = 0; i < sentence.Length; i++)
        {
            if (sentence[i] >= '0' && sentence[i] <= '9')
            {
                sbAtom.Append(sentence[i]);
                if (i == maxLen)
                {
                    table[size][0] = "M";
                    table[size][1] = sbAtom.ToString();
                    ++size;
                    sbAtom.setLength(0);
                    break;
                }
                char c = sentence[++i];
                while (c == '.' || c == '%' || (c >= '0' && c <= '9'))
                {
                    sbAtom.Append(sentence[i]);
                    if (i == maxLen)
                    {
                        table[size][0] = "M";
                        table[size][1] = sbAtom.ToString();
                        ++size;
                        sbAtom.setLength(0);
                        break _out;
                    }
                    c = sentence[++i];
                }
                table[size][0] = "M";
                table[size][1] = sbAtom.ToString();
                ++size;
                sbAtom.setLength(0);
                --i;
            }
            else if (CharacterHelper.isEnglishLetter(sentence[i]) || sentence[i] == ' ')
            {
                sbAtom.Append(sentence[i]);
                if (i == maxLen)
                {
                    table[size][0] = "W";
                    table[size][1] = sbAtom.ToString();
                    ++size;
                    sbAtom.setLength(0);
                    break;
                }
                char c = sentence[++i];
                while (CharacterHelper.isEnglishLetter(c) || c == ' ')
                {
                    sbAtom.Append(sentence[i]);
                    if (i == maxLen)
                    {
                        table[size][0] = "W";
                        table[size][1] = sbAtom.ToString();
                        ++size;
                        sbAtom.setLength(0);
                        break _out;
                    }
                    c = sentence[++i];
                }
                table[size][0] = "W";
                table[size][1] = sbAtom.ToString();
                ++size;
                sbAtom.setLength(0);
                --i;
            }
            else
            {
                table[size][0] = table[size][1] = string.valueOf(sentence[i]);
                ++size;
            }
        }

        return resizeArray(table, size);
    }

    /**
     * 数组减肥，原子分词可能会导致表格比原来的短
     *
     * @param array
     * @param size
     * @return
     */
    private static string[][] resizeArray(string[][] array, int size)
    {
        if (array.Length == size) return array;
        string[][] nArray = new string[size][];
        Array.Copy(array, 0, nArray, 0, size);
        return nArray;
    }

    //@Override
    public Segment enableNumberQuantifierRecognize(bool enable)
    {
        throw new InvalidOperationException("暂不支持");
//        enablePartOfSpeechTagging(enable);
//        return base.enableNumberQuantifierRecognize(enable);
    }
}
