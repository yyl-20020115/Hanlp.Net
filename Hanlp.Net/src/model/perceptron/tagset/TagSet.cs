/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-26 下午4:40</create-date>
 *
 * <copyright file="Tag.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.perceptron.tagset;



/**
 * @author hankcs
 */
public class TagSet : IIdStringMap, IStringIdMap, Iterable<Map.Entry<String, Integer>>, ICacheAble
{
    private Map<String, Integer> stringIdMap;
    private ArrayList<String> idStringMap;
    private int[] allTags;
    public TaskType type;

    public TagSet(TaskType type)
    {
        stringIdMap = new TreeMap<String, Integer>();
        idStringMap = new ArrayList<String>();
        this.type = type;
    }

    public int add(String tag)
    {
//        assertUnlock();
        Integer id = stringIdMap.get(tag);
        if (id == null)
        {
            id = stringIdMap.size();
            stringIdMap.put(tag, id);
            idStringMap.add(tag);
        }

        return id;
    }

    public int size()
    {
        return stringIdMap.size();
    }

    public int sizeIncludingBos()
    {
        return size() + 1;
    }

    public int bosId()
    {
        return size();
    }

    public void lock()
    {
//        assertUnlock();
        allTags = new int[size()];
        for (int i = 0; i < size(); i++)
        {
            allTags[i] = i;
        }
    }

//    private void assertUnlock()
//    {
//        if (allTags != null)
//        {
//            throw new IllegalStateException("标注集已锁定，无法修改");
//        }
//    }

    //@Override
    public String stringOf(int id)
    {
        return idStringMap.get(id);
    }

    //@Override
    public int idOf(String string)
    {
        Integer id = stringIdMap.get(string);
        if (id == null) id = -1;
        return id;
    }

    //@Override
    public Iterator<Map.Entry<String, Integer>> iterator()
    {
        return stringIdMap.entrySet().iterator();
    }

    /**
     * 获取所有标签及其下标
     *
     * @return
     */
    public int[] allTags()
    {
        return allTags;
    }

    public void save(DataOutputStream out) 
    {
        out.writeInt(type.ordinal());
        out.writeInt(size());
        for (String tag : idStringMap)
        {
            out.writeUTF(tag);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        idStringMap.clear();
        stringIdMap.clear();
        int size = byteArray.nextInt();
        for (int i = 0; i < size; i++)
        {
            String tag = byteArray.nextUTF();
            idStringMap.add(tag);
            stringIdMap.put(tag, i);
        }
        lock();
        return true;
    }

    public void load(DataInputStream in) 
    {
        idStringMap.clear();
        stringIdMap.clear();
        int size = in.readInt();
        for (int i = 0; i < size; i++)
        {
            String tag = in.readUTF();
            idStringMap.add(tag);
            stringIdMap.put(tag, i);
        }
        lock();
    }

    public Collection<String> tags()
    {
        return idStringMap;
    }
}