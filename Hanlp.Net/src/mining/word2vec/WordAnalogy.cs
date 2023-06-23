namespace com.hankcs.hanlp.mining.word2vec;



public class WordAnalogy : AbstractClosestVectors
{

    protected WordAnalogy(string file)
        : base(file)
    {
        ;
    }

    static void Usage()
    {
        Console.Error.WriteLine("Usage: java %s <FILE>\nwhere FILE Contains word projections in the text Format\n",
                          typeof(WordAnalogy).Name);
        Environment.Exit(0);
    }

    public static void Main(string[] args) 
    {
        if (args.Length < 1) usage();
        new WordAnalogy(args[0]).execute();
    }

    protected override Result getTargetVector()
    {
        int words = vectorsReader.getNumWords();
        int size = vectorsReader.getSize();

        string[] input = null;
        while ((input = nextWords(3, "Enter 3 words")) != null)
        {
            // linear search the input word in vocabulary
            int[] bi = new int[input.Length];
            int found = 0;
            for (int k = 0; k < input.Length; k++)
            {
                for (int i = 0; i < words; i++)
                {
                    if (input[k].Equals(vectorsReader.getWord(i)))
                    {
                        bi[k] = i;
                        Console.WriteLine("\nWord: %s  Position in vocabulary: %d\n", input[k], bi[k]);
                        found++;
                    }
                }
                if (found == k)
                {
                    Console.WriteLine("%s : Out of dictionary word!\n", input[k]);
                }
            }
            if (found < input.Length)
            {
                continue;
            }

            float[] vec = new float[size];
            double len = 0;
            for (int j = 0; j < size; j++)
            {
                vec[j] = vectorsReader.getMatrixElement(bi[1], j) -
                        vectorsReader.getMatrixElement(bi[0], j) + vectorsReader.getMatrixElement(bi[2], j);
                len += vec[j] * vec[j];
            }

            len = Math.Sqrt(len);
            for (int i = 0; i < size; i++)
            {
                vec[i] = (float)(vec[i]/len);
            }

            return new Result(vec, bi);
        }

        return null;
    }
}
