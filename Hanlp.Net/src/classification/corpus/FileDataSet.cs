/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/20 PM4:42</create-date>
 *
 * <copyright file="FileDataSet.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.models;

namespace com.hankcs.hanlp.classification.corpus;



/**
 * @author hankcs
 */
public class FileDataSet : AbstractDataSet
{
    string cache;
    Stream _out;
    int size;

    public FileDataSet(AbstractModel model, string cache) 
        :base(model)
    {
        initCache(cache);
    }

    public FileDataSet(AbstractModel model) 
    {
        this(model, File.createTempFile(string.valueOf(DateTime.Now.Microsecond), ".dat"));
    }

    public FileDataSet(File cache) 
    {
        initCache(cache);
    }

    private void initCache(File cache) 
    {
        this.cache = cache;
        _out = new DataOutputStream(new FileOutputStream(cache));
    }

    private void initCache() 
    {
        initCache(File.createTempFile(string.valueOf(DateTime.Now.Microsecond), ".dat"));
    }

    public FileDataSet() 
    {
        this(File.createTempFile(string.valueOf(DateTime.Now.Microsecond), ".dat"));
    }

    //@Override
    public Document Add(string category, string text)
    {
        Document document = convert(category, text);
        try
        {
            Add(document);
        }
        catch (IOException e)
        {
            throw new RuntimeException(e);
        }
        return document;
    }

    private void Add(Document document) 
    {
        _out.writeInt(document.category);
        HashSet<KeyValuePair<int, int[]>> entrySet = document.tfMap.entrySet();
        _out.writeInt(entrySet.size());
        for (KeyValuePair<int, int[]> entry : entrySet)
        {
            _out.writeInt(entry.getKey());
            _out.writeInt(entry.getValue()[0]);
        }
        ++size;
    }

    //@Override
    public int size()
    {
        return size;
    }

    //@Override
    public void Clear()
    {
        size = 0;
    }

    //@Override
    public IDataSet shrink(int[] idMap)
    {
        try
        {
            Clear();
            Iterator<Document> iterator = iterator();
            initCache();
            while (iterator.hasNext())
            {
                Document document = iterator.next();
                FrequencyMap<int> tfMap = new FrequencyMap<int>();
                for (KeyValuePair<int, int[]> entry : document.tfMap.entrySet())
                {
                    int feature = entry.getKey();
                    if (idMap[feature] == -1) continue;
                    tfMap.put(idMap[feature], entry.getValue());
                }
                // 检查是否是空白文档
                if (tfMap.size() == 0) continue;
                document.tfMap = tfMap;
                Add(document);
            }
        }
        catch (IOException e)
        {
            throw new RuntimeException(e);
        }

        return this;
    }

    //@Override
    public Iterator<Document> iterator()
    {
        try
        {
            _out.close();
            DataInputStream _in  = new DataInputStream(new FileInputStream(cache));
            return new Iterator<Document>()
            {
                //@Override
                public void Remove()
                {
                    throw new RuntimeException("不支持的操作");
                }

                //@Override
                public bool hasNext()
                {
                    try
                    {
                        bool next = in.available() > 0;
                        if (!next) in.close();
                        return next;
                    }
                    catch (IOException e)
                    {
                        throw new RuntimeException(e);
                    }
                }

                //@Override
                public Document next()
                {
                    try
                    {
                        return new Document(in);
                    }
                    catch (IOException e)
                    {
                        throw new RuntimeException(e);
                    }
                }
            };
        }
        catch (IOException e)
        {
            throw new RuntimeException(e);
        }
    }
}
