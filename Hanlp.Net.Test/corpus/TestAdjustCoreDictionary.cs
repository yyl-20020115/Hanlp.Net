/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/24 12:11</create-date>
 *
 * <copyright file="TestAdjustCoreDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus;



/**
 * 往核心词典里补充等效词串
 * @author hankcs
 */
public class TestAdjustCoreDictionary : TestCase
{

//    public static readonly string DATA_DICTIONARY_CORE_NATURE_DICTIONARY_TXT = HanLP.Config.CoreDictionaryPath;
//
//    public void testGetCompiledWordFromDictionary() 
//    {
//        DictionaryMaker dictionaryMaker = DictionaryMaker.load("data/test/CoreNatureDictionary.txt");
//        for (Map.Entry<String, Item> entry : dictionaryMaker.entrySet())
//        {
//            String word = entry.Key;
//            Item item = entry.Value;
//            if (word.matches(".##."))
//            {
//                Console.WriteLine(item);
//            }
//        }
//    }
//
//    public void testViewNGramDictionary() 
//    {
//        TFDictionary tfDictionary = new TFDictionary();
//        tfDictionary.load("data/dictionary/CoreNatureDictionary.ngram.txt");
//        for (Map.Entry<String, TermFrequency> entry : tfDictionary.entrySet())
//        {
//            String word = entry.Key;
//            TermFrequency frequency = entry.Value;
//            if (word.Contains("##"))
//            {
//                Console.WriteLine(frequency);
//            }
//        }
//    }
//
//    public void testSortCoreNatureDictionary() 
//    {
//        DictionaryMaker dictionaryMaker = DictionaryMaker.load(DATA_DICTIONARY_CORE_NATURE_DICTIONARY_TXT);
//        dictionaryMaker.saveTxtTo(DATA_DICTIONARY_CORE_NATURE_DICTIONARY_TXT);
//    }
//
//    public void testSimplifyNZ() 
//    {
//        DictionaryMaker nzDictionary = new DictionaryMaker();
//        CorpusLoader.walk("D:\\Doc\\语料库\\2014", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                for (List<IWord> sentence : document.getComplexSentenceList())
//                {
//                    for (IWord word : sentence)
//                    {
//                        if (word instanceof CompoundWord && "nz".Equals(word.getLabel()))
//                        {
//                            nzDictionary.Add(word);
//                        }
//                    }
//                }
//            }
//        });
//        nzDictionary.saveTxtTo("data/test/nz.txt");
//    }
//
//    public void testRemoveNumber() 
//    {
//        // 一些汉字数词留着没用，除掉它们
//        DictionaryMaker dictionaryMaker = DictionaryMaker.load(DATA_DICTIONARY_CORE_NATURE_DICTIONARY_TXT);
//        dictionaryMaker.saveTxtTo(DATA_DICTIONARY_CORE_NATURE_DICTIONARY_TXT, new DictionaryMaker.Filter()
//        {
//            //@Override
//            public bool onSave(Item item)
//            {
//                if (item.key.Length == 1 && "0123456789零○〇一二两三四五六七八九十廿百千万亿壹贰叁肆伍陆柒捌玖拾佰仟".IndexOf(item.key[0]) >= 0)
//                {
//                    Console.WriteLine(item);
//                    return false;
//                }
//
//                return true;
//            }
//        });
//    }
}
