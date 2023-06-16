/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/18 16:23</create-date>
 *
 * <copyright file="TestSegment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.seg.common.wrapper;
using com.hankcs.hanlp.seg.Dijkstra;
using com.hankcs.hanlp.seg.Other;
using com.hankcs.hanlp.seg.Viterbi;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.seg;



/**
 * @author hankcs
 */
[TestClass]

public class SegmentTest : TestCase
{
    [TestMethod]
    public void TestSeg() 
    {
//        HanLP.Config.enableDebug();
        Segment segment = new DijkstraSegment();
//        Console.WriteLine(segment.seg(
//                "我遗忘我的密码了"
//        ));
    }
    [TestMethod]
    public void TestIssue880() 
    {
//        HanLP.Config.enableDebug();
        Segment segment = new DijkstraSegment();
        Console.WriteLine(segment.seg("龚学平等表示会保证金云鹏的安全"));
        Console.WriteLine(segment.seg("王中军代表蓝队发言"));
    }
    [TestMethod]

    public void TestViterbi() 
    {
//        HanLP.Config.enableDebug(true);
        CustomDictionary.add("网剧");
        Segment seg = new DijkstraSegment();
        List<Term> termList = seg.seg("优酷总裁魏明介绍了优酷2015年的内容战略，表示要以“大电影、大网剧、大综艺”为关键词");
//        Console.WriteLine(termList);
    }
    [TestMethod]

    public void TestExtendViterbi() 
    {
        HanLP.Config.enableDebug(false);
        String path = Environment.GetEnvironmentVariable("user.dir") + "/" + "data/dictionary/custom/CustomDictionary.txt;" +
            Environment.GetEnvironmentVariable("user.dir") + "/" + "data/dictionary/custom/全国地名大全.txt";
        path = path.Replace("\\", "/");
        String text = "一半天帕克斯曼是走不出丁字桥镇的";
        Segment segment = HanLP.newSegment().enableCustomDictionary(false);
        Segment seg = new ViterbiSegment(path);
        Console.WriteLine("不启用字典的分词结果：" + segment.seg(text));
        Console.WriteLine("默认分词结果：" + HanLP.segment(text));
        seg.enableCustomDictionaryForcing(true).enableCustomDictionary(true);
        List<Term> termList = seg.seg(text);
        Console.WriteLine("自定义字典的分词结果：" + termList);
    }
    [TestMethod]

    public void TestNotional() 
    {
//        Console.WriteLine(NotionalTokenizer.segment("算法可以宽泛的分为三类"));
    }
    [TestMethod]

    public void TestNGram() 
    {
//        Console.WriteLine(CoreBiGramTableDictionary.getBiFrequency("牺", "牲"));
    }
    [TestMethod]

    public void TestShortest() 
    {
        //        HanLP.Config.enableDebug();
        //        Segment segment = new ViterbiSegment().enableAllNamedEntityRecognize(true);
        //        Console.WriteLine(segment.seg("把市场经济奉行的等价交换原则引入党的生活和国家机关政务活动中"));
    }
    [TestMethod]


    public void TestIndexSeg() 
    {
//        Console.WriteLine(IndexTokenizer.segment("中科院预测科学研究中心学术委员会"));
    }
    [TestMethod]

    public void TestOffset() 
    {
        String text = "中华人民共和国在哪里";
//        for (int i = 0; i < text.Length(); ++i)
//        {
//            Console.print(text.charAt(i) + "" + i + " ");
//        }
//        Console.WriteLine();
        List<Term> termList = IndexTokenizer.segment(text);
        foreach (Term term in termList)
        {
            AssertEquals(term.word, text[term.offset .. (term.offset + term.length())]);
        }
    }
    [TestMethod]

    public void TestWrapper() 
    {
        SegmentWrapper wrapper = new SegmentWrapper(new StringReader("中科院预测科学研究中心学术委员会\nhaha"), tokenizer.StandardTokenizer.SEGMENT);
        Term fullTerm;
        while ((fullTerm = wrapper.next()) != null)
        {
//            Console.WriteLine(fullTerm);
        }
    }
    [TestMethod]

