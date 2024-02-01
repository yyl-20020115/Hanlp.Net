namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

public class FragmentToken : Token
{

    public FragmentToken(string fragment)
        : base(fragment)
    {
    }

    public override bool IsMatch => false;

    public override Emit Emit => null;
}
