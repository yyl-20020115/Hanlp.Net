namespace com.hankcs.hanlp.model.hmm;


public class HMMSegmenterTest : TestCase
{
    public void testTrain() 
    {
        HMMSegmenter segmenter = new HMMSegmenter();
        segmenter.train("data/test/my_cws_corpus.txt");
        Console.WriteLine(segmenter.segment("商品和服务"));
    }
}