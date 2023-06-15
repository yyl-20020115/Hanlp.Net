using com.hankcs.hanlp.utility;
namespace com.hankcs.hanlp;

public class TestCase
{
    public virtual void setUp()
    {

    }
    public static void assertTrue(bool condition)
    {
        Assert.IsTrue(condition);
    }
    public static void assertFalse(bool condition)
    {
        Assert.IsFalse(condition);
    }
    public static void assertEquals(object expected, object actual)
    {
        Assert.AreEqual(expected, actual);
    }
    public static void assertNotNull(object obj)
    {
        Assert.IsNull(obj);
    }
    [TestMethod]
    public void testGet()
    {
        String total = Predefine.POSTFIX_SINGLE;
        for (int i = 0; i < total.Length; ++i)
        {
            String single = String.valueOf(total.charAt(i));
            assertEquals(1, dictionary.get(single));
        }
        for (String single : Predefine.POSTFIX_MUTIPLE)
        {
            assertEquals(single.Length(), dictionary.get(single));
        }
    }
}