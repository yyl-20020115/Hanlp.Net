/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-04 PM7:29</create-date>
 *
 * <copyright file="IOUtility.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.utilities.io;
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dependency.nnparser;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.model;
using Instance = com.hankcs.hanlp.model.perceptron.instance.Instance;

namespace com.hankcs.hanlp.model.perceptron.utility;




/**
 * @author hankcs
 */
public class IOUtility : IOUtil
{
    private static Pattern PATTERN_SPACE = Pattern.compile("\\s+");

    public static string[] readLineToArray(string line)
    {
        line = line.Trim();
        if (line.Length == 0) return new string[0];
        return PATTERN_SPACE.Split(line);
    }

    public static int loadInstance(string path, InstanceHandler handler) 
    {
        ConsoleLogger logger = new ConsoleLogger();
        int size = 0;
        File root = new File(path);
        File[] allFiles;
        if (root.isDirectory())
        {
            allFiles = root.listFiles();
            /*
             * new FileFilter()
                        {
                            //@Override
                            public bool accept(File pathname)
                            {
                                return pathname.isFile() && pathname.getName().EndsWith(".txt");
                            }
                        }
             */
        }
        else
        {
            allFiles = new File[]{root};
        }

        foreach (File file in allFiles)
        {
            TextReader br = new TextReader(new InputStreamReader(new FileStream(file), "UTF-8"));
            string line;
            while ((line = br.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0)
                {
                    continue;
                }
                Sentence sentence = Sentence.create(line);
                if (sentence.wordList.Count == 0) continue;
                ++size;
                if (size % 1000 == 0)
                {
                    logger.err("%c语料: %dk...", 13, size / 1000);
                }
                // debug
//                if (size == 100) break;
                if (handler.process(sentence)) break;
            }
        }

        return size;
    }

    public static double[] evaluate(Instance[] instances, LinearModel model)
    {
        int[] stat = new int[2];
        for (int i = 0; i < instances.Length; i++)
        {
            evaluate(instances[i], model, stat);
            if (i % 100 == 0 || i == instances.Length - 1)
            {
                Console.Error.WriteLine("%c进度: %.2f%%", 13, (i + 1) / (float) instances.Length * 100);
                System.err.flush();
            }
        }
        return new double[]{stat[1] / (double) stat[0] * 100};
    }

    public static void evaluate(Instance instance, LinearModel model, int[] stat)
    {
        int[] predLabel = new int[instance.Length()];
        model.viterbiDecode(instance, predLabel);
        stat[0] += instance.tagArray.Length;
        for (int i = 0; i < predLabel.Length; i++)
        {
            if (predLabel[i] == instance.tagArray[i])
            {
                ++stat[1];
            }
        }
    }
}
