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
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer.pipe;
using System.Collections;

namespace com.hankcs.hanlp.seg;



/**
 * @author hankcs
 */
public class SegmentPipeline : Segment, Pipe<string, List<Term>>, List<Pipe<List<IWord>, List<IWord>>>
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
    public class Pipe1 : Pipe<string, List<IWord>>
    {
        //@Override
        public List<IWord> flow(string input)
        {
            List<IWord> task = new();
            task.Add(new Word(input, null));
            return task;
        }
    }
    public class Pipe2 : Pipe<List<IWord>, List<Term>>
    {
        //@Override
        public List<Term> flow(List<IWord> input)
        {
            List<Term> output = new (input.Count);
            foreach (IWord word in input)
            {
                if (word.getLabel() == null)
                {
                    output.AddRange(_delegate.seg(word.getValue()));
                }
                else
                {
                    output.Add(new Term(word.getValue(), Nature.create(word.getLabel())));
                }
            }
            return output;
        }
    }
    public SegmentPipeline(Segment _delegate)
        : this(new Pipe1(), new Pipe2())
    {
        ;
        config =_delegate.config;
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
    public bool Contains(Object o)
    {
        return pipeList.Contains(o);
    }

    //@Override
    public IEnumerator<Pipe<List<IWord>, List<IWord>>> iterator()
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
    public bool Add(Pipe<List<IWord>, List<IWord>> pipe)
    {
        return pipeList.Add(pipe);
    }

    //@Override
    public bool Remove(Object o)
    {
        return pipeList.Remove(o);
    }

    //@Override
    public bool containsAll(ICollection c)
    {
        return pipeList.containsAll(c);
    }

    //@Override
    public bool addAll(ICollection<Pipe<List<IWord>, List<IWord>>> c)
    {
        return pipeList.addAll(c);
    }

    //@Override
    public bool addAll(int index, ICollection<Pipe<List<IWord>, List<IWord>>> c)
    {
        return pipeList.addAll(c);
    }

    //@Override
    public bool removeAll(ICollection c)
    {
        return pipeList.removeAll(c);
    }

    //@Override
    public bool retainAll(ICollection c)
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
    public void Add(int index, Pipe<List<IWord>, List<IWord>> element)
    {
        pipeList.Add(index, element);
    }

    //@Override
    public Pipe<List<IWord>, List<IWord>> Remove(int index)
    {
        return pipeList.Remove(index);
    }

    //@Override
    public int IndexOf(Object o)
    {
        return pipeList.IndexOf(o);
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
    public IEnumerator<Pipe<List<IWord>, List<IWord>>> listIterator(int index)
    {
        return pipeList.listIterator(index);
    }

    //@Override
    public List<Pipe<List<IWord>, List<IWord>>> subList(int fromIndex, int toIndex)
    {
        return pipeList.subList(fromIndex, toIndex);
    }
}
