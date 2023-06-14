/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-15 下午5:39</create-date>
 *
 * <copyright file="DemoPerceptronLexicalAnalyzer.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.demo;



/**
 * 基于感知机序列标注的词法分析器，可选多个模型。
 * - large训练自一亿字的大型综合语料库，是已知范围内全世界最大的中文分词语料库。
 * - pku199801训练自个人修订版1998人民日报语料1月份，仅有183万字。
 * 语料库规模决定实际效果，面向生产环境的语料库应当在千万字量级。欢迎用户在自己的语料上训练新模型以适应新领域、识别新的命名实体。
 * 无论在何种语料上训练，都完全支持简繁全半角和大小写。
 *
 * @author hankcs
 */
public class DemoPerceptronLexicalAnalyzer : TestUtility
{
    public static void Main(String[] args) 
    {
        PerceptronLexicalAnalyzer analyzer = new PerceptronLexicalAnalyzer("data/model/perceptron/pku199801/cws.bin",
                                                                           HanLP.Config.PerceptronPOSModelPath,
                                                                           HanLP.Config.PerceptronNERModelPath);
        Console.WriteLine(analyzer.analyze("上海华安工业（集团）公司董事长谭旭光和秘书胡花蕊来到美国纽约现代艺术博物馆参观"));
        Console.WriteLine(analyzer.analyze("微软公司於1975年由比爾·蓋茲和保羅·艾倫創立，18年啟動以智慧雲端、前端為導向的大改組。"));

        // 任何模型总会有失误，特别是98年这种陈旧的语料库
        Console.WriteLine(analyzer.analyze("总统普京与特朗普通电话讨论太空探索技术公司"));
        // 支持在线学习
        analyzer.learn("与/c 特朗普/nr 通/v 电话/n 讨论/v [太空/s 探索/vn 技术/n 公司/n]/nt");
        // 学习到新知识
        Console.WriteLine(analyzer.analyze("总统普京与特朗普通电话讨论太空探索技术公司"));
        // 还可以举一反三
        Console.WriteLine(analyzer.analyze("主席和特朗普通电话"));

        // 知识的泛化不是死板的规则，而是比较灵活的统计信息
        Console.WriteLine(analyzer.analyze("我在浙江金华出生"));
        analyzer.learn("在/p 浙江/ns 金华/ns 出生/v");
        Console.WriteLine(analyzer.analyze("我在四川金华出生，我的名字叫金华"));

        // 在线学习后的模型支持序列化，以分词模型为例：
//        analyzer.getPerceptronSegmenter().getModel().save(HanLP.Config.PerceptronCWSModelPath);

        // 请用户按需执行对空格制表符等的预处理，只有你最清楚自己的文本中都有些什么奇怪的东西
        Console.WriteLine(analyzer.analyze("空格 \t\n\r\f&nbsp;统统都不要"
                                                .replaceAll("\\s+", "")    // 去除所有空白符
                                                .replaceAll("&nbsp;", "")  // 如果一些文本中含有html控制符
        ));
    }
}