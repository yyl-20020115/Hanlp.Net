namespace com.hankcs.hanlp.corpus.io;



[TestClass]
public class ByteArrayTest : TestCase
{
    static String DATA_TEST_OUT_BIN;
    private File tempFile;

    [TestInitialize]
    public void setUp() 
    {
        tempFile = File.createTempFile("hanlp-", ".dat");
        DATA_TEST_OUT_BIN = tempFile.getAbsolutePath();
    }
    [TestMethod]

    public void testReadDouble() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        double d = 0.123456789;
        _out.writeDouble(d);
        int i = 3389;
        _out.writeInt(i);
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        assertEquals(d, byteArray.nextDouble());
        assertEquals(i, byteArray.nextInt());
    }
    [TestMethod]

    public void testReadUTF() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        String utf = "hankcs你好123";
        _out.writeUTF(utf);
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        assertEquals(utf, byteArray.nextUTF());
    }
    [TestMethod]

    public void testReadUnsignedShort() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        int utflen = 123;
        _out.writeByte((byte) ((utflen >>> 8) & 0xFF));
        _out.writeByte((byte) ((utflen >>> 0) & 0xFF));
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        assertEquals(utflen, byteArray.nextUnsignedShort());
    }
    [TestMethod]

    public void testConvertCharToInt() 
    {
//        for (int i = 0; i < int.MAX_VALUE; ++i)
        for (int i = 0; i < 1024; ++i)
        {
            int n = i;
            char[] twoChar = ByteUtil.convertIntToTwoChar(n);
            assertEquals(n, ByteUtil.convertTwoCharToInt(twoChar[0], twoChar[1]));
        }
    }
    [TestMethod]

    public void testNextBoolean() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(tempFile));
        _out.writeBoolean(true);
        _out.writeBoolean(false);
        ByteArray byteArray = ByteArray.createByteArray(tempFile.getAbsolutePath());
        assertNotNull(byteArray);
        assertEquals(byteArray.nextBoolean(), true);
        assertEquals(byteArray.nextBoolean(), false);
        tempFile.deleteOnExit();
    }
    [TestMethod]

    public void testWriteAndRead() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        _out.writeChar('H');
        _out.writeChar('e');
        _out.writeChar('l');
        _out.writeChar('l');
        _out.writeChar('o');
        _out.close();
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        while (byteArray.hasMore())
        {
            byteArray.nextChar();
//            Console.WriteLine(byteArray.nextChar());
        }
    }
    [TestMethod]

    public void testWriteBigFile() 
    {
        DataOutputStream _out = new DataOutputStream(new FileOutputStream(DATA_TEST_OUT_BIN));
        for (int i = 0; i < 10000; i++)
        {
            _out.writeInt(i);
        }
        _out.close();
    }
    [TestMethod]

    public void testStream() 
    {
        ByteArray byteArray = ByteArrayFileStream.createByteArrayFileStream(DATA_TEST_OUT_BIN);
        while (byteArray.hasMore())
        {
            Console.WriteLine(byteArray.nextInt());
        }
    }

//    /**
//     * 无法在-Xms512m -Xmx512m -Xmn256m下运行<br>
//     *     java.lang.OutOfMemoryError: GC overhead limit exceeded
//     * @
//     */
//    public void testLoadByteArray() 
//    {
//        ByteArray byteArray = ByteArray.createByteArray(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//    }
//
//    /**
//     * 能够在-Xms512m -Xmx512m -Xmn256m下运行
//     * @
//     */
//    public void testLoadByteArrayStream() 
//    {
//        ByteArray byteArray = ByteArrayFileStream.createByteArrayFileStream(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//    }
//
//    public void testBenchmark() 
//    {
//        long start;
//
//        ByteArray byteArray = ByteArray.createByteArray(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//
//        byteArray = ByteArrayFileStream.createByteArrayFileStream(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//
//        start = System.currentTimeMillis();
//        byteArray = ByteArray.createByteArray(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//        Console.printf("ByteArray: %d ms\n", (System.currentTimeMillis() - start));
//
//        start = System.currentTimeMillis();
//        byteArray = ByteArrayFileStream.createByteArrayFileStream(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//        Console.printf("ByteArrayStream: %d ms\n", (System.currentTimeMillis() - start));
//
////        ByteArray: 2626 ms
////        ByteArrayStream: 4165 ms
//    }
}