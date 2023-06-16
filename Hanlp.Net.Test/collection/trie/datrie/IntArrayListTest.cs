using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.collection.trie.datrie;

[TestClass]
public class IntArrayListTest : TestCase
{
    IntArrayList array = new IntArrayList();

    [TestInitialize]
    public override void SetUp() 
    {
        for (int i = 0; i < 64; ++i)
        {
            array.Append(i);
        }
    }
    [TestMethod]
    public void TestSaveLoad() 
    {
        var tempFile = createTempFile("hanlp", ".intarray");
        array.save(new FileStream(tempFile,FileMode.Create));
        array.load(ByteArray.createByteArray(tempFile));
        for (int i = 0; i < 64; ++i)
        {
            AssertEquals(i, array.get(i)); 
        }
    }
}