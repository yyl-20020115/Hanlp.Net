namespace com.hankcs.hanlp.algorithm;



public class MaxHeapTest : TestCase
{
    static MaxHeap<Integer> heap = new MaxHeap<Integer>(5, new Comparator<Integer>()
    {
        //@Override
        public int compare(Integer o1, Integer o2)
        {
            return o1.compareTo(o2);
        }
    });

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

    public void testToList() 
    {
        assertEquals("[9, 8, 7, 6, 5]", heap.toList().toString());
    }
}