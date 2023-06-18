/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 17:53</create-date>
 *
 * <copyright file="Node.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dependency.CoNll;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.dependency.common;



/**
 * 节点
 * @author hankcs
 */
public class Node
{
    private static Dictionary<string, string> natureConverter = new ();
    static Node()
    {
        natureConverter.Add("begin", "root");
        natureConverter.Add("bg", "b");
        natureConverter.Add("e", "y");
        natureConverter.Add("g", "nz");
        natureConverter.Add("gb", "nz");
        natureConverter.Add("gbc", "nz");
        natureConverter.Add("gc", "nz");
        natureConverter.Add("gg", "nz");
        natureConverter.Add("gi", "nz");
        natureConverter.Add("gm", "nz");
        natureConverter.Add("gp", "nz");
        natureConverter.Add("i", "nz");
        natureConverter.Add("j", "nz");
        natureConverter.Add("l", "nz");
        natureConverter.Add("mg", "Mg");
        natureConverter.Add("nb", "nz");
        natureConverter.Add("nba", "nz");
        natureConverter.Add("nbc", "nz");
        natureConverter.Add("nbp", "nz");
        natureConverter.Add("nf", "n");
        natureConverter.Add("nh", "nz");
        natureConverter.Add("nhd", "nz");
        natureConverter.Add("nhm", "nz");
        natureConverter.Add("ni", "nt");
        natureConverter.Add("nic", "nt");
        natureConverter.Add("nis", "n");
        natureConverter.Add("nit", "nt");
        natureConverter.Add("nm", "n");
        natureConverter.Add("nmc", "nz");
        natureConverter.Add("nn", "n");
        natureConverter.Add("nnd", "n");
        natureConverter.Add("nnt", "n");
        natureConverter.Add("ntc", "nt");
        natureConverter.Add("ntcb", "nt");
        natureConverter.Add("ntcf", "nt");
        natureConverter.Add("ntch", "nt");
        natureConverter.Add("nth", "nt");
        natureConverter.Add("nto", "nt");
        natureConverter.Add("nts", "nt");
        natureConverter.Add("ntu", "nt");
        natureConverter.Add("nx", "x");
        natureConverter.Add("qg", "q");
        natureConverter.Add("rg", "Rg");
        natureConverter.Add("ud", "u");
        natureConverter.Add("udh", "u");
        natureConverter.Add("ug", "uguo");
        natureConverter.Add("uj", "u");
        natureConverter.Add("ul", "ulian");
        natureConverter.Add("uv", "u");
        natureConverter.Add("uz", "uzhe");
        natureConverter.Add("w", "x");
        natureConverter.Add("wb", "x");
        natureConverter.Add("wd", "x");
        natureConverter.Add("wf", "x");
        natureConverter.Add("wh", "x");
        natureConverter.Add("wj", "x");
        natureConverter.Add("wky", "x");
        natureConverter.Add("wkz", "x");
        natureConverter.Add("wm", "x");
        natureConverter.Add("wn", "x");
        natureConverter.Add("wp", "x");
        natureConverter.Add("ws", "x");
        natureConverter.Add("wt", "x");
        natureConverter.Add("ww", "x");
        natureConverter.Add("wyy", "x");
        natureConverter.Add("wyz", "x");
        natureConverter.Add("xu", "x");
        natureConverter.Add("xx", "x");
        natureConverter.Add("yg", "y");
        natureConverter.Add("zg", "z");
        NULL.label = "null";
    }
    public static Node NULL = new Node(new Term(CoNLLWord.NULL.NAME, Nature.n), -1);
    public string word;
    public string compiledWord;
    public string label;
    public int id;

    public Node(Term term, int id)
    {
        this.id = id;
        word = term.word;
        if (!natureConverter.TryGetValue(term.nature.ToString(),out this.label))
            label = term.nature.ToString();
        compiledWord = PosTagCompiler.compile(label, word);
    }

    //@Override
    public override string ToString()
    {
        return word + "/" + label;
    }
}
