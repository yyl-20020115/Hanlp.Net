/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-07-04 5:36 PM</create-date>
 *
 * <copyright file="PKU.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.corpus;



/**
 * @author hankcs
 */
public class PKU
{
    public static String PKU199801;
    public static String PKU199801_TRAIN = "data/test/pku98/199801-train.txt";
    public static String PKU199801_TEST = "data/test/pku98/199801-test.txt";
    public static String POS_MODEL = "/pos.bin";
    public static String NER_MODEL = "/ner.bin";
    public static readonly string PKU_98 = TestUtility.EnsureTestData("pku98", "http://hanlp.linrunsoft.com/release/corpus/pku98.zip");

    static PKU()
    {
        PKU199801 = PKU_98 + "/199801.txt";
        POS_MODEL = PKU_98 + POS_MODEL;
        NER_MODEL = PKU_98 +NER_MODEL;
        if (!IOUtil.isFileExisted(PKU199801_TRAIN))
        {
            List<String> all = new ();
            IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(PKU199801);
            while (lineIterator.MoveNext())
            {
                all.Add(lineIterator.next());
            }
            try
            {
                var bw = IOUtil.newBufferedWriter(PKU199801_TRAIN);
                foreach (String line in all.Take((int) (all.Count * 0.9)))
                {
                    bw.write(line);
                    bw.newLine();
                }
                bw.Close();

                bw = IOUtil.newBufferedWriter(PKU199801_TEST);
                foreach (String line in all.Skip((int) (all.Count * 0.9)).Take(all.Count))
                {
                    bw.write(line);
                    bw.newLine();
                }
                bw.Close();
            }
            catch (IOException e)
            {
                //e.printStackTrace();
            }
        }
    }
}
