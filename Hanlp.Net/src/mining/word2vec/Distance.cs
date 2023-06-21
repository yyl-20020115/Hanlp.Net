namespace com.hankcs.hanlp.mining.word2vec;



public class Distance : AbstractClosestVectors
{

    public Distance(string file)
    {
        base(file);
    }

    static void usage()
    {
        Console.Error.WriteLine("Usage: java %s <FILE>\nwhere FILE Contains word projections in the text Format\n",
                          Distance.c.getName());
        Environment.Exit(0);
    }

    public static void main(string[] args) 
    {
        if (args.Length < 1) usage();
        new Distance(args[0]).execute();
    }

    protected Result getTargetVector()
    {
        int words = vectorsReader.getNumWords();
        int size = vectorsReader.getSize();

        string[] input = null;
        while ((input = nextWords(1, "Enter a word")) != null)
        {
            // linear search the input word in vocabulary
            float[] vec = null;
            int bi = -1;
            double len = 0;
            for (int i = 0; i < words; i++)
            {
                if (input[0].Equals(vectorsReader.getWord(i)))
                {
                    bi = i;
                    Console.WriteLine("\nWord: %s  Position in vocabulary: %d\n", input[0], bi);
                    vec = new float[size];
                    for (int j = 0; j < size; j++)
                    {
                        vec[j] = vectorsReader.getMatrixElement(bi, j);
                        len += vec[j] * vec[j];
                    }
                }
            }
            if (vec == null)
            {
                Console.WriteLine("%s : Out of dictionary word!\n", input[0]);
                continue;
            }

            len = Math.Sqrt(len);
            for (int i = 0; i < size; i++)
            {
                vec[i] /= len;
            }

            return new Result(vec, new int[]{bi});
        }

        return null;
    }
}
