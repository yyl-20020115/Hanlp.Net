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
    public override Document Add(string category, string text)
    {
        if (editMode) return null;
        Document document = convert(category, text);
        documentList.Add(document);
        return document;
    }
    //@Override
    public int Count=> documentList.Count;

    //@Override
    public void Clear()
    {
        documentList.Clear();
    }

    //@Override
    public IDataSet shrink(int[] idMap)
    {
        var iterator = GetEnumerator();
        while (iterator.MoveNext())
        {
            Document document = iterator.Current;
            FrequencyMap<int> tfMap = new FrequencyMap<int>();
            foreach (KeyValuePair<int, int[]> entry in document.tfMap)
            {
                int feature = entry.Key;
                if (idMap[feature] == -1) continue;
                tfMap.Add(idMap[feature], entry.Value);
            }
            // 检查是否是空白文档
            if (tfMap.Count == 0) iterator.Remove();
            else document.tfMap = tfMap;
        }
        return this;
    }

    //@Override
    public IEnumerator<Document> GetEnumerator()
    {
        return documentList.GetEnumerator();
    }
}