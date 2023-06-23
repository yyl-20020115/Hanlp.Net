using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;

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

    public bool open(Stream stream)
    {
        try
        {
            Stream ois = new Stream(stream);
            int version = (int) ois.readObject();
            costFactor_ = (Double) ois.readObject();
            maxid_ = (int) ois.readObject();
            xsize_ = (int) ois.readObject();
            y_ = (List<string>) ois.readObject();
            unigramTempls_ = (List<string>) ois.readObject();
            bigramTempls_ = (List<string>) ois.readObject();
            dat = (MutableDoubleArrayTrieInteger) ois.readObject();
            alpha_ = (double[]) ois.readObject();
            ois.Close();
            return true;
        }
        catch (Exception e)
        {
            //e.printStackTrace();
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
            StreamWriter osw = new StreamWriter(IOUtil.newOutputStream(textmodel), "UTF-8");
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

            foreach (var pair in dat)
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
            while ((line = br.ReadLine()) != null && line.Length > 0)
            {
                string[] content = line.Trim().Split(" ");
                dat.Add(content[1], int.valueOf(content[0]));
            }
            List<Double> alpha = new ();
            while ((line = br.ReadLine()) != null && line.Length > 0)
            {
                alpha.Add(Double.valueOf(line));
            }
            Console.WriteLine("Done reading weights");
            alpha_ = new double[alpha.Count];
            for (int i = 0; i < alpha.Count; i++)
            {
                alpha_[i] = alpha[i];
            }
            br.Close();

            if (cacheBinModel)
            {
                Console.WriteLine("Writing binary model to " + binFileName);
                Stream oos = new Stream(IOUtil.newOutputStream(binFileName));
                oos.writeObject(version);
                oos.writeObject(costFactor_);
                oos.writeObject(maxid_);
                oos.writeObject(xsize_);
                oos.writeObject(y_);
                oos.writeObject(unigramTempls_);
                oos.writeObject(bigramTempls_);
                oos.writeObject(dat);
                oos.writeObject(alpha_);
                oos.Close();
            }
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
