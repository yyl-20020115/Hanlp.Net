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
        return text.replaceAll("\\p{P}", " ").replaceAll("\\s+", " ").toLowerCase(Locale.getDefault());
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
        Dictionary<string, int> counts = new HashMap<string, int>();

        int counter;
        for (int i = 0; i < keywordArray.Length; ++i)
        {
            counter = counts.get(keywordArray[i]);
            if (counter == null)
            {
                counter = 0;
            }
            counts.put(keywordArray[i], ++counter); //增加词频
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
        for (File folder : folders)
        {
            if (folder.isFile()) continue;
            File[] files = folder.listFiles();
            if (files == null) continue;
            string[] documents = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = IOUtil.readTxt(files[i].getAbsolutePath());
            }
            dataSet.put(folder.getName(), documents);
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
        if (folderPath == null) throw new IllegalArgumentException("参数 folderPath == null");
        File root = new File(folderPath);
        if (!root.exists()) throw new IllegalArgumentException(string.format("目录 %s 不存在", root.getAbsolutePath()));
        if (!root.isDirectory())
            throw new IllegalArgumentException(string.format("目录 %s 不是一个目录", root.getAbsolutePath()));

        Dictionary<string, string[]> dataSet = new TreeMap<string, string[]>();
        File[] folders = root.listFiles();
        if (folders == null) return null;
        for (File folder : folders)
        {
            if (folder.isFile()) continue;
            File[] files = folder.listFiles();
            if (files == null) continue;
            string[] documents = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                documents[i] = readTxt(files[i], charsetName);
            }
            dataSet.put(folder.getName(), documents);
        }

        return dataSet;
    }

    public static string readTxt(File file, string charsetName) 
    {
        FileInputStream is = new FileInputStream(file);
        byte[] targetArray = new byte[is.available()];
        int len;
        int off = 0;
        while ((len = is.read(targetArray, off, targetArray.Length - off)) != -1 && off < targetArray.Length)
        {
            off += len;
        }
        is.close();

        return new string(targetArray, charsetName);
    }

    public static Dictionary<string, string[]> loadCorpusWithException(string corpusPath) 
    {
        return loadCorpusWithException(corpusPath, "UTF-8");
    }
}