/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/20 17:24</create-date>
 *
 * <copyright file="WordNatureDependencyParser.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.dependency.CoNll;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.maxent;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dependency;



/**
 * 最大熵句法分析器
 *
 * @author hankcs
 */
public class MaxEntDependencyParser : MinimumSpanningTreeParser
{
    private MaxEntModel model;

    public MaxEntDependencyParser(MaxEntModel model)
    {
        this.model = model;
    }

    public MaxEntDependencyParser()
    {
        string path = HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT;
        model = GlobalObjectPool.get(path);
        if (model != null) return;
        long start = DateTime.Now.Microsecond;
        ByteArray byteArray = ByteArrayFileStream.createByteArrayFileStream(path);
        if (byteArray != null)
        {
            model = MaxEntModel.create(byteArray);
        }
        else
        {
            model = MaxEntModel.create(HanLP.Config.MaxEntModelPath);
        }
        if (model != null)
        {
            GlobalObjectPool.Add(path, model);
        }
        string result = model == null ? "失败" : "成功";
        logger.info("最大熵依存句法模型载入" + result + "，耗时" + (DateTime.Now.Microsecond - start) + " ms");
    }

    /**
     * 分析句子的依存句法
     *
     * @param termList 句子，可以是任何具有词性标注功能的分词器的分词结果
     * @return CoNLL格式的依存句法树
     */
    public static CoNLLSentence compute(List<Term> termList)
    {
        return new MaxEntDependencyParser().parse(termList);
    }

    /**
     * 分析句子的依存句法
     *
     * @param sentence 句子
     * @return CoNLL格式的依存句法树
     */
    public static CoNLLSentence compute(string sentence)
    {
        return new MaxEntDependencyParser().parse(sentence);
    }

    //@Override
    protected Edge makeEdge(Node[] nodeArray, int from, int to)
    {
        LinkedList<string> context = new LinkedList<string>();
        int index = from;
        for (int i = index - 2; i < index + 2 + 1; ++i)
        {
            Node w = i >= 0 && i < nodeArray.Length ? nodeArray[i] : Node.NULL;
            context.Add(w.compiledWord + "i" + (i - index));      // 在尾巴上做个标记，不然特征冲突了
            context.Add(w.label + "i" + (i - index));
        }
        index = to;
        for (int i = index - 2; i < index + 2 + 1; ++i)
        {
            Node w = i >= 0 && i < nodeArray.Length ? nodeArray[i] : Node.NULL;
            context.Add(w.compiledWord + "j" + (i - index));      // 在尾巴上做个标记，不然特征冲突了
            context.Add(w.label + "j" + (i - index));
        }
        context.Add(nodeArray[from].compiledWord + '→' + nodeArray[to].compiledWord);
        context.Add(nodeArray[from].label + '→' + nodeArray[to].label);
        context.Add(nodeArray[from].compiledWord + '→' + nodeArray[to].compiledWord + (from - to));
        context.Add(nodeArray[from].label + '→' + nodeArray[to].label + (from - to));
        Node wordBeforeI = from - 1 >= 0 ? nodeArray[from - 1] : Node.NULL;
        Node wordBeforeJ = to - 1 >= 0 ? nodeArray[to - 1] : Node.NULL;
        context.Add(wordBeforeI.compiledWord + '@' + nodeArray[from].compiledWord + '→' + nodeArray[to].compiledWord);
        context.Add(nodeArray[from].compiledWord + '→' + wordBeforeJ.compiledWord + '@' + nodeArray[to].compiledWord);
        context.Add(wordBeforeI.label + '@' + nodeArray[from].label + '→' + nodeArray[to].label);
        context.Add(nodeArray[from].label + '→' + wordBeforeJ.label + '@' + nodeArray[to].label);
        List<KeyValuePair<string, Double>> pairList = model.predict(context.ToArray());
        KeyValuePair<string, Double> maxPair = new KeyValuePair<string, Double>("null", -1.0);
//        Console.WriteLine(context);
//        Console.WriteLine(pairList);
        for (KeyValuePair<string, Double> pair : pairList)
        {
            if (pair.Value > maxPair.Value && !"null".Equals(pair.Key))
            {
                maxPair = pair;
            }
        }
//        Console.WriteLine(nodeArray[from].word + "→" + nodeArray[to].word + " : " + maxPair);

        return new Edge(from, to, maxPair.Key, (float) - Math.Log(maxPair.Value));
    }
}
