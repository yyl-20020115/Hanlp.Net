/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 上午3:04</create-date>
 *
 * <copyright file="CRFPOSTagger.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.model.crf.crfpp;
using com.hankcs.hanlp.model.perceptron;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.tokenizer.lexical;
using System.Text;

namespace com.hankcs.hanlp.model.crf;



/**
 * @author hankcs
 */
public class CRFPOSTagger : CRFTagger , POSTagger
{
    private PerceptronPOSTagger perceptronPOSTagger;

    public CRFPOSTagger() 
    {
        this(HanLP.Config.CRFPOSModelPath);
    }

    public CRFPOSTagger(string modelPath) 
    {
        base(modelPath);
        if (modelPath != null)
        {
            perceptronPOSTagger = new PerceptronPOSTagger(this.model);
        }
    }

    //@Override
    public void train(string trainCorpusPath, string modelPath) 
    {
        crf_learn.Option option = new crf_learn.Option();
        train(trainCorpusPath, modelPath, option.maxiter, 10, option.eta, option.cost,
              option.thread, option.shrinking_size, Encoder.Algorithm.fromString(option.algorithm));
    }

    //@Override
    protected void convertCorpus(Sentence sentence, TextWriter bw) 
    {
        List<Word> simpleWordList = sentence.toSimpleWordList();
        List<string> wordList = new (simpleWordList.size());
        for (Word word : simpleWordList)
        {
            wordList.Add(word.value);
        }
        string[] words = wordList.ToArray();
        Iterator<Word> iterator = simpleWordList.iterator();
        for (int i = 0; i < words.Length; i++)
        {
            string curWord = words[i];
            string[] cells = createCells(true);
            extractFeature(curWord, cells);
            cells[5] = iterator.next().label;
            for (int j = 0; j < cells.Length; j++)
            {
                bw.Write(cells[j]);
                if (j != cells.Length - 1)
                    bw.Write('\t');
            }
            bw.newLine();
        }
    }

    private string[] createCells(bool withTag)
    {
        return withTag ? new string[6] : new string[5];
    }

    private void extractFeature(string curWord, string[] cells)
    {
        int Length = curWord.Length;
        cells[0] = curWord;
        cells[1] = curWord.substring(0, 1);
        cells[2] = Length > 1 ? curWord.substring(0, 2) : "_";
        // Length > 2 ? curWord.substring(0, 3) : "<>"
        cells[3] = curWord.substring(Length - 1);
        cells[4] = Length > 1 ? curWord.substring(Length - 2) : "_";
        // Length > 2 ? curWord.substring(Length - 3) : "<>"
    }

    //@Override
    protected string getDefaultFeatureTemplate()
    {
        return "# Unigram\n" +
            "U0:%x[-1,0]\n" +
            "U1:%x[0,0]\n" +
            "U2:%x[1,0]\n" +
            "U3:%x[0,1]\n" +
            "U4:%x[0,2]\n" +
            "U5:%x[0,3]\n" +
            "U6:%x[0,4]\n" +
//            "U7:%x[0,5]\n" +
//            "U8:%x[0,6]\n" +
            "\n" +
            "# Bigram\n" +
            "B";
    }

    public string[] tag(List<string> wordList)
    {
        varwords = wordList.ToArray();
        return tag(words);
    }

    //@Override
    public string[] tag(params string[] words)
    {
        return perceptronPOSTagger.tag(createInstance(words));
    }

    private POSInstance createInstance(string[] words)
    {
        FeatureTemplate[] featureTemplateArray = model.getFeatureTemplateArray();
        string[][] table = new string[words.Length][5];
        for (int i = 0; i < words.Length; i++)
        {
            extractFeature(words[i], table[i]);
        }

        return new CI(words, model.featureMap);
    }
    
    public class CI: POSInstance
    {
        //@Override
        protected int[] extractFeature(string[] words, FeatureMap featureMap, int position)
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
                    int j = offset[1];
                    if (t < 0)
                        sbFeature.Append(FeatureIndex.BOS[-(t + 1)]);
                    else if (t >= words.Length)
                        sbFeature.Append(FeatureIndex.EOS[t - words.Length]);
                    else
                        sbFeature.Append(table[t][j]);
                    if (delimiterIterator.MoveNext())
                        sbFeature.Append(delimiterIterator.next());
                    else
                        sbFeature.Append(i);
                }
                addFeatureThenClear(sbFeature, featureVec, featureMap);
            }
            return toFeatureArray(featureVec);
        }
    }
}