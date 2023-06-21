
using System.Text;

namespace com.hankcs.hanlp.mining.word2vec;


/**
 * 一些工具方法
 */
public class Utility
{
    private static readonly int SECOND = 1000;
    private static readonly int MINUTE = 60 * SECOND;
    private static readonly int HOUR = 60 * MINUTE;
    private static readonly int DAY = 24 * HOUR;

    public static string humanTime(long ms)
    {
        var text = new StringBuilder();
        if (ms > DAY)
        {
            text.Append(ms / DAY).Append(" d ");
            ms %= DAY;
        }
        if (ms > HOUR)
        {
            text.Append(ms / HOUR).Append(" h ");
            ms %= HOUR;
        }
        if (ms > MINUTE)
        {
            text.Append(ms / MINUTE).Append(" m ");
            ms %= MINUTE;
        }
        if (ms > SECOND)
        {
            long s = ms / SECOND;
            if (s < 10)
            {
                text.Append('0');
            }
            text.Append(s).Append(" s ");
//            ms %= SECOND;
        }
//        text.Append(ms + " ms");

        return text.ToString();
    }

    /**
     * @param c
     */
    public static void closeQuietly(Closeable c)
    {
        try
        {
            if (c != null) c.Close();
        }
        catch (IOException ignored)
        {
        }
    }

    /**
     * @param raf
     */
    public static void closeQuietly(RandomAccessFile raf)
    {
        try
        {
            if (raf != null) raf.Close();
        }
        catch (IOException ignored)
        {
        }
    }

    public static void closeQuietly(Stream _is)
    {
        try
        {
            if (_is != null) _is.Close();
        }
        catch (IOException ignored)
        {
        }
    }

    public static void closeQuietly(TextReader r)
    {
        try
        {
            if (r != null) r.Close();
        }
        catch (IOException ignored)
        {
        }
    }

    public static void closeQuietly(Stream os)
    {
        try
        {
            if (os != null) os.Close();
        }
        catch (IOException ignored)
        {
        }
    }

    public static void closeQuietly(TextWriter w)
    {
        try
        {
            if (w != null) w.Close();
        }
        catch (IOException ignored)
        {
        }
    }

    /**
     * 数组分割
     *
     * @param from 源
     * @param to   目标
     * @param <T>  类型
     * @return 目标
     */
    public static  T[] shrink<T>(T[] from, T[] to)
    {
        //assert to.Length <= from.Length;
        Array.Copy(from, 0, to, 0, to.Length);
        return to;
    }
}
