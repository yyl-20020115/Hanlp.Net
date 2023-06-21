/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/29 21:11</create-date>
 *
 * <copyright file="TestICWB.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus;



/**
 * 玩玩ICWB的数据
 *
 * @author hankcs
 */
public class TestICWB : TestCase
{

//    public static readonly string PATH = "D:\\Doc\\语料库\\icwb2-data\\training\\msr_training.utf8";
//
//    public void testGenerateBMES() 
//    {
//        TextWriter bw = new TextWriter(new StreamWriter(new FileStream(PATH + ".bmes.txt")));
//        for (String line : IOUtil.readLineListWithLessMemory(PATH))
//        {
//            String[] wordArray = line.Split("\\s");
//            for (String word : wordArray)
//            {
//                if (word.Length == 1)
//                {
//                    bw.Write(word + "\tS\n");
//                }
//                else if (word.Length > 1)
//                {
//                    bw.Write(word[0] + "\tB\n");
//                    for (int i = 1; i < word.Length - 1; ++i)
//                    {
//                        bw.Write(word.charAt(i) + "\tM\n");
//                    }
//                    bw.Write(word.charAt(word.Length - 1) + "\tE\n");
//                }
//            }
//            bw.newLine();
//        }
//        bw.Close();
//    }
//
//    public void testDumpPeople2014ToBEMS() 
//    {
//        TextWriter bw = new TextWriter(new StreamWriter(new FileStream("D:\\Tools\\CRF++-0.58\\example\\seg_cn\\2014.txt")));
//        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                List<List<Word>> simpleSentenceList = document.getSimpleSentenceList();
//                for (List<Word> wordList : simpleSentenceList)
//                {
//                    try
//                    {
//                        for (Word word : wordList)
//                        {
//
//                            bw.Write(word.value);
//                            bw.Write(' ');
//
//                        }
//                        bw.newLine();
//                    }
//                    catch (IOException e)
//                    {
//                        //e.printStackTrace();
//                    }
//                }
//            }
//        });
//        bw.Close();
//    }
}
