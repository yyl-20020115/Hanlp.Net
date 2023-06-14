/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/7 20:14</create-date>
 *
 * <copyright file="DemoPosTagging.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.demo;


/**
 * 词性标注
 * @author hankcs
 */
public class DemoPosTagging
{
    public static void Main(String[] args)
    {
        String text = "教授正在教授自然语言处理课程";
        Segment segment = HanLP.newSegment();

        Console.WriteLine("未标注：" + segment.seg(text));
        segment.enablePartOfSpeechTagging(true);
        Console.WriteLine("标注后：" + segment.seg(text));
    }
}