    public void TestSpeechTagging() 
    {
//        HanLP.Config.enableDebug();
        String text = "教授正在教授自然语言处理课程";
        DijkstraSegment segment = new DijkstraSegment();

//        Console.WriteLine("未标注：" + segment.seg(text));
        segment.enablePartOfSpeechTagging(true);
//        Console.WriteLine("标注后：" + segment.seg(text));
    }
    [TestMethod]

    public void TestFactory() 
    {
        Segment segment = HanLP.newSegment();
    }
    [TestMethod]

    public void TestCustomDictionary() 
    {
        CustomDictionary.insert("肯德基", "ns 1000");
        Segment segment = new ViterbiSegment();
//        Console.WriteLine(segment.seg("肯德基"));
    }
    [TestMethod]

    public void TestNT() 
    {
//        HanLP.Config.enableDebug();
        Segment segment = new DijkstraSegment().enableOrganizationRecognize(true);
//        Console.WriteLine(segment.seg("张克智与潍坊地铁建设工程公司"));
    }
    [TestMethod]

    public void TestACSegment() 
    {
        Segment segment = new DoubleArrayTrieSegment();
        segment.enablePartOfSpeechTagging(true);
//        Console.WriteLine(segment.seg("江西鄱阳湖干枯，中国最大淡水湖变成大草原"));
    }
    [TestMethod]

    public void TestIssue2() 
    {
//        HanLP.Config.enableDebug();
        String text = "BENQphone";
//        Console.WriteLine(HanLP.segment(text));
        CustomDictionary.insert("BENQ");
//        Console.WriteLine(HanLP.segment(text));
    }
    [TestMethod]

    public void TestIssue3() 
    {
        AssertEquals(CharType.CT_DELIMITER, CharType.get('*'));
//        Console.WriteLine(HanLP.segment("300g*2"));
//        Console.WriteLine(HanLP.segment("３００ｇ＊２"));
//        Console.WriteLine(HanLP.segment("鱼300克*2/组"));
    }
    [TestMethod]

    public void TestIssue313() 
    {
//        Console.WriteLine(HanLP.segment("hello\n" + "world"));
    }
    [TestMethod]

    public void TestQuickAtomSegment() 
    {
        String text = "你好1234abc Good一二三四3.14";
//        Console.WriteLine(Segment.quickAtomSegment(text.ToCharArray(), 0, text.Length()));
    }
    [TestMethod]

    public void TestJP() 
    {
        String text = "明天8.9你好abc对了";
        Segment segment = new ViterbiSegment().enableCustomDictionary(false).enableAllNamedEntityRecognize(false);
//        Console.WriteLine(segment.seg(text));
    }

    //    public void testSpeedOfSecondViterbi() 
    //    {
    //        String text = "王总和小丽结婚了";
    //        Segment segment = new ViterbiSegment().enableAllNamedEntityRecognize(false)
    //                .enableNameRecognize(false) // 人名识别需要二次维特比，比较慢
    //                .enableCustomDictionary(false);
    //        Console.WriteLine(segment.seg(text));
    //        long start = DateTime.Now.Microsecond;
    //        int pressure = 1000000;
    //        for (int i = 0; i < pressure; ++i)
    //        {
    //            segment.seg(text);
    //        }
    //        double costTime = (DateTime.Now.Microsecond - start) / (double) 1000;
    //        Console.printf("分词速度：%.2f字每秒", text.Length() * pressure / costTime);
    //    }
    [TestMethod]

    public void TestNumberAndQuantifier() 
    {
        StandardTokenizer.SEGMENT.enableNumberQuantifierRecognize(true);
        String[] testCase = new String[]
            {
                "十九元套餐包括什么",
                "九千九百九十九朵玫瑰",
                "壹佰块钱都不给我",
                "９０１２３４５６７８只蚂蚁",
            };
        foreach (String sentence in testCase)
        {
//            Console.WriteLine(StandardTokenizer.segment(sentence));
        }
    }
    [TestMethod]

