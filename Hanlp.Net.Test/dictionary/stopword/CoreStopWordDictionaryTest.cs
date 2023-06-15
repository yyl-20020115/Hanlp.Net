using com.hankcs.hanlp.collection.MDAG;

namespace com.hankcs.hanlp.dictionary.stopword;


[TestClass]
public class CoreStopWordDictionaryTest : TestCase
{
    [TestMethod]
    public void TestContains()
    {
        assertTrue(CoreStopWordDictionary.contains("这就是说"));
    }
    [TestMethod]
    public void TestContainsSomeWords()
    {
        assertEquals(true, CoreStopWordDictionary.contains("可以"));
    }
    [TestMethod]

    public void TestMDAG()
    {
        List<String> wordList = new ();
        wordList.Add("zoo");
        wordList.Add("hello");
        wordList.Add("world");
        MDAGSet set = new MDAGSet(wordList);
        set.add("bee");
        assertEquals(true, set.Contains("bee"));
        set.remove("bee");
        assertEquals(false, set.Contains("bee"));
    }

//    public void testRemoveDuplicateEntries() 
//    {
//        StopWordDictionary dictionary = new StopWordDictionary(new File(HanLP.Config.CoreStopWordDictionaryPath));
//        BufferedWriter bw = IOUtil.newBufferedWriter(HanLP.Config.CoreStopWordDictionaryPath);
//        for (String word : dictionary)
//        {
//            bw.write(word);
//            bw.newLine();
//        }
//        bw.close();
//    }
}