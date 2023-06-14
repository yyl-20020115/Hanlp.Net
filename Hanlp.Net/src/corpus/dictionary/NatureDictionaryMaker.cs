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
namespace com.hankcs.hanlp.corpus.dictionary;



/**
 * @author hankcs
 */
public class NatureDictionaryMaker : CommonDictionaryMaker
{
    public NatureDictionaryMaker()
    {
        super(null);
    }

    //@Override
    protected void addToDictionary(List<List<IWord>> sentenceList)
    {
        logger.info("开始制作词典");
        // 制作NGram词典
        for (List<IWord> wordList : sentenceList)
        {
            IWord pre = null;
            for (IWord word : wordList)
            {
                // 制作词性词频词典
                dictionaryMaker.add(word);
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
    static bool makeCoreDictionary(String inPath, String outPath)
    {
        final DictionaryMaker dictionaryMaker = new DictionaryMaker();
        final TreeSet<String> labelSet = new TreeSet<String>();

        CorpusLoader.walk(inPath, new CorpusLoader.Handler()
        {
            //@Override
            public void handle(Document document)
            {
                for (List<Word> sentence : document.getSimpleSentenceList(true))
                {
                    for (Word word : sentence)
                    {
                        if (shouldInclude(word))
                            dictionaryMaker.add(word);
                    }
                }
//                for (List<Word> sentence : document.getSimpleSentenceList(false))
//                {
//                    for (Word word : sentence)
//                    {
//                        if (shouldInclude(word))
//                            dictionaryMaker.add(word);
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
                if ("m".equals(word.label) || "mq".equals(word.label) || "w".equals(word.label) || "t".equals(word.label))
                {
                    if (!TextUtility.isAllChinese(word.value)) return false;
                }
                else if ("nr".equals(word.label))
                {
                    return false;
                }

                return true;
            }
        });
        if (outPath != null)
        return dictionaryMaker.saveTxtTo(outPath);
        return false;
    }
}
