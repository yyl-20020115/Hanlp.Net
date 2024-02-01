namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

/**
 * 一个片段
 */
public abstract class Token
{
    /**
     * 对应的片段
     */
    private readonly string fragment;

    public Token(string fragment)
    {
        this.fragment = fragment;
    }

    public string Fragment => this.fragment;

    public abstract bool IsMatch { get; }

    public abstract Emit Emit { get; }

    public override string ToString() => $"{fragment}/{IsMatch}";
}
