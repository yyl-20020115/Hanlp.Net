/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/05/2014/5/21 18:05</create-date>
 *
 * <copyright file="Graph.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.mining.word2vec;

namespace com.hankcs.hanlp.seg.common;


/**
 * @author hankcs
 */
public class Graph
{
    /**
     * 顶点
     */
    public Vertex[] vertexes;

    /**
     * 边，到达下标i
     */
    public List<EdgeFrom>[] edgesTo;

    /**
     * 将一个词网转为词图
     * @param vertexes 顶点数组
     */
    public Graph(Vertex[] vertexes)
    {
        int size = vertexes.Length;
        this.vertexes = vertexes;
        edgesTo = new List<EdgeFrom>[size];
        for (int i = 0; i < size; ++i)
        {
            edgesTo[i] = new List<EdgeFrom>();
        }
    }

    /**
     * 连接两个节点
     * @param from 起点
     * @param to 终点
     * @param weight 花费
     */
    public void connect(int from, int to, double weight)
    {
        edgesTo[to].Add(new EdgeFrom(from, weight, vertexes[from].word + '@' + vertexes[to].word));
    }


    /**
     * 获取到达顶点to的边列表
     * @param to 到达顶点to
     * @return 到达顶点to的边列表
     */
    public List<EdgeFrom> getEdgeListTo(int to)
    {
        return edgesTo[to];
    }

    //@Override
    public override string ToString()
    {
        return "Graph{" +
                "vertexes=" + String.Join(',',vertexes) +
                ", edgesTo=" + String.Join(',',edgesTo) +
                '}';
    }

    public string printByTo()
    {
        StringBuffer sb = new StringBuffer();
        sb.Append("========按终点打印========\n");
        for (int to = 0; to < edgesTo.Length; ++to)
        {
            List<EdgeFrom> edgeFromList = edgesTo[to];
            foreach (EdgeFrom edgeFrom in edgeFromList)
            {
                sb.Append(string.Format("to:%3d, from:%3d, weight:%05.2f, word:%s\n", to, edgeFrom.from, edgeFrom.weight, edgeFrom.name));
            }
        }

        return sb.ToString();
    }

    /**
     * 根据节点下标数组解释出对应的路径
     * @param path
     * @return
     */
    public List<Vertex> parsePath(int[] path)
    {
        List<Vertex> vertexList = new ();
        foreach (int i in path)
        {
            vertexList.Add(vertexes[i]);
        }

        return vertexList;
    }

    /**
     * 从一个路径中转换出空格隔开的结果
     * @param path
     * @return
     */
    public static string parseResult(List<Vertex> path)
    {
        if (path.Count < 2)
        {
            throw new RuntimeException("路径节点数小于2:" + path);
        }
        StringBuffer sb = new StringBuffer();

        for (int i = 1; i < path.Count - 1; ++i)
        {
            Vertex v = path[i];
            sb.Append(v.getRealWord() + " ");
        }

        return sb.ToString();
    }

    public Vertex[] getVertexes()
    {
        return vertexes;
    }

    public List<EdgeFrom>[] getEdgesTo()
    {
        return edgesTo;
    }
}
