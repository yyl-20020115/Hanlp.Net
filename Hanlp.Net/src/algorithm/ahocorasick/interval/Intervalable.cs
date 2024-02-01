namespace com.hankcs.hanlp.algorithm.ahocorasick.interval;

using System;
/**
 * 区间接口
 */
public interface Intervalable : IComparable
{
    /**
     * 起点
     * @return
     */
    int Start { get; }

    /**
     * 终点
     * @return
     */
    int End { get; }

    /**
     * 长度
     * @return
     */
    int Count { get; }

}
