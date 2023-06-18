/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/18 19:47</create-date>
 *
 * <copyright file="NatureDictionaryMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * @author hankcs
 */
public class NatureDictionaryMaker : CommonDictionaryMaker
{
    public NatureDictionaryMaker()
        :base(null)
    {
    }

    //@Override
    protected void addToDictionary(List<List<IWord>> sentenceList)
    {
        logger.info("开始制作词典");
        // 制作NGram词典
        foreach (List<IWord> wordList in sentenceList)
        {
            IWord pre = null;
            foreach (IWord word in wordList)
            {
                // 制作词性词频词典
                dictionaryMaker.Add(word);
                if (pre != null)
                {
                    nGramDictionaryMaker.addPair(pre, word);
                }
                pre = word;
            }
        }
    }

    //@Override
    protected void roleTag(List<List<IWord>> sentenceList)
    {
        logger.info("开始标注");
        int i = 0;
        for (List<IWord> wordList : sentenceList)
        {
            logger.info(++i + " / " + sentenceList.size());
            for (IWord word : wordList)
            {
                Precompiler.compile(word);  // 编译为等效字符串
            }
            LinkedList<IWord> wordLinkedList = (LinkedList<IWord>) wordList;
            wordLinkedList.addFirst(new Word(Predefine.TAG_BIGIN, Nature.begin.toString()));
            wordLinkedList.addLast(new Word(Predefine.TAG_END, Nature.end.toString()));
        }
    }

    /**
     * 指定语料库文件夹，制作一份词频词典
     * @return
     */
    static bool makeCoreDictionary(string inPath, string outPath)
    {
        DictionaryMaker dictionaryMaker = new DictionaryMaker();
        HashSet<string> labelSet = new HashSet<string>();

        CorpusLoader.walk(inPath, new CT());
        if (outPath != null)
        return dictionaryMaker.saveTxtTo(outPath);
        return false;
    }
    public class CT: CorpusLoader.Handler
    {
        //@Override
        public void handle(Document document)
        {
            foreach (List<Word> sentence in document.getSimpleSentenceList(true))
            {
                foreach (Word word in sentence)
                {
                    if (shouldInclude(word))
                        dictionaryMaker.Add(word);
                }
            }
            //                for (List<Word> sentence : document.getSimpleSentenceList(false))
            //                {
            //                    for (Word word : sentence)
            //                    {
            //                        if (shouldInclude(word))
            //                            dictionaryMaker.Add(word);
            //                    }
            //                }
        }

        /**
         * 是否应当计算这个词语
         * @param word
         * @return
         */
        bool shouldInclude(Word word)
        {
            if ("m".Equals(word.label) || "mq".Equals(word.label) || "w".Equals(word.label) || "t".Equals(word.label))
            {
                if (!TextUtility.isAllChinese(word.value)) return false;
            }
            else if ("nr".Equals(word.label))
            {
                return false;
            }

            return true;
        }
    }
}
