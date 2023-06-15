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
        String path = HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT;
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
            GlobalObjectPool.put(path, model);
        }
        String result = model == null ? "失败" : "成功";
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
    public static CoNLLSentence compute(String sentence)
    {
        return new MaxEntDependencyParser().parse(sentence);
    }

    //@Override
    protected Edge makeEdge(Node[] nodeArray, int from, int to)
    {
        LinkedList<String> context = new LinkedList<String>();
        int index = from;
        for (int i = index - 2; i < index + 2 + 1; ++i)
        {
            Node w = i >= 0 && i < nodeArray.length ? nodeArray[i] : Node.NULL;
            context.add(w.compiledWord + "i" + (i - index));      // 在尾巴上做个标记，不然特征冲突了
            context.add(w.label + "i" + (i - index));
        }
        index = to;
        for (int i = index - 2; i < index + 2 + 1; ++i)
        {
            Node w = i >= 0 && i < nodeArray.length ? nodeArray[i] : Node.NULL;
            context.add(w.compiledWord + "j" + (i - index));      // 在尾巴上做个标记，不然特征冲突了
            context.add(w.label + "j" + (i - index));
        }
        context.add(nodeArray[from].compiledWord + '→' + nodeArray[to].compiledWord);
        context.add(nodeArray[from].label + '→' + nodeArray[to].label);
        context.add(nodeArray[from].compiledWord + '→' + nodeArray[to].compiledWord + (from - to));
        context.add(nodeArray[from].label + '→' + nodeArray[to].label + (from - to));
        Node wordBeforeI = from - 1 >= 0 ? nodeArray[from - 1] : Node.NULL;
        Node wordBeforeJ = to - 1 >= 0 ? nodeArray[to - 1] : Node.NULL;
        context.add(wordBeforeI.compiledWord + '@' + nodeArray[from].compiledWord + '→' + nodeArray[to].compiledWord);
        context.add(nodeArray[from].compiledWord + '→' + wordBeforeJ.compiledWord + '@' + nodeArray[to].compiledWord);
        context.add(wordBeforeI.label + '@' + nodeArray[from].label + '→' + nodeArray[to].label);
        context.add(nodeArray[from].label + '→' + wordBeforeJ.label + '@' + nodeArray[to].label);
        List<Pair<String, Double>> pairList = model.predict(context.toArray(new String[0]));
        Pair<String, Double> maxPair = new Pair<String, Double>("null", -1.0);
//        System._out.println(context);
//        System._out.println(pairList);
        for (Pair<String, Double> pair : pairList)
        {
            if (pair.getValue() > maxPair.getValue() && !"null".equals(pair.getKey()))
            {
                maxPair = pair;
            }
        }
//        System._out.println(nodeArray[from].word + "→" + nodeArray[to].word + " : " + maxPair);

        return new Edge(from, to, maxPair.getKey(), (float) - Math.log(maxPair.getValue()));
    }
}
