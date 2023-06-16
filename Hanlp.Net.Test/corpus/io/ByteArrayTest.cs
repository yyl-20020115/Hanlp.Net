using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.corpus.io;



[TestClass]
public class ByteArrayTest : TestCase
{
    static String DATA_TEST_OUT_BIN;
    private string tempFile;

    [TestInitialize]
    public override void SetUp() 
    {
        tempFile = createTempFile("hanlp-", ".dat"); 
        DATA_TEST_OUT_BIN = tempFile;
    }
    [TestMethod]

    public void TestReadDouble() 
    {
        var _out = new FileStream(DATA_TEST_OUT_BIN, FileMode.Create);
        double d = 0.123456789;
        _out.Write(BitConverter.GetBytes(d));
        int i = 3389;
        _out.Write(BitConverter.GetBytes(i));
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        AssertEquals(d, byteArray.nextDouble());
        AssertEquals(i, byteArray.nextInt());
    }
    [TestMethod]

    public void TestReadUTF() 
    {
        var _out = (new FileStream(DATA_TEST_OUT_BIN, FileMode.Create));
        String utf = "hankcs你好123";
        _out.Write(Encoding.UTF8.GetBytes(utf));
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        AssertEquals(utf, byteArray.nextUTF());
    }
    [TestMethod]

    public void TestReadUnsignedShort() 
    {
        var _out = (new FileStream(DATA_TEST_OUT_BIN,FileMode.Create));
        int utflen = 123;
        _out.WriteByte((byte) ((utflen >>> 8) & 0xFF));
        _out.WriteByte((byte) ((utflen >>> 0) & 0xFF));
        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        AssertEquals(utflen, byteArray.nextUnsignedShort());
    }
    [TestMethod]

    public void TestConvertCharToInt() 
    {
//        for (int i = 0; i < int.MAX_VALUE; ++i)
        for (int i = 0; i < 1024; ++i)
        {
            int n = i;
            char[] twoChar = ByteUtil.convertIntToTwoChar(n);
            AssertEquals(n, ByteUtil.convertTwoCharToInt(twoChar[0], twoChar[1]));
        }
    }
    [TestMethod]

    public void TestNextBoolean()
    {
        var fs = new FileStream(tempFile, FileMode.Create);
        var _out = new BinaryWriter(fs);
        _out.Write(true);
        _out.Write(false);
        fs.Close();
        ByteArray byteArray = ByteArray.createByteArray(tempFile);
        AssertNotNull(byteArray);
        AssertEquals(byteArray.nextBoolean(), true);
        AssertEquals(byteArray.nextBoolean(), false);
        //tempFile.deleteOnExit();
    }
    [TestMethod]

    public void TestWriteAndRead()
    {
        var fs = new FileStream(DATA_TEST_OUT_BIN, FileMode.Create);
        var _out = new BinaryWriter(fs);
        _out.Write('H');
        _out.Write('e');
        _out.Write('l');
        _out.Write('l');
        _out.Write('o');
        _out.Close();
        fs.Close();

        ByteArray byteArray = ByteArray.createByteArray(DATA_TEST_OUT_BIN);
        while (byteArray.hasMore())
        {
            byteArray.nextChar();
//            Console.WriteLine(byteArray.nextChar());
        }
    }
    [TestMethod]

    public void TestWriteBigFile() 
    {
        var fs = new FileStream(DATA_TEST_OUT_BIN, FileMode.Create);
        var _out = new BinaryWriter(fs);
        for (int i = 0; i < 10000; i++)
        {
            _out.Write(i);
        }
        _out.Close();
        fs.Close();
    }
    [TestMethod]

    public void TestStream() 
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
//        start = DateTime.Now.Microsecond;
//        byteArray = ByteArray.createByteArray(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//        Console.printf("ByteArray: %d ms\n", (DateTime.Now.Microsecond - start));
//
//        start = DateTime.Now.Microsecond;
//        byteArray = ByteArrayFileStream.createByteArrayFileStream(HanLP.Config.MaxEntModelPath + Predefine.BIN_EXT);
//        MaxEntModel.create(byteArray);
//        Console.printf("ByteArrayStream: %d ms\n", (DateTime.Now.Microsecond - start));
//
////        ByteArray: 2626 ms
////        ByteArrayStream: 4165 ms
//    }
}