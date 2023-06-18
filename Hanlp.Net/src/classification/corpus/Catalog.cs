/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/10 PM4:56</create-date>
 *
 * <copyright file="Catalog.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.datrie;

namespace com.hankcs.hanlp.classification.corpus;


/**
 * 类目名称和id的对应关系
 * @author hankcs
 */
public class Catalog : Serializable
{
    public Dictionary<string, int> categoryId;
    public List<string> idCategory;

    public Catalog()
    {
        categoryId = new ();
        idCategory = new ();
    }

    public Catalog(string[] catalog)
        :this()
    {
        for (int i = 0; i < catalog.Length; i++)
        {
            categoryId.Add(catalog[i], i);
            idCategory.Add(catalog[i]);
        }
    }

    public int addCategory(string category)
    {
        if (!categoryId.TryGetValue(category,out var id))
        {
            id = categoryId.Count;
            categoryId.Add(category, id);
            //assert idCategory.size() == id;
            idCategory.Add(category);
        }

        return id;
    }

    public int getId(string category)
    {
        return categoryId.TryGetValue(category,out var id)?id:-1;
    }

    public string getCategory(int id)
    {
        //assert 0 <= id;
        //assert id < idCategory.size();

        return idCategory[id];
    }

    public int size()
    {
        return idCategory.Count;
    }

    public string[] ToArray()
    {
        return idCategory.ToArray();
    }
}
