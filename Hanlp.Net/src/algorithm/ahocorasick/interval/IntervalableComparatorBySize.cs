namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

using System.Collections.Generic;

/**
 * 按照长度比较区间，如果长度相同，则比较起点
 */
public class IntervalableComparatorBySize : IComparer<Intervalable>
{
    public int Compare(Intervalable? intervalable, Intervalable? intervalable2)
    {
        int comparison = intervalable2.Count - intervalable.Count;
        if (comparison == 0)
        {
            comparison = intervalable.getStart() - intervalable2.getStart();
        }
        return comparison;
    }
}
