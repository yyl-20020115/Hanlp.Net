using System.Text;

namespace com.hankcs.hanlp.mining.word2vec;



class Word2VecTraining
{
    static readonly int EXP_TABLE_SIZE = 1000;
    static readonly int MAX_EXP = 6;
    static readonly int TABLE_SIZE = 100000000;
    static readonly int MAX_SENTENCE_LENGTH = 1000;
    static readonly Encoding ENCODING = Encoding.UTF8;

    long timeStart;
    static double[] syn0, syn1, syn1neg;
    int[] table;

    private Config config;

    static readonly double[] expTable = new double[EXP_TABLE_SIZE + 1];

    static Word2VecTraining()
    {
        for (int i = 0; i < EXP_TABLE_SIZE; i++)
        {
            expTable[i] = Math.Exp((i / (double) EXP_TABLE_SIZE * 2 - 1) * MAX_EXP); // Precompute the exp() table
            expTable[i] = expTable[i] / (expTable[i] + 1);                          // Precompute f(x) = x / (x + 1)
        }
    }


    public Word2VecTraining(Config config)
    {
        this.config = config;
    }

    public Config getConfig()
    {
        return config;
    }

    class TrainModelThread //: Thread
    {
        Word2VecTraining vec;
        Corpus corpus;
        Config config;
        float alpha;
        float startingAlpha;
        float trainWords;    // #19
        int id;
        int vocabSize;
        long timeStart;
        int[] table;
        VocabWord[] vocab;
        static int wordCountActual = 0;

        public TrainModelThread(Word2VecTraining vec, Corpus corpus, Config config, int id)
        {
            this.vec = vec;
            this.corpus = corpus;
            this.config = config;
            this.alpha = config.getAlpha();
            this.startingAlpha = alpha;
            this.id = id;
            this.table = vec.table;
            this.trainWords = corpus.getTrainWords();
            this.timeStart = vec.timeStart;
            this.vocabSize = corpus.getVocabSize();
            this.vocab = corpus.getVocab();
        }

