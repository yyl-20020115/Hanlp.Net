namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

/**
 * 区间
 */
public class Interval : Intervalable
{
    /**
     * 起点
     */
    private readonly int start;
    /**
     * 终点
     */
    private readonly int end;

    /**
     * 构造一个区间
     * @param start
     * @param end
     */
    public Interval(int start, int end)
    {
        this.start = start;
        this.end = end;
    }

    public int getStart() => this.start;

    public int getEnd() => this.end;

    public int Count => end - start + 1;

    /**
     * 是否与另一个区间交叉（有一部分重叠）
     * @param other
     * @return
     */
    public bool overlapsWith(Interval other) 
        => this.start <= other.getEnd() &&
                this.end >= other.getStart();

    /**
     * 区间是否覆盖了这个点
     * @param point
     * @return
     */
    public bool overlapsWith(int point) 
        => this.start <= point && point <= this.end;

    ////@Override
    public override bool Equals(object? o)
        => o is Intervalable other && this.start == other.getStart() &&
                this.end == other.getEnd();

    ////@Override
    public override int GetHashCode() => (start % 100) + this.end % 100;

    ////@Override
    public int CompareTo(object? o)
    {
        if (o is Intervalable other)
        {
            int comparison = this.start - other.getStart();
            return comparison != 0 ? comparison : this.end - other.getEnd();
        }
        return -1;
    }

    public override string ToString() => this.start + ":" + this.end;
}