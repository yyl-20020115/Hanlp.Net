namespace com.hankcs.hanlp.collection.trie.datrie;


[TestClass]
public class Utf8CharacterMappingTest : TestCase
{
    [TestMethod]

    public void TestToIdList() 
    {
        Utf8CharacterMapping ucm = new Utf8CharacterMapping();
        String s = "汉字\uD801\uDC00\uD801\uDC00ab\uD801\uDC00\uD801\uDC00cd";
        int[] bytes1 = ucm.toIdList(s);
        Console.WriteLine("UTF-8: " + bytes1.Length);
        {
            int charCount = 1;
            int start = 0;
            for (int i = 0; i < s.Length(); i += charCount)
            {
                int codePoint = s.codePointAt(i);
                charCount = Character.charCount(codePoint);

                int[] arr = ucm.toIdList(codePoint);
                for (int j = 0; j < arr.Length; j++, start++)
                {
                    if (bytes1[start] != arr[j])
                    {
                        Console.WriteLine("error: " + start + "," + j);
                        System.exit(-1);
                    }
                }
            }
            if (start != bytes1.Length)
            {
                Console.WriteLine("error: " + start + "," + bytes1.Length);
            }
        }
    }
}