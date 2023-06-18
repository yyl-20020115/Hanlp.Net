/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/2 21:17</create-date>
 *
 * <copyright file="PosTagUtil.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.dependency.nnparser.util;



/**
 * @author hankcs
 */
public class PosTagUtil
{
    private static Dictionary<string, string> posConverter = new ();

    static PosTagUtil()
    {
        posConverter.Add("Mg", "m");
        posConverter.Add("Rg", "r");
        posConverter.Add("ad", "a");
        posConverter.Add("ag", "a");
        posConverter.Add("al", "a");
        posConverter.Add("an", "a");
        posConverter.Add("begin", "x");
        posConverter.Add("bg", "b");
        posConverter.Add("bl", "b");
        posConverter.Add("cc", "c");
        posConverter.Add("dg", "d");
        posConverter.Add("dl", "d");
        posConverter.Add("end", "x");
        posConverter.Add("f", "nd");
        posConverter.Add("g", "nz");
        posConverter.Add("gb", "nz");
        posConverter.Add("gbc", "nz");
        posConverter.Add("gc", "nz");
        posConverter.Add("gg", "nz");
        posConverter.Add("gi", "nz");
        posConverter.Add("gm", "nz");
        posConverter.Add("gp", "nz");
        posConverter.Add("l", "i");
        posConverter.Add("mg", "m");
        posConverter.Add("mq", "m");
        posConverter.Add("nb", "nz");
        posConverter.Add("nba", "nz");
        posConverter.Add("nbc", "nz");
        posConverter.Add("nbp", "nz");
        posConverter.Add("nf", "n");
        posConverter.Add("ng", "n");
        posConverter.Add("nh", "nz");
        posConverter.Add("nhd", "nz");
        posConverter.Add("nhm", "nz");
        posConverter.Add("ni", "n");
        posConverter.Add("nic", "nt");
        posConverter.Add("nis", "nt");
        posConverter.Add("nit", "nt");
        posConverter.Add("nl", "n");
        posConverter.Add("nm", "nz");
        posConverter.Add("nmc", "nz");
        posConverter.Add("nn", "nz");
        posConverter.Add("nnd", "nz");
        posConverter.Add("nnt", "nz");
        posConverter.Add("nr", "nh");
        posConverter.Add("nr1", "nh");
        posConverter.Add("nr2", "nh");
        posConverter.Add("nrf", "nh");
        posConverter.Add("nrj", "nh");
        posConverter.Add("nsf", "ns");
        posConverter.Add("nt", "ni");
        posConverter.Add("ntc", "ni");
        posConverter.Add("ntcb", "ni");
        posConverter.Add("ntcf", "ni");
        posConverter.Add("ntch", "ni");
        posConverter.Add("nth", "ni");
        posConverter.Add("nto", "ni");
        posConverter.Add("nts", "ni");
        posConverter.Add("ntu", "ni");
        posConverter.Add("nx", "ws");
        posConverter.Add("pba", "p");
        posConverter.Add("pbei", "p");
        posConverter.Add("qg", "q");
        posConverter.Add("qt", "q");
        posConverter.Add("qv", "q");
        posConverter.Add("rg", "r");
        posConverter.Add("rr", "r");
        posConverter.Add("ry", "r");
        posConverter.Add("rys", "r");
        posConverter.Add("ryt", "r");
        posConverter.Add("ryv", "r");
        posConverter.Add("rz", "r");
        posConverter.Add("rzs", "r");
        posConverter.Add("rzt", "r");
        posConverter.Add("rzv", "r");
        posConverter.Add("s", "nl");
        posConverter.Add("t", "nt");
        posConverter.Add("tg", "nt");
        posConverter.Add("ud", "u");
        posConverter.Add("ude1", "u");
        posConverter.Add("ude2", "u");
        posConverter.Add("ude3", "u");
        posConverter.Add("udeng", "u");
        posConverter.Add("udh", "u");
        posConverter.Add("ug", "u");
        posConverter.Add("uguo", "u");
        posConverter.Add("uj", "u");
        posConverter.Add("ul", "u");
        posConverter.Add("ule", "u");
        posConverter.Add("ulian", "u");
        posConverter.Add("uls", "u");
        posConverter.Add("usuo", "u");
        posConverter.Add("uv", "u");
        posConverter.Add("uyy", "u");
        posConverter.Add("uz", "u");
        posConverter.Add("uzhe", "u");
        posConverter.Add("uzhi", "u");
        posConverter.Add("vd", "v");
        posConverter.Add("vf", "v");
        posConverter.Add("vg", "v");
        posConverter.Add("vi", "v");
        posConverter.Add("vl", "v");
        posConverter.Add("vn", "v");
        posConverter.Add("vshi", "v");
        posConverter.Add("vx", "v");
        posConverter.Add("vyou", "v");
        posConverter.Add("w", "wp");
        posConverter.Add("wb", "wp");
        posConverter.Add("wd", "wp");
        posConverter.Add("wf", "wp");
        posConverter.Add("wh", "wp");
        posConverter.Add("wj", "wp");
        posConverter.Add("wky", "wp");
        posConverter.Add("wkz", "wp");
        posConverter.Add("wm", "wp");
        posConverter.Add("wn", "wp");
        posConverter.Add("ws", "wp");
        posConverter.Add("wt", "wp");
        posConverter.Add("ww", "wp");
        posConverter.Add("wyy", "wp");
        posConverter.Add("wyz", "wp");
        posConverter.Add("xu", "x");
        posConverter.Add("xx", "x");
        posConverter.Add("y", "e");
        posConverter.Add("yg", "u");
        posConverter.Add("z", "u");
        posConverter.Add("zg", "u");
    }

