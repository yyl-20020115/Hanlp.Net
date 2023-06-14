namespace com.hankcs.hanlp.corpus.dictionary;


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
//            int length = text.length();
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
//            if (text.length() != length)
//            {
//                BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(file)));
//                bw.write(text);
//                bw.close();
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
//        final TFDictionary tfDictionary = new TFDictionary();
//        CorpusLoader.walk("D:\\JavaProjects\\CorpusToolBox\\data\\2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                for (List<IWord> wordList : document.getComplexSentenceList())
//                {
//                    for (IWord word : wordList)
//                    {
//                        if (word instanceof CompoundWord && word.getLabel().equals("ns"))
//                        {
//                            tfDictionary.add(word.toString());
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
//        BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(HanLP.Config.BiGramDictionaryPath + "adjust.txt"), "UTF-8"));
//        while (iterator.hasNext())
//        {
//            String line = iterator.next();
//            String[] params = line.split(" ");
//            String first = params[0].split("@", 2)[0];
//            String second = params[0].split("@", 2)[1];
////            if (params.length != 2)
////                System.err.println(line);
//            int biFrequency = Integer.parseInt(params[1]);
//            CoreDictionary.Attribute attribute = CoreDictionary.get(first + second);
//            if (attribute != null && (first.length() == 1 || second.length() == 1))
//            {
//                Console.WriteLine(line);
//                continue;
//            }
//            bw.write(line);
//            bw.newLine();
//        }
//        bw.close();
//    }
//
//    public void testRemoveLabelD() 
//    {
//        Set<String> nameFollowers = new TreeSet<String>();
//        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(HanLP.Config.BiGramDictionaryPath);
//        while (lineIterator.hasNext())
//        {
//            String line = lineIterator.next();
//            String[] words = line.split("\\s")[0].split("@");
//            if (words[0].equals(Predefine.TAG_PEOPLE))
//            {
//                nameFollowers.add(words[1]);
//            }
//        }
//        DictionaryMaker dictionary = DictionaryMaker.load(HanLP.Config.PersonDictionaryPath);
//        for (Map.Entry<String, Item> entry : dictionary.entrySet())
//        {
//            String key = entry.getKey();
//            int dF = entry.getValue().getFrequency("D");
//            if (key.length() == 1 && 0 < dF && dF < 100)
//            {
//                CoreDictionary.Attribute attribute = CoreDictionary.get(key);
//                if (nameFollowers.contains(key)
//                    || (attribute != null && attribute.hasNatureStartsWith("v") && attribute.totalFrequency > 1000)
//                    )
//                {
//                    Console.WriteLine(key);
//                    entry.getValue().removeLabel("D");
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
//                    if (inner.getLabel().equals("nr"))
//                    {
//                        continue;
//                    }
//                    // 如果需要人名，注销上面这句即可
//                    dictionaryMaker.add(inner);
//                }
//            }
//            // 暂时不统计人名
//            if (word.getLabel().equals("nr"))
//            {
//                continue;
//            }
//            // 如果需要人名，注销上面这句即可
//            dictionaryMaker.add(word);
//        }
//    }
//
//    public void testMakeDictionary() 
//    {
//        final DictionaryMaker dictionaryMaker = new DictionaryMaker();
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
//        Map<String, Integer> labelMap = new TreeMap<String, Integer>();
//        for (Item item : itemList)
//        {
//            for (Map.Entry<String, Integer> entry : item.labelMap.entrySet())
//            {
//                Integer frequency = labelMap.get(entry.getKey());
//                if (frequency == null) frequency = 0;
//                labelMap.put(entry.getKey(), frequency + entry.getValue());
//            }
//        }
//        for (String label : labelMap.keySet())
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