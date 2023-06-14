namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

using System.Collections.Generic;
/**
 * 按起点比较区间
 */
public class IntervalableComparatorByPosition : IComparer<Intervalable>
{
    public int Compare(Intervalable intervalable, Intervalable intervalable2)
    {
        return intervalable.getStart() - intervalable2.getStart();
    }
}
