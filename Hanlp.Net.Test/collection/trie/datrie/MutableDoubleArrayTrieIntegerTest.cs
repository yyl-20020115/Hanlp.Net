namespace com.hankcs.hanlp.collection.trie.datrie;



[TestClass]
public class MutableDoubleArrayTrieIntegerTest : TestCase
{
    MutableDoubleArrayTrieInteger mdat;
    private int size;

    [TestInitialize]
    public override void SetUp() 
    {
        mdat = new MutableDoubleArrayTrieInteger();
        size = 64;
        for (int i = 0; i < size; ++i)
        {
            mdat.put(i.ToString(), i);
        }
    }
    [TestMethod]

    public void TestSaveLoad() 
    {
        File tempFile = File.createTempFile("hanlp", ".mdat");
        mdat.save(new DataOutputStream(new FileOutputStream(tempFile)));
        mdat = new MutableDoubleArrayTrieInteger();
        mdat.load(ByteArray.createByteArray(tempFile.getAbsolutePath()));
        AssertEquals(size, mdat.size());
        for (int i = 0; i < size; ++i)
        {
            AssertEquals(i, mdat.get((i.ToString())));
        }

        for (int i = size; i < 2 * size; ++i)
        {
            AssertEquals(-1, mdat.get((i.ToString())));
        }
    }
}