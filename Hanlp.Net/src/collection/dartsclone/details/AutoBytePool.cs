/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone.details;

/**
 * 动态数组<br>
 * Memory management of resizable array.
 *
 * @author
 */
class AutoBytePool
{
    /**
     * 获取缓冲区
     * @return 缓冲区
     */
    byte[] Buffer => _buf;

    /**
     * 取字节
     * @param id 字节下标
     * @return 字节
     */
    public byte Get(int id)
    {
        return _buf[id];
    }
    public byte this[int id]=>this.Get(id);
    /**
     * 设置值
     * @param id 下标
     * @param value 值
     */
    public void Set(int id, byte value)
    {
        _buf[id] = value;
    }

    /**
     * 是否为空
     * @return true表示为空
     */
    public bool IsEmpty => (_size == 0);

    /**
     * 缓冲区大小
     * @return 大小
     */
    public int Count => _size;

    /**
     * 清空缓存
     */
    public void Clear()
    {
        Resize(0);
        _buf = null;
        _size = 0;
        _capacity = 0;
    }

    /**
     * 在末尾加一个值
     * @param value 值
     */
    public void Add(byte value)
    {
        if (_size == _capacity)
        {
            ResizeBuffer(_size + 1);
        }
        _buf[_size++] = value;
    }

    /**
     * 将最后一个值去掉
     */
    void deleteLast()
    {
        --_size;
    }

    /**
     * 重设大小
     * @param size 大小
     */
    public void Resize(int size)
    {
        if (size > _capacity)
        {
            ResizeBuffer(size);
        }
        _size = size;
    }

    /**
     * 重设大小，并且在末尾加一个值
     * @param size 大小
     * @param value 值
     */
    void Resize(int size, byte value)
    {
        if (size > _capacity)
        {
            ResizeBuffer(size);
        }
        while (_size < size)
        {
            _buf[_size++] = value;
        }
    }

    /**
     * 增加容量
     * @param size 容量
     */
    void Reserve(int size)
    {
        if (size > _capacity)
        {
            ResizeBuffer(size);
        }
    }

    /**
     * 设置缓冲区大小
     * @param size 大小
     */
    private void ResizeBuffer(int size)
    {
        int capacity;
        if (size >= _capacity * 2)
        {
            capacity = size;
        }
        else
        {
            capacity = 1;
            while (capacity < size)
            {
                capacity <<= 1;
            }
        }
        byte[] buf = new byte[capacity];
        if (_size > 0)
        {
            //Array.Copy(_buf, 0, buf, 0, _size);
            Array.Copy(_buf,0,buf,0,_size);
        }
        _buf = buf;
        _capacity = capacity; 
    }

    /**
     * 缓冲区
     */
    private byte[] _buf;
    /**
     * 大小
     */
    private int _size;
    /**
     * 容量
     */
    private int _capacity;
}
