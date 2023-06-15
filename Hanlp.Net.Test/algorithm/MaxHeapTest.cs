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
    static MaxHeap<int> heap = new MaxHeap<int>(5,new Comparer() );
    [TestInitialize]
    //@Override
    public void setUp() 
    {
        heap.add(1);
        heap.add(3);
        heap.add(5);
        heap.add(7);
        heap.add(9);
        heap.add(8);
        heap.add(6);
        heap.add(4);
        heap.add(2);
        heap.add(0);
    }
    [TestMethod]
    public void testToList() 
    {
        assertEquals("[9, 8, 7, 6, 5]", heap.toList().ToString());
    }
}