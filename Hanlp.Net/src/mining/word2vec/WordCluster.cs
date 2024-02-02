namespace com.hankcs.hanlp.mining.word2vec;


public class WordCluster
{

    static void usage()
    {
        Console.Error.WriteLine("Usage: java %s <query-file> <k> <Out-file>\n", WordCluster.s.Name);
        Console.Error.WriteLine("\t<query-file> Contains word projections in the text Format\n");
        Console.Error.WriteLine("\t<k> number of clustering\n");
        Console.Error.WriteLine("\t<Out-file> output file\n");
        Environment.Exit(0);
    }

    public static void main(string[] args)
    {
        if (args.Length < 3) usage();

         string vectorFile = args[0];
         int k = int.parseInt(args[1]);
         string outFile = args[2];
         VectorsReader vectorsReader = new VectorsReader(vectorFile);
        vectorsReader.readVectorFile();

        KMeansClustering kmc = new KMeansClustering(vectorsReader, k, outFile);
        kmc.clustering();
    }
}
