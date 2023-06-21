/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/8 23:04</create-date>
 *
 * <copyright file="Util.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.corpus.io;


/**
 * 一些常用的IO操作
 *
 * @author hankcs
 */
public class IOUtil
{
    /**
     * 序列化对象
     *
     * @param o
     * @param path
     * @return
     */
    public static bool saveObjectTo(Object o, string path)
    {
        try
        {
            ObjectOutputStream oos = new ObjectOutputStream(IOUtil.newOutputStream(path));
            oos.writeObject(o);
            oos.Close();
        }
        catch (IOException e)
        {
            logger.warning("在保存对象" + o + "到" + path + "时发生异常" + e);
            return false;
        }

        return true;
    }

    /**
     * 反序列化对象
     *
     * @param path
     * @return
     */
    public static Object readObjectFrom(string path)
    {
        ObjectInputStream ois = null;
        try
        {
            ois = new ObjectInputStream(IOUtil.newInputStream(path));
            Object o = ois.readObject();
            ois.Close();
            return o;
        }
        catch (Exception e)
        {
            logger.warning("在从" + path + "读取对象时发生异常" + e);
        }

        return null;
    }

    /**
     * 一次性读入纯文本
     *
     * @param path
     * @return
     */
    public static string readTxt(string path)
    {
        if (path == null) return null;
        try
        {
            InputStream _in = IOAdapter == null ? new FileStream(path) :
                    IOAdapter.open(path);
            byte[] fileContent = new byte[_in.available()];
            int read = readBytesFromOtherInputStream(_in, fileContent);
            _in.Close();
            // 处理 UTF-8 BOM
            if (read >= 3 && fileContent[0] == -17 && fileContent[1] == -69 && fileContent[2] == -65)
                return new string(fileContent, 3, fileContent.Length - 3, Encoding.forName("UTF-8"));
            return new string(fileContent, Encoding.forName("UTF-8"));
        }
        catch (FileNotFoundException e)
        {
            logger.warning("找不到" + path + e);
            return null;
        }
        catch (IOException e)
        {
            logger.warning("读取" + path + "发生IO异常" + e);
            return null;
        }
    }

    public static LinkedList<string[]> readCsv(string path)
    {
        LinkedList<string[]> resultList = new LinkedList<string[]>();
        LinkedList<string> lineList = readLineList(path);
        foreach (string line in lineList)
        {
            resultList.Add(line.Split(","));
        }
        return resultList;
    }

    /**
     * 快速保存
     *
     * @param path
     * @param content
     * @return
     */
    public static bool saveTxt(string path, string content)
    {
        try
        {
            FileChannel fc = new FileStream(path).getChannel();
            fc.write(ByteBuffer.wrap(content.getBytes()));
            fc.Close();
        }
        catch (Exception e)
        {
            logger.throwing("IOUtil", "saveTxt", e);
            logger.warning("IOUtil saveTxt 到" + path + "失败" + e.ToString());
            return false;
        }
        return true;
    }

    public static bool saveTxt(string path, StringBuilder content)
    {
        return saveTxt(path, content.ToString());
    }

