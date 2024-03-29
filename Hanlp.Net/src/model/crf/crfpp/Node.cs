namespace com.hankcs.hanlp.model.crf.crfpp;


/**
 * 图模型中的节点
 *
 * @author zhifac
 */
public class Node
{
    public int x;
    public int y;
    public double alpha;
    public double beta;
    public double cost;
    public double bestCost;
    public Node prev;
    public List<int> fVector;
    public List<Path> lpath;
    public List<Path> rpath;
    public static double LOG2 = 0.69314718055;
    public static int MINUS_LOG_EPSILON = 50;

    public Node()
    {
        lpath = new ();
        rpath = new ();
        Clear();
        bestCost = 0.0;
        prev = null;
    }

    public static double logsumexp(double x, double y, bool flg)
    {
        if (flg)
        {
            return y;
        }
        double vmin = Math.Min(x, y);
        double vmax = Math.Max(x, y);
        if (vmax > vmin + MINUS_LOG_EPSILON)
        {
            return vmax;
        }
        else
        {
            return vmax + Math.Log(Math.Exp(vmin - vmax) + 1.0);
        }
    }

    public void calcAlpha()
    {
        alpha = 0.0;
        foreach (Path p in lpath)
        {
            alpha = logsumexp(alpha, p.cost + p.lnode.alpha, p == lpath.get(0));
        }
        alpha += cost;
    }

    public void calcBeta()
    {
        beta = 0.0;
        foreach (Path p in rpath)
        {
            beta = logsumexp(beta, p.cost + p.rnode.beta, p == rpath[(0)]);
        }
        beta += cost;
    }

    /**
     * 计算节点期望
     *
     * @param expected 输出期望
     * @param Z        规范化因子
     * @param size     标签个数
     */
    public void calcExpectation(double[] expected, double Z, int size)
    {
        double c = Math.Exp(alpha + beta - cost - Z);
        for (int i = 0; fVector[i] != -1; i++)
        {
            int idx = fVector[i] + y;
            expected[idx] += c;
        }
        foreach (Path p in lpath)
        {
            p.calcExpectation(expected, Z, size);
        }
    }

    public void Clear()
    {
        x = y = 0;
        alpha = beta = cost = 0;
        prev = null;
        fVector = null;
        lpath.Clear();
        rpath.Clear();
    }
}
