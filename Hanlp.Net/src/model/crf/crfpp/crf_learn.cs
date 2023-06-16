namespace com.hankcs.hanlp.model.crf.crfpp;




/**
 * 对应crf_learn
 *
 * @author zhifac
 */
public class crf_learn
{
    public static class Option
    {
        @Argument(description = "use features that occur no less than INT(default 1)", alias = "f")
        public int freq = 1;
        @Argument(description = "set INT for max iterations in LBFGS routine(default 10k)", alias = "m")
        public  int maxiter = 10000;
        @Argument(description = "set FLOAT for cost parameter(default 1.0)", alias = "c")
        public  Double cost = 1.0;
        @Argument(description = "set FLOAT for termination criterion(default 0.0001)", alias = "e")
        public  Double eta = 0.0001;
        @Argument(description = "convert text model to binary model", alias = "C")
        public  Boolean convert = false;
        @Argument(description = "convert binary model to text model", alias = "T")
        public  Boolean convert_to_text = false;
        @Argument(description = "build also text model file for debugging", alias = "t")
        public  Boolean textmodel = false;
        @Argument(description = "(CRF|CRF-L1|CRF-L2|MIRA)\", \"select training algorithm", alias = "a")
        public  string algorithm = "CRF-L2";
        @Argument(description = "set INT for number of iterations variable needs to be optimal before considered for shrinking. (default 20)", alias = "H")
        public  int shrinking_size = 20;
        @Argument(description = "show this help and exit", alias = "h")
        public  Boolean help = false;
        @Argument(description = "number of threads(default auto detect)")
        public  int thread = Runtime.getRuntime().availableProcessors();
    }

    public static bool run(string args)
    {
        return run(args.Split("\\s"));
    }

    public static bool run(string[] args)
    {
        Option option = new Option();
        List<string> unkownArgs = null;
        try
        {
            unkownArgs = Args.parse(option, args, false);
        }
        catch (IllegalArgumentException e)
        {
            Console.Error.WriteLine(e.getMessage());
            Args.usage(option);
            return false;
        }

        bool convert = option.convert;
        bool convertToText = option.convert_to_text;
        string[] restArgs = unkownArgs.toArray(new string[0]);
        if (option.help || ((convertToText || convert) && restArgs.Length != 2) ||
            (!convert && !convertToText && restArgs.Length != 3))
        {
            Args.usage(option);
            return option.help;
        }
        int freq = option.freq;
        int maxiter = option.maxiter;
        double C = option.cost;
        double eta = option.eta;
        bool textmodel = option.textmodel;
        int threadNum = option.thread;
        if (threadNum <= 0)
        {
            threadNum = Runtime.getRuntime().availableProcessors();
        }
        int shrinkingSize = option.shrinking_size;

        string algorithm = option.algorithm;
        algorithm = algorithm.toLowerCase();
        Encoder.Algorithm algo = Encoder.Algorithm.CRF_L2;
        if (algorithm.equals("crf") || algorithm.equals("crf-l2"))
        {
            algo = Encoder.Algorithm.CRF_L2;
        }
        else if (algorithm.equals("crf-l1"))
        {
            algo = Encoder.Algorithm.CRF_L1;
        }
        else if (algorithm.equals("mira"))
        {
            algo = Encoder.Algorithm.MIRA;
        }
        else
        {
            Console.Error.WriteLine("unknown algorithm: " + algorithm);
            return false;
        }
        if (convert)
        {
            EncoderFeatureIndex featureIndex = new EncoderFeatureIndex(1);
            if (!featureIndex.convert(restArgs[0], restArgs[1]))
            {
                Console.Error.WriteLine("fail to convert text model");
                return false;
            }
        }
        else if (convertToText)
        {
            DecoderFeatureIndex featureIndex = new DecoderFeatureIndex();
            if (!featureIndex.convert(restArgs[0], restArgs[1]))
            {
                Console.Error.WriteLine("fail to convert binary model");
                return false;
            }
        }
        else
        {
            Encoder encoder = new Encoder();
            if (!encoder.learn(restArgs[0], restArgs[1], restArgs[2],
                               textmodel, maxiter, freq, eta, C, threadNum, shrinkingSize, algo))
            {
                Console.Error.WriteLine("fail to learn model");
                return false;
            }
        }
        return true;
    }

    public static void main(string[] args)
    {
        crf_learn.run(args);
    }
}
