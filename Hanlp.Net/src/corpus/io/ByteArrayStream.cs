/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-07 PM5:25</create-date>
 *
 * <copyright file="ByteArrayStream.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.io;


/**
 * @author hankcs
 */
public abstract class ByteArrayStream : ByteArray
{
    /**
     * 每次读取1mb
     */
    protected int bufferSize;

    public ByteArrayStream(byte[] bytes, int bufferSize)
    {
        base(bytes);
        this.bufferSize = bufferSize;
    }

    public static ByteArrayStream createByteArrayStream(string path)
    {
        if (IOAdapter == null) return ByteArrayFileStream.createByteArrayFileStream(path);

        try
        {
            Stream @is = IOAdapter.open(path);
            if (@is is FileStream) return ByteArrayFileStream.createByteArrayFileStream((FileStream) @is);
            return ByteArrayOtherStream.CreateByteArrayOtherStream(@is);
        }
        catch (IOException e)
        {
            logger.warning("打开失败：" + path);
            return null;
        }
    }

    //@Override
    public int Next()
    {
        ensureAvailableBytes(4);
        return base.Next();
    }

    //@Override
    public char nextChar()
    {
        ensureAvailableBytes(2);
        return base.nextChar();
    }

    //@Override
    public double nextDouble()
    {
        ensureAvailableBytes(8);
        return base.nextDouble();
    }

    //@Override
    public byte nextByte()
    {
        ensureAvailableBytes(1);
        return base.nextByte();
    }

    //@Override
    public float nextFloat()
    {
        ensureAvailableBytes(4);
        return base.nextFloat();
    }

    protected abstract void ensureAvailableBytes(int size);
}
