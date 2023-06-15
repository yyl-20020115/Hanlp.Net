/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/10 PM5:29</create-date>
 *
 * <copyright file="MemoryDataSet.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.collections;
using com.hankcs.hanlp.classification.models;

namespace com.hankcs.hanlp.classification.corpus;




/**
 * @author hankcs
 */
public class MemoryDataSet : AbstractDataSet
{
    List<Document> documentList;
    bool editMode;

    public MemoryDataSet()
        :base()
    {
        documentList = new ();
    }

    public MemoryDataSet(AbstractModel model)
        :base(model)
    {
        documentList = new ();
    }

    //@Override
    public Document add(string category, string text)
    {
        if (editMode) return null;
        Document document = convert(category, text);
        documentList.Add(document);
        return document;
    }
    //@Override
    public int size()
    {
        return documentList.Count;
    }

    //@Override
    public void clear()
    {
        documentList.Clear();
    }

    //@Override
    public IDataSet shrink(int[] idMap)
    {
        var iterator = iterator();
        while (iterator.hasNext())
        {
            Document document = iterator.next();
            FrequencyMap<int> tfMap = new FrequencyMap<int>();
            foreach (KeyValuePair<int, int[]> entry in document.tfMap.entrySet())
            {
                int feature = entry.getKey();
                if (idMap[feature] == -1) continue;
                tfMap.put(idMap[feature], entry.getValue());
            }
            // 检查是否是空白文档
            if (tfMap.size() == 0) iterator.remove();
            else document.tfMap = tfMap;
        }
        return this;
    }

    //@Override
    public IEnumerator<Document> iterator()
    {
        return documentList.GetEnumerator();
    }
}