/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-03-30 上午2:51</create-date>
 *
 * <copyright file="CRFTagger.java" company="码农场">
 * Copyright (c) 2018, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.utility;
using System.Text;

namespace com.hankcs.hanlp.model.crf;



/**
 * @author hankcs
 */
public abstract class CRFTagger
{
    protected LogLinearModel model;

    public CRFTagger()
    {
    }

    public CRFTagger(string modelPath) 
    {
        if (modelPath == null) return; // 训练模式
        model = new LogLinearModel(modelPath);
    }

    /**
     * 训练
     *
     * @param templFile     模板文件
     * @param trainFile     训练文件
     * @param modelFile     模型文件
     * @param maxitr        最大迭代次数
     * @param freq          特征最低频次
     * @param eta           收敛阈值
     * @param C             cost-factor
     * @param threadNum     线程数
     * @param shrinkingSize
     * @param algorithm     训练算法
     * @return
     */
    public void train(string templFile, string trainFile, string modelFile,
                      int maxitr, int freq, double eta, double C, int threadNum, int shrinkingSize,
                      Encoder.Algorithm algorithm) 
    {
        Encoder encoder = new Encoder();
        if (!encoder.learn(templFile, trainFile, modelFile,
                           true, maxitr, freq, eta, C, threadNum, shrinkingSize, algorithm))
        {
            throw new IOException("fail to learn model");
        }
        convert(modelFile);
    }

    /**
     * 将CRF++格式转为HanLP格式
     *
     * @param modelFile
     * @
     */
    private void convert(string modelFile) 
    {
        this.model = new LogLinearModel(modelFile + ".txt", modelFile);
    }

    public void train(string trainCorpusPath, string modelPath) 
    {
        crf_learn.Option option = new crf_learn.Option();
        train(trainCorpusPath, modelPath, option.maxiter, option.freq, option.eta, option.cost,
              option.thread, option.shrinking_size, Encoder.Algorithm.fromString(option.algorithm));
    }

    public void train(string trainFile, string modelFile,
                      int maxitr, int freq, double eta, double C, int threadNum, int shrinkingSize,
                      Encoder.Algorithm algorithm) 
    {
        string templFile = null;
        File tmpTemplate = File.createTempFile("crfpp-template-" + new Date().getTime(), ".txt");
        tmpTemplate.deleteOnExit();
        templFile = tmpTemplate;
        string template = getDefaultFeatureTemplate();
        IOUtil.saveTxt(templFile, template);

        File tmpTrain = File.createTempFile("crfpp-train-" + new Date().getTime(), ".txt");
        tmpTrain.deleteOnExit();
        convertCorpus(trainFile, tmpTrain);
        trainFile = tmpTrain;
        Console.WriteLine("Java效率低，建议安装CRF++，执行下列等价训练命令（不要终止本进程，否则临时语料库和特征模板将被清除）：\n" +
                              "crf_learn -m %d -f %d -e %f -c %f -p %d -H %d -a %s -t %s %s %s\n", maxitr, freq, eta,
                          C, threadNum, shrinkingSize, algorithm.ToString().replace('_', '-'),
                          templFile, trainFile, modelFile);
        Encoder encoder = new Encoder();
        if (!encoder.learn(templFile, trainFile, modelFile,
                           true, maxitr, freq, eta, C, threadNum, shrinkingSize, algorithm))
        {
            throw new IOException("fail to learn model");
        }
        convert(modelFile);
    }

    protected abstract void convertCorpus(Sentence sentence, TextWriter bw) ;

    protected abstract string getDefaultFeatureTemplate();

    public void convertCorpus(string pkuPath, string tsvPath) 
    {
         TextWriter bw = IOUtil.newBufferedWriter(tsvPath);
        IOUtility.loadInstance(pkuPath, new IT());
        bw.Close();
    }
    public class IT:
         InstanceHandler
    {
        //@Override
        public bool process(Sentence sentence)
        {
            Utility.normalize(sentence);
            try
            {
                convertCorpus(sentence, bw);
                bw.newLine();
            }
            catch (IOException e)
            {
                throw new RuntimeException(e);
            }
            return false;
        }
    }
    /**
     * 导出特征模板
     *
     * @param templatePath
     * @
     */
    public void dumpTemplate(string templatePath) 
    {
        TextWriter bw = IOUtil.newBufferedWriter(templatePath);
        string template = getTemplate();
        bw.write(template);
        bw.Close();
    }

    /**
     * 获取特征模板
     *
     * @return
     */
    public string getTemplate()
    {
        string template = getDefaultFeatureTemplate();
        if (model != null && model.getFeatureTemplateArray() != null)
        {
            StringBuilder sbTemplate = new StringBuilder();
        foreach (FeatureTemplate featureTemplate in model.getFeatureTemplateArray())
            {
                sbTemplate.Append(featureTemplate.getTemplate()).Append('\n');
            }
        }
        return template;
    }
}
