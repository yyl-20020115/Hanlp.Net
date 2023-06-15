namespace com.hankcs.hanlp.model.crf.crfpp;



/**
 * @author zhifac
 */
public class EncoderFeatureIndex : FeatureIndex
{
    private MutableDoubleArrayTrieInteger dic_;
    private IntArrayList frequency;
    private int bId = int.MAX_VALUE;

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
            dic_.put(key, maxid_);
            frequency.Append(1);
            int n = maxid_;
            if (key.charAt(0) == 'U')
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
            BufferedReader br = new BufferedReader(isr);
            string line;
            while ((line = br.readLine()) != null)
            {
                if (line.length() == 0 || line.charAt(0) == ' ' || line.charAt(0) == '#')
                {
                    continue;
                }
                else if (line.charAt(0) == 'U')
                {
                    unigramTempls_.add(line.trim());
                }
                else if (line.charAt(0) == 'B')
                {
                    bigramTempls_.add(line.trim());
                }
                else
                {
                    Console.Error.WriteLine("unknown type: " + line);
                }
            }
            br.close();
            templs_ = makeTempls(unigramTempls_, bigramTempls_);
        }
        catch (Exception e)
        {
            if (isr != null)
            {
                try
                {
                    isr.close();
                }
                catch (Exception e2)
                {
                }
            }
            e.printStackTrace();
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
        y_.clear();
        try
        {
            isr = new InputStreamReader(IOUtil.newInputStream(filename), "UTF-8");
            BufferedReader br = new BufferedReader(isr);
            string line;
            while ((line = br.readLine()) != null)
            {
                if (line.length() == 0)
                {
                    continue;
                }
                char firstChar = line.charAt(0);
                if (firstChar == '\0' || firstChar == ' ' || firstChar == '\t')
                {
                    continue;
                }
                string[] cols = line.Split("[\t ]", -1);
                if (max_size == 0)
                {
                    max_size = cols.length;
                }
                if (max_size != cols.length)
                {
                    string msg = "inconsistent column size: " + max_size +
                        " " + cols.length + " " + filename;
                    throw new RuntimeException(msg);
                }
                xsize_ = cols.length - 1;
                if (y_.indexOf(cols[max_size - 1]) == -1)
                {
                    y_.add(cols[max_size - 1]);
                }
            }
            Collections.sort(y_);
            br.close();
        }
        catch (Exception e)
        {
            if (isr != null)
            {
                try
                {
                    isr.close();
                }
                catch (Exception e2)
                {
                }
            }
            e.printStackTrace();
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
                xsize_ = Math.min(xsize_, max_xsize_);
            }
            oos.writeObject(xsize_);
            oos.writeObject(y_);
            oos.writeObject(unigramTempls_);
            oos.writeObject(bigramTempls_);
            oos.writeObject(dic_);
//            List<string> keyList = new ArrayList<string>(dic_.size());
//            int[] values = new int[dic_.size()];
//            int i = 0;
//            for (MutableDoubleArrayTrieInteger.KeyValuePair pair : dic_)
//            {
//                keyList.add(pair.key());
//                values[i++] = pair.value();
//            }
//            DoubleArray doubleArray = new DoubleArray();
//            doubleArray.build(keyList, values);
//            oos.writeObject(doubleArray);
            oos.writeObject(alpha_);
            oos.close();

            if (textModelFile)
            {
                OutputStreamWriter osw = new OutputStreamWriter(IOUtil.newOutputStream(filename + ".txt"), "UTF-8");
                osw.write("version: " + Encoder.MODEL_VERSION + "\n");
                osw.write("cost-factor: " + costFactor_ + "\n");
                osw.write("maxid: " + maxid_ + "\n");
                osw.write("xsize: " + xsize_ + "\n");
                osw.write("\n");
                for (string y : y_)
                {
                    osw.write(y + "\n");
                }
                osw.write("\n");
                for (string utempl : unigramTempls_)
                {
                    osw.write(utempl + "\n");
                }
                for (string bitempl : bigramTempls_)
                {
                    osw.write(bitempl + "\n");
                }
                osw.write("\n");
                for (MutableDoubleArrayTrieInteger.KeyValuePair pair : dic_)
                {
                    osw.write(pair.getValue() + " " + pair.getKey() + "\n");
                }
                osw.write("\n");

                for (int k = 0; k < maxid_; k++)
                {
                    string val = new DecimalFormat("0.0000000000000000").format(alpha_[k]);
                    osw.write(val + "\n");
                }
                osw.close();
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
            Console.Error.WriteLine("Error saving model to " + filename);
            return false;
        }
        return true;
    }

    public void clear()
    {

    }

    public void shrink(int freq, List<TaggerImpl> taggers)
    {
        if (freq <= 1)
        {
            return;
        }
        int newMaxId = 0;
        Dictionary<int, int> old2new = new TreeMap<int, int>();
        List<string> deletedKeys = new ArrayList<string>(dic_.size() / 8);
        List<KeyValuePair<string, int>> l = new LinkedList<KeyValuePair<string, int>>(dic_.entrySet());
        // update dictionary in key order, to make result compatible with crfpp
        for (MutableDoubleArrayTrieInteger.KeyValuePair pair : dic_)
        {
            string key = pair.key();
            int id = pair.value();
            int cid = continuousId(id);
            int f = frequency.get(cid);
            if (f >= freq)
            {
                old2new.put(id, newMaxId);
                pair.setValue(newMaxId);
                newMaxId += (key.charAt(0) == 'U' ? y_.size() : y_.size() * y_.size());
            }
            else
            {
                deletedKeys.add(key);
            }
        }
        for (string key : deletedKeys)
        {
            dic_.remove(key);
        }

        for (TaggerImpl tagger : taggers)
        {
            List<List<int>> featureCache = tagger.getFeatureCache_();
            for (int k = 0; k < featureCache.size(); k++)
            {
                List<int> featureCacheItem = featureCache.get(k);
                List<int> newCache = new ArrayList<int>();
                for (int it : featureCacheItem)
                {
                    if (it == -1)
                    {
                        continue;
                    }
                    int nid = old2new.get(it);
                    if (nid != null)
                    {
                        newCache.add(nid);
                    }
                }
                newCache.add(-1);
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
            BufferedReader br = new BufferedReader(isr);
            string line;

            int version = int.valueOf(br.readLine().substring("version: ".length()));
            costFactor_ = Double.valueOf(br.readLine().substring("cost-factor: ".length()));
            maxid_ = int.valueOf(br.readLine().substring("maxid: ".length()));
            xsize_ = int.valueOf(br.readLine().substring("xsize: ".length()));
            System._out.println("Done reading meta-info");
            br.readLine();

            while ((line = br.readLine()) != null && line.length() > 0)
            {
                y_.add(line);
            }
            System._out.println("Done reading labels");
            while ((line = br.readLine()) != null && line.length() > 0)
            {
                if (line.startsWith("U"))
                {
                    unigramTempls_.add(line);
                }
                else if (line.startsWith("B"))
                {
                    bigramTempls_.add(line);
                }
            }
            System._out.println("Done reading templates");
            dic_ = new MutableDoubleArrayTrieInteger();
            while ((line = br.readLine()) != null && line.length() > 0)
            {
                string[] content = line.trim().Split(" ");
                if (content.length != 2)
                {
                    Console.Error.WriteLine("feature indices format error");
                    return false;
                }
                dic_.put(content[1], int.valueOf(content[0]));
            }
            System._out.println("Done reading feature indices");
            List<Double> alpha = new ArrayList<Double>();
            while ((line = br.readLine()) != null && line.length() > 0)
            {
                alpha.add(Double.valueOf(line));
            }
            System._out.println("Done reading weights");
            alpha_ = new double[alpha.size()];
            for (int i = 0; i < alpha.size(); i++)
            {
                alpha_[i] = alpha.get(i);
            }
            br.close();
            System._out.println("Writing binary model to " + binarymodel);
            return save(binarymodel, false);
        }
        catch (Exception e)
        {
            e.printStackTrace();
            return false;
        }
    }

    public static void main(string[] args)
    {
        if (args.length < 2)
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
