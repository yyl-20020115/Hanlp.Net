namespace com.hankcs.hanlp.corpus.io;



public class IIOAdapterTest : TestCase
{
    /**
     * 这个方法演示通过IOAdapter阻止HanLP加载和生成缓存
     *
     * @
     */
    public void testReturnNullInIOAdapter() 
    {
        HanLP.Config.IOAdapter = new FileIOAdapter()
        {
            //@Override
            public InputStream open(String path) throws FileNotFoundException
            {
                if (path.endsWith(Predefine.BIN_EXT)) return null;
                return super.open(path);
            }

            //@Override
            public OutputStream create(String path) throws FileNotFoundException
            {
                if (path.endsWith(Predefine.BIN_EXT)) return null;
                return super.create(path);
            }
        };

        HanLP.Config.enableDebug(false);
        assertEquals(true, CoreStopWordDictionary.contains("的"));
    }
}