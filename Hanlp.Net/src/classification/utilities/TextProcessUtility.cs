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
    public static string preprocess(string text)
    {
        return text.Replace("\\p{P}", " ").Replace("\\s+", " ").ToLower();
    }

    /**
     * 提取关键词，在真实的应用场景中，还应该涉及到短语
     *
     * @param text
     * @return
     */
    public static string[] extractKeywords(string text)
    {
        List<Term> termList = NotionalTokenizer.segment(text);
        string[] wordArray = new string[termList.size()];
        Iterator<Term> iterator = termList.iterator();
        for (int i = 0; i < wordArray.Length; i++)
        {
            wordArray[i] = iterator.next().word;
        }
        return wordArray;
    }

    /**
     * 统计每个词的词频
     *
     * @param keywordArray
     * @return
     */
    public static Dictionary<string, int> getKeywordCounts(string[] keywordArray)
    {
        Dictionary<string, int> counts = new ();

        int counter;
        for (int i = 0; i < keywordArray.Length; ++i)
        {
            counter = counts.get(keywordArray[i]);
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
    public static Dictionary<string, string[]> loadCorpus(string path)
    {
        Dictionary<string, string[]> dataSet = new ();
        File root = new File(path);
        File[] folders = root.listFiles();
        if (folders == null) return null;
        foreach (File folder in folders)
        {
            if (folder.isFile()) continue;
            File[] files = folder.listFiles();
            if (files == null) continue;
            string[] documents = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = IOUtil.readTxt(files[i]);
            }
            dataSet.Add(folder.getName(), documents);
        }

        return dataSet;
    }

    /**
     * 加载一个文件夹下的所有语料
     *
     * @param folderPath
     * @return
     */
    public static Dictionary<string, string[]> loadCorpusWithException(string folderPath, string charsetName) 
    {
        if (folderPath == null) throw new ArgumentException("参数 folderPath == null");
        File root = new File(folderPath);
        if (!root.exists()) throw new ArgumentException(string.Format("目录 %s 不存在", root));
        if (!root.isDirectory())
            throw new ArgumentException(string.Format("目录 %s 不是一个目录", root));

        Dictionary<string, string[]> dataSet = new Dictionary<string, string[]>();
        File[] folders = root.listFiles();
        if (folders == null) return null;
        foreach (File folder in folders)
        {
            if (folder.isFile()) continue;
            File[] files = folder.listFiles();
            if (files == null) continue;
            string[] documents = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = readTxt(files[i], charsetName);
            }
            dataSet.Add(folder.getName(), documents);
        }

        return dataSet;
    }

    public static string readTxt(File file, string charsetName) 
    {
        Stream _is = new Stream(file);
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

    public static Dictionary<string, string[]> loadCorpusWithException(string corpusPath) 
    {
        return loadCorpusWithException(corpusPath, "UTF-8");
    }
}