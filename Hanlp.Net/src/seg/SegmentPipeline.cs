/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-29 5:05 PM</create-date>
 *
 * <copyright file="SegmentPipeline.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * See LICENSE file in the project root for full license information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer.pipe;

namespace com.hankcs.hanlp.seg;



/**
 * @author hankcs
 */
public class SegmentPipeline : Segment , Pipe<string, List<Term>>, List<Pipe<List<IWord>, List<IWord>>>
{
    Pipe<string, List<IWord>> first;
    Pipe<List<IWord>, List<Term>> last;
    List<Pipe<List<IWord>, List<IWord>>> pipeList;

    private SegmentPipeline(Pipe<string, List<IWord>> first, Pipe<List<IWord>, List<Term>> last)
    {
        this.first = first;
        this.last = last;
        pipeList = new List<Pipe<List<IWord>, List<IWord>>>();
    }

    public SegmentPipeline(Segment _delegate)
    {
        this(new Pipe<string, List<IWord>>()
             {
                 //@Override
                 public List<IWord> flow(string input)
                 {
                     List<IWord> task = new LinkedList<IWord>();
                     task.add(new Word(input, null));
                     return task;
                 }
             },
             new Pipe<List<IWord>, List<Term>>()
             {
                 //@Override
                 public List<Term> flow(List<IWord> input)
                 {
                     List<Term> output = new ArrayList<Term>(input.size());
                     for (IWord word : input)
                     {
                         if (word.getLabel() == null)
                         {
                             output.addAll(delegate.seg(word.getValue()));
                         }
                         else
                         {
                             output.add(new Term(word.getValue(), Nature.create(word.getLabel())));
                         }
                     }
                     return output;
                 }
             });
        config = delegate.config;
    }


    //@Override
    protected List<Term> segSentence(char[] sentence)
    {
        return seg(new string(sentence));
    }

    //@Override
    public List<Term> seg(string text)
    {
        return flow(text);
    }

    //@Override
    public List<Term> flow(string input)
    {
        List<IWord> i = first.flow(input);
        for (Pipe<List<IWord>, List<IWord>> pipe : pipeList)
        {
            i = pipe.flow(i);
        }
        return last.flow(i);
    }

    //@Override
    public int size()
    {
        return pipeList.size();
    }

    //@Override
    public bool isEmpty()
    {
        return pipeList.isEmpty();
    }

    //@Override
    public bool contains(Object o)
    {
        return pipeList.contains(o);
    }

    //@Override
    public Iterator<Pipe<List<IWord>, List<IWord>>> iterator()
    {
        return pipeList.iterator();
    }

    //@Override
    public Object[] toArray()
    {
        return pipeList.toArray();
    }

    //@Override
    public <T> T[] toArray(T[] a)
    {
        return pipeList.toArray(a);
    }

    //@Override
    public bool add(Pipe<List<IWord>, List<IWord>> pipe)
    {
        return pipeList.add(pipe);
    }

    //@Override
    public bool remove(Object o)
    {
        return pipeList.remove(o);
    }

    //@Override
    public bool containsAll(Collection<?> c)
    {
        return pipeList.containsAll(c);
    }

    //@Override
    public bool addAll(Collection<? : Pipe<List<IWord>, List<IWord>>> c)
    {
        return pipeList.addAll(c);
    }

    //@Override
    public bool addAll(int index, Collection<? : Pipe<List<IWord>, List<IWord>>> c)
    {
        return pipeList.addAll(c);
    }

    //@Override
    public bool removeAll(Collection<?> c)
    {
        return pipeList.removeAll(c);
    }

    //@Override
    public bool retainAll(Collection<?> c)
    {
        return pipeList.retainAll(c);
    }

    //@Override
    public void clear()
    {
        pipeList.clear();
    }

    //@Override
    public bool equals(Object o)
    {
        return pipeList.equals(o);
    }

    //@Override
    public int hashCode()
    {
        return pipeList.hashCode();
    }

    //@Override
    public Pipe<List<IWord>, List<IWord>> get(int index)
    {
        return pipeList.get(index);
    }

    //@Override
    public Pipe<List<IWord>, List<IWord>> set(int index, Pipe<List<IWord>, List<IWord>> element)
    {
        return pipeList.set(index, element);
    }

    //@Override
    public void add(int index, Pipe<List<IWord>, List<IWord>> element)
    {
        pipeList.add(index, element);
    }

    //@Override
    public Pipe<List<IWord>, List<IWord>> remove(int index)
    {
        return pipeList.remove(index);
    }

    //@Override
    public int indexOf(Object o)
    {
        return pipeList.indexOf(o);
    }

    //@Override
    public int lastIndexOf(Object o)
    {
        return pipeList.lastIndexOf(o);
    }

    //@Override
    public ListIterator<Pipe<List<IWord>, List<IWord>>> listIterator()
    {
        return pipeList.listIterator();
    }

    //@Override
    public ListIterator<Pipe<List<IWord>, List<IWord>>> listIterator(int index)
    {
        return pipeList.listIterator(index);
    }

    //@Override
    public List<Pipe<List<IWord>, List<IWord>>> subList(int fromIndex, int toIndex)
    {
        return pipeList.subList(fromIndex, toIndex);
    }
}
