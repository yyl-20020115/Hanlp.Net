/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/5 15:37</create-date>
 *
 * <copyright file="CharType.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.other;




/**
 * 字符类型
 *
 * @author hankcs
 */
public class CharType
{
    /**
     * 单字节
     */
    public const byte CT_SINGLE = 5;

    /**
     * 分隔符"!,.?()[]{}+=
     */
    public const byte CT_DELIMITER = CT_SINGLE + 1;

    /**
     * 中文字符
     */
    public const byte CT_CHINESE = CT_SINGLE + 2;

    /**
     * 字母
     */
    public const byte CT_LETTER = CT_SINGLE + 3;

    /**
     * 数字
     */
    public const byte CT_NUM = CT_SINGLE + 4;

    /**
     * 序号
     */
    public const byte CT_INDEX = CT_SINGLE + 5;

    /**
     * 中文数字
     */
    public const byte CT_CNUM = CT_SINGLE + 6;

    /**
     * 其他
     */
    public const byte CT_OTHER = CT_SINGLE + 12;

    public static byte[] type;

    static CharType()
    {
        type = new byte[65536];
        logger.info("字符类型对应表开始加载 " + HanLP.Config.CharTypePath);
        long start = DateTime.Now.Microsecond;
        ByteArray byteArray = ByteArray.createByteArray(HanLP.Config.CharTypePath);
        if (byteArray == null)
        {
            try
            {
                byteArray = generate();
            }
            catch (IOException e)
            {
                throw new ArgumentException("字符类型对应表 " + HanLP.Config.CharTypePath + " 加载失败： " + TextUtility.exceptionToString(e));
            }
        }
        while (byteArray.hasMore())
        {
            int b = byteArray.nextChar();
            int e = byteArray.nextChar();
            byte t = byteArray.nextByte();
            for (int i = b; i <= e; ++i)
            {
                type[i] = t;
            }
        }
        logger.info("字符类型对应表加载成功，耗时" + (DateTime.Now.Microsecond - start) + " ms");
    }

    private static ByteArray generate() 
    {
        int preType = 5;
        int preChar = 0;
        List<int[]> typeList = new ();
        for (int i = 0; i <= char.MaxValue; ++i)
        {
            int type = TextUtility.charType((char) i);
//            Console.WriteLine("%d %d\n", i, TextUtility.charType((char) i));
            if (type != preType)
            {
                int[] array = new int[3];
                array[0] = preChar;
                array[1] = i - 1;
                array[2] = preType;
                typeList.Add(array);
//                Console.WriteLine("%d %d %d\n", array[0], array[1], array[2]);
                preChar = i;
            }
            preType = type;
        }
        {
            int[] array = new int[3];
            array[0] = preChar;
            array[1] = (int) char.MaxValue;
            array[2] = preType;
            typeList.Add(array);
        }
//        Console.Write("int[" + typeList.Count + "][3] array = \n");
        Stream _out = new Stream(new FileStream(HanLP.Config.CharTypePath));
        foreach (int[] array in typeList)
        {
//            Console.WriteLine("%d %d %d\n", array[0], array[1], array[2]);
            _out.writeChar(array[0]);
            _out.writeChar(array[1]);
            _out.writeByte(array[2]);
        }
        _out.Close();
        ByteArray byteArray = ByteArray.createByteArray(HanLP.Config.CharTypePath);
        return byteArray;
    }

    /**
     * 获取字符的类型
     *
     * @param c
     * @return
     */
    public static byte get(char c)
    {
        return type[(int) c];
    }

    /**
     * 设置字符类型
     *
     * @param c 字符
     * @param t 类型
     */
    public static void set(char c, byte t)
    {
        type[c] = t;
    }
}
