using com.hankcs.hanlp.dictionary.stopword;
using com.hankcs.hanlp.utility;

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
    public void TestReturnNullInIOAdapter() 
    {
        HanLP.Config.IOAdapter = new IOP();

        HanLP.Config.enableDebug(false);
        AssertEquals(true, CoreStopWordDictionary.Contains("的"));
    }

    public class IOP : FileIOAdapter
    {
        public IOP()
        {
                
        }
        //@Override
        public Stream open(String path)
        {
            if (path.EndsWith(Predefine.BIN_EXT)) return null;
            return base.open(path);
        }

        //@Override
        public Stream create(String path)
        {
            if (path.EndsWith(Predefine.BIN_EXT)) return null;
            return base.create(path);
        }
    }
}