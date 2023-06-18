namespace com.hankcs.hanlp.algorithm;


[TestClass]

public class MaxHeapTest : TestCase
{
    public class Comparer : IComparer<int>
    {
        //@Override
        public int Compare(int o1, int o2)
        {
            return o1.CompareTo(o2);
        }
    }
    static MaxHeap<int> heap = new (5,new Comparer());
    [TestInitialize]
    //@Override
    public override void SetUp() 
    {
        heap.Add(1);
        heap.Add(3);
        heap.Add(5);
        heap.Add(7);
        heap.Add(9);
        heap.Add(8);
        heap.Add(6);
        heap.Add(4);
        heap.Add(2);
        heap.Add(0);
    }
    [TestMethod]
    public void TestToList()
    {
        AssertEquals("[9, 8, 7, 6, 5]","["+string.Join(", ", heap.ToList())+"]");
    }
}