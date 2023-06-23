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
using com.hankcs.hanlp.classification.collections;
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
        this(model, createTempFile(string.valueOf(DateTime.Now.Microsecond), ".dat"));
    }

    public FileDataSet(string cache) 
    {
        initCache(cache);
    }

    private void initCache(string cache) 
    {
        this.cache = cache;
        _out = new Stream(new FileStream(cache));
    }

    private void initCache() 
    {
        initCache(createTempFile(string.valueOf(DateTime.Now.Microsecond), ".dat"));
    }

    public FileDataSet() 
    {
        this(createTempFile(string.valueOf(DateTime.Now.Microsecond), ".dat"));
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
        _out.writeInt(entrySet.Count);
        foreach (KeyValuePair<int, int[]> entry in entrySet)
        {
            _out.writeInt(entry.Key);
            _out.writeInt(entry.Value[0]);
        }
        ++size;
    }

    //@Override
    public int Count => this.size;

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
            IEnumerator<Document> iterator = GetEnumerator();
            initCache();
            while (iterator.MoveNext())
            {
                Document document = iterator.next();
                FrequencyMap<int> tfMap = new FrequencyMap<int>();
                foreach (KeyValuePair<int, int[]> entry in document.tfMap.entrySet())
                {
                    int feature = entry.Key;
                    if (idMap[feature] == -1) continue;
                    tfMap.Add(idMap[feature], entry.Value);
                }
                // 检查是否是空白文档
                if (tfMap.Count == 0) continue;
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
    public IEnumerator<Document> GetEnumerator()
    {
        try
        {
            _out.Close();
            Stream _in  = (new FileStream(cache));
            return new ST();
        }
        catch (IOException e)
        {
            throw new RuntimeException(e);
        }
    }
    public class ST: IEnumerator<Document>
    {
        //@Override
        public void Remove()
        {
            throw new RuntimeException("不支持的操作");
        }

        //@Override
        public bool MoveNext()
        {
            try
            {
                bool next = @in.available() > 0;
                if (!next) @in.Close();
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
                return new Document(@in);
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }
    }
}
