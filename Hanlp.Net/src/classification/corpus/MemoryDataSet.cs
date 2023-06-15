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
namespace com.hankcs.hanlp.classification.corpus;




/**
 * @author hankcs
 */
public class MemoryDataSet : AbstractDataSet
{
    List<Document> documentList;
    bool editMode;

    public MemoryDataSet()
    {
        super();
        documentList = new LinkedList<Document>();
    }

    public MemoryDataSet(AbstractModel model)
    {
        super(model);
        documentList = new LinkedList<Document>();
    }

    //@Override
    public Document add(String category, String text)
    {
        if (editMode) return null;
        Document document = convert(category, text);
        documentList.add(document);
        return document;
    }
    //@Override
    public int size()
    {
        return documentList.size();
    }

    //@Override
    public void clear()
    {
        documentList.clear();
    }

    //@Override
    public IDataSet shrink(int[] idMap)
    {
        Iterator<Document> iterator = iterator();
        while (iterator.hasNext())
        {
            Document document = iterator.next();
            FrequencyMap<int> tfMap = new FrequencyMap<int>();
            for (Map.Entry<int, int[]> entry : document.tfMap.entrySet())
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
    public Iterator<Document> iterator()
    {
        return documentList.iterator();
    }
}