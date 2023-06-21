using System.Text;

namespace com.hankcs.hanlp.mining.word2vec;

public class VocabWord : IComparable<VocabWord>
{

    public static readonly int MAX_CODE_LENGTH = 40;

    public int cn, codelen;
    public int[] point;
    public string word;
    public char[] code;

    public VocabWord(string word)
    {
        this.word = word;
        cn = 0;
        point = new int[MAX_CODE_LENGTH];
        code = new char[MAX_CODE_LENGTH];
    }

    public void setCn(int cn)
    {
        this.cn = cn;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("[%s] cn=%d, codelen=%d, ", word, cn, codelen));
        sb.Append("code=(");
        for (int i = 0; i < codelen; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(code[i]);
        }
        sb.Append("), point=(");
        for (int i = 0; i < codelen; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(point[i]);
        }
        sb.Append(")");
        return sb.ToString();
    }

    //@Override
    public int CompareTo(VocabWord that)
    {
        return that.cn - this.cn;
    }
}
