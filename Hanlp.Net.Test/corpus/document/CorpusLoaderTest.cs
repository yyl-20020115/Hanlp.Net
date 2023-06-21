namespace com.hankcs.hanlp.corpus.document;



[TestClass]
public class CorpusLoaderTest : TestCase
{
//    public void testMultiThread() 
//    {
//        CorpusLoader.HandlerThread[] handlerThreadArray = new CorpusLoader.HandlerThread[4];
//        for (int i = 0; i < handlerThreadArray.Length; ++i)
//        {
//            handlerThreadArray[i] = new CorpusLoader.HandlerThread(String.valueOf(i))
//            {
//                //@Override
//                public void handle(Document document)
//                {
//
//                }
//            };
//        }
//        CorpusLoader.walk("data/2014", handlerThreadArray);
//    }
//
//    public void testSingleThread() 
//    {
//        CorpusLoader.walk("data/2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//
//            }
//        });
//    }
//
//    public void testCombineToTxt() 
//    {
//        TextWriter bw = new TextWriter(new StreamWriter(new FileStream("D:\\Doc\\语料库\\2014_cn.txt"), "UTF-8"));
//        CorpusLoader.walk("D:\\Doc\\语料库\\2014_hankcs", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                try
//                {
//                    for (List<Word> sentence : document.getSimpleSentenceList())
//                    {
//                        for (IWord word : sentence)
//                        {
//                            bw.Write(word.Value);
//                            bw.Write(' ');
//                        }
//                        bw.newLine();
//                    }
//                    bw.newLine();
//                }
//                catch (Exception e)
//                {
//                    //e.printStackTrace();
//                }
//            }
//        });
//        bw.Close();
//    }
//
//    public void testConvert2SimpleSentenceList() 
//    {
//        List<List<Word>> simpleSentenceList = CorpusLoader.convert2SimpleSentenceList("data/2014");
//        Console.WriteLine(simpleSentenceList.get(0));
//    }
//
//    public void testMakePersonCustomDictionary() 
//    {
//        DictionaryMaker dictionaryMaker = new DictionaryMaker();
//        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                List<List<IWord>> complexSentenceList = document.getComplexSentenceList();
//                for (List<IWord> wordList : complexSentenceList)
//                {
//                    for (IWord word : wordList)
//                    {
//                        if (word.getLabel().StartsWith("nr"))
//                        {
//                            dictionaryMaker.Add(word);
//                        }
//                    }
//                }
//            }
//        });
//        dictionaryMaker.saveTxtTo("data/dictionary/custom/人名词典.txt");
//    }
//
//    public void testMakeOrganizationCustomDictionary() 
//    {
//        DictionaryMaker dictionaryMaker = new DictionaryMaker();
//        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                List<List<IWord>> complexSentenceList = document.getComplexSentenceList();
//                for (List<IWord> wordList : complexSentenceList)
//                {
//                    for (IWord word : wordList)
//                    {
//                        if (word.getLabel().StartsWith("nt"))
//                        {
//                            dictionaryMaker.Add(word);
//                        }
//                    }
//                }
//            }
//        });
//        dictionaryMaker.saveTxtTo("data/dictionary/custom/机构名词典.txt");
//    }
//
//    /**
//     * 语料库中有很多句号标注得不对，尝试纠正它们
//     * 比如“方言/n 版/n [新年/t 祝福/vn]/nz 。你/rr 的/ude1 一段/mq 话/n ”
//     * @
//     */
//    public void testAdjustDot() 
//    {
//        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new CorpusLoader.Handler()
//        {
//            int id = 0;
//            //@Override
//            public void handle(Document document)
//            {
//                try
//                {
//                    TextWriter bw = new TextWriter(new StreamWriter(new FileStream("D:\\Doc\\语料库\\2014_hankcs\\" + (++id) + ".txt"), "UTF-8"));
//                    for (List<IWord> wordList : document.getComplexSentenceList())
//                    {
//                        if (wordList.size() == 0) continue;
//                        for (IWord word : wordList)
//                        {
//                            if (word.Value.Length > 1 && word.Value[0] == '。')
//                            {
//                                bw.Write("。/w");
//                                bw.Write(word.Value.substring(1));
//                                bw.Write('/');
//                                bw.Write(word.getLabel());
//                                bw.Write(' ');
//                                continue;
//                            }
//                            bw.Write(word.ToString());
//                            bw.Write(' ');
//                        }
//                        bw.newLine();
//                    }
//                    bw.Close();
//                }
//                catch (FileNotFoundException e)
//                {
//                    //e.printStackTrace();
//                }
//                catch (UnsupportedEncodingException e)
//                {
//                    //e.printStackTrace();
//                }
//                catch (IOException e)
//                {
//                    //e.printStackTrace();
//                }
//            }
//        });
//    }
//
//    public void testLoadMyCorpus() 
//    {
//        CorpusLoader.walk("D:\\Doc\\语料库\\2014_hankcs\\", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                for (List<IWord> wordList : document.getComplexSentenceList())
//                {
//                    Console.WriteLine(wordList);
//                }
//            }
//        });
//
//    }
//
//    /**
//     * 有些引号不对
//     * @
//     */
//    public void testFindQuote() 
//    {
//        CorpusLoader.walk("D:\\Doc\\语料库\\2014_hankcs\\", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                for (List<Word> wordList : document.getSimpleSentenceList())
//                {
//                    for (Word word : wordList)
//                    {
//                        if(word.value.Length > 1 && word.value.EndsWith("\""))
//                        {
//                            Console.WriteLine(word);
//                        }
//                    }
//                }
//            }
//        });
//    }
}