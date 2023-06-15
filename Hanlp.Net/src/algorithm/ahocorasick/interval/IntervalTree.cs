namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

using System.Collections.Generic;

/**
 * 线段树，用于检查区间重叠
 */
public class IntervalTree
{
    /**
     * 根节点
     */
    private IntervalNode rootNode = null;

    /**
     * 构造线段树
     *
     * @param intervals
     */
    public IntervalTree(List<Intervalable> intervals)
    {
        this.rootNode = new IntervalNode(intervals);
    }

    /**
     * 从区间列表中移除重叠的区间
     *
     * @param intervals
     * @return
     */
    public List<Intervalable> removeOverlaps(List<Intervalable> intervals)
    {
        // 排序，按照先大小后左端点的顺序
        intervals.Sort(new IntervalableComparatorBySize());
        HashSet<Intervalable> removeIntervals = new();

        foreach (Intervalable interval in intervals)
        {
            // 如果区间已经被移除了，就忽略它
            if (removeIntervals.Contains(interval))
            {
                continue;
            }

            // 否则就移除它
            removeIntervals.UnionWith(findOverlaps(interval));
        }

        // 移除所有的重叠区间
        foreach (Intervalable removeInterval in removeIntervals)
        {
            intervals.Remove(removeInterval);
        }

        // 排序，按照左端顺序
        intervals.Sort(new IntervalableComparatorByPosition());

        return intervals;
    }

    /**
     * 寻找重叠区间
     *
     * @param interval 与这个区间重叠
     * @return 重叠的区间列表
     */
    public List<Intervalable> findOverlaps(Intervalable interval)
    {
        return rootNode.findOverlaps(interval);
    }

}
