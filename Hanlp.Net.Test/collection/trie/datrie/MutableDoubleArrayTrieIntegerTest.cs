namespace com.hankcs.hanlp.collection.trie.datrie;



[TestClass]
public class MutableDoubleArrayTrieIntegerTest : TestCase
{
    MutableDoubleArrayTrieInteger mdat;
    private int size;

    [TestInitialize]
    public void setUp() 
    {
        mdat = new MutableDoubleArrayTrieInteger();
        size = 64;
        for (int i = 0; i < size; ++i)
        {
            mdat.put(String.valueOf(i), i);
        }
    }
    [TestMethod]

    public void testSaveLoad() 
    {
        File tempFile = File.createTempFile("hanlp", ".mdat");
        mdat.save(new DataOutputStream(new FileOutputStream(tempFile)));
        mdat = new MutableDoubleArrayTrieInteger();
        mdat.load(ByteArray.createByteArray(tempFile.getAbsolutePath()));
        assertEquals(size, mdat.size());
        for (int i = 0; i < size; ++i)
        {
            assertEquals(i, mdat.get(String.valueOf(i)));
        }

        for (int i = size; i < 2 * size; ++i)
        {
            assertEquals(-1, mdat.get(String.valueOf(i)));
        }
    }
}