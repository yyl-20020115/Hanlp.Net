/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone.details;

/**
 * key set，其实也包含值（每个key都有一个整型数）
 * @author manabe
 */
public class Keyset
{
    /**
     * 构造一个KeySet
     * @param keys 字节类型的key
     * @param values 每个key对应的值
     */
    public Keyset(byte[][] keys, int[] values)
    {
        _keys = keys;
        _values = values;
    }

    /**
     * keyset的容量
     * @return
     */
    public int NumKeys => _keys.Length;

    /**
     * 根据id获取key
     * @param id
     * @return
     */
    public byte[] GetKey(int id) => _keys[id];

    /**
     * 获取某个key的某一个字节
     * @param keyId key的id
     * @param byteId 字节的下标（第几个字节）
     * @return 字节，返回0表示越界了
     */
    public byte GetKeyByte(int keyId, int byteId)
    {
        if (byteId >= _keys[keyId].Length)
        {
            return 0;
        }
        return _keys[keyId][byteId];
    }

    /**
     * 是否含有值
     * @return
     */
     public bool HasValues => _values != null;

    /**
     * 根据下标获取值
     * @param id
     * @return
     */
    public int GetValue(int id)
    {
        if (HasValues)
        {
            return _values[id];
        }
        return id;
    }

    /**
     * 键
     */
    private readonly byte[][] _keys;
    /**
     * 值
     */
    private readonly int[] _values;
}