    public static bool saveCollectionToTxt<T>(ICollection<T> collection, string path)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Object o in collection)
        {
            sb.Append(o);
            sb.Append('\n');
        }
        return saveTxt(path, sb.ToString());
    }

    /**
     * 将整个文件读取为字节数组
     *
     * @param path
     * @return
     */
    public static byte[] readBytes(string path)
    {
        try
        {
            if (IOAdapter == null) return readBytesFromFileInputStream(new FileStream(path));

            InputStream _is = IOAdapter.open(path);
            if (_is is FileStream)
                return readBytesFromFileInputStream((FileStream)_is);
            else
                return readBytesFromOtherInputStream(_is);
        }
        catch (Exception e)
        {
            logger.warning("读取" + path + "时发生异常" + e);
        }

        return null;
    }

    public static string readTxt(string file, string charsetName) 
    {
        InputStream _is = IOAdapter.open(file);
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

    public static string baseName(string path)
    {
        if (path == null || path.Length == 0)
            return "";
        path = path.Replace("[/\\\\]+", "/");
        int len = path.Length,
                upCount = 0;
        while (len > 0)
        {
            //Remove trailing separator
            if (path[(len - 1)] == '/')
            {
                len--;
                if (len == 0)
                    return "";
            }
            int lastInd = path.LastIndexOf('/', len - 1);
            string fileName = path.substring(lastInd + 1, len);
            if (fileName.Equals("."))
            {
                len--;
            }
            else if (fileName.Equals(".."))
            {
                len -= 2;
                upCount++;
            }
            else
            {
                if (upCount == 0)
                    return fileName;
                upCount--;
                len -= fileName.Length;
            }
        }
        return "";
    }

    private static byte[] readBytesFromFileInputStream(FileStream fis) 
    {
        FileChannel channel = fis.getChannel();
        int fileSize = (int) channel.size();
        ByteBuffer byteBuffer = ByteBuffer.allocate(fileSize);
        channel.read(byteBuffer);
        byteBuffer.flip();
        byte[] bytes = byteBuffer.array();
        byteBuffer.Clear();
        channel.Close();
        fis.Close();
        return bytes;
    }

    /**
     * 将非FileStream的某InputStream中的全部数据读入到字节数组中
     *
     * @param is
     * @return
     * @
     */
    public static byte[] readBytesFromOtherInputStream(FileStream fs) 
    {
        var buffer = new byte[fs.Length];
        fs.Read(buffer);
        return buffer;
    }

    /**
     * 从InputStream读取指定长度的字节出来
     * @param is 流
     * @param targetArray output
     * @return 实际读取了多少字节，返回0表示遇到了文件尾部
     * @
     */
    public static int readBytesFromOtherInputStream(Stream _is, byte[] targetArray) 
    {
        //assert targetArray != null;
        if (targetArray.Length == 0) return 0;
        int len;
        int off = 0;
        while (off < targetArray.Length && (len = _is.read(targetArray, off, targetArray.Length - off)) != -1)
        {
            off += len;
        }
        return off;
    }

    public static LinkedList<string> readLineList(string path)
    {
        LinkedList<string> result = new LinkedList<string>();
        string txt = readTxt(path);
        if (txt == null) return result;
        StringTokenizer tokenizer = new StringTokenizer(txt, "\n");
        while (tokenizer.hasMoreTokens())
        {
            result.Add(tokenizer.nextToken());
        }

        return result;
    }

    /**
     * 用省内存的方式读取大文件
     *
     * @param path
     * @return
     */
    public static LinkedList<string> readLineListWithLessMemory(string path)
    {
        LinkedList<string> result = new LinkedList<string>();
        string line = null;
        bool first = true;
        try
        {
            TextReader bw = new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            while ((line = bw.ReadLine()) != null)
            {
                if (first)
                {
                    first = false;
                    if (!line.isEmpty() && line[0] == '\uFEFF')
                        line = line.substring(1);
                }
                result.Add(line);
            }
            bw.Close();
        }
        catch (Exception e)
        {
            logger.warning("加载" + path + "失败，" + e);
        }

        return result;
    }

    public static bool saveMapToTxt(Dictionary<Object, Object> map, string path)
    {
        return saveMapToTxt(map, path, "=");
    }

    public static bool saveMapToTxt(Dictionary<Object, Object> map, string path, string separator)
    {
        map = new (map);
        return saveEntrySetToTxt(map.entrySet(), path, separator);
    }

    public static bool saveEntrySetToTxt(HashSet<KeyValuePair<Object, Object>> entrySet, string path, string separator)
    {
        StringBuilder sbOut = new StringBuilder();
        foreach (KeyValuePair<Object, Object> entry in entrySet)
        {
            sbOut.Append(entry.Key);
            sbOut.Append(separator);
            sbOut.Append(entry.Value);
            sbOut.Append('\n');
        }
        return saveTxt(path, sbOut.ToString());
    }

    /**
     * 获取文件所在目录的路径
     * @param path
     * @return
     */
    public static string dirname(string path)
    {
        int index = path.LastIndexOf('/');
        if (index == -1) return path;
        return path.substring(0, index + 1);
    }

    public static LineIterator readLine(string path)
    {
        return new LineIterator(path);
    }

    /**
     * 删除本地文件
     * @param path
     * @return
     */
    public static bool deleteFile(string path)
    {
        return new File(path).delete();
    }

    /**
     * 去除文件第一行中的UTF8 BOM<br>
     *     这是Java的bug，且官方不会修复。参考 https://stackoverflow.com/questions/4897876/reading-utf-8-bom-marker
     * @param line 文件第一行
     * @return 去除BOM的部分
     */
    public static string removeUTF8BOM(string line)
    {
        if (line != null && line.StartsWith("\uFEFF")) // UTF-8 byte order mark (EF BB BF)
        {
            line = line.substring(1);
        }
        return line;
    }

    /**
     * 递归遍历获取目录下的所有文件
     *
     * @param path 根目录
     * @return 文件列表
     */
    public static List<File> fileList(string path)
    {
        List<File> fileList = new LinkedList<File>();
        File folder = new File(path);
        if (folder.isDirectory())
            enumerate(folder, fileList);
        else
            fileList.Add(folder); // 兼容路径为文件的情况
        return fileList;
    }

    /**
     * 递归遍历目录
     *
     * @param folder   目录
     * @param fileList 储存文件
     */
    private static void enumerate(File folder, List<File> fileList)
    {
        File[] fileArray = folder.listFiles();
        if (fileArray != null)
        {
            foreach (File file in fileArray)
            {
                if (file.isFile() && !file.getName().StartsWith(".")) // 过滤隐藏文件
                {
                    fileList.Add(file);
                }
                else
                {
                    enumerate(file, fileList);
                }
            }
        }
    }

    /**
     * 方便读取按行读取大文件
     */
    public class LineIterator : IEnumerable<string>, IEnumerator<string>
    {
        TextReader bw;
        string line;

        public LineIterator(TextReader bw)
        {
            this.bw = bw;
            try
            {
                line = bw.ReadLine();
                line = IOUtil.removeUTF8BOM(line);
            }
            catch (IOException e)
            {
                logger.warning("在读取过程中发生错误" + TextUtility.exceptionToString(e));
                bw = null;
            }
        }

        public LineIterator(string path)
        {
            try
            {
                bw = new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
                line = bw.ReadLine();
                line = IOUtil.removeUTF8BOM(line);
            }
            catch (FileNotFoundException e)
            {
                logger.warning("文件" + path + "不存在，接下来的调用会返回null\n" + TextUtility.exceptionToString(e));
                bw = null;
            }
            catch (IOException e)
            {
                logger.warning("在读取过程中发生错误" + TextUtility.exceptionToString(e));
                bw = null;
            }
        }

        public void Close()
        {
            if (bw == null) return;
            try
            {
                bw.Close();
                bw = null;
            }
            catch (IOException e)
            {
                logger.warning("关闭文件失败" + TextUtility.exceptionToString(e));
            }
            return;
        }

        //@Override
        public bool MoveNext()
        {
            if (bw == null) return false;
            if (line == null)
            {
                try
                {
                    bw.Close();
                    bw = null;
                }
                catch (IOException e)
                {
                    logger.warning("关闭文件失败" + TextUtility.exceptionToString(e));
                }
                return false;
            }

            return true;
        }

        //@Override
        public string next()
        {
            string preLine = line;
            try
            {
                if (bw != null)
                {
                    line = bw.ReadLine();
                    if (line == null && bw != null)
                    {
                        try
                        {
                            bw.Close();
                            bw = null;
                        }
                        catch (IOException e)
                        {
                            logger.warning("关闭文件失败" + TextUtility.exceptionToString(e));
                        }
                    }
                }
                else
                {
                    line = null;
                }
            }
            catch (IOException e)
            {
                logger.warning("在读取过程中发生错误" + TextUtility.exceptionToString(e));
            }
            return preLine;
        }

        //@Override
        public void Remove()
        {
            throw new InvalidOperationException("只读，不可写！");
        }

        //@Override
        public Iterator<string> iterator()
        {
            return this;
        }
    }

    /**
     * 创建一个TextWriter
     *
     * @param path
     * @return
     * @throws FileNotFoundException
     * @throws UnsupportedEncodingException
     */
    public static TextWriter newBufferedWriter(string path) 
    {
        return new TextWriter(new StreamWriter(IOUtil.newOutputStream(path), "UTF-8"));
    }

    /**
     * 创建一个TextReader
     * @param path
     * @return
     * @throws FileNotFoundException
     * @throws UnsupportedEncodingException
     */
    public static TextReader newBufferedReader(string path) 
    {
        return new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
    }

    public static TextWriter newBufferedWriter(string path, bool Append) 
    {
        return new TextWriter(new StreamWriter(new FileStream(path, Append), "UTF-8"));
    }

    /**
     * 创建输入流（经过IO适配器创建）
     * @param path
     * @return
     * @
     */
    public static InputStream newInputStream(string path) 
    {
        if (IOAdapter == null) return new FileStream(path);
        return IOAdapter.open(path);
    }

    /**
     * 创建输出流（经过IO适配器创建）
     * @param path
     * @return
     * @
     */
    public static OutputStream newOutputStream(string path) 
    {
        if (IOAdapter == null) return new FileStream(path);
        return IOAdapter.create(path);
    }

    /**
     * 获取最后一个分隔符的后缀
     * @param name
     * @param delimiter
     * @return
     */
    public static string getSuffix(string name, string delimiter)
    {
        return name.substring(name.LastIndexOf(delimiter) + 1);
    }

    /**
     * 写数组，用制表符分割
     * @param bw
     * @param params
     * @
     */
    public static void writeLine(TextWriter bw, params string[] _params) 
    {
        for (int i = 0; i < _params.Length - 1; i++)
        {
            bw.write(_params[i]);
            bw.write('\t');
        }
        bw.write(_params[_params.Length - 1]);
    }

    /**
     * 加载词典，词典必须遵守HanLP核心词典格式
     * @param pathArray 词典路径，可以有任意个。每个路径支持用空格表示默认词性，比如“全国地名大全.txt ns”
     * @return 一个储存了词条的map
     * @ 异常表示加载失败
     */
    public static Dictionary<string, CoreDictionary.Attribute> loadDictionary(params string[] pathArray) 
    {
        var map = new Dictionary<string, CoreDictionary.Attribute>();
        foreach (string path in pathArray)
        {
            File file = new File(path);
            string fileName = file.getName();
            int natureIndex = fileName.LastIndexOf(' ');
            Nature defaultNature = Nature.n;
            if (natureIndex > 0)
            {
                string natureString = fileName.substring(natureIndex + 1);
                path = file.getParent() + File.separator + fileName.substring(0, natureIndex);
                if (natureString.Length > 0 && !natureString.EndsWith(".txt") && !natureString.EndsWith(".csv"))
                {
                    defaultNature = Nature.create(natureString);
                }
            }
            TextReader br = new TextReader(new InputStreamReader(IOUtil.newInputStream(path), "UTF-8"));
            loadDictionary(br, map, path.EndsWith(".csv"), defaultNature);
        }

        return map;
    }

    /**
     * 将一个TextReader中的词条加载到词典
     * @param br 源
     * @param storage 储存位置
     * @ 异常表示加载失败
     */
    public static void loadDictionary(TextReader br, Dictionary<string, CoreDictionary.Attribute> storage, bool isCSV, Nature defaultNature) 
    {
        string splitter = "\\s";
        if (isCSV)
        {
            splitter = ",";
        }
        string line;
        bool firstLine = true;
        while ((line = br.ReadLine()) != null)
        {
            if (firstLine)
            {
                line = IOUtil.removeUTF8BOM(line);
                firstLine = false;
            }
            string param = line.Split(splitter);

            int natureCount = (param.Length - 1) / 2;
            CoreDictionary.Attribute attribute;
            if (natureCount == 0)
            {
                attribute = new CoreDictionary.Attribute(defaultNature);
            }
            else
            {
                attribute = new CoreDictionary.Attribute(natureCount);
                for (int i = 0; i < natureCount; ++i)
                {
                    attribute.nature[i] = LexiconUtility.convertStringToNature(param[1 + 2 * i]);
                    attribute.frequency[i] = int.parseInt(param[2 + 2 * i]);
                    attribute.totalFrequency += attribute.frequency[i];
                }
            }
            storage.Add(param[0], attribute);
        }
        br.Close();
    }

    public static void writeCustomNature(Stream _out, HashSet<Nature> customNatureCollector) 
    {
        if (customNatureCollector.size() == 0) return;
        _out.writeInt(-customNatureCollector.size());
        foreach (Nature nature in customNatureCollector)
        {
            TextUtility.writeString(nature.ToString(), _out);
        }
    }

    /**
     * 本地文件是否存在
     * @param path
     * @return
     */
    public static bool isFileExisted(string path)
    {
        File file = new File(path);
        return file.isFile() && file.exists();
    }
}
