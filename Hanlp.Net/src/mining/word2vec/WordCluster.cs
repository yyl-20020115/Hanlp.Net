namespace com.hankcs.hanlp.mining.word2vec;


public final class WordCluster
{

    static void usage()
    {
        System.err.printf("Usage: java %s <query-file> <k> <_out-file>\n", WordCluster.class.getName());
        System.err.println("\t<query-file> contains word projections in the text format\n");
        System.err.println("\t<k> number of clustering\n");
        System.err.println("\t<_out-file> output file\n");
        System.exit(0);
    }

    public static void main(String[] args)
    {
        if (args.length < 3) usage();

        final String vectorFile = args[0];
        final int k = int.parseInt(args[1]);
        final String outFile = args[2];
        final VectorsReader vectorsReader = new VectorsReader(vectorFile);
        vectorsReader.readVectorFile();

        KMeansClustering kmc = new KMeansClustering(vectorsReader, k, outFile);
        kmc.clustering();
    }
}
