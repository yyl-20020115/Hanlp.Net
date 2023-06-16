using com.hankcs.hanlp.corpus.io;
using System.Text;

namespace com.hankcs.hanlp.mining.word2vec;



public class VectorsReader
{

    public Encoding ENCODING = Encoding.UTF8;
    int words, size;
    public string[] vocab;
    public float[][] matrix;
    string file;

    public VectorsReader(string file)
    {
        this.file = file;
    }

    public void readVectorFile() 
    {
        logger.info(string.format("reading %s file. please wait...\n", file));

        InputStream _is = null;
        Reader r = null;
        BufferedReader br = null;
        try
        {
            _is = IOUtil.newInputStream(file);
            r = new InputStreamReader(_is, ENCODING);
            br = new BufferedReader(r);

            string line = br.readLine();
            words = int.parseInt(line.Split("\\s+")[0].trim());
            size = int.parseInt(line.Split("\\s+")[1].trim());

            vocab = new string[words];
            matrix = new float[words][];

            for (int i = 0; i < words; i++)
            {
                line = br.readLine().trim();
                string[] _params = line.Split("\\s+");
                if (_params.Length != size + 1)
                {
                    logger.info("词向量有一行格式不规范（可能是单词含有空格）：" + line);
                    --words;
                    --i;
                    continue;
                }
                vocab[i] = _params[0];
                matrix[i] = new float[size];
                double len = 0;
                for (int j = 0; j < size; j++)
                {
                    matrix[i][j] = float.parseFloat(_params[j + 1]);
                    len += matrix[i][j] * matrix[i][j];
                }
                len = Math.sqrt(len);
                for (int j = 0; j < size; j++)
                {
                    matrix[i][j] /= len;
                }
            }
            if (words != vocab.Length)
            {
                vocab = Utility.shrink(vocab, new string[words]);
                matrix = Utility.shrink(matrix, new float[words][]);
            }
        }
        catch (IOException e)
        {
            Utility.closeQuietly(br);
            Utility.closeQuietly(r);
            Utility.closeQuietly(_is);

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
