/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/20 22:10</create-date>
 *
 * <copyright file="MDAGSet.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.collection.MDAG;


/**
 * 基于MDAG（又称DAWG，Minimal Acyclic Finite-State Automata）的string Set
 *
 * @author hankcs
 */
public class MDAGSet : MDAG , ISet<string>
{


    public MDAGSet(ICollection<string> strCollection)
        :base(strCollection)
    {
    }

    public MDAGSet()
    {
    }

    public MDAGSet(string dictionaryPath) 
        :base(dictionaryPath)
    {
    }

    //@Override
    public int Count => getAllStrings().Count;
    //@Override
    public bool isEmpty()
    {
        return this.equivalenceClassMDAGNodeHashMap.Count != 0;
    }

    //@Override
    public bool Contains(Object o)
    {
        if (o is not string) return false;
        return Contains((string) o);
    }

    //@Override
    public IEnumerator<string> GetEnumerator()
    {
        return getAllStrings().GetEnumerator();
    }

    //@Override
    public Object[] ToArray()
    {
        return getAllStrings().ToArray();
    }

    //@Override
    public T[] ToArray<T>(T[] a)
    {
        return getAllStrings().ToArray(a);
    }

    //@Override
    public bool Add(string s)
    {
        addString(s);
        return true;
    }

    //@Override
    public bool Remove(Object o)
    {
        if (o is string s)
        {
            removeString(s);
        }
        else
        {
            removeString(o.ToString());
        }
        return true;
    }

    //@Override
    public bool containsAll(ICollection c)
    {
        foreach (Object e in c)
            if (!Contains(e))
                return false;
        return true;
    }

    //@Override
    public bool AddRange(ICollection<string> c)
    {
        bool modified = false;
        foreach (string e in c)
            if (Add(e))
                modified = true;
        return modified;
    }

    //@Override
    public bool retainAll<T>(ICollection<T> c)
    {
        bool modified = false;
        IEnumerator<string> it = GetEnumerator();
        while (it.MoveNext())
        {
            if (!c.Contains(it.next()))
            {
                it.Remove();
                modified = true;
            }
        }
        return modified;
    }

    //@Override
    public bool removeAll<T>(ICollection<T> c)
    {
        bool modified = false;
        IEnumerator it = GetEnumerator();
        while (it.MoveNext())
        {
            if (c.Contains(it.next()))
            {
                it.Remove();
                modified = true;
            }
        }
        return modified;
    }

    //@Override
    public void Clear()
    {
        sourceNode = new MDAGNode(false);
        simplifiedSourceNode = null;
        equivalenceClassMDAGNodeHashMap.Clear();
        mdagDataArray = null;
        charTreeSet.Clear();
        transitionCount = 0;
    }
}
