namespace com.hankcs.hanlp.mining.word2vec;


[TestClass]

public class VectorsReaderTest : TestCase
{
    [TestMethod]
    public void TestReadVectorFile() 
    {
        File tempFile = File.createTempFile("hanlp-vector", ".txt");
        tempFile.deleteOnExit();
        BufferedWriter bw = IOUtil.newBufferedWriter(tempFile);
        bw.write("3 1\n" +
                     "cat 1.1\n" +
                     " 2.2\n" +
                     "dog 3.3\n"
        );
        bw.close();

        VectorsReader reader = new VectorsReader(tempFile);
        reader.readVectorFile();
        AssertEquals(2, reader.words);
        AssertEquals(2, reader.vocab.Length);
        AssertEquals(2, reader.matrix.Length);
        AssertEquals(1f, reader.matrix[1][0]);
    }
}