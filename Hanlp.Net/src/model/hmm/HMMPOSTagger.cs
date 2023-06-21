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
namespace com.hankcs.hanlp.model.hmm;



/**
 * @author hankcs
 */
public class HMMPOSTagger : HMMTrainer : POSTagger
{
    POSTagSet tagSet;

    public HMMPOSTagger(HiddenMarkovModel model)
    {
        base(model);
        tagSet = new POSTagSet();
    }

    public HMMPOSTagger()
    {
        base();
        tagSet = new POSTagSet();
    }

    //@Override
    protected List<string[]> convertToSequence(Sentence sentence)
    {
        List<Word> wordList = sentence.toSimpleWordList();
        List<string[]> xyList = new ArrayList<string[]>(wordList.size());
        for (Word word : wordList)
        {
            xyList.Add(new string[]{word.Value, word.getLabel()});
        }
        return xyList;
    }

    //@Override
    protected TagSet getTagSet()
    {
        return tagSet;
    }

    //@Override
    public string[] tag(string... words)
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
    public string[] tag(List<string> wordList)
    {
        return tag(wordList.ToArray(new string[0]));
    }
}
