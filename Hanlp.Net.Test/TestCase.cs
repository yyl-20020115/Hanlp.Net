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
}