/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/26 16:21</create-date>
 *
 * <copyright file="Evaluater.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.dependency.CoNll;


/**
 * 测试工具
 * @author hankcs
 */
public class Evaluator
{
    float U, L, D, A;
    int sentenceCount;
    long start;

    public Evaluator()
    {
        start = DateTime.Now.Microsecond;
    }

    public void e(CoNLLSentence right, CoNLLSentence test)
    {
        ++sentenceCount;
        A += right.word.length;
        for (int i = 0; i < test.word.length; ++i)
        {
            if (test.word[i].HEAD.ID == right.word[i].HEAD.ID)
            {
                ++U;
                if (right.word[i].DEPREL.equals(test.word[i].DEPREL))
                {
                    ++L;
                    if (test.word[i].HEAD.ID != 0)
                    {
                        ++D;
                    }
                }
            }
        }
    }

    public float getUA()
    {
        return U /  A;
    }

    public float getLA()
    {
        return L / A;
    }

    public float getDA()
    {
        return D / (A - sentenceCount);
    }

    //@Override
    public string toString()
    {
        NumberFormat percentFormat = NumberFormat.getPercentInstance();
        percentFormat.setMinimumFractionDigits(2);
        StringBuilder sb = new StringBuilder();
        sb.Append("UA: ");
        sb.Append(percentFormat.format(getUA()));
        sb.Append('\t');
        sb.Append("LA: ");
        sb.Append(percentFormat.format(getLA()));
        sb.Append('\t');
        sb.Append("DA: ");
        sb.Append(percentFormat.format(getDA()));
        sb.Append('\t');
        sb.Append("sentences: ");
        sb.Append(sentenceCount);
        sb.Append('\t');
        sb.Append("speed: ");
        sb.Append(sentenceCount / (float)(DateTime.Now.Microsecond - start) * 1000);
        sb.Append(" sent/s");
        return sb.toString();
    }
}
