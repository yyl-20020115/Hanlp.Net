/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-07 PM5:29</create-date>
 *
 * <copyright file="ByteArrayOtherStream.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.io;




/**
 * @author hankcs
 */
public class ByteArrayOtherStream : ByteArrayStream
{
    InputStream @is;

    public ByteArrayOtherStream(byte[] bytes, int bufferSize)
    {
        base(bytes, bufferSize);
    }

    public ByteArrayOtherStream(byte[] bytes, int bufferSize, InputStream @is)
    {
        base(bytes, bufferSize);
        this.@is = @isis;
    }

    public static ByteArrayOtherStream createByteArrayOtherStream(string path)
    {
        try
        {
            InputStream @isis = IOAdapter == null ? new FileInputStream(path) : IOAdapter.open(path);
            return createByteArrayOtherStream(is);
        }
        catch (Exception e)
        {
            logger.warning(TextUtility.exceptionToString(e));
            return null;
        }
    }

    public static ByteArrayOtherStream createByteArrayOtherStream(InputStream is) 
    {
        if (@is == null) return null;
        int size = @is.available();
        size = Math.Max(102400, size); // 有些网络InputStream实现会返回0，直到read的时候才知道到底是不是0
        int bufferSize = Math.Min(1048576, size); // 最终缓冲区在100KB到1MB之间
        byte[] bytes = new byte[bufferSize];
        if (IOUtil.readBytesFromOtherInputStream(is, bytes) == 0)
        {
            throw new IOException("读取了空文件，或参数InputStream已经到了文件尾部");
        }
        return new ByteArrayOtherStream(bytes, bufferSize, is);
    }

    //@Override
    protected void ensureAvailableBytes(int size)
    {
        if (offset + size > bufferSize)
        {
            try
            {
                int wantedBytes = offset + size - bufferSize; // 实际只需要这么多
                wantedBytes = Math.Max(wantedBytes, is.available()); // 如果非阻塞IO能读到更多，那越多越好
                wantedBytes = Math.Min(wantedBytes, offset); // 但不能超过脏区的大小
                byte[] bytes = new byte[wantedBytes];
                int readBytes = IOUtil.readBytesFromOtherInputStream(is, bytes);
                assert readBytes > 0 : "已到达文件尾部！";
                System.arraycopy(this.bytes, offset, this.bytes, offset - wantedBytes, bufferSize - offset);
                System.arraycopy(bytes, 0, this.bytes, bufferSize - wantedBytes, wantedBytes);
                offset -= wantedBytes;
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }
    }

    //@Override
    public void close()
    {
        base.close();
        if (is == null)
        {
            return;
        }
        try
        {
            is.close();
        }
        catch (IOException e)
        {
            logger.warning(TextUtility.exceptionToString(e));
        }
    }
}