    /**
     * 转为863标注集<br>
     * 863词性标注集，其各个词性含义如下表：
     * <p>
     * Tag	Description	Example	Tag	Description	Example
     * a	adjective	美丽	ni	organization name	保险公司
     * b	other noun-modifier	大型, 西式	nl	location noun	城郊
     * c	conjunction	和, 虽然	ns	geographical name	北京
     * d	adverb	很	nt	temporal noun	近日, 明代
     * e	exclamation	哎	nz	other proper noun	诺贝尔奖
     * g	morpheme	茨, 甥	o	onomatopoeia	哗啦
     * h	prefix	阿, 伪	p	preposition	在, 把
     * i	idiom	百花齐放	q	quantity	个
     * j	abbreviation	公检法	r	pronoun	我们
     * k	suffix	界, 率	u	auxiliary	的, 地
     * m	number	一, 第一	v	verb	跑, 学习
     * n	general noun	苹果	wp	punctuation	，。！
     * nd	direction noun	右侧	ws	foreign words	CPU
     * nh	person name	杜甫, 汤姆	x	non-lexeme	萄, 翱
     *
     * @param termList
     * @return
     */
    public static List<string> to863(List<Term> termList)
    {
        List<string> posTagList = new (termList.Count);
        foreach (Term term in termList)
        {
            string posTag = posConverter.get(term.nature.ToString());
            if (posTag == null)
                posTag = term.nature.ToString();
            posTagList.Add(posTag);
        }

        return posTagList;
    }

    /**
     * 评估词性标注器的准确率
     *
     * @param tagger 词性标注器
     * @param corpus 测试集
     * @return Accuracy百分比
     */
    public static float evaluate(POSTagger tagger, string corpus)
    {
        int correct = 0, total = 0;
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(corpus);
        for (string line : lineIterator)
        {
            Sentence sentence = Sentence.create(line);
            if (sentence == null) continue;
            string[][] wordTagArray = sentence.toWordTagArray();
            string[] prediction = tagger.tag(wordTagArray[0]);
            //assert prediction.Length == wordTagArray[1].Length;
            total += prediction.Length;
            for (int i = 0; i < prediction.Length; i++)
            {
                if (prediction[i].Equals(wordTagArray[1][i]))
                    ++correct;
            }
        }
        if (total == 0) return 0;
        return correct / (float) total * 100;
    }
}
