/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 下午7:42</create-date>
 *
 * <copyright file="AbstractLexicalAnalyzer.java">
 * Copyright (c) 2018, Han He. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.AhoCorasick;
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.dictionary;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;
using System.Text;

namespace com.hankcs.hanlp.tokenizer.lexical;



/**
 * 词法分析器基类（中文分词、词性标注和命名实体识别）
 *
 * @author hankcs
 */
public class AbstractLexicalAnalyzer : CharacterBasedSegment, LexicalAnalyzer
{
    protected Segmenter segmenter;
    protected POSTagger posTagger;
    protected NERecognizer neRecognizer;
    /**
     * 字符类型表
     */
    protected static byte[] typeTable;
    /**
     * 是否执行规则分词（英文数字标点等的规则预处理）。规则永远是丑陋的，默认关闭。
     */
    protected bool _enableRuleBasedSegment = false;

    static AbstractLexicalAnalyzer()
    {
        typeTable = new byte[CharType.type.Length];
        Array.Copy(CharType.type, 0, typeTable, 0, typeTable.Length);
        foreach (char c in Predefine.CHINESE_NUMBERS.ToCharArray())
        {
            typeTable[c] = CharType.CT_CHINESE;
        }
        typeTable[CharTable.convert('·')] = CharType.CT_CHINESE;
    }

    public AbstractLexicalAnalyzer()
    {
    }

    public AbstractLexicalAnalyzer(Segmenter segmenter)
    {
        this.segmenter = segmenter;
    }

    public AbstractLexicalAnalyzer(Segmenter segmenter, POSTagger posTagger)
    {
        this.segmenter = segmenter;
        this.posTagger = posTagger;
    }

    public AbstractLexicalAnalyzer(Segmenter segmenter, POSTagger posTagger, NERecognizer neRecognizer)
    {
        this.segmenter = segmenter;
        this.posTagger = posTagger;
        this.neRecognizer = neRecognizer;
        if (posTagger != null)
        {
            config.speechTagging = true;
            if (neRecognizer != null)
            {
                config.ner = true;
            }
        }
    }

    /**
     * 分词
     *
     * @param sentence      文本
     * @param normalized    正规化后的文本
     * @param wordList      储存单词列表
     * @param attributeList 储存用户词典中的词性，设为null表示不查询用户词典
     */
    protected void segment(string sentence, string normalized, List<string> wordList, List<CoreDictionary.Attribute> attributeList)
    {
        if (attributeList != null)
        {
            int[] offset = new int[] { 0 };
            CustomDictionary.parseLongestText(sentence, new CPT());
            if (offset[0] != sentence.Length)
            {
                segmentAfterRule(sentence.Substring(offset[0]), normalized.Substring(offset[0]), wordList);
            }
        }
        else
        {
            segmentAfterRule(sentence, normalized, wordList);
        }
    }
    public class CPT : AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute>
    {
        //@Override
        public void Hit(int begin, int end, CoreDictionary.Attribute value)
        {
            if (begin != offset[0])
            {
                segmentAfterRule(sentence.substring(offset[0], begin), normalized.substring(offset[0], begin), wordList);
            }
            while (attributeList.Count < wordList.Count)
                attributeList.Add(null);
            wordList.Add( sentence.substring(begin, end));
            attributeList.Add(value);
            //assert wordList.Count == attributeList.Count : "词语列表与属性列表不等长";
            offset[0] = end;
        }
    }
    //@Override
    public void segment(string sentence, string normalized, List<string> wordList)
    {
        if (config.useCustomDictionary)
        {
            int[] offset = new int[] { 0 };
            CustomDictionary.parseLongestText(sentence, new Hit());
            if (offset[0] != sentence.Length)
            {
                segmentAfterRule(sentence.Substring(offset[0]), normalized.Substring(offset[0]), wordList);
            }
        }
        else
        {
            segmentAfterRule(sentence, normalized, wordList);
        }
    }
    public class Hit : AhoCorasickDoubleArrayTrie<CoreDictionary.Attribute>.IHit<CoreDictionary.Attribute>
    {
        //@Override
        public void Hit(int begin, int end, CoreDictionary.Attribute value)
        {
            if (begin != offset[0])
            {
                segmentAfterRule(sentence.substring(offset[0], begin), normalized.substring(offset[0], begin), wordList);
            }
            wordList.Add(sentence.substring(begin, end));
            offset[0] = end;
        }
    }

