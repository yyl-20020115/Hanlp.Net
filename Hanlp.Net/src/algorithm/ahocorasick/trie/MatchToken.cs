namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

/**
 * 匹配到的片段
 */
public class MatchToken : Token
{

    private Emit emit;

    public MatchToken(String fragment, Emit emit)
        : base(fragment)
    {
        this.emit = emit;
    }

    public override bool isMatch()
    {
        return true;
    }

    public override Emit getEmit()
    {
        return this.emit;
    }

}
