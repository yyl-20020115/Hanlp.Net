namespace com.hankcs.hanlp.mining.word2vec;


public class TextFileCorpus : Corpus
{

    private static readonly int VOCAB_MAX_SIZE = 30000000;

    private int minReduce = 1;
    private TextReader raf = null;
    private Stream cache;

    public TextFileCorpus(Config config) 
    {
        base(config);
    }

    //@Override
    public void shutdown() 
    {
        Utility.closeQuietly(raf);
        wordsBuffer = null;
    }

    //@Override
    public void rewind(int numThreads, int id) 
    {
        base.rewind(numThreads, id);
    }

    //@Override
    public string nextWord() 
    {
        return readWord(raf);
    }

    /**
     * Reduces the vocabulary by removing infrequent tokens
     */
    void reduceVocab()
    {
        table = new int[vocabSize];
        int j = 0;
        for (int i = 0; i < vocabSize; i++)
        {
            if (vocab[i].cn > minReduce)
            {
                vocab[j].cn = vocab[i].cn;
                vocab[j].word = vocab[i].word;
                table[vocabIndexMap.get(vocab[j].word)] = j;
                j++;
            }
            else
            {
                table[vocabIndexMap.get(vocab[j].word)] = -4;
            }
        }
        // adjust the index in the cache
        try
        {
            cache.Close();
            File fixingFile = new File(cacheFile + ".fixing");
            cache = new Stream(new FileStream(fixingFile));
            Stream oldCache = new Stream(new FileStream(cacheFile));
            while (oldCache.available() >= 4)
            {
                int oldId = oldCache.readInt();
                if (oldId < 0)
                {
                    cache.writeInt(oldId);
                    continue;
                }
                int id = table[oldId];
                if (id == -4) continue;
                cache.writeInt(id);
            }
            oldCache.Close();
            cache.Close();
            if (!fixingFile.renameTo(cacheFile))
            {
                throw new RuntimeException(string.Format("moving %s to %s failed", fixingFile, cacheFile.Name));
            }
            cache = new Stream(new FileStream(cacheFile));
        }
        catch (IOException e)
        {
            throw new RuntimeException(string.Format("failed to adjust cache file", e));
        }
        table = null;
        vocabSize = j;
        vocabIndexMap.Clear();
        for (int i = 0; i < vocabSize; i++)
        {
            vocabIndexMap.Add(vocab[i].word, i);
        }
        minReduce++;
    }

    public void learnVocab() 
    {
        vocab = new VocabWord[vocabMaxSize];
        vocabIndexMap = new Dictionary<string, int>();
        vocabSize = 0;

        File trainFile = new File(config.getInputFile());

        TextReader raf = null;
        FileStream fileInputStream = null;
        cache = null;
        vocabSize = 0;
        TrainingCallback callback = config.getCallback();
        try
        {
            fileInputStream = new FileStream(trainFile);
            raf = new TextReader(new InputStreamReader(fileInputStream, encoding));
            cacheFile = File.createTempFile(string.Format("corpus_%d", DateTime.Now.Microsecond), ".bin");
            cache = new Stream(new FileStream(cacheFile));
            while (true)
            {
                string word = readWord(raf);
                if (word == null && eoc) break;
                trainWords++;
                if (trainWords % 100000 == 0)
                {
                    if (callback == null)
                    {
                        Console.Error.WriteLine("%c%.2f%% %dK", 13,
                                          (1.f - fileInputStream.available() / (float) trainFile.Length) * 100.f,
                                          trainWords / 1000);
                        System.err.flush();
                    }
                    else
                    {
                        callback.corpusLoading((1.f - fileInputStream.available() / (float) trainFile.Length) * 100.f);
                    }
                }
                int idx = searchVocab(word);
                if (idx == -1)
                {
                    idx = addWordToVocab(word);
                    vocab[idx].cn = 1;
                }
                else vocab[idx].cn++;
                if (vocabSize > VOCAB_MAX_SIZE * 0.7)
                {
                    reduceVocab();
                    idx = searchVocab(word);
                }
                cache.writeInt(idx);
            }
        }
        finally
        {
            Utility.closeQuietly(fileInputStream);
            Utility.closeQuietly(raf);
            Utility.closeQuietly(cache);
            Console.Error.WriteLine();
        }

        if (callback == null)
        {
            Console.Error.WriteLine("%c100%% %dK", 13, trainWords / 1000);
            System.err.flush();
        }
        else
        {
            callback.corpusLoading(100);
            callback.corpusLoaded(vocabSize, trainWords, trainWords);
        }
    }

    string[] wordsBuffer = new string[0];
    int wbp = wordsBuffer.Length;

    /**
     * Reads a single word from a file, assuming space + tab + EOL to be word boundaries
     *
     * @param raf
     * @return null if EOF
     * @
     */
    string readWord(TextReader raf) 
    {
        while (true)
        {
            // check the buffer first
            if (wbp < wordsBuffer.Length)
            {
                return wordsBuffer[wbp++];
            }

            string line = raf.ReadLine();
            if (line == null)
            {      // end of corpus
                eoc = true;
                return null;
            }
            line = line.Trim();
            if (line.Length == 0)
            {
                continue;
            }
            cache.writeInt(-3); // mark end of sentence
            wordsBuffer = line.Split("\\s+");
            wbp = 0;
            eoc = false;
        }
    }
}
