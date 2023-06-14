namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

/**
 * 按起点比较区间
 */
public class IntervalableComparatorByPosition : Comparator<Intervalable>
{
    ////@Override
    public int compare(Intervalable intervalable, Intervalable intervalable2)
    {
        return intervalable.getStart() - intervalable2.getStart();
    }
}
