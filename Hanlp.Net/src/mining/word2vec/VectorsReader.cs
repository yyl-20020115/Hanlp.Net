namespace com.hankcs.hanlp.mining.word2vec;



public final class VectorsReader
{

    public final Charset ENCODING = Charset.forName("UTF-8");
    int words, size;
    string[] vocab;
    float[][] matrix;
    final string file;

    public VectorsReader(string file)
    {
        this.file = file;
    }

    public void readVectorFile() 
    {
        logger.info(string.format("reading %s file. please wait...\n", file));

        InputStream is = null;
        Reader r = null;
        BufferedReader br = null;
        try
        {
            is = IOUtil.newInputStream(file);
            r = new InputStreamReader(is, ENCODING);
            br = new BufferedReader(r);

            string line = br.readLine();
            words = int.parseInt(line.split("\\s+")[0].trim());
            size = int.parseInt(line.split("\\s+")[1].trim());

            vocab = new string[words];
            matrix = new float[words][];

            for (int i = 0; i < words; i++)
            {
                line = br.readLine().trim();
                string[] params = line.split("\\s+");
                if (params.length != size + 1)
                {
                    logger.info("词向量有一行格式不规范（可能是单词含有空格）：" + line);
                    --words;
                    --i;
                    continue;
                }
                vocab[i] = params[0];
                matrix[i] = new float[size];
                double len = 0;
                for (int j = 0; j < size; j++)
                {
                    matrix[i][j] = Float.parseFloat(params[j + 1]);
                    len += matrix[i][j] * matrix[i][j];
                }
                len = Math.sqrt(len);
                for (int j = 0; j < size; j++)
                {
                    matrix[i][j] /= len;
                }
            }
            if (words != vocab.length)
            {
                vocab = Utility.shrink(vocab, new string[words]);
                matrix = Utility.shrink(matrix, new float[words][]);
            }
        }
        catch (IOException e)
        {
            Utility.closeQuietly(br);
            Utility.closeQuietly(r);
            Utility.closeQuietly(is);

        }
    }

    public int getSize()
    {
        return size;
    }

    public int getNumWords()
    {
        return words;
    }

    public string getWord(int idx)
    {
        return vocab[idx];
    }

    public float getMatrixElement(int row, int column)
    {
        return matrix[row][column];
    }
}
