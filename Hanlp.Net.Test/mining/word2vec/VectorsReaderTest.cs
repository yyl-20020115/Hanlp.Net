using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.mining.word2vec;


[TestClass]

public class VectorsReaderTest : TestCase
{
    [TestMethod]
    public void TestReadVectorFile() 
    {
        string tempFile = createTempFile("hanlp-vector", ".txt");
        TextWriter bw = IOUtil.newBufferedWriter(tempFile);
        bw.Write("3 1\n" +
                     "cat 1.1\n" +
                     " 2.2\n" +
                     "dog 3.3\n"
        );
        bw.Close();

        VectorsReader reader = new VectorsReader(tempFile);
        reader.readVectorFile();
        AssertEquals(2, reader.words);
        AssertEquals(2, reader.vocab.Length);
        AssertEquals(2, reader.matrix.Length);
        AssertEquals(1f, reader.matrix[1][0]);
    }
}