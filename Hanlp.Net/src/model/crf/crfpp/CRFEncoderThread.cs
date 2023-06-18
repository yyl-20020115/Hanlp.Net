namespace com.hankcs.hanlp.model.crf.crfpp;


/**
 * @author zhifac
 */
public class CRFEncoderThread : Callable<int>
{
    public List<TaggerImpl> x;
    public int start_i;
    public int wSize;
    public int threadNum;
    public int zeroone;
    public int err;
    public int size;
    public double obj;
    public double[] expected;

    public CRFEncoderThread(int wsize)
    {
        if (wsize > 0)
        {
            this.wSize = wsize;
            expected = new double[wsize];
            Array.Fill(expected, 0.0);
        }
    }

    public int call()
    {
        obj = 0.0;
        err = zeroone = 0;
        if (expected == null)
        {
            expected = new double[wSize];
        }
        Array.Fill(expected, 0.0);
        for (int i = start_i; i < size; i = i + threadNum)
        {
            obj += x.get(i).gradient(expected);
            int errorNum = x.get(i).eval();
            x.get(i).clearNodes();
            err += errorNum;
            if (errorNum != 0)
            {
                ++zeroone;
            }
        }
        return err;
    }
}
