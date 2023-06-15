namespace com.hankcs.hanlp.mining.word2vec;



public class Train : AbstractTrainer
{

    //@Override
    protected void localUsage()
    {
        paramDesc("-input <file>", "Use text data from <file> to train the model");
        System.err.printf("\nExamples:\n");
        System.err.printf("java %s -input corpus.txt -output vectors.txt -size 200 -window 5 -sample 0.0001 -negative 5 -hs 0 -binary -cbow 1 -iter 3\n\n",
                          Train.class.getName());
    }

    void execute(string[] args) 
    {
        if (args.length <= 1) usage();

        Config config = new Config();

        setConfig(args, config);
        int i;
        if ((i = argPos("-input", args)) >= 0) config.setInputFile(args[i + 1]);

        Word2VecTraining w2v = new Word2VecTraining(config);
        System.err.printf("Starting training using text file %s\nthreads = %d, iter = %d\n",
                          config.getInputFile(),
                          config.getNumThreads(),
                          config.getIter());
        w2v.trainModel();
    }

    public static void main(string[] args) 
    {
        new Train().execute(args);
    }
}
