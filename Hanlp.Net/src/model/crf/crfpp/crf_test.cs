namespace com.hankcs.hanlp.model.crf.crfpp;




/**
 * 对应crf_test
 *
 * @author zhifac
 */
public class crf_test
{
    private static class Option
    {
        @Argument(description = "set FILE for model file", alias = "m", required = true)
        string model;
        @Argument(description = "output n-best results", alias = "n")
        int nbest = 0;
        @Argument(description = "set INT for verbose level", alias = "v")
        int verbose = 0;
        @Argument(description = "set cost factor", alias = "c")
        Double cost_factor = 1.0;
        @Argument(description = "output file path", alias = "o")
        string output;
        @Argument(description = "show this help and exit", alias = "h")
        Boolean help = false;
    }

    public static bool run(string[] args)
    {
        Option cmd = new Option();
        List<string> unkownArgs = null;
        try
        {
            unkownArgs = Args.parse(cmd, args, false);
        }
        catch (IllegalArgumentException e)
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
                System.err.println("open error");
                return false;
            }
            string[] restArgs = unkownArgs.toArray(new string[0]);
            if (restArgs.length == 0)
            {
                return false;
            }

            OutputStreamWriter osw = null;
            if (outputFile != null)
            {
                osw = new OutputStreamWriter(IOUtil.newOutputStream(outputFile));
            }
            for (string inputFile : restArgs)
            {
                InputStream fis = IOUtil.newInputStream(inputFile);
                InputStreamReader isr = new InputStreamReader(fis, "UTF-8");
                BufferedReader br = new BufferedReader(isr);

                while (true)
                {
                    TaggerImpl.ReadStatus status = tagger.read(br);
                    if (TaggerImpl.ReadStatus.ERROR == status)
                    {
                        System.err.println("read error");
                        return false;
                    }
                    else if (TaggerImpl.ReadStatus.EOF == status && tagger.empty())
                    {
                        break;
                    }
                    if (!tagger.parse())
                    {
                        System.err.println("parse error");
                        return false;
                    }
                    if (osw == null)
                    {
                        System._out.print(tagger.toString());
                    }
                    else
                    {
                        osw.write(tagger.toString());
                    }
                }
                if (osw != null)
                {
                    osw.flush();
                }
                br.close();
            }
            if (osw != null)
            {
                osw.close();
            }
        }
        catch (Exception e)
        {
            e.printStackTrace();
            return false;
        }
        return true;
    }

    public static void main(string[] args)
    {
        crf_test.run(args);
    }
}
