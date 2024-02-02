/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-07-02 8:49 PM</create-date>
 *
 * <copyright file="HMMPOSTagger.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.hmm;



/**
 * @author hankcs
 */
public class HMMPOSTagger : HMMTrainer , POSTagger
{
    POSTagSet tagSet;

    public HMMPOSTagger(HiddenMarkovModel model)
        : base(model)
    {
        ;
        tagSet = new POSTagSet();
    }

    public HMMPOSTagger()
        : base()
    {
       ;
        tagSet = new POSTagSet();
    }

    //@Override
    protected List<string[]> convertToSequence(Sentence sentence)
    {
        List<Word> wordList = sentence.toSimpleWordList();
        List<string[]> xyList = new (wordList.Count);
        foreach (Word word in wordList)
        {
            xyList.Add(new string[]{word.Value, word.Label});
        }
        return xyList;
    }

    //@Override
    protected override TagSet getTagSet()
    {
        return tagSet;
    }

    //@Override
    public string[] Tag(params string[] words)
    {
        int[] obsArray = new int[words.Length];
        for (int i = 0; i < obsArray.Length; i++)
        {
            obsArray[i] = vocabulary.idOf(words[i]);
        }
        int[] tagArray = new int[obsArray.Length];
        model.predict(obsArray, tagArray);
        string[] tags = new string[obsArray.Length];
        for (int i = 0; i < tagArray.Length; i++)
        {
            tags[i] = tagSet.stringOf(tagArray[i]);
        }

        return tags;
    }

    //@Override
    public string[] Tag(List<string> wordList)
    {
        return Tag(wordList.ToArray());
    }
}
