namespace com.hankcs.hanlp.mining.word2vec;

public class VocabWord implements Comparable<VocabWord>
{

    public static final int MAX_CODE_LENGTH = 40;

    int cn, codelen;
    int[] point;
    String word;
    char[] code;

    public VocabWord(String word)
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
    public String toString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(String.format("[%s] cn=%d, codelen=%d, ", word, cn, codelen));
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
        return sb.toString();
    }

    //@Override
    public int compareTo(VocabWord that)
    {
        return that.cn - this.cn;
    }
}
