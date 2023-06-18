/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-07-02 9:15 PM</create-date>
 *
 * <copyright file="HMMNERecognizer.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.model.hmm;



/**
 * @author hankcs
 */
public class HMMNERecognizer : HMMTrainer : NERecognizer
{
    NERTagSet tagSet;

    public HMMNERecognizer(HiddenMarkovModel model)
    {
        super(model);
        tagSet = new NERTagSet();
        tagSet.nerLabels.Add("nr");
        tagSet.nerLabels.Add("ns");
        tagSet.nerLabels.Add("nt");
    }

    public HMMNERecognizer()
    {
        this(new FirstOrderHiddenMarkovModel());
    }

    //@Override
    protected List<string[]> convertToSequence(Sentence sentence)
    {
        List<string[]> collector = Utility.convertSentenceToNER(sentence, tagSet);
        for (string[] pair : collector)
        {
            pair[1] = pair[2];
        }

        return collector;
    }

    //@Override
    protected TagSet getTagSet()
    {
        return tagSet;
    }

    //@Override
    public string[] recognize(string[] wordArray, string[] posArray)
    {
        int[] obsArray = new int[wordArray.Length];
        for (int i = 0; i < obsArray.Length; i++)
        {
            obsArray[i] = vocabulary.idOf(wordArray[i]);
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
    public NERTagSet getNERTagSet()
    {
        return tagSet;
    }
}
