/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/31 20:48</create-date>
 *
 * <copyright file="Dependency.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dependency.nnparser;


/**
 * @author hankcs
 */
public class Dependency
{
    public List<int> forms;
    public List<int> postags;
    public List<int> heads;
    public List<int> deprels;

    private static List<int> allocate()
    {
        return new ();
    }

    public Dependency()
    {
        forms = allocate();
        postags = allocate();
        heads = allocate();
        deprels = allocate();
    }

    public int Count=> forms.Count;
}
