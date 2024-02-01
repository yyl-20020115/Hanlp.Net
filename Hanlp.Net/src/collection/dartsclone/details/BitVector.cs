/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone.details;

/**
 * Bit向量，类似于C++中的bitset
 * @author
 */
public class BitVector
{
    /**
     * 获取某一位的比特
     * @param id 位
     * @return 比特是1还是0
     */
    public bool Get(int id) => (_units.Get(id / UNIT_SIZE) >>> (id % UNIT_SIZE) & 1) == 1;

    /**
     * 设置某一位的比特
     * @param id 位
     * @param bit 比特
     */
    public void set(int id, bool bit)
    {
        if (bit)
        {
            _units.Set(id / UNIT_SIZE, _units.Get(id / UNIT_SIZE)
                    | 1 << (id % UNIT_SIZE));
        }
    }

    /**
     *
     * @param id
     * @return
     */
    public int rank(int id)
    {
        int unit_id = id / UNIT_SIZE;
        return _ranks[unit_id] + popCount(_units.Get(unit_id)
                                                  & (~0 >>> (UNIT_SIZE - (id % UNIT_SIZE) - 1)));
    }

    /**
     * 是否为空
     * @return
     */
    public bool empty()
    {
        return _units.IsEmpty;
    }

    /**
     * 1的数量
     * @return
     */
    public int numOnes()
    {
        return _numOnes;
    }

    /**
     * 大小
     * @return
     */
    public int Count => _size;

    /**
     * 在末尾追加
     */
    public void Append()
    {
        if ((_size % UNIT_SIZE) == 0)
        {
            _units.Add(0);
        }
        ++_size;
    }

    /**
     * 构建
     */
    public void build()
    {
        _ranks = new int[_units.Count];

        _numOnes = 0;
        for (int i = 0; i < _units.Count; ++i)
        {
            _ranks[i] = _numOnes;
            _numOnes += popCount(_units[i]);
        }
    }

    /**
     * 清空
     */
    public void Clear()
    {
        _units.Clear();
        _ranks = null;
    }

    /**
     * 整型大小
     */
    private static readonly int UNIT_SIZE = 32; // sizeof(int) * 8

    /**
     * 1的数量
     * @param unit
     * @return
     */
    private static int popCount(int unit)
    {
        unit = (int)((unit & 0xAAAAAAAA) >>> 1) + (unit & 0x55555555);
        unit = (int)((unit & 0xCCCCCCCC) >>> 2) + (unit & 0x33333333);
        unit = ((unit >>> 4) + unit) & 0x0F0F0F0F;
        unit += unit >>> 8;
        unit += unit >>> 16;
        return unit & 0xFF;
    }

    /**
     * 储存空间
     */
    private AutoIntPool _units = new AutoIntPool();
    /**
     * 是每个元素的1的个数的累加
     */
    private int[] _ranks;
    /**
     * 1的数量
     */
    private int _numOnes;
    /**
     * 大小
     */
    private int _size;
}
