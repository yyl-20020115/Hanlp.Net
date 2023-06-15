namespace com.hankcs.hanlp.corpus.io;



[TestClass]
public class IIOAdapterTest : TestCase
{
    /**
     * 这个方法演示通过IOAdapter阻止HanLP加载和生成缓存
     *
     * @
     */
    [TestMethod]
    public void testReturnNullInIOAdapter() 
    {
        HanLP.Config.IOAdapter = new ;

        HanLP.Config.enableDebug(false);
        assertEquals(true, CoreStopWordDictionary.Contains("的"));
    }

    public class IOP : FileIOAdapter
    {
        //@Override
        public InputStream open(String path)
        {
            if (path.endsWith(Predefine.BIN_EXT)) return null;
            return super.open(path);
        }

        //@Override
        public OutputStream create(String path)
        {
            if (path.endsWith(Predefine.BIN_EXT)) return null;
            return super.create(path);
        }
    }
}