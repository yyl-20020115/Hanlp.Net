using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.model.crf.crfpp;



/**
 * @author zhifac
 */
public class EncoderFeatureIndex : FeatureIndex
{
    private MutableDoubleArrayTrieInteger dic_;
    private IntArrayList frequency;
    private int bId = int.MaxValue;

    public EncoderFeatureIndex(int n)
    {
        threadNum_ = n;
        dic_ = new MutableDoubleArrayTrieInteger();
        frequency = new IntArrayList();
    }

    public int getID(string key)
    {
        int k = dic_.get(key);
        if (k == -1)
        {
            dic_.Add(key, maxid_);
            frequency.Append(1);
            int n = maxid_;
            if (key[0] == 'U')
            {
                maxid_ += y_.size();
            }
            else
            {
                bId = n;
                maxid_ += y_.size() * y_.size();
            }
            return n;
        }
        else
        {
            int cid = continuousId(k);
            int oldVal = frequency.get(cid);
            frequency.set(cid, oldVal + 1);
            return k;
        }
    }

    private int continuousId(int id)
    {
        if (id <= bId)
        {
            return id / y_.size();
        }
        else
        {
            return id / y_.size() - y_.size() + 1;
        }
    }

    /**
     * 读取特征模板文件
     *
     * @param filename
     * @return
     */
    private bool openTemplate(string filename)
    {
        InputStreamReader isr = null;
        try
        {
            isr = new InputStreamReader(IOUtil.newInputStream(filename), "UTF-8");
            TextReader br = new TextReader(isr);
            string line;
            while ((line = br.ReadLine()) != null)
            {
                if (line.Length == 0 || line[0] == ' ' || line[0] == '#')
                {
                    continue;
                }
                else if (line[0] == 'U')
                {
                    unigramTempls_.Add(line.Trim());
                }
                else if (line[0] == 'B')
                {
                    bigramTempls_.Add(line.Trim());
                }
                else
                {
                    Console.Error.WriteLine("unknown type: " + line);
                }
            }
            br.Close();
            templs_ = makeTempls(unigramTempls_, bigramTempls_);
        }
        catch (Exception e)
        {
            if (isr != null)
            {
                try
                {
                    isr.Close();
                }
                catch (Exception e2)
                {
                }
            }
            //e.printStackTrace();
            Console.Error.WriteLine("Error reading " + filename);
            return false;
        }
        return true;
    }

