namespace com.hankcs.hanlp.mining.word2vec;


public abstract class AbstractClosestVectors
{

    static readonly int N = 40;
    protected Scanner scanner;
    protected VectorsReader vectorsReader;

    protected AbstractClosestVectors(string file)
    {
        vectorsReader = new VectorsReader(file);
    }

    protected string[] nextWords(int n, string msg)
    {
        Console.WriteLine(msg + " ('q' to break): ");
        string[] words = new string[n];

        for (int i = 0; i < n; i++)
        {
            string word = nextWord();
            if (word == null) return null;
            words[i] = word;
        }

        return words;
    }

    protected string nextWord()
    {
        string word = scanner.next();
        return word == null || word.Length == 0 || word.Equals("q") ? null : word;
    }

    protected abstract Result getTargetVector();

    protected void execute()
    {
        vectorsReader.readVectorFile();
        int words = vectorsReader.getNumWords();
        int size = vectorsReader.getSize();

        try
        {
            scanner = new Scanner(System._in);
            Result result = null;
            while ((result = getTargetVector()) != null)
            {

                double[] bestd = new double[N];
                string[] bestw = new string[N];
                int i = 0;
            next_word:
                for (; i < words; i++)
                {
                    foreach (int bi in result.bi)
                    {
                        if (i == bi) goto next_word;
                    }
                    double dist = 0;
                    for (int j = 0; j < size; j++)
                    {
                        dist += result.vec[j] * vectorsReader.getMatrixElement(i, j);
                    }
                    for (int j = 0; j < N; j++)
                    {
                        if (dist > bestd[j])
                        {
                            for (int k = N - 1; k > j; k--)
                            {
                                bestd[k] = bestd[k - 1];
                                bestw[k] = bestw[k - 1];
                            }
                            bestd[j] = dist;
                            bestw[j] = vectorsReader.getWord(i);
                            break;
                        }
                    }
                }

                Console.WriteLine("\n                                              Word       Cosine cosine\n------------------------------------------------------------------------\n");
                for (int j = 0; j < N; j++)
                    Console.WriteLine("%50s\t\t%f\n", bestw[j], bestd[j]);
            }
        }
        finally
        {
            scanner.Close();
        }
    }

    public class Result
    {

        public float[] vec;
        public int[] bi;

        public Result(float[] vec, int[] bi)
        {
            this.vec = vec;
            this.bi = bi;
        }
    }
}