    public void TestIssue10() 
    {
        StandardTokenizer.SEGMENT.enableNumberQuantifierRecognize(true);
        IndexTokenizer.SEGMENT.enableNumberQuantifierRecognize(true);
        var termList = StandardTokenizer.segment("此帐号有欠费业务是什么");
//        Console.WriteLine(termList);
        termList = IndexTokenizer.segment("此帐号有欠费业务是什么");
//        Console.WriteLine(termList);
        termList = StandardTokenizer.segment("15307971214话费还有多少");
//        Console.WriteLine(termList);
        termList = IndexTokenizer.segment("15307971214话费还有多少");
//        Console.WriteLine(termList);
    }

    //    public void testIssue199() 
    //    {
    //        Segment segment = new CRFSegment();
    //        segment.enableCustomDictionary(false);// 开启自定义词典
    //        segment.enablePartOfSpeechTagging(true);
    //        List<Term> termList = segment.seg("更多采购");
    ////        Console.WriteLine(termList);
    //        for (Term term : termList)
    //        {
    //            if (term.nature == null)
    //            {
    ////                Console.WriteLine("识别到新词：" + term.word);
    //            }
    //        }
    //    }

    //    public void testMultiThreading() 
    //    {
    //        Segment segment = BasicTokenizer.SEGMENT;
    //        // 测个速度
    //        String text = "江西鄱阳湖干枯，中国最大淡水湖变成大草原。";
    //        Console.WriteLine(segment.seg(text));
    //        int pressure = 100000;
    //        StringBuilder sbBigText = new StringBuilder(text.Length() * pressure);
    //        for (int i = 0; i < pressure; i++)
    //        {
    //            sbBigText.append(text);
    //        }
    //        text = sbBigText.ToString();
    //        long start = DateTime.Now.Microsecond;
    //        List<Term> termList1 = segment.seg(text);
    //        double costTime = (DateTime.Now.Microsecond - start) / (double) 1000;
    //        Console.printf("单线程分词速度：%.2f字每秒\n", text.Length() / costTime);
    //
    //        segment.enableMultithreading(4);
    //        start = DateTime.Now.Microsecond;
    //        List<Term> termList2 = segment.seg(text);
    //        costTime = (DateTime.Now.Microsecond - start) / (double) 1000;
    //        Console.printf("四线程分词速度：%.2f字每秒\n", text.Length() / costTime);
    //
    //        assertEquals(termList1.size(), termList2.size());
    //        Iterator<Term> iterator1 = termList1.iterator();
    //        Iterator<Term> iterator2 = termList2.iterator();
    //        while (iterator1.hasNext())
    //        {
    //            Term term1 = iterator1.next();
    //            Term term2 = iterator2.next();
    //            assertEquals(term1.word, term2.word);
    //            assertEquals(term1.nature, term2.nature);
    //            assertEquals(term1.offset, term2.offset);
    //        }
    //    }

    //    public void testTryToCrashSegment() 
    //    {
    //        String text = "尝试玩坏分词器";
    //        Segment segment = new ViterbiSegment().enableMultithreading(100);
    //        Console.WriteLine(segment.seg(text));
    //    }

    //    public void testCRFSegment() 
    //    {
    //        HanLP.Config.enableDebug();
    ////        HanLP.Config.ShowTermNature = false;
    //        Segment segment = new CRFSegment();
    //        Console.WriteLine(segment.seg("有句谚语叫做一个萝卜一个坑儿"));
    //    }
    [TestMethod]

    public void TestIssue16() 
    {
        CustomDictionary.insert("爱听4g", "nz 1000");
        Segment segment = new ViterbiSegment();
//        Console.WriteLine(segment.seg("爱听4g"));
//        Console.WriteLine(segment.seg("爱听4G"));
//        Console.WriteLine(segment.seg("爱听４G"));
//        Console.WriteLine(segment.seg("爱听４Ｇ"));
//        Console.WriteLine(segment.seg("愛聽４Ｇ"));
    }
    [TestMethod]