    /**
     * 读取训练文件中的标注集
     *
     * @param filename
     * @return
     */
    private bool openTagSet(string filename)
    {
        int max_size = 0;
        InputStreamReader isr = null;
        y_.Clear();
        try
        {
            isr = new InputStreamReader(IOUtil.newInputStream(filename), "UTF-8");
            TextReader br = new TextReader(isr);
            string line;
            while ((line = br.ReadLine()) != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }
                char firstChar = line[0];
                if (firstChar == '\0' || firstChar == ' ' || firstChar == '\t')
                {
                    continue;
                }
                string[] cols = line.Split("[\t ]", -1);
                if (max_size == 0)
                {
                    max_size = cols.Length;
                }
                if (max_size != cols.Length)
                {
                    string msg = "inconsistent column size: " + max_size +
                        " " + cols.Length + " " + filename;
                    throw new RuntimeException(msg);
                }
                xsize_ = cols.Length - 1;
                if (y_.IndexOf(cols[max_size - 1]) == -1)
                {
                    y_.Add(cols[max_size - 1]);
                }
            }
            Collections.sort(y_);
            br.Close();
        }
        catch (Exception e)
        {
            if (isr != null)
            {
                try
                {
                    isr.Close();
                }
                catch (Exception e2)
                {
                }
            }
            //e.printStackTrace();
            Console.Error.WriteLine("Error reading " + filename);
            return false;
        }
        return true;
    }

    public bool open(string filename1, string filename2)
    {
        checkMaxXsize_ = true;
        return openTemplate(filename1) && openTagSet(filename2);
    }

    public bool save(string filename, bool textModelFile)
    {
        try
        {
            ObjectOutputStream oos = new ObjectOutputStream(IOUtil.newOutputStream(filename));
            oos.writeObject(Encoder.MODEL_VERSION);
            oos.writeObject(costFactor_);
            oos.writeObject(maxid_);
            if (max_xsize_ > 0)
            {
                xsize_ = Math.Min(xsize_, max_xsize_);
            }
            oos.writeObject(xsize_);
            oos.writeObject(y_);
            oos.writeObject(unigramTempls_);
            oos.writeObject(bigramTempls_);
            oos.writeObject(dic_);
//            List<string> keyList = new (dic_.size());
//            int[] values = new int[dic_.size()];
//            int i = 0;
//            for (MutableDoubleArrayTrieInteger.KeyValuePair pair : dic_)
//            {
//                keyList.Add(pair.key());
//                values[i++] = pair.value();
//            }
//            DoubleArray doubleArray = new DoubleArray();
//            doubleArray.build(keyList, values);
//            oos.writeObject(doubleArray);
            oos.writeObject(alpha_);
            oos.Close();

            if (textModelFile)
            {
                StreamWriter osw = new StreamWriter(IOUtil.newOutputStream(filename + ".txt"), "UTF-8");
                osw.Write("version: " + Encoder.MODEL_VERSION + "\n");
                osw.Write("cost-factor: " + costFactor_ + "\n");
                osw.Write("maxid: " + maxid_ + "\n");
                osw.Write("xsize: " + xsize_ + "\n");
                osw.Write("\n");
                foreach (string y in y_)
                {
                    osw.Write(y + "\n");
                }
                osw.Write("\n");
                foreach (string utempl in unigramTempls_)
                {
                    osw.Write(utempl + "\n");
                }
                foreach (string bitempl in bigramTempls_)
                {
                    osw.Write(bitempl + "\n");
                }
                osw.Write("\n");
                foreach (MutableDoubleArrayTrieInteger.KeyValuePair pair in dic_)
                {
                    osw.Write(pair.Value + " " + pair.Key + "\n");
                }
                osw.Write("\n");

                for (int k = 0; k < maxid_; k++)
                {
                    string val = new DecimalFormat("0.0000000000000000").Format(alpha_[k]);
                    osw.Write(val + "\n");
                }
                osw.Close();
            }
        }
        catch (Exception e)
        {
            //e.printStackTrace();
            Console.Error.WriteLine("Error saving model to " + filename);
            return false;
        }
        return true;
    }

    public void Clear()
    {

    }

    public void shrink(int freq, List<TaggerImpl> taggers)
    {
        if (freq <= 1)
        {
            return;
        }
        int newMaxId = 0;
        Dictionary<int, int> old2new = new Dictionary<int, int>();
        List<string> deletedKeys = new (dic_.size() / 8);
        List<KeyValuePair<string, int>> l = new LinkedList<KeyValuePair<string, int>>(dic_.entrySet());
        // update dictionary in key order, to make result compatible with crfpp
        foreach(MutableDoubleArrayTrieInteger.KeyValuePair pair in dic_)
        {
            string key = pair.key();
            int id = pair.value();
            int cid = continuousId(id);
            int f = frequency.get(cid);
            if (f >= freq)
            {
                old2new.Add(id, newMaxId);
                pair.setValue(newMaxId);
                newMaxId += (key[0] == 'U' ? y_.size() : y_.size() * y_.size());
            }
            else
            {
                deletedKeys.Add(key);
            }
        }
        foreach (string key in deletedKeys)
        {
            dic_.Remove(key);
        }

        foreach (TaggerImpl tagger in taggers)
        {
            List<List<int>> featureCache = tagger.getFeatureCache_();
            for (int k = 0; k < featureCache.size(); k++)
            {
                List<int> featureCacheItem = featureCache.get(k);
                List<int> newCache = new ();
                foreach (int it in featureCacheItem)
                {
                    if (it == -1)
                    {
                        continue;
                    }
                    int nid = old2new.get(it);
                    if (nid != null)
                    {
                        newCache.Add(nid);
                    }
                }
                newCache.Add(-1);
                featureCache.set(k, newCache);
            }
        }
        maxid_ = newMaxId;
    }

    public bool convert(string textmodel, string binarymodel)
    {
        try
        {
            InputStreamReader isr = new InputStreamReader(IOUtil.newInputStream(textmodel), "UTF-8");
            TextReader br = new TextReader(isr);
            string line;

            int version = int.valueOf(br.ReadLine().substring("version: ".Length));
            costFactor_ = Double.valueOf(br.ReadLine().substring("cost-factor: ".Length));
            maxid_ = int.valueOf(br.ReadLine().substring("maxid: ".Length));
            xsize_ = int.valueOf(br.ReadLine().substring("xsize: ".Length));
            Console.WriteLine("Done reading meta-info");
            br.ReadLine();

            while ((line = br.ReadLine()) != null && line.Length > 0)
            {
                y_.Add(line);
            }
            Console.WriteLine("Done reading labels");
            while ((line = br.ReadLine()) != null && line.Length > 0)
            {
                if (line.StartsWith("U"))
                {
                    unigramTempls_.Add(line);
                }
                else if (line.StartsWith("B"))
                {
                    bigramTempls_.Add(line);
                }
            }
            Console.WriteLine("Done reading templates");
            dic_ = new MutableDoubleArrayTrieInteger();
            while ((line = br.ReadLine()) != null && line.Length > 0)
            {
                string[] content = line.Trim().Split(" ");
                if (content.Length != 2)
                {
                    Console.Error.WriteLine("feature indices Format error");
                    return false;
                }
                dic_.Add(content[1], int.valueOf(content[0]));
            }
            Console.WriteLine("Done reading feature indices");
            List<Double> alpha = new ArrayList<Double>();
            while ((line = br.ReadLine()) != null && line.Length > 0)
            {
                alpha.Add(Double.valueOf(line));
            }
            Console.WriteLine("Done reading weights");
            alpha_ = new double[alpha.size()];
            for (int i = 0; i < alpha.size(); i++)
            {
                alpha_[i] = alpha.get(i);
            }
            br.Close();
            Console.WriteLine("Writing binary model to " + binarymodel);
            return save(binarymodel, false);
        }
        catch (Exception e)
        {
            //e.printStackTrace();
            return false;
        }
    }

    public static void main(string[] args)
    {
        if (args.Length < 2)
        {
            return;
        }
        else
        {
            EncoderFeatureIndex featureIndex = new EncoderFeatureIndex(1);
            if (!featureIndex.convert(args[0], args[1]))
            {
                Console.Error.WriteLine("Fail to convert text model");
            }
        }
    }
}
