namespace com.hankcs.hanlp.corpus.document.sentence.word;


[TestClass]
public class WordTest : TestCase
{
    [TestMethod]
    public void testCreate() 
    {
        assertEquals("人民网/nz", Word.create("人民网/nz").ToString());
        assertEquals("[纽约/nsf 时报/n]/nz", CompoundWord.create("[纽约/nsf 时报/n]/nz").ToString());
        assertEquals("[中央/n 人民/n 广播/vn 电台/n]/nt", CompoundWord.create("[中央/n 人民/n 广播/vn 电台/n]nt").ToString());
    }
    [TestMethod]
    public void testSpace() 
    {
        CompoundWord compoundWord = CompoundWord.create("[9/m  11/m 后/f]/mq");
        assertEquals(3, compoundWord.innerList.size());
    }
}