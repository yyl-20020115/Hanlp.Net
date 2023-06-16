namespace com.hankcs.hanlp.corpus.io;



[TestClass]
public class IOUtilTest : TestCase
{
    [TestMethod]
    public void TestReadBytesFromOtherInputStream() 
    {
        Random random = new Random(DateTime.Now.Microsecond);
        byte[] originalData = new byte[1024 * 1024]; // 1MB
        random.NextBytes(originalData);
        using var fs = new FileStream("test.bin", FileMode.Open);
        byte[] readData = IOUtil.readBytesFromOtherInputStream(fs);
        AssertEquals(originalData.Length, readData.Length);
        for (int i = 0; i < originalData.Length; i++)
        {
            AssertEquals(originalData[i], readData[i]);
        }
    }

    //public class BAI : ByteArrayInputStream//(originalData)
    //{
    //    //@Override
    //    public /*synchronized*/ int available()
    //    {
    //        int realAvailable = base.available();
    //        if (realAvailable > 0)
    //        {
    //            return 2048; // 模拟某些网络InputStream
    //        }
    //        return realAvailable;
    //    }
    //}
    [TestMethod]
    public void TestUTF8BOM() 
    {
        var tempFile = createTempFile("hanlp-", ".txt");
        IOUtil.saveTxt(tempFile, "\uFEFF第1行\n第2行");
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(tempFile);
        int i = 1;
        foreach (String line in lineIterator)
        {
            AssertEquals(String.Format("第{0}行", i++), line);
        }
    }
}