namespace com.hankcs.hanlp.corpus.dictionary.item;


[TestClass]
public class ItemTest : TestCase
{
    [TestMethod]
    public void TestCreate() 
    {
        AssertEquals("希望 v 7685 vn 616", Item.create("希望 v 7685 vn 616").ToString());
    }
    [TestMethod]
    public void TestCombine() 
    {
        var itemA = SimpleItem.create("A 1 B 2");
        var itemB = SimpleItem.create("B 1 C 2 D 3");
        itemA.combine(itemB);
        AssertEquals("B 3 D 3 C 2 A 1 ", string.Join(' ',itemA));
    }
}