        public void run()
        {
            float iter = config.getIter();     // #19
            int layer1Size = config.getLayer1Size();
            int numThreads = config.getNumThreads();
            int window = config.getWindow();
            int negative = config.getNegative();
            bool cbow = config.useContinuousBagOfWords();
            bool hs = config.useHierarchicalSoftmax();
            float sample = config.getSample();

            try
            {
                int word = 0, sentence_length = 0, sentence_position = 0, a, b, c, d, last_word, l1, l2, target;
                int[] sen = new int[MAX_SENTENCE_LENGTH + 1];
                long cw;
                long word_count = 0, last_word_count = 0;
                long label, local_iter = (int) iter;
                long next_random = id;
                double f, g;
                long timeNow;
                double[] neu1 = new double[layer1Size];
                double[] neu1e = new double[layer1Size];

                corpus.rewind(numThreads, id);
                while (true)
                {
                    if (word_count - last_word_count > 10000)
                    {
                        wordCountActual += word_count - last_word_count;
                        last_word_count = word_count;
                        timeNow = DateTime.Now.Microsecond;
                        float percent = wordCountActual / (float) (iter * trainWords + 1);
                        long cost_time = timeNow - timeStart + 1;
                        if (config.getCallback() == null)
                        {
                            Console.Error.WriteLine("%cAlpha: %f  iter: %d  Progress: %.2f%%  Words/thread/sec: %.2fk", 13, alpha, local_iter,
                                              percent * 100,
                                              wordCountActual / (float) (cost_time));
                            string etd = Utility.humanTime((long) (cost_time / percent * (1.f - percent)));
                            if (etd.Length > 0) Console.Error.WriteLine("  ETD: %s", etd);
                            Console.Error.Flush();
                        }
                        else
                        {
                            config.getCallback().training(alpha, percent * 100);
                        }

                        alpha = startingAlpha * (1 - wordCountActual / (float) (iter * trainWords + 1));
                        if (alpha < startingAlpha * 0.0001) alpha = startingAlpha * 0.0001F;
                    }
                    if (sentence_length == 0)
                    {
                        while (true)
                        {
                            word = corpus.readWordIndex();
                            if (word == -2) break;                // EOF
                            if (word == -1) continue;             // Filtered Out
                            word_count++;
                            if (word == -3) break;                // End of sentence
                            // The subsampling randomly discards frequent words while keeping the ranking same
                            if (sample > 0)
                            {
                                double ran = (Math.Sqrt(vocab[word].cn / (sample * trainWords)) + 1) * (sample * trainWords) / vocab[word].cn;
                                next_random = nextRandom(next_random);
                                if (ran < (next_random & 0xFFFF) / (double) 65536) continue;
                            }
                            sen[sentence_length] = word;
                            sentence_length++;
                            if (sentence_length >= MAX_SENTENCE_LENGTH) break;
                        }
                        sentence_position = 0;
                    }
                    if (word == -2 /* eof? */ || (word_count > trainWords / numThreads))
                    {
                        wordCountActual += word_count - last_word_count;
                        local_iter--;
                        if (local_iter == 0) break;
                        word_count = 0;
                        last_word_count = 0;
                        sentence_length = 0;
                        corpus.rewind(numThreads, id);
                        continue;
                    }
                    word = sen[sentence_position];
                    if (word == -1) continue;
                    for (c = 0; c < layer1Size; c++) neu1[c] = 0;
                    for (c = 0; c < layer1Size; c++) neu1e[c] = 0;
                    next_random = nextRandom(next_random);
                    b = (int) next_random % window;
                    if (cbow)
                    {  //train the cbow architecture
                        // in -> hidden
                        cw = 0;
                        for (a = b; a < window * 2 + 1 - b; a++)
                            if (a != window)
                            {
                                c = sentence_position - window + a;
                                if (c < 0) continue;
                                if (c >= sentence_length) continue;
                                last_word = sen[c];
                                if (last_word == -1) continue;
                                for (c = 0; c < layer1Size; c++) neu1[c] += syn0[c + last_word * layer1Size];
                                cw++;
                            }
                        if (cw != 0)
                        {
                            for (c = 0; c < layer1Size; c++) neu1[c] /= cw;
                            if (hs) for (d = 0; d < vocab[word].codelen; d++)
                            {
                                f = 0;
                                l2 = vocab[word].point[d] * layer1Size;
                                // Propagate hidden -> output
                                for (c = 0; c < layer1Size; c++) f += neu1[c] * syn1[c + l2];
                                if (f <= -MAX_EXP) continue;
                                else if (f >= MAX_EXP) continue;
                                else f = expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];
                                // 'g' is the gradient multiplied by the learning rate
                                g = (1 - vocab[word].code[d] - f) * alpha;
                                // Propagate errors output -> hidden
                                for (c = 0; c < layer1Size; c++) neu1e[c] += g * syn1[c + l2];
                                // Learn weights hidden -> output
                                for (c = 0; c < layer1Size; c++) syn1[c + l2] += g * neu1[c];
                            }
                            // NEGATIVE SAMPLING
                            if (negative > 0) for (d = 0; d < negative + 1; d++)
                            {
                                if (d == 0)
                                {
                                    target = word;
                                    label = 1;
                                }
                                else
                                {
                                    next_random = nextRandom(next_random);
                                    target = table[Math.Abs((int) ((next_random >> 16) % TABLE_SIZE))];
                                    if (target == 0) target = Math.Abs((int) (next_random % (vocabSize - 1) + 1));
                                    if (target == word) continue;
                                    label = 0;
                                }
                                l2 = target * layer1Size;
                                f = 0;
                                for (c = 0; c < layer1Size; c++) f += neu1[c] * syn1neg[c + l2];
                                if (f > MAX_EXP) g = (label - 1) * alpha;
                                else if (f < -MAX_EXP) g = (label - 0) * alpha;
                                else
                                    g = (label - expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))]) * alpha;
                                for (c = 0; c < layer1Size; c++) neu1e[c] += g * syn1neg[c + l2];
                                for (c = 0; c < layer1Size; c++) syn1neg[c + l2] += g * neu1[c];
                            }
                            // hidden -> in
                            for (a = b; a < window * 2 + 1 - b; a++)
                                if (a != window)
                                {
                                    c = sentence_position - window + a;
                                    if (c < 0) continue;
                                    if (c >= sentence_length) continue;
                                    last_word = sen[c];
                                    if (last_word == -1) continue;
                                    for (c = 0; c < layer1Size; c++) syn0[c + last_word * layer1Size] += neu1e[c];
                                }
                        }
                    }
                    else
                    {  //train skip-gram
                        for (a = b; a < window * 2 + 1 - b; a++)
                            if (a != window)
                            {
                                c = sentence_position - window + a;
                                if (c < 0) continue;
                                if (c >= sentence_length) continue;
                                last_word = sen[c];
                                if (last_word == -1) continue;
                                l1 = last_word * layer1Size;
                                for (c = 0; c < layer1Size; c++) neu1e[c] = 0;
                                // HIERARCHICAL SOFTMAX
                                if (hs) for (d = 0; d < vocab[word].codelen; d++)
                                {
                                    f = 0;
                                    l2 = vocab[word].point[d] * layer1Size;
                                    // Propagate hidden -> output
                                    for (c = 0; c < layer1Size; c++) f += syn0[c + l1] * syn1[c + l2];
                                    if (f <= -MAX_EXP) continue;
                                    else if (f >= MAX_EXP) continue;
                                    else f = expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];
                                    // 'g' is the gradient multiplied by the learning rate
                                    g = (1 - vocab[word].code[d] - f) * alpha;
                                    // Propagate errors output -> hidden
                                    for (c = 0; c < layer1Size; c++) neu1e[c] += g * syn1[c + l2];
                                    // Learn weights hidden -> output
                                    for (c = 0; c < layer1Size; c++) syn1[c + l2] += g * syn0[c + l1];
                                }
                                // NEGATIVE SAMPLING
                                if (negative > 0) for (d = 0; d < negative + 1; d++)
                                {
                                    if (d == 0)
                                    {
                                        target = word;
                                        label = 1;
                                    }
                                    else
                                    {
                                        next_random = nextRandom(next_random);
                                        target = table[Math.Abs((int) ((next_random >> 16) % TABLE_SIZE))];
                                        if (target == 0) target = Math.Abs((int) (next_random % (vocabSize - 1) + 1));
                                        if (target == word) continue;
                                        label = 0;
                                    }
                                    l2 = target * layer1Size;
                                    f = 0;
                                    for (c = 0; c < layer1Size; c++) f += syn0[c + l1] * syn1neg[c + l2];
                                    if (f > MAX_EXP) g = (label - 1) * alpha;
                                    else if (f < -MAX_EXP) g = (label - 0) * alpha;
                                    else
                                        g = (label - expTable[(int) ((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))]) * alpha;
                                    for (c = 0; c < layer1Size; c++) neu1e[c] += g * syn1neg[c + l2];
                                    for (c = 0; c < layer1Size; c++) syn1neg[c + l2] += g * syn0[c + l1];
                                }
                                // Learn weights input -> hidden
                                for (c = 0; c < layer1Size; c++) syn0[c + l1] += neu1e[c];
                            }
                    }
                    sentence_position++;
                    if (sentence_position >= sentence_length)
                    {
                        sentence_length = 0;
                        continue;
                    }
                }
                corpus.shutdown();
            }
            catch (IOException e)
            {
                throw new InvalidOperationException(e);
            }
            // exit from thread
            lock (vec)
            {
                vec.threadCount--;
                vec.Notify();
            }
        }
    }

    int threadCount;

    public void trainModel() 
    {
        int layer1Size = config.getLayer1Size();
        TextFileCorpus corpus = new TextFileCorpus(config);

        logger.info("learning vocabulary");
        corpus.learnVocab();
        logger.info("sorting vocabulary");
        corpus.sortVocab();
        int vocabSize = corpus.getVocabSize();
        VocabWord[] vocab = corpus.getVocab();
        logger.info("Vocab size: " + vocabSize);
        logger.info("Words in train file: " + corpus.getTrainWords());

        if (config.getOutputFile() == null) return;

        initNet(corpus);
        if (config.getNegative() > 0)
            initUnigramTable(corpus);

        timeStart = DateTime.Now.Microsecond;

        threadCount = config.getNumThreads();
        for (int i = 0; i < config.getNumThreads(); i++)
        {
            new TrainModelThread(this, new CacheCorpus(corpus), config, i).start();
        }
        corpus.shutdown();
        lock (this)
        {
            while (threadCount > 0)
            {
                try
                {
                    wait();
                }
                catch (InterruptedException ignored)
                {
                }
            }
        }

        Console.Error.WriteLine();
        logger.info(string.Format("finished training in %s", Utility.humanTime(DateTime.Now.Microsecond - timeStart)));
        // lose weight
        syn1 = null;
        table = null;

        OutputStream os = null;
        Writer w = null;
        TextWriter pw = null;

        try
        {
            os = new FileStream(config.getOutputFile());
            w = new StreamWriter(os, ENCODING);
            pw = new TextWriter(w);

            // Save the word vectors
            logger.info("now saving the word vectors to the file " + config.getOutputFile());
            pw.printf("%d %d\n", vocabSize, layer1Size);
            for (int i = 0; i < vocabSize; i++)
            {
                pw.print(vocab[i].word);
                for (int j = 0; j < layer1Size; j++)
                {
                    pw.printf(" %f", syn0[i * layer1Size + j]);
                }
                pw.println();
            }
        }
        finally
        {
            corpus.Close();
            Utility.closeQuietly(pw);
            Utility.closeQuietly(w);
            Utility.closeQuietly(os);
        }
    }

    /**
     * Used later for sorting by word counts
     */
    class VocabWordComparator : IComparer<VocabWord>
    {
        //////@Override
        public int Compare(VocabWord o1, VocabWord o2)
        {
            return o2.cn - o1.cn;
        }
    }

    void initUnigramTable(Corpus corpus)
    {
        int vocabSize = corpus.getVocabSize();
        VocabWord[] vocab = corpus.getVocab();
        long trainWordsPow = 0;
        double d1, power = 0.75;
        table = new int[TABLE_SIZE];
        for (int i = 0; i < vocabSize; i++)
        {
            trainWordsPow += Math.Pow(vocab[i].cn, power);
        }
        int i = 0;
        d1 = Math.Pow(vocab[i].cn, power) / (double) trainWordsPow;
        for (int j = 0; j < TABLE_SIZE; j++)
        {
            table[j] = i;
            if ((double) j / (double) TABLE_SIZE > d1)
            {
                i++;
                d1 += Math.Pow(vocab[i].cn, power) / (double) trainWordsPow;
            }
            if (i >= vocabSize)
                i = vocabSize - 1;
        }
    }

    void initNet(Corpus corpus)
    {
        int layer1Size = config.getLayer1Size();
        int vocabSize = corpus.getVocabSize();

        syn0 = posixMemAlign128(vocabSize * layer1Size);

        if (config.useHierarchicalSoftmax())
        {
            syn1 = posixMemAlign128(vocabSize * layer1Size);
            for (int i = 0; i < vocabSize; i++)
            {
                for (int j = 0; j < layer1Size; j++)
                {
                    syn1[i * layer1Size + j] = 0;
                }
            }
        }

        if (config.getNegative() > 0)
        {
            syn1neg = posixMemAlign128(vocabSize * layer1Size);
            for (int i = 0; i < vocabSize; i++)
            {
                for (int j = 0; j < layer1Size; j++)
                {
                    syn1neg[i * layer1Size + j] = 0;
                }
            }
        }

        long nextRandom = 1;
        for (int i = 0; i < vocabSize; i++)
        {
            for (int j = 0; j < layer1Size; j++)
            {
                nextRandom = nextRandom(nextRandom);
                syn0[i * layer1Size + j] = (((nextRandom & 0xFFFF) / (double) 65536) - 0.5) / layer1Size;
            }
        }
        corpus.createBinaryTree();
    }

    static double[] posixMemAlign128(int size)
    {
        int surplus = size % 128;
        if (surplus > 0)
        {
            int div = size / 128;
            return new double[(div + 1) * 128];
        }
        return new double[size];
    }

    static long nextRandom(long nextRandom)
    {
        return nextRandom * 25214903917L + 11;
    }
}
