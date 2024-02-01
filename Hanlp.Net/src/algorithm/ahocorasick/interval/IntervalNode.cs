namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

using System.Collections.Generic;

/**
 * 线段树上面的节点，实际上是一些区间的集合，并且按中点维护了两个节点
 */
public class IntervalNode
{
    /**
     * 方向
     */
    public enum Direction : int
    {
        LEFT,
        RIGHT
    }

    /**
     * 区间集合的最左端
     */
    private IntervalNode left = null;
    /**
     * 最右端
     */
    private IntervalNode right = null;
    /**
     * 中点
     */
    private int point;
    /**
     * 区间集合
     */
    private List<Intervalable> intervals = new ();

    /**
     * 构造一个节点
     * @param intervals
     */
    public IntervalNode(List<Intervalable> intervals)
    {
        this.point = DetermineMedian(intervals);

        var toLeft = new List<Intervalable>();  // 以中点为界靠左的区间
        var toRight = new List<Intervalable>(); // 靠右的区间

        foreach (var interval in intervals)
        {
            if (interval.End < this.point)
            {
                toLeft.Add(interval);
            }
            else if (interval.Start > this.point)
            {
                toRight.Add(interval);
            }
            else
            {
                this.intervals.Add(interval);
            }
        }

        if (toLeft.Count > 0)
        {
            this.left = new IntervalNode(toLeft);
        }
        if (toRight.Count > 0)
        {
            this.right = new IntervalNode(toRight);
        }
    }

    /**
     * 计算中点
     * @param intervals 区间集合
     * @return 中点坐标
     */
    public int DetermineMedian(List<Intervalable> intervals)
    {
        int start = -1;
        int end = -1;
        foreach (var interval in intervals)
        {
            int currentStart = interval.Start;
            int currentEnd = interval.End;
            if (start == -1 || currentStart < start)
            {
                start = currentStart;
            }
            if (end == -1 || currentEnd > end)
            {
                end = currentEnd;
            }
        }
        return (start + end) / 2;
    }

    /**
     * 寻找与interval有重叠的区间
     * @param interval
     * @return
     */
    public List<Intervalable> FindOverlaps(Intervalable interval)
    {

        List<Intervalable> overlaps = new();

        if (this.point < interval.Start)
        {
            // 右边找找
            AddToOverlaps(interval, overlaps, FindOverlappingRanges(this.right, interval));
            AddToOverlaps(interval, overlaps, CheckForOverlapsToTheRight(interval));
        }
        else if (this.point > interval.End)
        {
            // 左边找找
            AddToOverlaps(interval, overlaps, FindOverlappingRanges(this.left, interval));
            AddToOverlaps(interval, overlaps, CheckForOverlapsToTheLeft(interval));
        }
        else
        {
            // 否则在当前区间
            AddToOverlaps(interval, overlaps, this.intervals);
            AddToOverlaps(interval, overlaps, FindOverlappingRanges(this.left, interval));
            AddToOverlaps(interval, overlaps, FindOverlappingRanges(this.right, interval));
        }

        return overlaps;
    }

    /**
     * 添加到重叠区间列表中
     * @param interval 跟此区间重叠
     * @param overlaps 重叠区间列表
     * @param newOverlaps 希望将这些区间加入
     */
    protected void AddToOverlaps(Intervalable interval, List<Intervalable> overlaps, List<Intervalable> newOverlaps)
    {
        foreach (var currentInterval in newOverlaps)
        {
            if (!currentInterval.Equals(interval))
            {
                overlaps.Add(currentInterval);
            }
        }
    }

    /**
     * 往左边寻找重叠
     * @param interval
     * @return
     */
    protected List<Intervalable> CheckForOverlapsToTheLeft(Intervalable interval) => CheckForOverlaps(interval, Direction.LEFT);

    /**
     * 往右边寻找重叠
     * @param interval
     * @return
     */
    protected List<Intervalable> CheckForOverlapsToTheRight(Intervalable interval) => CheckForOverlaps(interval, Direction.RIGHT);

    /**
     * 寻找重叠
     * @param interval 一个区间，与该区间重叠
     * @param direction 方向，表明重叠区间在interval的左边还是右边
     * @return
     */
    protected List<Intervalable> CheckForOverlaps(Intervalable interval, Direction direction)
    {
        List<Intervalable> overlaps = new List<Intervalable>();
        foreach (Intervalable currentInterval in this.intervals)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    if (currentInterval.Start <= interval.End)
                    {
                        overlaps.Add(currentInterval);
                    }
                    break;
                case Direction.RIGHT:
                    if (currentInterval.End >= interval.Start)
                    {
                        overlaps.Add(currentInterval);
                    }
                    break;
            }
        }
        return overlaps;
    }

    /**
     * 是对IntervalNode.findOverlaps(Intervalable)的一个包装，防止NPE
     * @see com.hankcs.hanlp.algorithm.ahocorasick.interval.IntervalNode#findOverlaps(Intervalable)
     * @param node
     * @param interval
     * @return
     */
    protected static List<Intervalable> FindOverlappingRanges(IntervalNode node, Intervalable interval)
    {
        return node != null ? node.FindOverlaps(interval) : new();
    }

}
