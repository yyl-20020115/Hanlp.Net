/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/7 11:04</create-date>
 *
 * <copyright file="HMMSegment.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
namespace com.hankcs.hanlp.seg.HMM;



/**
 * 基于2阶HMM（A Second-Order Hidden Markov Model, TriGram3阶文法模型）+ BMES序列标注的分词器
 *
 * @author hankcs
 */
public class HMMSegment : CharacterBasedSegment
{
    CharacterBasedGenerativeModel model;

    public HMMSegment()
    {
        this(HanLP.Config.HMMSegmentModelPath);
    }

    public HMMSegment(string modelPath)
    {
        model = GlobalObjectPool.get(modelPath);
        if (model != null) return;
        model = new CharacterBasedGenerativeModel();
        long start = DateTime.Now.Microsecond;
        logger.info("开始从[ " + modelPath + " ]加载2阶HMM模型");
        try
        {
            ByteArray byteArray = ByteArray.createByteArray(modelPath);
            if (byteArray == null)
            {
                throw new ArgumentException("HMM分词模型[ " + modelPath + " ]不存在");
            }
            model.load(byteArray);
        }
        catch (Exception e)
        {
            throw new ArgumentException("发生了异常：" + TextUtility.exceptionToString(e));
        }
        logger.info("加载成功，耗时：" + (DateTime.Now.Microsecond - start) + " ms");
        GlobalObjectPool.put(modelPath, model);
    }

    //@Override
    protected List<Term> roughSegSentence(char[] sentence)
    {
        char[] tag = model.tag(sentence);
        List<Term> termList = new LinkedList<Term>();
        int offset = 0;
        for (int i = 0; i < tag.Length; offset += 1, ++i)
        {
            switch (tag[i])
            {
                case 'b':
                {
                    int begin = offset;
                    while (tag[i] != 'e')
                    {
                        offset += 1;
                        ++i;
                        if (i == tag.Length)
                        {
                            break;
                        }
                    }
                    if (i == tag.Length)
                    {
                        termList.Add(new Term(new string(sentence, begin, offset - begin), null));
                    }
                    else
                        termList.Add(new Term(new string(sentence, begin, offset - begin + 1), null));
                }
                break;
                default:
                {
                    termList.Add(new Term(new string(sentence, offset, 1), null));
                }
                break;
            }
        }

        return termList;
    }
}
