namespace com.hankcs.hanlp.model.bigram;


public class BigramDependencyModelTest : TestCase
{
    public void testLoad() 
    {
        assertEquals("限定", BigramDependencyModel.get("传", "v", "角落", "n"));
    }
}