using com.hankcs.hanlp.utility;
namespace com.hankcs.hanlp;

public class TestCase
{
    public static string createTempFile(string filename,string extension)
    {
        var r = Random.Shared;

        return filename + r.Next() + extension;
    }
    public virtual void SetUp()
    {

    }
    public static void AssertTrue(bool condition)
    {
        Assert.IsTrue(condition);
    }
    public static void AssertFalse(bool condition)
    {
        Assert.IsFalse(condition);
    }
    public static void AssertEquals(object expected, object actual)
    {
        Assert.AreEqual(expected, actual);
    }
    public static void AssertNotNull(object obj)
    {
        Assert.IsNull(obj);
    }
    
}