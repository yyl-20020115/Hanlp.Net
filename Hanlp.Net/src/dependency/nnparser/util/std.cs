/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/31 21:26</create-date>
 *
 * <copyright file="std.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dependency.nnparser.util;


/**
 * @author hankcs
 */
public class std
{
    public static void fill<E>(List<E> list, E value)
    {
        if (list == null) return;
        ListIterator<E> listIterator = list.GetEnumerator();
        while (listIterator.MoveNext()) listIterator.set(value);
    }

    public static List<E> create<E>(int size, E value)
    {
        List<E> list = new (size);
        for (int i = 0; i < size; i++)
        {
            list.Add(value);
        }

        return list;
    }

    public static E pop_back<E>(List<E> list)
    {
        E back = list.get(list.size() - 1);
        list.Remove(list.size() - 1);
        return back;
    }
}
