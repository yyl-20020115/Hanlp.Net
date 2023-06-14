namespace com.hankcs.hanlp.corpus.dictionary.item;


public class ItemTest : TestCase
{
    public void testCreate() 
    {
        assertEquals("希望 v 7685 vn 616", Item.create("希望 v 7685 vn 616").toString());
    }

    public void testCombine() 
    {
        SimpleItem itemA = SimpleItem.create("A 1 B 2");
        SimpleItem itemB = SimpleItem.create("B 1 C 2 D 3");
        itemA.combine(itemB);
        assertEquals("B 3 D 3 C 2 A 1 ", itemA.toString());
    }
}