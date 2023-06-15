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
                System.err.println("Fail to read binary model " + binarymodel);
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
                string val = new DecimalFormat("0.0000000000000000").format(alpha_[k]);
                osw.write(val + "\n");
            }
            osw.close();
            return true;
        }
        catch (Exception e)
        {
            System.err.println(binarymodel + " does not exist");
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
                    System._out.println("Found binary model " + binFileName);
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
            while ((line = br.readLine()) != null && line.length() > 0)
            {
                string[] content = line.trim().split(" ");
                dat.put(content[1], int.valueOf(content[0]));
            }
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

            if (cacheBinModel)
            {
                System._out.println("Writing binary model to " + binFileName);
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
            System.err.println("Error reading " + filename1);
            return false;
        }
        return true;
    }

    public static void main(string[] args)
    {
        if (args.length < 2)
        {
            return;
        }
        else
        {
            DecoderFeatureIndex featureIndex = new DecoderFeatureIndex();
            if (!featureIndex.convert(args[0], args[1]))
            {
                System.err.println("fail to convert binary model to text model");
            }
        }
    }
}