    /**
     * 中文分词
     *
     * @param sentence
     * @return
     */
    public List<string> segment(string sentence)
    {
        return segment(sentence, CharTable.convert(sentence));
    }

    //@Override
    public string[] recognize(string[] wordArray, string[] posArray)
    {
        return neRecognizer.recognize(wordArray, posArray);
    }

    //@Override
    public string[] tag(params string[] words)
    {
        return posTagger.tag(words);
    }

    //@Override
    public string[] tag(List<string> wordList)
    {
        return posTagger.tag(wordList);
    }

    //@Override
    public NERTagSet getNERTagSet()
    {
        return neRecognizer.getNERTagSet();
    }

    //@Override
    public Sentence analyze(string sentence)
    {
        if (sentence.Length==0)
        {
            return new Sentence(new List<corpus.document.sentence.word.IWord>());
        }
        string normalized = CharTable.convert(sentence);
        List<string> wordList = new ();
        List<CoreDictionary.Attribute> attributeList = segmentWithAttribute(sentence, normalized, wordList);

        string[] wordArray = new string[wordList.Count];
        int offset = 0;
        int id = 0;
        foreach (string word in wordList)
        {
            wordArray[id] = normalized.substring(offset, offset + word.Length);
            ++id;
            offset += word.Length;
        }

        List<IWord> termList = new (wordList.Count);
        if (posTagger != null)
        {
            string[] posArray = tag(wordArray);
            if (neRecognizer != null)
            {
                string[] nerArray = neRecognizer.recognize(wordArray, posArray);
                overwriteTag(attributeList, posArray);
                wordList.ToArray(wordArray);

                List<Word> result = new ();
                result.Add(new Word(wordArray[0], posArray[0]));
                string prePos = posArray[0];

                NERTagSet tagSet = getNERTagSet();
                for (int i = 1; i < nerArray.Length; i++)
                {
                    if (nerArray[i][0] == tagSet.B_TAG_CHAR || nerArray[i][0] == tagSet.S_TAG_CHAR || nerArray[i][0] == tagSet.O_TAG_CHAR)
                    {
                        termList.Add(result.Count > 1 ? new CompoundWord(result, prePos) : result.get(0));
                        result = new ();
                    }
                    result.Add(new Word(wordArray[i], posArray[i]));
                    if (nerArray[i][0] == tagSet.O_TAG_CHAR || nerArray[i][0] == tagSet.S_TAG_CHAR)
                    {
                        prePos = posArray[i];
                    }
                    else
                    {
                        prePos = NERTagSet.posOf(nerArray[i]);
                    }
                }
                if (result.Count != 0)
                {
                    termList.Add(result.Count > 1 ? new CompoundWord(result, prePos) : result.get(0));
                }
            }
            else
            {
                overwriteTag(attributeList, posArray);
                wordArray = wordList.ToArray();
                for (int i = 0; i < wordArray.Length; i++)
                {
                    termList.Add(new Word(wordArray[i], posArray[i]));
                }
            }
        }
        else
        {
            wordArray = wordList.ToArray();
            foreach (string word in wordArray)
            {
                termList.Add(new Word(word, null));
            }
        }

        return new Sentence(termList);
    }

    private void overwriteTag(List<CoreDictionary.Attribute> attributeList, string[] posArray)
    {
        int id;
        if (attributeList != null)
        {
            id = 0;
            foreach (CoreDictionary.Attribute attribute in attributeList)
            {
                if (attribute != null)
                    posArray[id] = attribute.nature[0].ToString();
                ++id;
            }
        }
    }

