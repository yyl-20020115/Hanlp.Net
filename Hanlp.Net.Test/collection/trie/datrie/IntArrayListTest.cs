using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.trie.datrie;

[TestClass]
public class IntArrayListTest : TestCase
{
    IntArrayList array = new IntArrayList();

    [TestInitialize]
    public override void setUp() 
    {
        for (int i = 0; i < 64; ++i)
        {
            array.Append(i);
        }
    }
    [TestMethod]
    public void TestSaveLoad() 
    {
        File tempFile = File.createTempFile("hanlp", ".intarray");
        array.save(new DataOutputStream(new FileOutputStream(tempFile.getAbsolutePath())));
        array.load(ByteArray.createByteArray(tempFile.getAbsolutePath()));
        for (int i = 0; i < 64; ++i)
        {
            assertEquals(i, array.get(i));
        }
    }
}