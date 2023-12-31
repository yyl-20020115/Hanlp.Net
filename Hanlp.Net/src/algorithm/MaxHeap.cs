/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/22 13:23</create-date>
 *
 * <copyright file="MaxHeap.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.algorithm;


/**
 * 用固定容量的优先队列模拟的最大堆，用于解决求topN大的问题
 *
 * @author hankcs
 */
public class MaxHeap<E> : IEnumerable<E>
{
    /**
     * 优先队列
     */
    private PriorityQueue<E, E> queue;
    /**
     * 堆的最大容量
     */
    private int maxSize;

    /**
     * 构造最大堆
     * @param maxSize 保留多少个元素
     * @param comparator 比较器，生成最大堆使用o1-o2，生成最小堆使用o2-o1，并修改 e.compareTo(peek) 比较规则
     */
    public MaxHeap(int maxSize, IComparer<E> comparator)
    {
        if (maxSize <= 0)
            throw new ArgumentException();
        this.maxSize = maxSize;
        this.queue = new PriorityQueue<E,E>(maxSize, comparator);
    }

    /**
     * 添加一个元素
     * @param e 元素
     * @return 是否添加成功
     */
    public bool Add(E e)
    {
        if (queue.Count < maxSize)
        { // 未达到最大容量，直接添加
            queue.Enqueue(e,e);
            return true;
        }
        else
        { // 队列已满
            E peek = queue.Peek();
            if (queue.Comparer.Compare(e, peek) > 0)
            { // 将新元素与当前堆顶元素比较，保留较小的元素
                queue.Dequeue();
                queue.Enqueue(e, e);
                return true;
            }
        }
        return false;
    }

    /**
     * 添加许多元素
     * @param collection
     */
    public MaxHeap<E> AddRange(ICollection<E> collection)
    {
        foreach(var e in collection)
        {
            Add(e);
        }

        return this;
    }

    /**
     * 转为有序列表，自毁性操作
     * @return
     */
    public List<E> ToList()
    {
        var list = new List<E>(queue.Count);
        while (queue.Count>0)
        {
            list.Insert(0, queue.Dequeue());
        }

        return list;
    }

    //@Override
    public IEnumerator<E> iterator()
    {
        return queue.GetEnumerator()();
    }

    public int size()
    {
        return queue.Count;
    }
}
