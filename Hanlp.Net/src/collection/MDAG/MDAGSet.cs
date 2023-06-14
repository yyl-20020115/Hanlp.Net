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
 * 基于MDAG（又称DAWG，Minimal Acyclic Finite-State Automata）的String Set
 *
 * @author hankcs
 */
public class MDAGSet : MDAG : Set<String>
{

    public MDAGSet(File dataFile) 
    {
        super(dataFile);
    }

    public MDAGSet(Collection<String> strCollection)
    {
        super(strCollection);
    }

    public MDAGSet()
    {
    }

    public MDAGSet(String dictionaryPath) 
    {
        super(dictionaryPath);
    }

    //@Override
    public int size()
    {
        return getAllStrings().size();
    }

    //@Override
    public bool isEmpty()
    {
        return this.equivalenceClassMDAGNodeHashMap.size() != 0;
    }

    //@Override
    public bool contains(Object o)
    {
        if (o.getClass() != String.class) return false;
        return contains((String) o);
    }

    //@Override
    public Iterator<String> iterator()
    {
        return getAllStrings().iterator();
    }

    //@Override
    public Object[] toArray()
    {
        return getAllStrings().toArray();
    }

    //@Override
    public <T> T[] toArray(T[] a)
    {
        return getAllStrings().toArray(a);
    }

    //@Override
    public bool add(String s)
    {
        addString(s);
        return true;
    }

    //@Override
    public bool remove(Object o)
    {
        if (o.getClass() == String.class)
        {
            removeString((String) o);
        }
        else
        {
            removeString(o.toString());
        }
        return true;
    }

    //@Override
    public bool containsAll(Collection<?> c)
    {
        for (Object e : c)
            if (!contains(e))
                return false;
        return true;
    }

    //@Override
    public bool addAll(Collection<? : String> c)
    {
        bool modified = false;
        for (String e : c)
            if (add(e))
                modified = true;
        return modified;
    }

    //@Override
    public bool retainAll(Collection<?> c)
    {
        bool modified = false;
        Iterator<String> it = iterator();
        while (it.hasNext())
        {
            if (!c.contains(it.next()))
            {
                it.remove();
                modified = true;
            }
        }
        return modified;
    }

    //@Override
    public bool removeAll(Collection<?> c)
    {
        bool modified = false;
        Iterator<?> it = iterator();
        while (it.hasNext())
        {
            if (c.contains(it.next()))
            {
                it.remove();
                modified = true;
            }
        }
        return modified;
    }

    //@Override
    public void clear()
    {
        sourceNode = new MDAGNode(false);
        simplifiedSourceNode = null;
        equivalenceClassMDAGNodeHashMap.clear();
        mdagDataArray = null;
        charTreeSet.clear();
        transitionCount = 0;
    }
}
