/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone.details;

/**
 * 整型动态数组<br>
 * Memory management of resizable array.
 *
 * @author
 */
public class AutoIntPool
{
    public int[] Buffer => _buf;

    public int Get(int id) => _buf[id];
    public int this[int id] => this._buf[id];

    public void Set(int id, int value) => _buf[id] = value;

    public bool IsEmpty => (_size == 0);

    public int Count => _size;

    public void Clear()
    {
        Resize(0);
        _buf = null;
        _size = 0;
        _capacity = 0;
    }

    public void Add(int value)
    {
        if (_size == _capacity)
        {
            ResizeBuf(_size + 1);
        }
        _buf[_size++] = value;
    }

    public void deleteLast()
    {
        --_size;
    }

    public void Resize(int size)
    {
        if (size > _capacity)
        {
            ResizeBuf(size);
        }
        _size = size;
    }

    void Resize(int size, int value)
    {
        if (size > _capacity)
        {
            ResizeBuf(size);
        }
        while (_size < size)
        {
            _buf[_size++] = value;
        }
    }

    public void Reserve(int size)
    {
        if (size > _capacity)
        {
            ResizeBuf(size);
        }
    }

    private void ResizeBuf(int size)
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
        int[] buf = new int[capacity];
        if (_size > 0)
        {
           // Array.Copy(_buf, 0, buf, 0, _size);
            Array.Copy(_buf,0,buf, 0, _size);
        }
        _buf = buf;
        _capacity = capacity;
    }

    private int[] _buf;
    private int _size;
    private int _capacity;
}
