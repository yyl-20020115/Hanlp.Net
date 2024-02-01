namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

using System.Collections.Generic;
/**
 * 按起点比较区间
 */
public class IntervalableComparatorByPosition : IComparer<Intervalable>
{
    public int Compare(Intervalable? intervalable, Intervalable? intervalable2) 
        => intervalable.Start - intervalable2.Start;
}
