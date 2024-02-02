/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-13 2:05 PM</create-date>
 *
 * <copyright file="HMMSegmenter.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.tokenizer.lexical;
using System.Text;

namespace com.hankcs.hanlp.model.hmm;



/**
 * @author hankcs
 */
public class HMMSegmenter : HMMTrainer , Segmenter
{
    CWSTagSet tagSet;

    public HMMSegmenter(HiddenMarkovModel model)
        :base(model)
    {
        tagSet = new CWSTagSet();
    }

    public HMMSegmenter()
    {
        tagSet = new CWSTagSet();
    }

    //@Override
    public List<string> Segment(string text)
    {
        List<string> wordList = new ();
        Segment(text, CharTable.convert(text), wordList);
        return wordList;
    }

    //@Override
    public void Segment(string text, string normalized, List<string> output)
    {
        int[] obsArray = new int[text.Length];
        for (int i = 0; i < obsArray.Length; i++)
        {
            obsArray[i] = vocabulary.idOf(normalized.substring(i, i + 1));
        }
        int[] tagArray = new int[text.Length];
        model.predict(obsArray, tagArray);
        StringBuilder result = new StringBuilder();
        result.Append(text[0]);

        for (int i = 1; i < tagArray.Length; i++)
        {
            if (tagArray[i] == tagSet.B || tagArray[i] == tagSet.S)
            {
                output.Add(result.ToString());
                result.Length=0;
            }
            result.Append(text[i]);
        }
        if (result.Length != 0)
        {
            output.Add(result.ToString());
        }
    }

    //@Override
    protected List<string[]> convertToSequence(Sentence sentence)
    {
        List<string[]> charList = new ();
        foreach (Word w in sentence.toSimpleWordList())
        {
            string word = CharTable.convert(w.value);
            if (word.Length == 1)
            {
                charList.Add(new string[]{word, "S"});
            }
            else
            {
                charList.Add(new string[]{word.substring(0, 1), "B"});
                for (int i = 1; i < word.Length - 1; ++i)
                {
                    charList.Add(new string[]{word.substring(i, i + 1), "M"});
                }
                charList.Add(new string[]{word.substring(word.Length - 1), "E"});
            }
        }
        return charList;
    }

    //@Override
    protected override TagSet getTagSet()
    {
        return tagSet;
    }

    /**
     * 获取兼容旧的Segment接口
     *
     * @return
     */
    public Segment toSegment()
    {
        return new CustomSegment().enableCustomDictionary(false);
    }
    public class CustomSegment: Segment
    {
        //@Override
        protected override List<Term> segSentence(char[] sentence)
        {
            List<string> wordList = Segment(new string(sentence));
            List<Term> termList = new ();
            foreach (string word in wordList)
            {
                termList.Add(new Term(word, null));
            }
            return termList;
        }
    }
}