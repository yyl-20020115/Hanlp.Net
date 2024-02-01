using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer;

namespace com.hankcs.hanlp.classification.utilities;



/**
 * 文件预处理工具
 */
public class TextProcessUtility
{
    /**
     * 预处理，去除标点，空格和停用词
     *
     * @param text
     * @return
     */
    public static string Preprocess(string text)
    {
        return text.Replace("\\p{P}", " ").Replace("\\s+", " ").ToLower();
    }

    /**
     * 提取关键词，在真实的应用场景中，还应该涉及到短语
     *
     * @param text
     * @return
     */
    public static string[] ExtractKeywords(string text)
    {
        List<Term> termList = NotionalTokenizer.segment(text);
        string[] wordArray = new string[termList.Count];
        IEnumerator<Term> iterator = termList.GetEnumerator();
        for (int i = 0; i < wordArray.Length && iterator.MoveNext(); i++)
        {
            wordArray[i] = iterator.Current.word;
        }
        return wordArray;
    }

    /**
     * 统计每个词的词频
     *
     * @param keywordArray
     * @return
     */
    public static Dictionary<string, int> GetKeywordCounts(string[] keywordArray)
    {
        Dictionary<string, int> counts = new ();

        int counter;
        for (int i = 0; i < keywordArray.Length; ++i)
        {
            counter = counts[(keywordArray[i])];
            if (counter == null)
            {
                counter = 0;
            }
            counts.Add(keywordArray[i], ++counter); //增加词频
        }

        return counts;
    }

    /**
     * 加载一个文件夹下的所有语料
     *
     * @param path
     * @return
     */
    public static Dictionary<string, string[]> LoadCorpus(string path)
    {
        Dictionary<string, string[]> dataSet = new ();
        string root = new string(path);
        string[] folders = root.listFiles();
        if (folders == null) return null;
        foreach (string folder in folders)
        {
            if (folder.isFile()) continue;
            string[] files = folder.listFiles();
            if (files == null) continue;
            string[] documents = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = IOUtil.readTxt(files[i]);
            }
            dataSet.Add(folder.Name, documents);
        }

        return dataSet;
    }

    /**
     * 加载一个文件夹下的所有语料
     *
     * @param folderPath
     * @return
     */
    public static Dictionary<string, string[]> LoadCorpusWithException(string folderPath, string charsetName) 
    {
        if (folderPath == null) throw new ArgumentException("参数 folderPath == null");
        string root = new string(folderPath);
        if (!root.exists()) throw new ArgumentException(string.Format("目录 %s 不存在", root));
        if (!root.isDirectory())
            throw new ArgumentException(string.Format("目录 %s 不是一个目录", root));

        Dictionary<string, string[]> dataSet = new Dictionary<string, string[]>();
        string[] folders = root.listFiles();
        if (folders == null) return null;
        foreach (string folder in folders)
        {
            if (folder.isFile()) continue;
            string[] files = folder.listFiles();
            if (files == null) continue;
            string[] documents = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = ReadTxt(files[i], charsetName);
            }
            dataSet.Add(folder.Name, documents);
        }

        return dataSet;
    }

    public static string ReadTxt(string file, string charsetName) 
    {
        Stream _is = new FileStream(file);
        byte[] targetArray = new byte[_is.available()];
        int len;
        int off = 0;
        while ((len = _is.read(targetArray, off, targetArray.Length - off)) != -1 && off < targetArray.Length)
        {
            off += len;
        }
        _is.Close();

        return new string(targetArray, charsetName);
    }

    public static Dictionary<string, string[]> LoadCorpusWithException(string corpusPath) 
    {
        return LoadCorpusWithException(corpusPath, "UTF-8");
    }
}