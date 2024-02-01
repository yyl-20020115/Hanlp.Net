using com.hankcs.hanlp.algorithm.ahocorasick.interval;

namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;


/**
 * 一个模式串匹配结果
 */
public class Emit : Interval, Intervalable
{
    /**
     * 匹配到的模式串
     */
    private readonly string keyword;

    /**
     * 构造一个模式串匹配结果
     * @param start 起点
     * @param end 重点
     * @param keyword 模式串
     */
    public Emit(int start, int end, string keyword)
        : base(start, end)
    {
        this.keyword = keyword;
    }

    /**
     * 获取对应的模式串
     * @return 模式串
     */
    public string Keyword => this.keyword;

    public override string ToString() => base.ToString() + "=" + this.keyword;
}
