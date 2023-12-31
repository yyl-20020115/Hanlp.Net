/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/7 19:38</create-date>
 *
 * <copyright file="DemoJapaneseNameRecognition.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.demo;



/**
 * 日本人名识别
 * @author hankcs
 */
public class DemoJapaneseNameRecognition
{
    public static void Main(String[] args)
    {
        String[] testCase = new String[]{
                "北川景子参演了林诣彬导演的《速度与激情3》",
                "林志玲亮相网友:确定不是波多野结衣？",
                "龟山千广和近藤公园在龟山公园里喝酒赏花",
        };
        Segment segment = HanLP.newSegment().enableJapaneseNameRecognize(true);
        foreach (String sentence in testCase)
        {
            List<Term> termList = segment.seg(sentence);
            Console.WriteLine(termList);
        }
    }
}
