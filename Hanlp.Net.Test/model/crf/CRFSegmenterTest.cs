namespace com.hankcs.hanlp.model.crf;


[TestClass]
public class CRFSegmenterTest : TestCase
{

    public static readonly string CWS_MODEL_PATH = HanLP.Config.CRFCWSModelPath;
    [TestMethod]
    public void testTrain() 
    {
        CRFSegmenter segmenter = new CRFSegmenter(null);
        segmenter.train("data/test/pku98/199801.txt", CWS_MODEL_PATH);
    }
    [TestMethod]
    public void testConvert() 
    {
        crf_learn.run("-T " + CWS_MODEL_PATH + " " + CWS_MODEL_PATH + ".txt");
    }
    [TestMethod]
    public void testConvertCorpus() 
    {
        CRFSegmenter segmenter = new CRFSegmenter(null);
        segmenter.convertCorpus("data/test/pku98/199801.txt", "data/test/crf/cws-corpus.tsv");
        segmenter.dumpTemplate("data/test/crf/cws-template.txt");
    }
    [TestMethod]
    public void testLoad() 
    {
        CRFSegmenter segmenter = new CRFSegmenter("data/test/converted.txt");
        List<String> wordList = segmenter.segment("商品和服务");
        Console.WriteLine(wordList);
    }
    [TestMethod]
    public void testOutput() 
    {
//        final CRFSegmenter segmenter = new CRFSegmenter(CWS_MODEL_PATH);
//
//        final BufferedWriter bw = IOUtil.newBufferedWriter("data/test/crf/cws/mdat.txt");
//        IOUtility.loadInstance("data/test/pku98/199801.txt", new InstanceHandler()
//        {
//            //@Override
//            public bool process(Sentence instance)
//            {
//                String text = instance.text().replace("0", "").replace("X", "");
//                try
//                {
//                    for (String term : segmenter.segment(text))
//                    {
//
//                        bw.write(term);
//                        bw.write(" ");
//                    }
//                    bw.newLine();
//                }
//                catch (IOException e)
//                {
//                    e.printStackTrace();
//                }
//                return false;
//            }
//        });
//        bw.close();
    }

}