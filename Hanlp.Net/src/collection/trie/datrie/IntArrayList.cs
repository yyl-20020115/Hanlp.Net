using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.trie.datrie;



/**
 * 动态数组
 */
public class IntArrayList : Serializable, ICacheAble
{
    private static readonly long serialVersionUID = 1908530358259070518L;
    private int[] data;
    /**
     * 实际size
     */
    private int _size;
    /**
     * 线性递增
     */
    private int linearExpandFactor;

    public void setLinearExpandFactor(int linearExpandFactor)
    {
        this.linearExpandFactor = linearExpandFactor;
    }

    /**
     * 是否指数递增
     */
    private bool exponentialExpanding = false;

    public bool isExponentialExpanding()
    {
        return exponentialExpanding;
    }

    public void setExponentialExpanding(bool multiplyExpanding)
    {
        this.exponentialExpanding = multiplyExpanding;
    }

    private double exponentialExpandFactor = 1.5;

    public double getExponentialExpandFactor()
    {
        return exponentialExpandFactor;
    }

    public void setExponentialExpandFactor(double exponentialExpandFactor)
    {
        this.exponentialExpandFactor = exponentialExpandFactor;
    }

    public IntArrayList()
        : this(1024)
    {
        ;
    }

    public IntArrayList(int capacity)
        : this(capacity, 10240)
    {
        ;
    }

    public IntArrayList(int capacity, int linearExpandFactor)
    {
        this.data = new int[capacity];
        this._size = 0;
        this.linearExpandFactor = linearExpandFactor;
    }

    private void expand()
    {
        if (!exponentialExpanding)
        {
            int[] newData = new int[this.data.Length + this.linearExpandFactor];
            System.arraycopy(this.data, 0, newData, 0, this.data.Length);
            this.data = newData;
        }
        else
        {
            int[] newData = new int[(int) (this.data.Length * exponentialExpandFactor)];
            System.arraycopy(this.data, 0, newData, 0, this.data.Length);
            this.data = newData;
        }
    }

    /**
     * 在数组尾部新增一个元素
     *
     * @param element
     */
    public void Append(int element)
    {
        if (this._size == this.data.Length)
        {
            expand();
        }
        this.data[this._size] = element;
        this._size += 1;
    }

    /**
     * 去掉多余的buffer
     */
    public void loseWeight()
    {
        if (_size == data.Length)
        {
            return;
        }
        int[] newData = new int[_size];
        System.arraycopy(this.data, 0, newData, 0, size);
        this.data = newData;
    }

    public int size()
    {
        return this._size;
    }

    public int getLinearExpandFactor()
    {
        return this.linearExpandFactor;
    }

    public void set(int index, int value)
    {
        this.data[index] = value;
    }

    public int get(int index)
    {
        return this.data[index];
    }

    public void removeLast()
    {
        --_size;
    }

    public int getLast()
    {
        return data[_size - 1];
    }

    public void setLast(int value)
    {
        data[_size - 1] = value;
    }

    public int pop()
    {
        return data[--_size];
    }

    //@Override
    public void save(Stream _out) 
    {
        _out.writeInt(size);
        for (int i = 0; i < _size; i++)
        {
            _out.writeInt(data[i]);
        }
        _out.writeInt(linearExpandFactor);
        _out.writeBoolean(exponentialExpanding);
        _out.writeDouble(exponentialExpandFactor);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        if (byteArray == null)
        {
            return false;
        }
        _size = byteArray.nextInt();
        data = new int[_size];
        for (int i = 0; i < _size; i++)
        {
            data[i] = byteArray.nextInt();
        }
        linearExpandFactor = byteArray.nextInt();
        exponentialExpanding = byteArray.nextBoolean();
        exponentialExpandFactor = byteArray.nextDouble();
        return true;
    }

    private void writeObject(Stream _out) 
    {
        loseWeight();
        _out.writeInt(size);
        _out.writeObject(data);
        _out.writeInt(linearExpandFactor);
        _out.writeBoolean(exponentialExpanding);
        _out.writeDouble(exponentialExpandFactor);
    }

    private void readObject(Stream _in)
    {
        _size = _in.readInt();
        data = (int[])_in.readObject();
        linearExpandFactor = _in.readInt();
        exponentialExpanding = _in.readBoolean();
        exponentialExpandFactor = _in.readDouble();
    }

    //@Override
    public override string ToString()
    {
        var head = new List<int>(20);
        for (int i = 0; i < Math.Min(_size, 20); ++i)
        {
            head.Add(data[i]);
        }
        return head.ToString();
    }
}
