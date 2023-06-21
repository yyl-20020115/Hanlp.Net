/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 上午3:45</create-date>
 *
 * <copyright file="CRFNERecognizer.java">
 * Copyright (c) 2018, Han He. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.tokenizer.lexical;

namespace com.hankcs.hanlp.model.crf;



/**
 * @author hankcs
 */
public class CRFNERecognizer : CRFTagger , NERecognizer
{
    private NERTagSet tagSet;
    /**
     * 复用感知机的解码模块
     */
    private PerceptronNERecognizer perceptronNERecognizer;

    public CRFNERecognizer() 
        : this(HanLP.Config.CRFNERModelPath)
    {
    }

    public CRFNERecognizer(string modelPath)
        : base(modelPath)
    {
        if (model == null)
        {
            tagSet = new NERTagSet();
            addDefaultNERLabels();
        }
        else
        {
            perceptronNERecognizer = new PerceptronNERecognizer(this.model);
            tagSet = perceptronNERecognizer.getNERTagSet();
        }
    }

    protected void addDefaultNERLabels()
    {
        tagSet.nerLabels.Add("nr");
        tagSet.nerLabels.Add("ns");
        tagSet.nerLabels.Add("nt");
    }

    //@Override
    protected void convertCorpus(Sentence sentence, TextWriter bw) 
    {
        List<string[]> collector = Utility.convertSentenceToNER(sentence, tagSet);
        foreach (string[] tuple in collector)
        {
            bw.Write(tuple[0]);
            bw.Write('\t');
            bw.Write(tuple[1]);
            bw.Write('\t');
            bw.Write(tuple[2]);
            bw.newLine();
        }
    }

    //@Override
    public string[] recognize(string[] wordArray, string[] posArray)
    {
        return perceptronNERecognizer.recognize(createInstance(wordArray, posArray));
    }

    //@Override
    public NERTagSet getNERTagSet()
    {
        return tagSet;
    }

    private NERInstance createInstance(string[] wordArray, string[] posArray)
    {
        FeatureTemplate[] featureTemplateArray = model.getFeatureTemplateArray();
        return new NERInstance(wordArray, posArray, model.featureMap)
        {
            //@Override
            protected int[] extractFeature(string[] wordArray, string[] posArray, FeatureMap featureMap, int position)
            {
                StringBuilder sbFeature = new StringBuilder();
                List<int> featureVec = new LinkedList<int>();
                for (int i = 0; i < featureTemplateArray.Length; i++)
                {
                    Iterator<int[]> offsetIterator = featureTemplateArray[i].offsetList.iterator();
                    Iterator<string> delimiterIterator = featureTemplateArray[i].delimiterList.iterator();
                    delimiterIterator.next(); // ignore U0 之类的id
                    while (offsetIterator.MoveNext())
                    {
                        int[] offset = offsetIterator.next();
                        int t = offset[0] + position;
                        bool first = offset[1] == 0;
                        if (t < 0)
                            sbFeature.Append(FeatureIndex.BOS[-(t + 1)]);
                        else if (t >= wordArray.Length)
                            sbFeature.Append(FeatureIndex.EOS[t - wordArray.Length]);
                        else
                            sbFeature.Append(first ? wordArray[t] : posArray[t]);
                        if (delimiterIterator.MoveNext())
                            sbFeature.Append(delimiterIterator.next());
                        else
                            sbFeature.Append(i);
                    }
                    addFeatureThenClear(sbFeature, featureVec, featureMap);
                }
                return toFeatureArray(featureVec);
            }
        };
    }

    //@Override
    protected string getDefaultFeatureTemplate()
    {
        return "# Unigram\n" +
            // form
            "U0:%x[-2,0]\n" +
            "U1:%x[-1,0]\n" +
            "U2:%x[0,0]\n" +
            "U3:%x[1,0]\n" +
            "U4:%x[2,0]\n" +
            // pos
            "U5:%x[-2,1]\n" +
            "U6:%x[-1,1]\n" +
            "U7:%x[0,1]\n" +
            "U8:%x[1,1]\n" +
            "U9:%x[2,1]\n" +
            // pos 2-gram
            "UA:%x[-2,1]%x[-1,1]\n" +
            "UB:%x[-1,1]%x[0,1]\n" +
            "UC:%x[0,1]%x[1,1]\n" +
            "UD:%x[1,1]%x[2,1]\n" +
            "UE:%x[2,1]%x[3,1]\n" +
            "\n" +
            "# Bigram\n" +
            "B";
    }
}
