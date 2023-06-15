namespace com.hankcs.hanlp.model.crf.crfpp;


/**
 * @author zhifac
 */
public abstract class FeatureIndex
{
    public static string[] BOS = {"_B-1", "_B-2", "_B-3", "_B-4", "_B-5", "_B-6", "_B-7", "_B-8"};
    public static string[] EOS = {"_B+1", "_B+2", "_B+3", "_B+4", "_B+5", "_B+6", "_B+7", "_B+8"};
    protected int maxid_;
    protected double[] alpha_;
    protected float[] alphaFloat_;
    protected double costFactor_;
    protected int xsize_;
    protected bool checkMaxXsize_;
    protected int max_xsize_;
    protected int threadNum_;
    protected List<string> unigramTempls_;
    protected List<string> bigramTempls_;
    protected string templs_;
    protected List<string> y_;
    protected List<List<Path>> pathList_;
    protected List<List<Node>> nodeList_;

    public FeatureIndex()
    {
        maxid_ = 0;
        alpha_ = null;
        alphaFloat_ = null;
        costFactor_ = 1.0;
        xsize_ = 0;
        checkMaxXsize_ = false;
        max_xsize_ = 0;
        threadNum_ = 1;
        unigramTempls_ = new ArrayList<string>();
        bigramTempls_ = new ArrayList<string>();
        y_ = new ArrayList<string>();
    }

    protected abstract int getID(string s);

    /**
     * 计算状态特征函数的代价
     *
     * @param node
     */
    public void calcCost(Node node)
    {
        node.cost = 0.0;
        if (alphaFloat_ != null)
        {
            float c = 0.0f;
            for (int i = 0; node.fVector.get(i) != -1; i++)
            {
                c += alphaFloat_[node.fVector.get(i) + node.y];
            }
            node.cost = costFactor_ * c;
        }
        else
        {
            double c = 0.0;
            for (int i = 0; node.fVector.get(i) != -1; i++)
            {
                c += alpha_[node.fVector.get(i) + node.y];
            }
            node.cost = costFactor_ * c;
        }
    }

    /**
     * 计算转移特征函数的代价
     *
     * @param path 边
     */
    public void calcCost(Path path)
    {
        path.cost = 0.0;
        if (alphaFloat_ != null)
        {
            float c = 0.0f;
            for (int i = 0; path.fvector.get(i) != -1; i++)
            {
                c += alphaFloat_[path.fvector.get(i) + path.lnode.y * y_.size() + path.rnode.y];
            }
            path.cost = costFactor_ * c;
        }
        else
        {
            double c = 0.0;
            for (int i = 0; path.fvector.get(i) != -1; i++)
            {
                c += alpha_[path.fvector.get(i) + path.lnode.y * y_.size() + path.rnode.y];
            }
            path.cost = costFactor_ * c;
        }
    }

    public string makeTempls(List<string> unigramTempls, List<string> bigramTempls)
    {
        StringBuilder sb = new StringBuilder();
        for (string temp : unigramTempls)
        {
            sb.Append(temp).Append("\n");
        }
        for (string temp : bigramTempls)
        {
            sb.Append(temp).Append("\n");
        }
        return sb.toString();
    }

    public string getTemplate()
    {
        return templs_;
    }

    public string getIndex(string[] idxStr, int cur, TaggerImpl tagger)
    {
        int row = int.valueOf(idxStr[0]);
        int col = int.valueOf(idxStr[1]);
        int pos = row + cur;
        if (row < -EOS.length || row > EOS.length || col < 0 || col >= tagger.xsize())
        {
            return null;
        }

        //TODO(taku): very dirty workaround
        if (checkMaxXsize_)
        {
            max_xsize_ = Math.max(max_xsize_, col + 1);
        }
        if (pos < 0)
        {
            return BOS[-pos - 1];
        }
        else if (pos >= tagger.size())
        {
            return EOS[pos - tagger.size()];
        }
        else
        {
            return tagger.x(pos, col);
        }
    }

    public string applyRule(string str, int cur, TaggerImpl tagger)
    {
        StringBuilder sb = new StringBuilder();
        for (string tmp : str.split("%x", -1))
        {
            if (tmp.startsWith("U") || tmp.startsWith("B"))
            {
                sb.Append(tmp);
            }
            else if (tmp.length() > 0)
            {
                string[] tuple = tmp.split("]");
                string[] idx = tuple[0].replace("[", "").split(",");
                string r = getIndex(idx, cur, tagger);
                if (r != null)
                {
                    sb.Append(r);
                }
                if (tuple.length > 1)
                {
                    sb.Append(tuple[1]);
                }
            }
        }

        return sb.toString();
    }

