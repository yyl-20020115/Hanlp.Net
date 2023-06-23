namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

/**
 * ����
 */
public class Interval : Intervalable
{
    /**
     * ���
     */
    private readonly int start;
    /**
     * �յ�
     */
    private readonly int end;

    /**
     * ����һ������
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
     * �Ƿ�����һ�����佻�棨��һ�����ص���
     * @param other
     * @return
     */
    public bool overlapsWith(Interval other) 
        => this.start <= other.getEnd() &&
                this.end >= other.getStart();

    /**
     * �����Ƿ񸲸��������
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