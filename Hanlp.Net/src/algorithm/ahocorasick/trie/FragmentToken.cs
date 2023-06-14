namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;

public class FragmentToken : Token
{

    public FragmentToken(String fragment)
        :base(fragment)
    {
    }

    public override bool isMatch()
    {
        return false;
    }

    public override Emit getEmit()
    {
        return null;
    }
}