    /**
     * 这个方法会查询用户词典
     *
     * @param sentence
     * @param normalized
     * @return
     */
    public List<string> segment( string sentence,  string normalized)
    {
         List<string> wordList = new ();
        segment(sentence, normalized, wordList);
        return wordList;
    }

    /**
     * 分词时查询到一个用户词典中的词语，此处控制是否接受它
     *
     * @param begin 起始位置
     * @param end   终止位置
     * @param value 词性
     * @return true 表示接受
     * @deprecated 自1.6.7起废弃，强制模式下为最长匹配，否则按分词结果合并
     */
    protected bool acceptCustomWord(int begin, int end, CoreDictionary.Attribute value)
    {
        return config.forceCustomDictionary || (end - begin >= 4 && !value.hasNatureStartsWith("nr") && !value.hasNatureStartsWith("ns") && !value.hasNatureStartsWith("nt"));
    }

    //@Override
    protected List<Term> roughSegSentence(char[] sentence)
    {
        return null;
    }

    //@Override
    protected List<Term> segSentence(char[] sentence)
    {
        if (sentence.Length == 0)
        {
            return new();
        }
        string original = new string(sentence);
        CharTable.normalization(sentence);
        string normalized = new string(sentence);
        List<string> wordList = new ();
        List<CoreDictionary.Attribute> attributeList;
        attributeList = segmentWithAttribute(original, normalized, wordList);
        List<Term> termList = new (wordList.Count);
        int offset = 0;
        foreach (string word in wordList)
        {
            Term term = new Term(word, null);
            term.offset = offset;
            offset += term.Length;
            termList.Add(term);
        }
        if (config.speechTagging)
        {
            if (posTagger != null)
            {
                string[] wordArray = new string[wordList.Count];
                offset = 0;
                int id = 0;
                foreach (string word in wordList)
                {
                    wordArray[id] = normalized.substring(offset, offset + word.Length);
                    ++id;
                    offset += word.Length;
                }
                string[] posArray = tag(wordArray);
                IEnumerator<Term> iterator = termList.GetEnumerator();
                IEnumerator<CoreDictionary.Attribute> attributeIterator = attributeList == null ? null : attributeList.GetEnumerator();
                for (int i = 0; i < posArray.Length; i++)
                {
                    if (attributeIterator != null && attributeIterator.MoveNext())
                    {
                        CoreDictionary.Attribute attribute = attributeIterator.next();
                        if (attribute != null)
                        {
                            iterator.next().nature = attribute.nature[0]; // 使用词典中的词性覆盖词性标注器的结果
                            continue;
                        }
                    }
                    iterator.next().nature = Nature.create(posArray[i]);
                }

                if (config.ner && neRecognizer != null)
                {
                    List<Term> childrenList = null;
                    if (config.isIndexMode())
                    {
                        childrenList = new ();
                        iterator = termList.GetEnumerator();
                    }
                    termList = new (termList.Count);
                    string[] nerArray = recognize(wordArray, posArray);
                    wordArray = wordList.ToArray();
                    StringBuilder result = new StringBuilder();
                    result.Append(wordArray[0]);
                    if (childrenList != null)
                    {
                        childrenList.Add(iterator.next());
                    }
                    string prePos = posArray[0];
                    offset = 0;

                    for (int i = 1; i < nerArray.Length; i++)
                    {
                        NERTagSet tagSet = getNERTagSet();
                        if (nerArray[i][0] == tagSet.B_TAG_CHAR || nerArray[i][0] == tagSet.S_TAG_CHAR || nerArray[i][0] == tagSet.O_TAG_CHAR)
                        {
                            Term term = new Term(result.ToString(), Nature.create(prePos));
                            term.offset = offset;
                            offset += term.Length;
                            termList.Add(term);
                            if (childrenList != null)
                            {
                                if (childrenList.Count > 1)
                                {
                                    foreach (Term shortTerm in childrenList)
                                    {
                                        if (shortTerm.Length >= config.indexMode)
                                        {
                                            termList.Add(shortTerm);
                                        }
                                    }
                                }
                                childrenList.Clear();
                            }
                            result.Length=0;
                        }
                        result.Append(wordArray[i]);
                        if (childrenList != null)
                        {
                            childrenList.Add(iterator.next());
                        }
                        if (nerArray[i][0] == tagSet.O_TAG_CHAR || nerArray[i][0] == tagSet.S_TAG_CHAR)
                        {
                            prePos = posArray[i];
                        }
                        else
                        {
                            prePos = NERTagSet.posOf(nerArray[i]);
                        }
                    }
                    if (result.Length != 0)
                    {
                        Term term = new Term(result.ToString(), Nature.create(prePos));
                        term.offset = offset;
                        termList.Add(term);
                        if (childrenList != null)
                        {
                            if (childrenList.Count > 1)
                            {
                                foreach (Term shortTerm in childrenList)
                                {
                                    if (shortTerm.Length >= config.indexMode)
                                    {
                                        termList.Add(shortTerm);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Term term in termList)
                {
                    CoreDictionary.Attribute attribute = CoreDictionary.get(term.word);
                    if (attribute != null)
                    {
                        term.nature = attribute.nature[0];
                    }
                    else
                    {
                        term.nature = Nature.n;
                    }
                }
            }
        }
        return termList;
    }

    /**
     * CT_CHINESE区间交给统计分词，否则视作整个单位
     *
     * @param sentence
     * @param normalized
     * @param start
     * @param end
     * @param preType
     * @param wordList
     */
    private void pushPiece(string sentence, string normalized, int start, int end, byte preType, List<string> wordList)
    {
        if (preType == CharType.CT_CHINESE)
        {
            segmenter.segment(sentence[start .. end], normalized[start .. end], wordList);
        }
        else
        {
            wordList.Add(sentence[start .. end]);
        }
    }

    /**
     * 丑陋的规则系统
     *
     * @param sentence
     * @param normalized
     * @param wordList
     */
    protected void segmentAfterRule(string sentence, string normalized, List<string> wordList)
    {
        if (!_enableRuleBasedSegment)
        {
            segmenter.segment(sentence, normalized, wordList);
            return;
        }
        int start = 0;
        int end = start;
        byte preType = typeTable[normalized[(end)]];
        byte curType;
        while (++end < normalized.Length)
        {
            curType = typeTable[normalized[(end)]];
            if (curType != preType)
            {
                if (preType == CharType.CT_NUM)
                {
                    // 浮点数识别
                    if ("，,．.".IndexOf(normalized[(end)]) != -1)
                    {
                        if (end + 1 < normalized.Length)
                        {
                            if (typeTable[normalized[(end + 1)]] == CharType.CT_NUM)
                            {
                                continue;
                            }
                        }
                    }
                    else if ("年月日时分秒".IndexOf(normalized[(end)]) != -1)
                    {
                        preType = curType; // 交给统计分词
                        continue;
                    }
                }
                pushPiece(sentence, normalized, start, end, preType, wordList);
                start = end;
            }
            preType = curType;
        }
        if (end == normalized.Length)
            pushPiece(sentence, normalized, start, end, preType, wordList);
    }

    /**
     * 返回用户词典中的attribute的分词
     *
     * @param original
     * @param normalized
     * @param wordList
     * @return
     */
    private List<CoreDictionary.Attribute> segmentWithAttribute(string original, string normalized, List<string> wordList)
    {
        List<CoreDictionary.Attribute> attributeList;
        if (config.useCustomDictionary)
        {
            if (config.forceCustomDictionary)
            {
                attributeList = new ();
                segment(original, normalized, wordList, attributeList);
            }
            else
            {
                segmentAfterRule(original, normalized, wordList);
                attributeList = combineWithCustomDictionary(wordList);
            }
        }
        else
        {
            segmentAfterRule(original, normalized, wordList);
            attributeList = null;
        }
        return attributeList;
    }

    /**
     * 使用用户词典合并粗分结果
     *
     * @param vertexList 粗分结果
     * @return 合并后的结果
     */
    protected static List<CoreDictionary.Attribute> combineWithCustomDictionary(List<string> vertexList)
    {
        var wordNet= vertexList.ToArray();
        CoreDictionary.Attribute[] attributeArray = new CoreDictionary.Attribute[wordNet.Length];
        // DAT合并
        DoubleArrayTrie<CoreDictionary.Attribute> dat = CustomDictionary.dat;
        int Length = wordNet.Length;
        for (int i = 0; i < Length; ++i)
        {
            int state = 1;
            state = dat.transition(wordNet[i], state);
            if (state > 0)
            {
                int to = i + 1;
                int end = to;
                CoreDictionary.Attribute value = dat.output(state);
                for (; to < Length; ++to)
                {
                    state = dat.transition(wordNet[to], state);
                    if (state < 0) break;
                    CoreDictionary.Attribute output = dat.output(state);
                    if (output != null)
                    {
                        value = output;
                        end = to + 1;
                    }
                }
                if (value != null)
                {
                    combineWords(wordNet, i, end, attributeArray, value);
                    i = end - 1;
                }
            }
        }
        // BinTrie合并
        if (CustomDictionary.trie != null)
        {
            for (int i = 1; i < Length; ++i)
            {
                if (wordNet[i] == null) continue;
                BaseNode<CoreDictionary.Attribute> state = CustomDictionary.trie.transition(wordNet[i], 0);
                if (state != null)
                {
                    int to = i + 1;
                    int end = to;
                    CoreDictionary.Attribute value = state.Value();
                    for (; to < Length; ++to)
                    {
                        if (wordNet[to] == null) continue;
                        state = state.transition(wordNet[to], 0);
                        if (state == null) break;
                        if (state.Value != null)
                        {
                            value = state.Value();
                            end = to + 1;
                        }
                    }
                    if (value != null)
                    {
                        combineWords(wordNet, i, end, attributeArray, value);
                        i = end - 1;
                    }
                }
            }
        }
        vertexList.Clear();
        List<CoreDictionary.Attribute> attributeList = new ();
        for (int i = 0; i < wordNet.Length; i++)
        {
            if (wordNet[i] != null)
            {
                vertexList.Add(wordNet[i]);
                attributeList.Add(attributeArray[i]);
            }
        }
        return attributeList;
    }

    /**
     * 将连续的词语合并为一个
     *
     * @param wordNet 词图
     * @param start   起始下标（包含）
     * @param end     结束下标（不包含）
     * @param value   新的属性
     */
    private static void combineWords(string[] wordNet, int start, int end, CoreDictionary.Attribute[] attributeArray, CoreDictionary.Attribute value)
    {
        if (start + 1 != end)   // 小优化，如果只有一个词，那就不需要合并，直接应用新属性
        {
            StringBuilder sbTerm = new StringBuilder();
            for (int j = start; j < end; ++j)
            {
                if (wordNet[j] == null) continue;
                sbTerm.Append(wordNet[j]);
                wordNet[j] = null;
            }
            wordNet[start] = sbTerm.ToString();
        }
        attributeArray[start] = value;
    }

    /**
     * 是否执行规则分词（英文数字标点等的规则预处理）。规则永远是丑陋的，默认关闭。
     *
     * @param enableRuleBasedSegment 是否激活
     * @return 词法分析器对象
     */
    public AbstractLexicalAnalyzer enableRuleBasedSegment(bool enableRuleBasedSegment)
    {
        this._enableRuleBasedSegment = enableRuleBasedSegment;
        return this;
    }
}
