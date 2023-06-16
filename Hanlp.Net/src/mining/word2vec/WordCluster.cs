namespace com.hankcs.hanlp.mining.word2vec;


public final class WordCluster
{

    static void usage()
    {
        Console.Error.WriteLine("Usage: java %s <query-file> <k> <_out-file>\n", WordCluster.class.getName());
        Console.Error.WriteLine("\t<query-file> contains word projections in the text format\n");
        Console.Error.WriteLine("\t<k> number of clustering\n");
        Console.Error.WriteLine("\t<_out-file> output file\n");
        System.exit(0);
    }

    public static void main(string[] args)
    {
        if (args.Length < 3) usage();

        final string vectorFile = args[0];
        final int k = int.parseInt(args[1]);
        final string outFile = args[2];
        final VectorsReader vectorsReader = new VectorsReader(vectorFile);
        vectorsReader.readVectorFile();

        KMeansClustering kmc = new KMeansClustering(vectorsReader, k, outFile);
        kmc.clustering();
    }
}