    public void TestIssuse17() 
    {
//        Console.WriteLine(CharType.get('\u0000'));
//        Console.WriteLine(CharType.get(' '));
        AssertEquals(CharTable.convert(' '), ' ');
//        Console.WriteLine(CharTable.convert('﹗'));
        HanLP.Config.Normalization = true;
//        Console.WriteLine(StandardTokenizer.segment("号 "));
    }
    [TestMethod]

    public void TestIssue22() 
    {
        StandardTokenizer.SEGMENT.enableNumberQuantifierRecognize(false);
        CoreDictionary.Attribute attribute = CoreDictionary.get("年");
//        Console.WriteLine(attribute);
        List<Term> termList = StandardTokenizer.segment("三年");
//        Console.WriteLine(termList);
        AssertEquals(attribute.nature[0], termList[1].nature);
//        Console.WriteLine(StandardTokenizer.segment("三元"));
        StandardTokenizer.SEGMENT.enableNumberQuantifierRecognize(true);
//        Console.WriteLine(StandardTokenizer.segment("三年"));
    }
    [TestMethod]

    public void TestIssue71() 
    {
        Segment segment = HanLP.newSegment();
        segment = segment.enableAllNamedEntityRecognize(true);
        segment = segment.enableNumberQuantifierRecognize(true);
//        Console.WriteLine(segment.seg("曾幻想过，若干年后的我就是这个样子的吗"));
    }
    [TestMethod]

    public void TestIssue193() 
    {
        String[] testCase = new String[]{
            "以每台约200元的价格送到苹果售后维修中心换新机（苹果的保修基本是免费换新机）",
            "可能以2500~2800元的价格回收",
            "3700个益农信息社打通服务“最后一公里”",
            "一位李先生给高政留言说上周五可以帮忙献血",
            "一位浩宁达高层透露",
            "五和万科长阳天地5个普宅项目",
            "以1974点低点和5178点高点作江恩角度线",
            "纳入统计的18家京系基金公司",
            "华夏基金与嘉实基金两家京系基金公司",
            "则应从排名第八的投标人开始依次递补三名投标人"
        };
        Segment segment = HanLP.newSegment().enableOrganizationRecognize(true).enableNumberQuantifierRecognize(true);
        foreach (String sentence in testCase)
        {
            List<Term> termList = segment.seg(sentence);
//            Console.WriteLine(termList);
        }
    }
    [TestMethod]

    public void TestTime() 
    {
        TraditionalChineseTokenizer.segment("认可程度");
    }
    [TestMethod]

    public void TestBuildASimpleSegment() 
    {
        var dictionary = new Dictionary<String, String>();
        dictionary.Add("HanLP", "名词");
        dictionary.Add("特别", "副词");
        dictionary.Add("方便", "形容词");
        AhoCorasickDoubleArrayTrie<String> acdat = new AhoCorasickDoubleArrayTrie<String>();
        acdat.build(dictionary);
        LinkedList<ResultTerm<String>> termList =
            CommonAhoCorasickSegmentUtil.segment("HanLP是不是特别方便？", acdat);
//        Console.WriteLine(termList);
    }
    [TestMethod]

    public void TestNLPSegment() 
    {
        String text = "2013年4月27日11时54分";
//        Console.WriteLine(NLPTokenizer.segment(text));
    }
    [TestMethod]

    public void TestTraditionalSegment() 
    {
        String text = "吵架吵到快取消結婚了";
//        Console.WriteLine(TraditionalChineseTokenizer.segment(text));
    }
    [TestMethod]

    public void TestIssue290() 
    {
//        HanLP.Config.enableDebug();
        String txt = "而其他肢解出去的七个贝尔公司如西南贝尔、太平洋贝尔、大西洋贝尔。";
        Segment seg_viterbi = new ViterbiSegment().enablePartOfSpeechTagging(true).enableOffset(true).enableNameRecognize(true).enablePlaceRecognize(true).enableOrganizationRecognize(true).enableNumberQuantifierRecognize(true);
//        Console.WriteLine(seg_viterbi.seg(txt));
    }
    [TestMethod]

