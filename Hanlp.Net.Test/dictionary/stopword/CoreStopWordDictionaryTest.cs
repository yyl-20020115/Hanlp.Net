using com.hankcs.hanlp.collection.MDAG;

namespace com.hankcs.hanlp.dictionary.stopword;


[TestClass]
public class CoreStopWordDictionaryTest : TestCase
{
    [TestMethod]
    public void TestContains()
    {
        AssertTrue(CoreStopWordDictionary.Contains("这就是说"));
    }
    [TestMethod]
    public void TestContainsSomeWords()
    {
        AssertEquals(true, CoreStopWordDictionary.Contains("可以"));
    }
    [TestMethod]

    public void TestMDAG()
    {
        List<String> wordList = new ();
        wordList.Add("zoo");
        wordList.Add("hello");
        wordList.Add("world");
        MDAGSet set = new MDAGSet(wordList);
        set.Add("bee");
        AssertEquals(true, set.Contains("bee"));
        set.Remove("bee");
        AssertEquals(false, set.Contains("bee"));
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