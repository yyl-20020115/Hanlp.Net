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

    public int Start => this.start;

    public int End => this.end;

    public int Count => end - start + 1;

    /**
     * �Ƿ�����һ�����佻�棨��һ�����ص���
     * @param other
     * @return
     */
    public bool OverlapsWith(Interval other)
        => this.start <= other.End &&
                this.end >= other.Start;

    /**
     * �����Ƿ񸲸��������
     * @param point
     * @return
     */
    public bool OverlapsWith(int point) => this.start <= point && point <= this.end;

    ////@Override
    public override bool Equals(object? o)
        => o is Intervalable other && this.start == other.Start &&
                this.end == other.End;

    ////@Override
    public override int GetHashCode() => (start % 100) + this.end % 100;

    ////@Override
    public int CompareTo(object? o)
    {
        switch (o)
        {
            case Intervalable other:
                {
                    int comparison = start - other.Start;
                    return comparison != 0 ? comparison : end - other.End;
                }

            default:
                return -1;
        }
    }

    public override string ToString() => this.start + ":" + this.end;
}