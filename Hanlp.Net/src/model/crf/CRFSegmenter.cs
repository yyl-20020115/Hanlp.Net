/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 上午1:07</create-date>
 *
 * <copyright file="CRFSegmenter.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.dictionary.other;
using com.hankcs.hanlp.model.crf.crfpp;
using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.tokenizer.lexical;
using System.Text;

namespace com.hankcs.hanlp.model.crf;


//
/**
 * @author hankcs
 */
public class CRFSegmenter : CRFTagger , Segmenter
{

    private PerceptronSegmenter perceptronSegmenter;

    public CRFSegmenter() 
        : this(HanLP.Config.CRFCWSModelPath)
    {
        ;
    }

    public CRFSegmenter(String modelPath) 
        : base(modelPath)
    {
        ;
        if (modelPath != null)
        {
            perceptronSegmenter = new PerceptronSegmenter(this.model);
        }
    }

    //@Override
    protected void convertCorpus(Sentence sentence, BufferedWriter bw) 
    {
        foreach (Word w in sentence.toSimpleWordList())
        {
            String word = CharTable.convert(w.value);
            if (word.Length == 1)
            {
                bw.write(word);
                bw.write('\t');
                bw.write('S');
                bw.write('\n');
            }
            else
            {
                bw.write(word.charAt(0));
                bw.write('\t');
                bw.write('B');
                bw.write('\n');
                for (int i = 1; i < word.Length - 1; ++i)
                {
                    bw.write(word.charAt(i));
                    bw.write('\t');
                    bw.write('M');
                    bw.write('\n');
                }
                bw.write(word.charAt(word.length() - 1));
                bw.write('\t');
                bw.write('E');
                bw.write('\n');
            }
        }
    }

    public List<String> segment(String text)
    {
        List<String> wordList = new ();
        segment(text, CharTable.convert(text), wordList);

        return wordList;
    }

    //@Override
    public void segment(String text, String normalized, List<String> wordList)
    {
        perceptronSegmenter.segment(text, createInstance(normalized), wordList);
    }

    private CWSInstance createInstance(String text)
    {
        FeatureTemplate[] featureTemplateArray = model.getFeatureTemplateArray();
        return new CWSIns(text, model.featureMap);
    }
    public class CWSIns : CWSInstance
    {
        public CWSIns(String sentence, FeatureMap featureMap)
            :base(sentence,featureMap)
        {

        }
        //@Override
        protected int[] extractFeature(String sentence, FeatureMap featureMap, int position)
        {
            StringBuilder sbFeature = new StringBuilder();
            List<int> featureVec = new ();
            for (int i = 0; i < featureTemplateArray.length; i++)
            {
                Iterator<int[]> offsetIterator = featureTemplateArray[i].offsetList.iterator();
                Iterator<String> delimiterIterator = featureTemplateArray[i].delimiterList.iterator();
                delimiterIterator.next(); // ignore U0 之类的id
                while (offsetIterator.hasNext())
                {
                    int offset = offsetIterator.next()[0] + position;
                    if (offset < 0)
                        sbFeature.Append(FeatureIndex.BOS[-(offset + 1)]);
                    else if (offset >= sentence.length())
                        sbFeature.Append(FeatureIndex.EOS[offset - sentence.length()]);
                    else
                        sbFeature.Append(sentence.charAt(offset));
                    if (delimiterIterator.hasNext())
                        sbFeature.Append(delimiterIterator.next());
                    else
                        sbFeature.Append(i);
                }
                addFeatureThenClear(sbFeature, featureVec, featureMap);
            }
            return toFeatureArray(featureVec);
        }
    }    //@Override
    protected String getDefaultFeatureTemplate()
    {
        return "# Unigram\n" +
            "U0:%x[-1,0]\n" +
            "U1:%x[0,0]\n" +
            "U2:%x[1,0]\n" +
            "U3:%x[-2,0]%x[-1,0]\n" +
            "U4:%x[-1,0]%x[0,0]\n" +
            "U5:%x[0,0]%x[1,0]\n" +
            "U6:%x[1,0]%x[2,0]\n" +
            "\n" +
            "# Bigram\n" +
            "B";
    }
}
