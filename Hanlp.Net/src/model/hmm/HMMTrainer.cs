/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-13 2:17 PM</create-date>
 *
 * <copyright file="HMMTrainer.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.model.perceptron.utility;

namespace com.hankcs.hanlp.model.hmm;



/**
 * @author hankcs
 */
public abstract class HMMTrainer
{
    HiddenMarkovModel model;
    Vocabulary vocabulary;

    public HMMTrainer(HiddenMarkovModel model, Vocabulary vocabulary)
    {
        this.model = model;
        this.vocabulary = vocabulary;
    }

    public HMMTrainer(HiddenMarkovModel model)
    {
        this(model, new Vocabulary());
    }

    public HMMTrainer()
    {
        this(new FirstOrderHiddenMarkovModel());
    }

    public class IH: InstanceHandler
    {
        //@Override
        public bool process(Sentence sentence)
        {
            sequenceList.Add(convertToSequence(sentence));
            return false;
        }
    }
    public void train(string corpus) 
    {
        List<List<string[]>> sequenceList = new ();
        IOUtility.loadInstance(corpus, new IH());

        TagSet tagSet = getTagSet();

        List<int[][]> sampleList = new (sequenceList.size());
        foreach (List<string[]> sequence in sequenceList)
        {
            int[][] sample = new int[2][sequence.size()];
            int i = 0;
            foreach (string[] os in sequence)
            {
                sample[0][i] = vocabulary.idOf(os[0]);
                //assert sample[0][i] != -1;
                sample[1][i] = tagSet.Add(os[1]);
                //assert sample[1][i] != -1;
                ++i;
            }
            sampleList.Add(sample);
        }

        model.train(sampleList);
        vocabulary.mutable = false;
    }

    protected abstract List<string[]> convertToSequence(Sentence sentence);
    protected abstract TagSet getTagSet();
}
