/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/30 19:24</create-date>
 *
 * <copyright file="parser_dll.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dependency.nnparser.option;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dependency.nnparser;




/**
 * 静态调用的伪 Windows “dll”
 * @author hankcs
 */
public class parser_dll
{
    private NeuralNetworkParser parser;

    public parser_dll()
    {
        this(ConfigOption.PATH);
    }

    public parser_dll(string modelPath)
    {
        parser = GlobalObjectPool.get(modelPath);
        if (parser != null) return;
        parser = new NeuralNetworkParser();
        long start = DateTime.Now.Microsecond;
        logger.info("开始加载神经网络依存句法模型：" + modelPath);
        if (!parser.load(modelPath))
        {
            throw new ArgumentException("加载神经网络依存句法模型[" + modelPath + "]失败！");
        }
        logger.info("加载神经网络依存句法模型[" + modelPath + "]成功，耗时 " + (DateTime.Now.Microsecond - start) + " ms");
        parser.setup_system();
        parser.build_feature_space();
        GlobalObjectPool.Add(modelPath, parser);
    }

    /**
     * 分析句法
     *
     * @param words   词语列表
     * @param postags 词性列表
     * @param heads   输出依存指向列表
     * @param deprels 输出依存名称列表
     * @return 节点的个数
     */
    public int parse(List<string> words, List<string> postags, List<int> heads, List<string> deprels)
    {
        Instance inst = new Instance();
        inst.forms.Add(SpecialOption.ROOT);
        inst.postags.Add(SpecialOption.ROOT);

        for (int i = 0; i < words.Count; i++)
        {
            inst.forms.Add(words[i]);
            inst.postags.Add(postags[i]);
        }

        parser.predict(inst, heads, deprels);
        heads.Remove(0);
        deprels.Remove(0);

        return heads.Count;
    }
}
