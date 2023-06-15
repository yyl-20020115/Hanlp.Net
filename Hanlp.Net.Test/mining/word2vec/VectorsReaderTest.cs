namespace com.hankcs.hanlp.mining.word2vec;


[TestClass]

public class VectorsReaderTest : TestCase
{
    [TestMethod]
    public void TestReadVectorFile() 
    {
        File tempFile = File.createTempFile("hanlp-vector", ".txt");
        tempFile.deleteOnExit();
        BufferedWriter bw = IOUtil.newBufferedWriter(tempFile.getAbsolutePath());
        bw.write("3 1\n" +
                     "cat 1.1\n" +
                     " 2.2\n" +
                     "dog 3.3\n"
        );
        bw.close();

        VectorsReader reader = new VectorsReader(tempFile.getAbsolutePath());
        reader.readVectorFile();
        assertEquals(2, reader.words);
        assertEquals(2, reader.vocab.Length);
        assertEquals(2, reader.matrix.Length);
        assertEquals(1f, reader.matrix[1][0]);
    }
}