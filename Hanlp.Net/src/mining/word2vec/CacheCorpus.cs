namespace com.hankcs.hanlp.mining.word2vec;


/**
 * load corpus from disk cache
 *
 * @author hankcs
 */
public class CacheCorpus : Corpus
{
    private RandomAccessFile raf;

    public CacheCorpus(Corpus cloneSrc) 
    {
        base(cloneSrc);
        raf = new RandomAccessFile(((TextFileCorpus) cloneSrc).cacheFile, "r");
    }

    //@Override
    public string nextWord() 
    {
        return null;
    }

    //@Override
    public int readWordIndex() 
    {
        int id = nextId();
        while (id == -4)
        {
            id = nextId();
        }
        return id;
    }

    private int nextId() 
    {
        if (raf.Length - raf.getFilePointer() >= 4)
        {
            int id = raf.readInt();
            return id < 0 ? id : table[id];
        }

        return -2;
    }

    //@Override
    public void rewind(int numThreads, int id) 
    {
        base.rewind(numThreads, id);
        raf.seek(raf.Length / 4 / numThreads * id * 4);   // spilt by id, not by bytes
    }
}