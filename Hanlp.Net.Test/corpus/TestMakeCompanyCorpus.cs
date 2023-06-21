/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/18 19:48</create-date>
 *
 * <copyright file="TestMakeCompanyCorpus.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus;




/**
 * @author hankcs
 */
[TestClass]
public class TestMakeCompanyCorpus : TestCase
{
//    public void testMake() 
//    {
//        DijkstraSegment segment = new DijkstraSegment();
//        String line = null;
//        TextReader bw = new TextReader(new InputStreamReader(new FileStream("D:\\Doc\\语料库\\company.dic")));
//        TextWriter br = new TextWriter(new StreamWriter(new FileStream("data/test/nt/company.txt")));
//        int limit = int.MaxValue;
//        while ((line = bw.ReadLine()) != null && limit-- > 0)
//        {
//            if (line.EndsWith("）")) continue;
//            if (line.Length < 4) continue;
//            if (line.Contains("个体") || line.Contains("个人"))
//            {
//                continue;
//            }
//            List<Term> termList = segment.seg(line);
//            if (termList.size() == 0) continue;
//            Term last = termList.get(termList.size() - 1);
//            last.nature = Nature.nis;
//            br.Write("[");
//            for (Term term : termList)
//            {
//                br.Write(term.ToString());
//                if (term != last) br.Write(" ");
//            }
//            br.Write("]/ntc");
//            br.newLine();
//            br.flush();
//        }
//        bw.Close();
//        br.Close();
//    }
//
//    public void testParse() 
//    {
//        EasyDictionary dictionary = EasyDictionary.create("data/dictionary/2014_dictionary.txt");
//        NTDictionaryMaker nsDictionaryMaker = new NTDictionaryMaker(dictionary);
//        // CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\", new CorpusLoader.Handler()
//        CorpusLoader.walk("data/test/nt/part/", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                nsDictionaryMaker.compute(document.getComplexSentenceList());
//            }
//        });
//        nsDictionaryMaker.saveTxtTo("D:\\JavaProjects\\HanLP\\data\\dictionary\\organization\\outerNT");
//    }
//
//    public void testSplitLargeFile() 
//    {
//        String line = null;
//        TextReader br = new TextReader(new InputStreamReader(new FileStream("data/test/nt/company.txt")));
//        int id = 1;
//        TextWriter bw = new TextWriter(new StreamWriter(new FileStream("data/test/nt/part/" + id + ".txt")));
//        int count = 1;
//        while ((line = br.ReadLine()) != null)
//        {
//            if (count == 1000)
//            {
//                bw.Close();
//                bw = new TextWriter(new StreamWriter(new FileStream("data/test/nt/part/" + id + ".txt")));
//                ++id;
//                count = 0;
//            }
//            bw.Write(line);
//            bw.newLine();
//            ++count;
//        }
//        br.Close();
//    }
//
//    public void testCase() 
//    {
//        HanLP.Config.enableDebug();
//        DijkstraSegment segment = new DijkstraSegment();
//        segment.enableOrganizationRecognize(true);
//        Console.WriteLine(segment.seg("黑龙江建筑职业技术学院近百学生发生冲突"));
//    }
//
//    public void testCombine() 
//    {
//        DictionaryMaker.combine("data/dictionary/organization/nt.txt", "data/dictionary/organization/outerNT.txt").saveTxtTo("data/dictionary/organization/nt.txt");
//    }
}
