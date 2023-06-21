/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-29 4:51 PM</create-date>
 *
 * <copyright file="Pipeline.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * See LICENSE file in the project root for full license information.
 * </copyright>
 */
namespace com.hankcs.hanlp.tokenizer.pipe;


/**
 * 流水线
 *
 * @author hankcs
 */
public class Pipeline<I, M, O> : List<Pipe<M, M>>, Pipe<I, O>
{
    /**
     * 入口
     */
    protected Pipe<I, M> first;
    /**
     * 出口
     */
    protected Pipe<M, O> last;
    /**
     * 中间部分
     */
    protected LinkedList<Pipe<M, M>> pipeList;

    public Pipeline(Pipe<I, M> first, Pipe<M, O> last)
    {
        this.first = first;
        this.last = last;
        pipeList = new LinkedList<Pipe<M, M>>();
    }

    //@Override
    public O flow(I input)
    {
        M i = first.flow(input);
        foreach (Pipe<M, M> pipe in pipeList)
        {
            i = pipe.flow(i);
        }
        return last.flow(i);
    }

    //@Override
    public int size()
    {
        return pipeList.Count;
    }

    //@Override
    public bool isEmpty()
    {
        return pipeList.isEmpty();
    }

    //@Override
    public bool Contains(Object o)
    {
        return pipeList.Contains(o);
    }

    //@Override
    public IEnumerator<Pipe<M, M>> iterator()
    {
        return pipeList.iterator();
    }

    //@Override
    public Object[] ToArray()
    {
        return pipeList.ToArray();
    }

    //@Override
    public  T[] ToArray<T>(T[] a)
    {
        return pipeList.ToArray(a);
    }

    //@Override
    public bool Add(Pipe<M, M> pipe)
    {
        return pipeList.Add(pipe);
    }

    //@Override
    public bool Remove(Object o)
    {
        return pipeList.Remove(o);
    }

    //@Override
    public bool containsAll(Collection c)
    {
        return pipeList.containsAll(c);
    }

    //@Override
    public bool AddRange(Collection<Pipe<M, M>> c)
    {
        return pipeList.AddRange(c);
    }

    //@Override
    public bool AddRange(int index, Collection<Pipe<M, M>> c)
    {
        return pipeList.AddRange(c);
    }

    //@Override
    public bool removeAll(Collection c)
    {
        return pipeList.removeAll(c);
    }

    //@Override
    public bool retainAll(Collection c)
    {
        return pipeList.retainAll(c);
    }

    //@Override
    public void Clear()
    {
        pipeList.Clear();
    }

    //@Override
    public bool Equals(Object o)
    {
        return pipeList.Equals(o);
    }

    //@Override
    public int GetHashCode()
    {
        return pipeList.GetHashCode();
    }

    //@Override
    public Pipe<M, M> get(int index)
    {
        return pipeList.get(index);
    }

    //@Override
    public Pipe<M, M> set(int index, Pipe<M, M> element)
    {
        return pipeList.set(index, element);
    }

    //@Override
    public void Add(int index, Pipe<M, M> element)
    {
        pipeList.Add(index, element);
    }

    /**
     * 以最高优先级加入管道
     *
     * @param pipe
     */
    public void addFirst(Pipe<M, M> pipe)
    {
        pipeList.addFirst(pipe);
    }

    /**
     * 以最低优先级加入管道
     *
     * @param pipe
     */
    public void addLast(Pipe<M, M> pipe)
    {
        pipeList.addLast(pipe);
    }

    //@Override
    public Pipe<M, M> Remove(int index)
    {
        return pipeList.Remove(index);
    }

    //@Override
    public int IndexOf(Object o)
    {
        return pipeList.IndexOf(o);
    }

    //@Override
    public int LastIndexOf(Object o)
    {
        return pipeList.LastIndexOf(o);
    }

    //@Override
    public ListIterator<Pipe<M, M>> GetEnumerator()
    {
        return pipeList.GetEnumerator();
    }

    //@Override
    public ListIterator<Pipe<M, M>> listIterator(int index)
    {
        return pipeList.listIterator(index);
    }

    //@Override
    public List<Pipe<M, M>> subList(int fromIndex, int toIndex)
    {
        return pipeList.subList(fromIndex, toIndex);
    }
}