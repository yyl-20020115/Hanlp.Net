/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/2 20:54</create-date>
 *
 * <copyright file="NeuralNetworkDependencyParser.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dependency.nnparser;



/**
 * 基于神经网络分类模型arc-standard转移动作的判决式依存句法分析器
 * @author hankcs
 */
public class NeuralNetworkDependencyParser : AbstractDependencyParser
{
    private parser_dll parser_dll;

    public NeuralNetworkDependencyParser(Segment segment)
    {
        super(segment);
        parser_dll = new parser_dll();
        setDeprelTranslater(ConfigOption.DEPRL_DESCRIPTION_PATH).enableDeprelTranslator(true);
    }

    public NeuralNetworkDependencyParser()
    {
        this(NLPTokenizer.ANALYZER);
    }

    //@Override
    public CoNLLSentence parse(List<Term> termList)
    {
        List<string> posTagList = PosTagUtil.to863(termList);
        List<string> wordList = new ArrayList<string>(termList.size());
        for (Term term : termList)
        {
            wordList.add(term.word);
        }
        List<int> heads = new ArrayList<int>(termList.size());
        List<string> deprels = new ArrayList<string>(termList.size());
        parser_dll.parse(wordList, posTagList, heads, deprels);

        CoNLLWord[] wordArray = new CoNLLWord[termList.size()];
        for (int i = 0; i < wordArray.Length; ++i)
        {
            wordArray[i] = new CoNLLWord(i + 1, wordList.get(i), posTagList.get(i), termList.get(i).nature.toString());
            wordArray[i].DEPREL = deprels.get(i);
        }
        for (int i = 0; i < wordArray.Length; ++i)
        {
            int index = heads.get(i) - 1;
            if (index < 0)
            {
                wordArray[i].HEAD = CoNLLWord.ROOT;
                continue;
            }
            wordArray[i].HEAD = wordArray[index];
        }
        return new CoNLLSentence(wordArray);
    }

    /**
     * 分析句子的依存句法
     *
     * @param termList 句子，可以是任何具有词性标注功能的分词器的分词结果
     * @return CoNLL格式的依存句法树
     */
    public static CoNLLSentence compute(List<Term> termList)
    {
        return new NeuralNetworkDependencyParser().parse(termList);
    }

    /**
     * 分析句子的依存句法
     *
     * @param sentence 句子
     * @return CoNLL格式的依存句法树
     */
    public static CoNLLSentence compute(string sentence)
    {
        return new NeuralNetworkDependencyParser().parse(sentence);
    }
}
