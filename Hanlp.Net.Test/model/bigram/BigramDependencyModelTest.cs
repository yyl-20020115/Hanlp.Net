namespace com.hankcs.hanlp.model.bigram;

[TestClass]
public class BigramDependencyModelTest : TestCase
{
    [TestMethod]
    public void TestLoad() 
    {
        assertEquals("限定", BigramDependencyModel.get("传", "v", "角落", "n"));
    }
}