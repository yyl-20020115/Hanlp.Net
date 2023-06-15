/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-28 15:46</create-date>
 *
 * <copyright file="DemoTrainNER.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.perceptron;



/**
 * @author hankcs
 */
public class DemoTrainNER
{
    public static void Main(String[] args)
    {
        PerceptronTrainer trainer = new NERTrainer();
        trainer.train("data/test/pku98/199801.txt", Config.NER_MODEL_FILE);
    }

    public static void trainYourNER()
    {
        PerceptronTrainer trainer = new PT();
    }

    public class PT: NERTrainer
    {
        //@Override
        protected override TagSet createTagSet()
        {
            NERTagSet tagSet = new NERTagSet();
            tagSet.nerLabels.add("YourNER1");
            tagSet.nerLabels.add("YourNER2");
            tagSet.nerLabels.add("YourNER3");
            return tagSet;
        }
    }
}
