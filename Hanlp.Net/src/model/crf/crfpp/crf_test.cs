using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.model.crf.crfpp;




/**
 * 对应crf_test
 *
 * @author zhifac
 */
public class crf_test
{
    private class Option
    {
        //@Argument(description = "set FILE for model file", alias = "m", required = true)
        public string model;
        //@Argument(description = "output n-best results", alias = "n")
        public int nbest = 0;
        //@Argument(description = "set INT for verbose level", alias = "v")
        public int verbose = 0;
        //@Argument(description = "set cost factor", alias = "c")
        public Double cost_factor = 1.0;
        //@Argument(description = "output file path", alias = "o")
        public string output;
        //@Argument(description = "show this help and exit", alias = "h")
        public Boolean help = false;
    }

    public static bool run(string[] args)
    {
        Option cmd = new Option();
        List<string> unkownArgs = null;
        try
        {
            unkownArgs = Args.parse(cmd, args, false);
        }
        catch (ArgumentException e)
        {
            Args.usage(cmd);
            return false;
        }
        if (cmd.help)
        {
            Args.usage(cmd);
            return true;
        }
        int nbest = cmd.nbest;
        int vlevel = cmd.verbose;
        double costFactor = cmd.cost_factor;
        string model = cmd.model;
        string outputFile = cmd.output;

        TaggerImpl tagger = new TaggerImpl(TaggerImpl.Mode.TEST);
        try
        {
            InputStream stream = IOUtil.newInputStream(model);
            if (!tagger.open(stream, nbest, vlevel, costFactor))
            {
                Console.Error.WriteLine("open error");
                return false;
            }
            string[] restArgs = unkownArgs.ToArray();
            if (restArgs.Length == 0)
            {
                return false;
            }

            StreamWriter osw = null;
            if (outputFile != null)
            {
                osw = new StreamWriter(IOUtil.newOutputStream(outputFile));
            }
            foreach (string inputFile in restArgs)
            {
                InputStream fis = IOUtil.newInputStream(inputFile);
                InputStreamReader isr = new InputStreamReader(fis, "UTF-8");
                TextReader br = new TextReader(isr);

                while (true)
                {
                    TaggerImpl.ReadStatus status = tagger.read(br);
                    if (TaggerImpl.ReadStatus.ERROR == status)
                    {
                        Console.Error.WriteLine("read error");
                        return false;
                    }
                    else if (TaggerImpl.ReadStatus.EOF == status && tagger.empty())
                    {
                        break;
                    }
                    if (!tagger.parse())
                    {
                        Console.Error.WriteLine("parse error");
                        return false;
                    }
                    if (osw == null)
                    {
                        Console.Write(tagger.ToString());
                    }
                    else
                    {
                        osw.Write(tagger.ToString());
                    }
                }
                if (osw != null)
                {
                    osw.flush();
                }
                br.Close();
            }
            if (osw != null)
            {
                osw.Close();
            }
        }
        catch (Exception e)
        {
            //e.printStackTrace();
            return false;
        }
        return true;
    }

    public static void main(string[] args)
    {
        crf_test.run(args);
    }
}
