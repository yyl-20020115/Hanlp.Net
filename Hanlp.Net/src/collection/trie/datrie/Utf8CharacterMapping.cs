using System.Text;

namespace com.hankcs.hanlp.collection.trie.datrie;


/**
 * UTF-8编码到int的映射
 */
public class Utf8CharacterMapping : CharacterMapping//, Serializable
{
    private static readonly long serialVersionUID = -6529481088518753872L;
    private static readonly int N = 256;
    private static readonly int[] EMPTYLIST = new int[0];
    public static readonly Encoding UTF_8 = Encoding.UTF8;

    //@Override
    public int getInitSize()
    {
        return N;
    }

    //@Override
    public int getCharsetSize()
    {
        return N;
    }

    //@Override
    public int zeroId()
    {
        return 0;
    }

    //@Override
    public int[] toIdList(string key)
    {

        byte[] bytes = key.GetBytes(UTF_8);
        int[] res = new int[bytes.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = bytes[i] & 0xFF; // unsigned byte
        }
        if ((res.Length == 1) && (res[0] == 0))
        {
            return EMPTYLIST;
        }
        return res;
    }

    /**
     * codes ported from iconv lib in utf8.h utf8_codepointtomb
     */
    //@Override
    public int[] toIdList(int codePoint)
    {
        int count;
        if (codePoint < 0x80)
            count = 1;
        else if (codePoint < 0x800)
            count = 2;
        else if (codePoint < 0x10000)
            count = 3;
        else if (codePoint < 0x200000)
            count = 4;
        else if (codePoint < 0x4000000)
            count = 5;
        else if (codePoint <= 0x7fffffff)
            count = 6;
        else
            return EMPTYLIST;
        int[] r = new int[count];
        switch (count)
        { /* note: code falls through cases! */
            case 6:
                r[5] = (char) (0x80 | (codePoint & 0x3f));
                codePoint = codePoint >> 6;
                codePoint |= 0x4000000;
            case 5:
                r[4] = (char) (0x80 | (codePoint & 0x3f));
                codePoint = codePoint >> 6;
                codePoint |= 0x200000;
            case 4:
                r[3] = (char) (0x80 | (codePoint & 0x3f));
                codePoint = codePoint >> 6;
                codePoint |= 0x10000;
            case 3:
                r[2] = (char) (0x80 | (codePoint & 0x3f));
                codePoint = codePoint >> 6;
                codePoint |= 0x800;
            case 2:
                r[1] = (char) (0x80 | (codePoint & 0x3f));
                codePoint = codePoint >> 6;
                codePoint |= 0xc0;
            case 1:
                r[0] = (char) codePoint;
        }
        return r;
    }

    //@Override
    public override string ToString(int[] ids)
    {
        byte[] bytes = new byte[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            bytes[i] = (byte) ids[i];
        }
        try
        {
            return new string(bytes, "UTF-8");
        }
        catch (UnsupportedEncodingException e)
        {
            return null;
        }
    }
}