    private bool buildFeatureFromTempl(List<int> feature, List<string> templs, int curPos, TaggerImpl tagger)
    {
        for (string tmpl : templs)
        {
            string featureID = applyRule(tmpl, curPos, tagger);
            if (featureID == null || featureID.length() == 0)
            {
                Console.Error.WriteLine("format error");
                return false;
            }
            int id = getID(featureID);
            if (id != -1)
            {
                feature.add(id);
            }
        }
        return true;
    }

    public bool buildFeatures(TaggerImpl tagger)
    {
        List<int> feature = new ArrayList<int>();
        List<List<int>> featureCache = tagger.getFeatureCache_();
        tagger.setFeature_id_(featureCache.size());

        for (int cur = 0; cur < tagger.size(); cur++)
        {
            if (!buildFeatureFromTempl(feature, unigramTempls_, cur, tagger))
            {
                return false;
            }
            feature.add(-1);
            featureCache.add(feature);
            feature = new ArrayList<int>();
        }
        for (int cur = 1; cur < tagger.size(); cur++)
        {
            if (!buildFeatureFromTempl(feature, bigramTempls_, cur, tagger))
            {
                return false;
            }
            feature.add(-1);
            featureCache.add(feature);
            feature = new ArrayList<int>();
        }
        return true;
    }

    public void rebuildFeatures(TaggerImpl tagger)
    {
        int fid = tagger.getFeature_id_();
        List<List<int>> featureCache = tagger.getFeatureCache_();
        for (int cur = 0; cur < tagger.size(); cur++)
        {
            List<int> f = featureCache.get(fid++);
            for (int i = 0; i < y_.size(); i++)
            {
                Node n = new Node();
                n.clear();
                n.x = cur;
                n.y = i;
                n.fVector = f;
                tagger.set_node(n, cur, i);
            }
        }
        for (int cur = 1; cur < tagger.size(); cur++)
        {
            List<int> f = featureCache.get(fid++);
            for (int j = 0; j < y_.size(); j++)
            {
                for (int i = 0; i < y_.size(); i++)
                {
                    Path p = new Path();
                    p.clear();
                    p.add(tagger.node(cur - 1, j), tagger.node(cur, i));
                    p.fvector = f;
                }
            }
        }
    }

    public bool open(string file)
    {
        return true;
    }

    public bool open(InputStream stream)
    {
        return true;
    }

    public void clear()
    {

    }

    public int size()
    {
        return getMaxid_();
    }

    public int ysize()
    {
        return y_.size();
    }

    public int getMaxid_()
    {
        return maxid_;
    }

    public void setMaxid_(int maxid_)
    {
        this.maxid_ = maxid_;
    }

    public double[] getAlpha_()
    {
        return alpha_;
    }

    public void setAlpha_(double[] alpha_)
    {
        this.alpha_ = alpha_;
    }

    public float[] getAlphaFloat_()
    {
        return alphaFloat_;
    }

    public void setAlphaFloat_(float[] alphaFloat_)
    {
        this.alphaFloat_ = alphaFloat_;
    }

    public double getCostFactor_()
    {
        return costFactor_;
    }

    public void setCostFactor_(double costFactor_)
    {
        this.costFactor_ = costFactor_;
    }

    public int getXsize_()
    {
        return xsize_;
    }

    public void setXsize_(int xsize_)
    {
        this.xsize_ = xsize_;
    }

    public int getMax_xsize_()
    {
        return max_xsize_;
    }

    public void setMax_xsize_(int max_xsize_)
    {
        this.max_xsize_ = max_xsize_;
    }

    public int getThreadNum_()
    {
        return threadNum_;
    }

    public void setThreadNum_(int threadNum_)
    {
        this.threadNum_ = threadNum_;
    }

    public List<string> getUnigramTempls_()
    {
        return unigramTempls_;
    }

    public void setUnigramTempls_(List<string> unigramTempls_)
    {
        this.unigramTempls_ = unigramTempls_;
    }

    public List<string> getBigramTempls_()
    {
        return bigramTempls_;
    }

    public void setBigramTempls_(List<string> bigramTempls_)
    {
        this.bigramTempls_ = bigramTempls_;
    }

    public List<string> getY_()
    {
        return y_;
    }

    public void setY_(List<string> y_)
    {
        this.y_ = y_;
    }

    public List<List<Path>> getPathList_()
    {
        return pathList_;
    }

    public void setPathList_(List<List<Path>> pathList_)
    {
        this.pathList_ = pathList_;
    }

    public List<List<Node>> getNodeList_()
    {
        return nodeList_;
    }

    public void setNodeList_(List<List<Node>> nodeList_)
    {
        this.nodeList_ = nodeList_;
    }
}
