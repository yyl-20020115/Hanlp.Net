namespace com.hankcs.hanlp.corpus.io;



[TestClass]
public class IOUtilTest : TestCase
{
    [TestMethod]
    public void TestReadBytesFromOtherInputStream() 
    {
        Random random = new Random(DateTime.Now.Microsecond);
        byte[] originalData = new byte[1024 * 1024]; // 1MB
        random.nextBytes(originalData);
        ByteArrayInputStream _is = ();
        byte[] readData = IOUtil.readBytesFromOtherInputStream(_is);
        assertEquals(originalData.Length, readData.Length);
        for (int i = 0; i < originalData.Length; i++)
        {
            assertEquals(originalData[i], readData[i]);
        }
    }

    public class BAI : ByteArrayInputStream//(originalData)
    {
        //@Override
        public /*synchronized*/ int available()
        {
            int realAvailable = base.available();
            if (realAvailable > 0)
            {
                return 2048; // 模拟某些网络InputStream
            }
            return realAvailable;
        }
    }
    [TestMethod]
    public void TestUTF8BOM() 
    {
        File tempFile = File.createTempFile("hanlp-", ".txt");
        tempFile.deleteOnExit();
        IOUtil.saveTxt(tempFile.getAbsolutePath(), "\uFEFF第1行\n第2行");
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(tempFile.getAbsolutePath());
        int i = 1;
        for (String line : lineIterator)
        {
            assertEquals(String.format("第%d行", i++), line);
        }
    }
}