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
        this.point = determineMedian(intervals);

        var toLeft = new List<Intervalable>();  // 以中点为界靠左的区间
        var toRight = new List<Intervalable>(); // 靠右的区间

        foreach (var interval in intervals)
        {
            if (interval.getEnd() < this.point)
            {
                toLeft.Add(interval);
            }
            else if (interval.getStart() > this.point)
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
    public int determineMedian(List<Intervalable> intervals)
    {
        int start = -1;
        int end = -1;
        foreach (var interval in intervals)
        {
            int currentStart = interval.getStart();
            int currentEnd = interval.getEnd();
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
    public List<Intervalable> findOverlaps(Intervalable interval)
    {

        List<Intervalable> overlaps = new List<Intervalable>();

        if (this.point < interval.getStart())
        {
            // 右边找找
            addToOverlaps(interval, overlaps, findOverlappingRanges(this.right, interval));
            addToOverlaps(interval, overlaps, checkForOverlapsToTheRight(interval));
        }
        else if (this.point > interval.getEnd())
        {
            // 左边找找
            addToOverlaps(interval, overlaps, findOverlappingRanges(this.left, interval));
            addToOverlaps(interval, overlaps, checkForOverlapsToTheLeft(interval));
        }
        else
        {
            // 否则在当前区间
            addToOverlaps(interval, overlaps, this.intervals);
            addToOverlaps(interval, overlaps, findOverlappingRanges(this.left, interval));
            addToOverlaps(interval, overlaps, findOverlappingRanges(this.right, interval));
        }

        return overlaps;
    }

    /**
     * 添加到重叠区间列表中
     * @param interval 跟此区间重叠
     * @param overlaps 重叠区间列表
     * @param newOverlaps 希望将这些区间加入
     */
    protected void addToOverlaps(Intervalable interval, List<Intervalable> overlaps, List<Intervalable> newOverlaps)
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
    protected List<Intervalable> checkForOverlapsToTheLeft(Intervalable interval)
    {
        return checkForOverlaps(interval, Direction.LEFT);
    }

    /**
     * 往右边寻找重叠
     * @param interval
     * @return
     */
    protected List<Intervalable> checkForOverlapsToTheRight(Intervalable interval)
    {
        return checkForOverlaps(interval, Direction.RIGHT);
    }

    /**
     * 寻找重叠
     * @param interval 一个区间，与该区间重叠
     * @param direction 方向，表明重叠区间在interval的左边还是右边
     * @return
     */
    protected List<Intervalable> checkForOverlaps(Intervalable interval, Direction direction)
    {
        List<Intervalable> overlaps = new List<Intervalable>();
        foreach (Intervalable currentInterval in this.intervals)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    if (currentInterval.getStart() <= interval.getEnd())
                    {
                        overlaps.Add(currentInterval);
                    }
                    break;
                case Direction.RIGHT:
                    if (currentInterval.getEnd() >= interval.getStart())
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
    protected static List<Intervalable> findOverlappingRanges(IntervalNode node, Intervalable interval)
    {
        return node != null ? node.findOverlaps(interval) : new();
    }

}
