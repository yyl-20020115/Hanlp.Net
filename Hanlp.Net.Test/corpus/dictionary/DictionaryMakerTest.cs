namespace com.hankcs.hanlp.corpus.dictionary;


[TestClass]
public class DictionaryMakerTest : TestCase
{
    // 部分标注有问题，比如逗号缺少标注等等，尝试修复它
//    public void testAdjust() 
//    {
//        List<File> fileList = FolderWalker.open("D:\\JavaProjects\\CorpusToolBox\\data\\2014\\");
//        for (File file : fileList)
//        {
//            handle(file);
//        }
//    }
//
//    private static void handle(File file)
//    {
//        try
//        {
//            String text = IOUtil.readTxt(file.getPath());
//            int Length = text.Length;
//            text = addW(text, "：");
//            text = addW(text, "？");
//            text = addW(text, "，");
//            text = addW(text, "）");
//            text = addW(text, "（");
//            text = addW(text, "！");
//            text = addW(text, "(");
//            text = addW(text, ")");
//            text = addW(text, ",");
//            text = addW(text, "‘");
//            text = addW(text, "’");
//            text = addW(text, "“");
//            text = addW(text, "”");
//            text = addW(text, ";");
//            text = addW(text, "……");
//            text = addW(text, "。");
//            text = addW(text, "、");
//            text = addW(text, "《");
//            text = addW(text, "》");
//            if (text.Length != Length)
//            {
//                TextWriter bw = new TextWriter(new StreamWriter(new FileStream(file)));
//                bw.write(text);
//                bw.Close();
//                Console.WriteLine("修正了" + file);
//            }
//        }
//        catch (Exception e)
//        {
//            e.printStackTrace();
//        }
//    }
//
//    private static String addW(String text, String c)
//    {
//        text = text.replaceAll("\\" + c + "/w ", c);
//        return text.replaceAll("\\" + c, c + "/w ");
//    }
//
//    public void testPlay() 
//    {
//        TFDictionary tfDictionary = new TFDictionary();
//        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                for (List<IWord> wordList : document.getComplexSentenceList())
//                {
//                    for (IWord word : wordList)
//                    {
//                        if (word instanceof CompoundWord && word.getLabel().Equals("ns"))
//                        {
//                            tfDictionary.Add(word.ToString());
//                        }
//                    }
//                }
//            }
//        });
//        tfDictionary.saveTxtTo("data/test/complex_ns.txt");
//    }
//
//    public void testAdjustNGram() 
//    {
//        IOUtil.LineIterator iterator = new IOUtil.LineIterator(HanLP.Config.BiGramDictionaryPath);
//        TextWriter bw = new TextWriter(new StreamWriter(new FileStream(HanLP.Config.BiGramDictionaryPath + "adjust.txt"), "UTF-8"));
//        while (iterator.MoveNext())
//        {
//            String line = iterator.next();
//            String[] params = line.Split(" ");
//            String first = params[0].Split("@", 2)[0];
//            String second = params[0].Split("@", 2)[1];
////            if (params.Length != 2)
////                Console.Error.WriteLine(line);
//            int biFrequency = int.parseInt(params[1]);
//            CoreDictionary.Attribute attribute = CoreDictionary.get(first + second);
//            if (attribute != null && (first.Length == 1 || second.Length == 1))
//            {
//                Console.WriteLine(line);
//                continue;
//            }
//            bw.write(line);
//            bw.newLine();
//        }
//        bw.Close();
//    }
//
//    public void testRemoveLabelD() 
//    {
//        HashSet<String> nameFollowers = new TreeSet<String>();
//        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(HanLP.Config.BiGramDictionaryPath);
//        while (lineIterator.MoveNext())
//        {
//            String line = lineIterator.next();
//            String[] words = line.Split("\\s")[0].Split("@");
//            if (words[0].Equals(Predefine.TAG_PEOPLE))
//            {
//                nameFollowers.Add(words[1]);
//            }
//        }
//        DictionaryMaker dictionary = DictionaryMaker.load(HanLP.Config.PersonDictionaryPath);
//        for (Map.Entry<String, Item> entry : dictionary.entrySet())
//        {
//            String key = entry.Key;
//            int dF = entry.Value.getFrequency("D");
//            if (key.Length == 1 && 0 < dF && dF < 100)
//            {
//                CoreDictionary.Attribute attribute = CoreDictionary.get(key);
//                if (nameFollowers.Contains(key)
//                    || (attribute != null && attribute.hasNatureStartsWith("v") && attribute.totalFrequency > 1000)
//                    )
//                {
//                    Console.WriteLine(key);
//                    entry.Value.removeLabel("D");
//                }
//            }
//        }
//
//        dictionary.saveTxtTo(HanLP.Config.PersonDictionaryPath);
//    }

//    public void testSingleDocument() 
//    {
//        Document document = CorpusLoader.convert2Document(new File("data/2014/0101/c1002-23996898.txt"));
//        DictionaryMaker dictionaryMaker = new DictionaryMaker();
//        Console.WriteLine(document);
//        addToDictionary(document, dictionaryMaker);
//        dictionaryMaker.saveTxtTo("data/dictionaryTest.txt");
//    }
//
//    private void addToDictionary(Document document, DictionaryMaker dictionaryMaker)
//    {
//        for (IWord word : document.getWordList())
//        {
//            if (word instanceof CompoundWord)
//            {
//                for (Word inner : ((CompoundWord)word).innerList)
//                {
//                    // 暂时不统计人名
//                    if (inner.getLabel().Equals("nr"))
//                    {
//                        continue;
//                    }
//                    // 如果需要人名，注销上面这句即可
//                    dictionaryMaker.Add(inner);
//                }
//            }
//            // 暂时不统计人名
//            if (word.getLabel().Equals("nr"))
//            {
//                continue;
//            }
//            // 如果需要人名，注销上面这句即可
//            dictionaryMaker.Add(word);
//        }
//    }
//
//    public void testMakeDictionary() 
//    {
//        DictionaryMaker dictionaryMaker = new DictionaryMaker();
//        CorpusLoader.walk("data/2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                addToDictionary(document, dictionaryMaker);
//            }
//        });
//        dictionaryMaker.saveTxtTo("data/2014_dictionary.txt");
//    }
//
//    public void testLoadItemList() 
//    {
//        List<Item> itemList = DictionaryMaker.loadAsItemList("data/2014_dictionary.txt");
//        Map<String, int> labelMap = new Dictionary<String, int>();
//        for (Item item : itemList)
//        {
//            for (Map.Entry<String, int> entry : item.labelMap.entrySet())
//            {
//                int frequency = labelMap.get(entry.Key);
//                if (frequency == null) frequency = 0;
//                labelMap.Add(entry.Key, frequency + entry.Value);
//            }
//        }
//        for (String label : labelMap.Keys)
//        {
//            Console.WriteLine(label);
//        }
//        Console.WriteLine(labelMap.size());
//    }
//
//    public void testLoadEasyDictionary() 
//    {
//        EasyDictionary dictionary = EasyDictionary.create("data/2014_dictionary.txt");
//        Console.WriteLine(dictionary.GetWordInfo("高峰"));
//    }
}