    public void TestIssue343() 
    {
        CustomDictionary.insert("酷我");
        CustomDictionary.insert("酷我音乐");
        Segment segment = HanLP.newSegment().enableIndexMode(true);
//        Console.WriteLine(segment.seg("1酷我音乐2酷我音乐3酷我4酷我音乐6酷7酷我音乐"));
    }
    [TestMethod]

    public void TestIssue358() 
    {
//        HanLP.Config.enableDebug();
        String text = "受约束，需要遵守心理学会所定的道德原则，所需要时须说明该实验与所能得到的知识的关系";

        Segment segment = StandardTokenizer.SEGMENT.enableAllNamedEntityRecognize(false).enableCustomDictionary(false)
            .enableOrganizationRecognize(true);

//        Console.WriteLine(segment.seg(text));
    }
    [TestMethod]

    public void TestIssue496() 
    {
        Segment segment = HanLP.newSegment().enableIndexMode(true);
//        Console.WriteLine(segment.seg("中医药"));
//        Console.WriteLine(segment.seg("中医药大学"));
    }
    [TestMethod]

    public void TestIssue513() 
    {
        List<Term> termList = IndexTokenizer.segment("南京市长江大桥");
        foreach (Term term in termList)
        {
//            Console.WriteLine(term + " [" + term.offset + ":" + (term.offset + term.word.Length()) + "]");
        }
    }
    [TestMethod]

    public void TestIssue519() 
    {
        String[] testCase = new String[]{
            "评审委员会",
            "商标评审委员会",
            "铁道部运输局",
            "铁道部运输局营运部货运营销计划处",
        };
        foreach (String sentence in testCase)
        {
//            Console.WriteLine(sentence);
            List<Term> termList = IndexTokenizer.segment(sentence);
            foreach (Term term in termList)
            {
//                Console.WriteLine(term + " [" + term.offset + ":" + (term.offset + term.word.Length()) + "]");
            }
//            Console.WriteLine();
        }
    }
    [TestMethod]

    public void TestIssue542() 
    {
        Segment seg = HanLP.newSegment();
        seg.enableAllNamedEntityRecognize(true);
        seg.enableNumberQuantifierRecognize(true);
//        Console.WriteLine(seg.seg("一分钟就累了"));
    }
    [TestMethod]

    public void TestIssue623() 
    {
        StandardTokenizer.SEGMENT.enableCustomDictionary(false);
//        Console.WriteLine(HanLP.segment("赵四158开头的号码"));
//        Console.WriteLine(HanLP.segment("上周四18:00召开股东大会"));
    }
    [TestMethod]

    public void TestIssue633() 
    {
        CustomDictionary.add("钱管家");
        StandardTokenizer.SEGMENT.enableCustomDictionaryForcing(true);
//        Console.WriteLine(HanLP.segment("钱管家中怎么绑定网银"));
    }
    [TestMethod]

    public void TestIssue784() 
    {
        String s = "苏苏中级会计什么时候更新";
        CustomDictionary.add("苏苏");
        StandardTokenizer.SEGMENT.enableCustomDictionaryForcing(true);
        AssertTrue(HanLP.segment(s).ToString().Contains("苏苏"));
    }
    [TestMethod]

    public void TestIssue790() 
    {
        Segment seg = HanLP.newSegment();
        seg.enableOrganizationRecognize(true);
        seg.enableNumberQuantifierRecognize(true);

        String raw = "1名卫技人员资源和社会保障局余姚市";
//        Console.WriteLine(seg.seg(raw));
        seg.seg(raw);
    }
    [TestMethod]

    public void TestTimeIssue() 
    {
        AssertTrue(HanLP.segment("1月中旬应该会发生什么").ToString().Contains("1月"));
    }
    [TestMethod]

    public void TestIssue932() 
    {
        Segment segment = new DijkstraSegment().enableOrganizationRecognize(true);
        HanLP.Config.enableDebug();
        Console.WriteLine(segment.seg("福哈生态工程有限公司"));
    }
}
