/*
 * <summary></summary>
 * <author>hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/6 21:32</create-date>
 *
 * <copyright file="ByteArrayStream.java">
 * Copyright (c) 2003-2015, hankcs. All Right Reserved, http://www.hankcs.com/
 * </copyright>
 */
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.corpus.io;



/**
 * 流式的字节数组，降低读取时的内存峰值
 * @author hankcs
 */
public class ByteArrayFileStream : ByteArrayStream
{
    private FileChannel fileChannel;

    public ByteArrayFileStream(byte[] bytes, int bufferSize, FileChannel fileChannel)
    {
        base(bytes, bufferSize);
        this.fileChannel = fileChannel;
    }

    public static ByteArrayFileStream createByteArrayFileStream(string path)
    {
        try
        {
            FileStream fileInputStream = new FileStream(path);
            return createByteArrayFileStream(fileInputStream);
        }
        catch (Exception e)
        {
            logger.warning(TextUtility.exceptionToString(e));
            return null;
        }
    }

    public static ByteArrayFileStream createByteArrayFileStream(FileStream fileInputStream) 
    {
        FileChannel channel = fileInputStream.getChannel();
        long size = channel.size();
        int bufferSize = (int) Math.Min(1048576, size);
        ByteBuffer byteBuffer = ByteBuffer.allocate(bufferSize);
        if (channel.read(byteBuffer) == size)
        {
            channel.Close();
            channel = null;
        }
        byteBuffer.flip();
        byte[] bytes = byteBuffer.array();
        return new ByteArrayFileStream(bytes, bufferSize, channel);
    }

    //@Override
    public bool hasMore()
    {
        return offset < bufferSize || fileChannel != null;
    }

    /**
     * 确保buffer数组余有size个字节
     * @param size
     */
    //@Override
    protected void ensureAvailableBytes(int size)
    {
        if (offset + size > bufferSize)
        {
            try
            {
                int availableBytes = (int) (fileChannel.size() - fileChannel.position());
                ByteBuffer byteBuffer = ByteBuffer.allocate(Math.Min(availableBytes, offset));
                int readBytes = fileChannel.read(byteBuffer);
                if (readBytes == availableBytes)
                {
                    fileChannel.Close();
                    fileChannel = null;
                }
                //assert readBytes > 0 : "已到达文件尾部！";
                byteBuffer.flip();
                byte[] bytes = byteBuffer.array();
                Array.Copy(this.bytes, offset, this.bytes, offset - readBytes, bufferSize - offset);
                Array.Copy(bytes, 0, this.bytes, bufferSize - readBytes, readBytes);
                offset -= readBytes;
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
        }
    }

    //@Override
    public void Close()
    {
        base.Close();
        try
        {
            if (fileChannel == null) return;
            fileChannel.Close();
        }
        catch (IOException e)
        {
            logger.warning(TextUtility.exceptionToString(e));
        }
    }
}
