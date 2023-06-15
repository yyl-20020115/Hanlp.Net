namespace com.hankcs.hanlp.corpus.document.sentence.word;


[TestClass]
public class WordTest : TestCase
{
    [TestMethod]
    public void TestCreate() 
    {
        AssertEquals("人民网/nz", Word.create("人民网/nz").ToString());
        AssertEquals("[纽约/nsf 时报/n]/nz", CompoundWord.create("[纽约/nsf 时报/n]/nz").ToString());
        AssertEquals("[中央/n 人民/n 广播/vn 电台/n]/nt", CompoundWord.create("[中央/n 人民/n 广播/vn 电台/n]nt").ToString());
    }
    [TestMethod]
    public void TestSpace() 
    {
        CompoundWord compoundWord = CompoundWord.create("[9/m  11/m 后/f]/mq");
        AssertEquals(3, compoundWord.innerList.Count);
    }
}