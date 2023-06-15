/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/5/11 12:46</create-date>
 *
 * <copyright file="DemoMultithreadingSegment.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.model.crf;
using com.hankcs.hanlp.seg;
using System.Text;

namespace com.hankcs.demo;



/**
 * 演示多线程并行分词
 * 由于HanLP的任何分词器都是线程安全的，所以用户只需调用一个配置接口就可以启用任何分词器的并行化
 *
 * @author hankcs
 */
public class DemoMultithreadingSegment
{
    public static void Main(String[] args) 
    {
        Segment segment = new CRFLexicalAnalyzer(HanLP.Config.CRFCWSModelPath).enableCustomDictionary(false); // CRF分词器效果好，速度慢，并行化之后可以提高一些速度

        String text = "举办纪念活动铭记二战历史，不忘战争带给人类的深重灾难，是为了防止悲剧重演，确保和平永驻；" +
                "铭记二战历史，更是为了提醒国际社会，需要共同捍卫二战胜利成果和国际公平正义，" +
                "必须警惕和抵制在历史认知和维护战后国际秩序问题上的倒行逆施。";
        HanLP.Config.ShowTermNature = false;
        Console.WriteLine(segment.seg(text));
        int pressure = 10000;
        StringBuilder sbBigText = new StringBuilder(text.Length * pressure);
        for (int i = 0; i < pressure; i++)
        {
            sbBigText.Append(text);
        }
        text = sbBigText.ToString();
        GC.Collect();

        long start;
        double costTime;
        // 测个速度

        segment.enableMultithreading(false);
        start = DateTime.Now.Microsecond;
        segment.seg(text);
        costTime = (DateTime.Now.Microsecond - start) / (double) 1000;
        Console.WriteLine("单线程分词速度：%.2f字每秒\n", text.Length / costTime);
        GC.Collect();

        segment.enableMultithreading(true); // 或者 segment.enableMultithreading(4);
        start = DateTime.Now.Microsecond;
        segment.seg(text);
        costTime = (DateTime.Now.Microsecond - start) / (double) 1000;
        Console.WriteLine("多线程分词速度：%.2f字每秒\n", text.Length / costTime);
        GC.Collect();

        // Note:
        // 内部的并行化机制可以对1万字以上的大文本开启多线程分词
        // 另一方面，HanLP中的任何Segment本身都是线程安全的。
        // 你可以开10个线程用同一个CRFSegment对象切分任意文本，不需要任何线程同步的措施，每个线程都可以得到正确的结果。
    }
}
