namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

/**
 * 匹配到的片段
 */
public class MatchToken : Token
{

    private readonly Emit emit;

    public MatchToken(string fragment, Emit emit)
        : base(fragment)
    {
        this.emit = emit;
    }

    public override bool IsMatch => true;

    public override Emit Emit => this.emit;

}
