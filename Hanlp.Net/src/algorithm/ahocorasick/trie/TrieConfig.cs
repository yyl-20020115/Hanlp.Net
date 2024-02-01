namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

/**
 * 配置
 */
public class TrieConfig
{
    /**
     * 允许重叠
     */
    private bool allowOverlaps = true;

    /**
     * 只保留最长匹配
     */
    public bool remainLongest = false;

    /**
     * 是否允许重叠
     *
     * @return
     */
    /**
 * 设置是否允许重叠
 *
 * @param allowOverlaps
 */
    public bool AllowOverlaps { get => allowOverlaps; set => this.allowOverlaps = value; }
}
