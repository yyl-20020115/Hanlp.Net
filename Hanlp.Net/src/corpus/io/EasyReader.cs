/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/7/29 16:35</create-date>
 *
 * <copyright file="DumpReader.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.io;


/**
 * 文本读取工具
 * @author hankcs
 */
public class EasyReader
{
    /**
     * 根目录
     */
    string root;
    /**
     * 是否输出进度
     */
    bool verbose = true;

    /**
     * 构造
     * @param root 根目录
     */
    public EasyReader(string root)
    {
        this.root = root;
    }

    /**
     * 构造
     * @param root 根目录
     * @param verbose 是否输出进度
     */
    public EasyReader(string root, bool verbose)
    {
        this.root = root;
        this.verbose = verbose;
    }

    /**
     * 读取
     * @param handler 处理逻辑
     * @param size 读取多少个文件
     * @throws Exception
     */
    public void read(LineHandler handler, int size)
    {
        File rootFile = new File(root);
        File[] files;
        if (rootFile.isDirectory())
        {
            files = rootFile.listFiles(new FileFilter()
            {
                //@Override
                public bool accept(File pathname)
                {
                    return pathname.isFile() && !pathname.getName().endsWith(".bin");
                }
            });
            if (files == null)
            {
                if (rootFile.isFile())
                    files = new File[]{rootFile};
                else return;
            }
        }
        else
        {
            files = new File[]{rootFile};
        }

        int n = 0;
        int totalAddress = 0;
        long start = DateTime.Now.Microsecond;
        for (File file : files)
        {
            if (size-- == 0) break;
            if (file.isDirectory()) continue;
            if (verbose) System._out.printf("正在处理%s, %d / %d\n", file.getName(), ++n, files.length);
            IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(file.getAbsolutePath());
            while (lineIterator.hasNext())
            {
                ++totalAddress;
                string line = lineIterator.next();
                if (line.length() == 0) continue;
                handler.handle(line);
            }
        }
        handler.done();
        if (verbose) System._out.printf("处理了 %.2f 万行，花费了 %.2f min\n", totalAddress / 10000.0, (DateTime.Now.Microsecond - start) / 1000.0 / 60.0);
    }

    /**
     * 读取
     * @param handler 处理逻辑
     * @throws Exception
     */
    public void read(LineHandler handler)
    {
        read(handler, int.MAX_VALUE);
    }
}
