/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/29 15:14</create-date>
 *
 * <copyright file="Segment.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.recognition.nr;
using com.hankcs.hanlp.recognition.ns;
using com.hankcs.hanlp.recognition.nt;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.seg.Dijkstra.Path;

namespace com.hankcs.hanlp.seg.Dijkstra;



/**
 * 最短路径分词
 * @author hankcs
 */
public class DijkstraSegment : WordBasedSegment
{
    //@Override
    public override List<Term> segSentence(char[] sentence)
    {
        WordNet wordNetOptimum = new WordNet(sentence);
        WordNet wordNetAll = new WordNet(wordNetOptimum.charArray);
        ////////////////生成词网////////////////////
        generateWordNet(wordNetAll);
        ///////////////生成词图////////////////////
        Graph graph = generateBiGraph(wordNetAll);
        if (HanLP.Config.DEBUG)
        {
            Console.WriteLine("粗分词图：%s\n", graph.printByTo());
        }
        List<Vertex> vertexList = dijkstra(graph);
//        fixResultByRule(vertexList);

        if (config.useCustomDictionary)
        {
            if (config.indexMode > 0)
                combineByCustomDictionary(vertexList, wordNetAll);
            else combineByCustomDictionary(vertexList);
        }

        if (HanLP.Config.DEBUG)
        {
            Console.WriteLine("粗分结果" + convert(vertexList, false));
        }

        // 数字识别
        if (config.numberQuantifierRecognize)
        {
            mergeNumberQuantifier(vertexList, wordNetAll, config);
        }

        // 实体命名识别
        if (config.ner)
        {
            wordNetOptimum.AddRange(vertexList);
            int preSize = wordNetOptimum.Count;
            if (config.nameRecognize)
            {
                PersonRecognition.recognition(vertexList, wordNetOptimum, wordNetAll);
            }
            if (config.translatedNameRecognize)
            {
                TranslatedPersonRecognition.recognition(vertexList, wordNetOptimum, wordNetAll);
            }
            if (config.japaneseNameRecognize)
            {
                JapanesePersonRecognition.recognition(vertexList, wordNetOptimum, wordNetAll);
            }
            if (config.placeRecognize)
            {
                PlaceRecognition.recognition(vertexList, wordNetOptimum, wordNetAll);
            }
            if (config.organizationRecognize)
            {
                // 层叠隐马模型——生成输出作为下一级隐马输入
                graph = generateBiGraph(wordNetOptimum);
                vertexList = dijkstra(graph);
                wordNetOptimum.Clear();
                wordNetOptimum.AddRange(vertexList);
                preSize = wordNetOptimum.Count;
                OrganizationRecognition.recognition(vertexList, wordNetOptimum, wordNetAll);
            }
            if (wordNetOptimum.Count != preSize)
            {
                graph = generateBiGraph(wordNetOptimum);
                vertexList = dijkstra(graph);
                if (HanLP.Config.DEBUG)
                {
                    Console.WriteLine("细分词网：\n%s\n", wordNetOptimum);
                    Console.WriteLine("细分词图：%s\n", graph.printByTo());
                }
            }
        }

        // 如果是索引模式则全切分
        if (config.indexMode > 0)
        {
            return decorateResultForIndexMode(vertexList, wordNetAll);
        }

        // 是否标注词性
        if (config.speechTagging)
        {
            speechTagging(vertexList);
        }

        return convert(vertexList, config.offset);
    }

    /**
     * dijkstra最短路径
     * @param graph
     * @return
     */
    private static List<Vertex> dijkstra(Graph graph)
    {
        List<Vertex> resultList = new ();
        Vertex[] vertexes = graph.getVertexes();
        List<EdgeFrom>[] edgesTo = graph.getEdgesTo();
        double[] d = new double[vertexes.Length];
        Array.Fill(d, Double.MaxValue);
        d[d.Length - 1] = 0;
        int[] path = new int[vertexes.Length];
        Array.Fill(path, -1);
        PriorityQueue<State,State> que = new ();
        que.Add(new State(0, vertexes.Length - 1));
        while (!que.isEmpty())
        {
            State p = que.poll();
            if (d[p.vertex] < p.cost) continue;
            foreach (EdgeFrom edgeFrom in edgesTo[p.vertex])
            {
                if (d[edgeFrom.from] > d[p.vertex] + edgeFrom.weight)
                {
                    d[edgeFrom.from] = d[p.vertex] + edgeFrom.weight;
                    que.Add(new State(d[edgeFrom.from], edgeFrom.from));
                    path[edgeFrom.from] = p.vertex;
                }
            }
        }
        for (int t = 0; t != -1; t = path[t])
        {
            resultList.Add(vertexes[t]);
        }
        return resultList;
    }

}
