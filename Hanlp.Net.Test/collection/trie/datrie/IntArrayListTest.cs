namespace com.hankcs.hanlp.collection.trie.datrie;



public class IntArrayListTest : TestCase
{
    IntArrayList array = new IntArrayList();

    //@Override
    public void setUp() 
    {
        for (int i = 0; i < 64; ++i)
        {
            array.append(i);
        }
    }

    public void testSaveLoad() 
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