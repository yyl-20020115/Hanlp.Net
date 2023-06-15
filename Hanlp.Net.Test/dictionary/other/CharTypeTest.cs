using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.dictionary.other;

[TestClass]
public class CharTypeTest : TestCase
{
    [TestMethod]

    public void TestNumber()
    {
//        for (int i = 0; i <= Character.MAX_VALUE; ++i)
//        {
//            if (CharType.get((char) i) == CharType.CT_NUM)
//                Console.WriteLine((char) i);
//        }
        AssertEquals(CharType.CT_NUM, CharType.get('1'));

    }
    [TestMethod]

    public void TestWhiteSpace()
    {
//        CharType.type[' '] = CharType.CT_OTHER;
        String text = "1 + 2 = 3; a+b= a + b";
        AssertEquals("[1/m,  /w, +/w,  /w, 2/m,  /w, =/w,  /w, 3/m, ;/w,  /w, a/nx, +/w, b/nx, =/w,  /w, a/nx,  /w, +/w,  /w, b/nx]", HanLP.segment(text).ToString());
    }
    [TestMethod]

    public void TestTab() 
    {
        AssertTrue(TextUtility.charType('\t') == CharType.CT_DELIMITER);
        AssertTrue(TextUtility.charType('\r') == CharType.CT_DELIMITER);
        AssertTrue(TextUtility.charType('\0') == CharType.CT_DELIMITER);

//        Console.WriteLine(HanLP.segment("\t"));
    }
}