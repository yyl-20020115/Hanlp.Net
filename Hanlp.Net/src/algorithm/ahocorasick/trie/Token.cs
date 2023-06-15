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

    public string getFragment()
    {
        return this.fragment;
    }

    public abstract bool isMatch();

    public abstract Emit getEmit();

    public override string ToString() => $"{fragment}/{isMatch()}";
}
