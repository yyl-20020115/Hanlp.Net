namespace com.hankcs.hanlp.corpus.tag;


[TestClass]
public class NatureTest : TestCase
{
    [TestMethod]
    public void TestFromString() 
    {
        Nature one = Nature.create("新词性1");
        Nature two = Nature.create("新词性2");

        AssertEquals(one, Nature.fromString("新词性1"));
        AssertEquals(two, Nature.fromString("新词性2"));
    }
}