namespace com.hankcs.hanlp.model.crf.crfpp;


/**
 * 边
 *
 * @author zhifac
 */
public class Path
{
    public Node rnode;
    public Node lnode;
    public List<int> fvector;
    public double cost;

    public Path()
    {
        Clear();
    }

    public void Clear()
    {
        rnode = lnode = null;
        fvector = null;
        cost = 0.0;
    }

    /**
     * 计算边的期望
     *
     * @param expected 输出期望
     * @param Z        规范化因子
     * @param size     标签个数
     */
    public void calcExpectation(double[] expected, double Z, int size)
    {
        double c = Math.Exp(lnode.alpha + cost + rnode.beta - Z);
        for (int i = 0; fvector[i] != -1; i++)
        {
            int idx = fvector[i] + lnode.y * size + rnode.y;
            expected[idx] += c;
        }
    }

    public void Add(Node _lnode, Node _rnode)
    {
        lnode = _lnode;
        rnode = _rnode;
        lnode.rpath.Add(this);
        rnode.lpath.Add(this);
    }
}
