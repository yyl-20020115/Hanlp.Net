namespace com.hankcs.hanlp.model.crf.crfpp;



/**
 * @author zhifac
 */
public class DecoderFeatureIndex : FeatureIndex
{
    private MutableDoubleArrayTrieInteger dat;

    public DecoderFeatureIndex()
    {
        dat = new MutableDoubleArrayTrieInteger();
    }

    public int getID(string key)
    {
        return dat.get(key);
    }

    public bool open(InputStream stream)
    {
        try
        {
            ObjectInputStream ois = new ObjectInputStream(stream);
            int version = (int) ois.readObject();
            costFactor_ = (Double) ois.readObject();
            maxid_ = (int) ois.readObject();
            xsize_ = (int) ois.readObject();
            y_ = (List<string>) ois.readObject();
            unigramTempls_ = (List<string>) ois.readObject();
            bigramTempls_ = (List<string>) ois.readObject();
            dat = (MutableDoubleArrayTrieInteger) ois.readObject();
            alpha_ = (double[]) ois.readObject();
            ois.close();
            return true;
        }
        catch (Exception e)
        {
            e.printStackTrace();
            return false;
        }
    }

    public bool convert(string binarymodel, string textmodel)
    {
        try
        {
            if (!open(IOUtil.newInputStream(binarymodel)))
            {
                Console.Error.WriteLine("Fail to read binary model " + binarymodel);
                return false;
            }
            OutputStreamWriter osw = new OutputStreamWriter(IOUtil.newOutputStream(textmodel), "UTF-8");
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

            for (MutableDoubleArrayTrieInteger.KeyValuePair pair : dat)
            {
                osw.write(pair.getValue() + " " + pair.getKey() + "\n");
            }

            osw.write("\n");

            for (int k = 0; k < maxid_; k++)
            {
                string val = new DecimalFormat("0.0000000000000000").Format(alpha_[k]);
                osw.write(val + "\n");
            }
            osw.close();
            return true;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(binarymodel + " does not exist");
            return false;
        }
    }

    public bool openTextModel(string filename1, bool cacheBinModel)
    {
        InputStreamReader isr = null;
        try
        {
            string binFileName = filename1 + ".bin";
            try
            {
                if (open(IOUtil.newInputStream(binFileName)))
                {
                    Console.WriteLine("Found binary model " + binFileName);
                    return true;
                }
            }
            catch (IOException e)
            {
                // load text model
            }

            isr = new InputStreamReader(IOUtil.newInputStream(filename1), "UTF-8");
            BufferedReader br = new BufferedReader(isr);
            string line;

            int version = int.valueOf(br.readLine().substring("version: ".Length));
            costFactor_ = Double.valueOf(br.readLine().substring("cost-factor: ".Length));
            maxid_ = int.valueOf(br.readLine().substring("maxid: ".Length));
            xsize_ = int.valueOf(br.readLine().substring("xsize: ".Length));
            Console.WriteLine("Done reading meta-info");
            br.readLine();

            while ((line = br.readLine()) != null && line.Length > 0)
            {
                y_.Add(line);
            }
            Console.WriteLine("Done reading labels");
            while ((line = br.readLine()) != null && line.Length > 0)
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
            while ((line = br.readLine()) != null && line.Length > 0)
            {
                string[] content = line.trim().Split(" ");
                dat.put(content[1], int.valueOf(content[0]));
            }
            List<Double> alpha = new ArrayList<Double>();
            while ((line = br.readLine()) != null && line.Length > 0)
            {
                alpha.Add(Double.valueOf(line));
            }
            Console.WriteLine("Done reading weights");
            alpha_ = new double[alpha.size()];
            for (int i = 0; i < alpha.size(); i++)
            {
                alpha_[i] = alpha.get(i);
            }
            br.close();

            if (cacheBinModel)
            {
                Console.WriteLine("Writing binary model to " + binFileName);
                ObjectOutputStream oos = new ObjectOutputStream(IOUtil.newOutputStream(binFileName));
                oos.writeObject(version);
                oos.writeObject(costFactor_);
                oos.writeObject(maxid_);
                oos.writeObject(xsize_);
                oos.writeObject(y_);
                oos.writeObject(unigramTempls_);
                oos.writeObject(bigramTempls_);
                oos.writeObject(dat);
                oos.writeObject(alpha_);
                oos.close();
            }
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
            Console.Error.WriteLine("Error reading " + filename1);
            return false;
        }
        return true;
    }

    public static void main(string[] args)
    {
        if (args.Length < 2)
        {
            return;
        }
        else
        {
            DecoderFeatureIndex featureIndex = new DecoderFeatureIndex();
            if (!featureIndex.convert(args[0], args[1]))
            {
                Console.Error.WriteLine("fail to convert binary model to text model");
            }
        }
    }
}
