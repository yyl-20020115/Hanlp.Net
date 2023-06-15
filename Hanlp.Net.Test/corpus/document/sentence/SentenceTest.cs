using com.hankcs.hanlp.corpus.document.sentence.word;
using System.Text.RegularExpressions;

namespace com.hankcs.hanlp.corpus.document.sentence;



[TestClass]
public class SentenceTest : TestCase
{
    [TestMethod]
    public void TestFindFirstWordIteratorByLabel()
    {
        Sentence sentence = Sentence.create("[上海/ns 华安/nz 工业/n （/w 集团/n ）/w 公司/n]/nt 董事长/n 谭旭光/nr 和/c 秘书/n 胡花蕊/nr 来到/v [美国/ns 纽约/ns 现代/t 艺术/n 博物馆/n]/ns 参观/v");
        ListIterator<IWord> nt = sentence.findFirstWordIteratorByLabel("nt");
        assertNotNull(nt);
        assertEquals("[上海/ns 华安/nz 工业/n （/w 集团/n ）/w 公司/n]/nt", nt.previous().ToString());
        CompoundWord apple = CompoundWord.create("[苹果/n 公司/n]/nt");
        nt.set(apple);
        AssertEquals(sentence.findFirstWordByLabel("nt"), apple);
        nt.remove();
        AssertEquals("董事长/n 谭旭光/nr 和/c 秘书/n 胡花蕊/nr 来到/v [美国/ns 纽约/ns 现代/t 艺术/n 博物馆/n]/ns 参观/v", sentence.ToString());
        ListIterator<IWord> ns = sentence.findFirstWordIteratorByLabel("ns");
        assertEquals("参观/v", ns.next().ToString());
    }
    [TestMethod]
    public void testToStandoff()
    {
        Sentence sentence = Sentence.create("[上海/ns 华安/nz 工业/n （/w 集团/n ）/w 公司/n]/nt 董事长/n 谭旭光/nr 和/c 秘书/n 胡花蕊/nr 来到/v [美国/ns 纽约/ns 现代/t 艺术/n 博物馆/n]/ns 参观/v");
        Console.WriteLine(sentence.toStandoff(true));
    }
    [TestMethod]
    public void TestText()
    {
        AssertEquals("人民网纽约时报", Sentence.create("人民网/nz [纽约/nsf 时报/n]/nz").text());
    }
    [TestMethod]
    public void TestCreate() 
    {
        String text = "人民网/nz 1月1日/t 讯/ng 据/p 《/w [纽约/nsf 时报/n]/nz 》/w 报道/v ，/w";
        Regex pattern = new Regex("(\\[(.+/[a-z]+)]/[a-z]+)|([^\\s]+/[a-z]+)");
        var matcher = pattern.Matches(text);
        while (matcher.find())
        {
            String param = matcher.group();
            AssertEquals(param, WordFactory.create(param).ToString());
        }
        AssertEquals(text, Sentence.create(text).ToString());
    }
    [TestMethod]
    public void TestCreateNoTag() 
    {
        String text = "商品 和 服务";
        Sentence sentence = Sentence.create(text);
        Console.WriteLine(sentence);
    }
    [TestMethod]
    public void TestMerge() 
    {
        Sentence sentence = Sentence.create("晚９时４０分/TIME ，/v 鸟/n 迷/v 、/v 专家/n 托尼/PERSON 率领/v 的/u [英国/ns “/w 野翅膀/nz ”/w 观/Vg 鸟/n 团/n]/ORGANIZATION 一行/n ２９/INTEGER 人/n ，/v 才/d 吃/v 完/v 晚饭/n 回到/v [金山/nz 宾馆/n]/ORGANIZATION 的/u 大/a 酒吧间/n ，/v 他们/r 一边/d 喝/v 着/u 青岛/LOCATION 啤酒/n ，/v 一边/d 兴致勃勃/i 地/u 回答/v 记者/n 的/u 提问/vn 。/w");
        Console.WriteLine(sentence.mergeCompoundWords());
    }
}