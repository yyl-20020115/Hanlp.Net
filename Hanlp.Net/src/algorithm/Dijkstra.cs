/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/9 12:18</create-date>
 *
 * <copyright file="Dijkstra.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.algorithm.ahocorasick.trie;
using com.hankcs.hanlp.seg.common;

namespace com.hankcs.hanlp.algorithm;



/**
 * 最短路径
 * @author hankcs
 */
public class Dijkstra
{
    public static List<Vertex> compute(Graph graph)
    {
        List<Vertex> resultList = new ();
        Vertex[] vertexes = graph.getVertexes();
        List<EdgeFrom>[] edgesTo = graph.getEdgesTo();
        double[] d = new double[vertexes.Length];
        Arrays.fill(d, double.MaxValue);
        d[^1] = 0;
        int[] path = new int[vertexes.Length];
        Arrays.fill(path, -1);
        PriorityQueue<State> que = new PriorityQueue<State>();
        que.add(new State(0, vertexes.Length - 1));
        while (!que.isEmpty())
        {
            State p = que.poll();
            if (d[p.vertex] < p.cost) continue;
            foreach (EdgeFrom edgeFrom in edgesTo[p.vertex])
            {
                if (d[edgeFrom.from] > d[p.vertex] + edgeFrom.weight)
                {
                    d[edgeFrom.from] = d[p.vertex] + edgeFrom.weight;
                    que.add(new State(d[edgeFrom.from], edgeFrom.from));
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
