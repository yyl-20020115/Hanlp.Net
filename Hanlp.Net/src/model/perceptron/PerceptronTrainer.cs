/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-26 下午5:51</create-date>
 *
 * <copyright file="PerceptronTrainer.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.classification.utilities.io;
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.dependency.nnparser;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.instance;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.model.perceptron.tagset;
using com.hankcs.hanlp.model.perceptron.utility;

namespace com.hankcs.hanlp.model.perceptron;




/**
 * 感知机训练基类
 *
 * @author hankcs
 */
public abstract class PerceptronTrainer : InstanceConsumer
{

    /**
     * 训练结果
     */
    public class Result
    {
        /**
         * 模型
         */
        LinearModel model;
        /**
         * 精确率(Precision), 召回率(Recall)和F1-Measure<br>
         * 中文参考：https://blog.argcv.com/articles/1036.c
         */
        double[] prf;

        public Result(LinearModel model, double[] prf)
        {
            this.model = model;
            this.prf = prf;
        }

        /**
         * 获取准确率
         *
         * @return
         */
        public double getAccuracy()
        {
            if (prf.Length == 3)
            {
                return prf[2];
            }
            return prf[0];
        }

        /**
         * 获取模型
         *
         * @return
         */
        public LinearModel getModel()
        {
            return model;
        }
    }

    /**
     * 创建标注集
     *
     * @return
     */
    protected abstract TagSet createTagSet();

    /**
     * 训练
     *
     * @param trainingFile  训练集
     * @param developFile   开发集
     * @param modelFile     模型保存路径
     * @param compressRatio 压缩比
     * @param maxIteration  最大迭代次数
     * @param threadNum     线程数
     * @return 一个包含模型和精度的结构
     * @
     */
    public Result train(string trainingFile, string developFile,
                        string modelFile,  double compressRatio,
                         int maxIteration,  int threadNum) 
    {
        if (developFile == null)
        {
            developFile = trainingFile;
        }
        // 加载训练语料
        TagSet tagSet = createTagSet();
        MutableFeatureMap mutableFeatureMap = new MutableFeatureMap(tagSet);
        ConsoleLogger logger = new ConsoleLogger();
        logger.start("开始加载训练集...\n");
        Instance[] instances = loadTrainInstances(trainingFile, mutableFeatureMap);
        tagSet._lock();
        logger.finish("\n加载完毕，实例一共%d句，特征总数%d\n", instances.length, mutableFeatureMap.size() * tagSet.size());

        // 开始训练
        ImmutableFeatureMap immutableFeatureMap = new ImmutableFeatureMap(mutableFeatureMap.featureIdMap, tagSet);
        mutableFeatureMap = null;
        double[] accuracy = null;

        if (threadNum == 1)
        {
            AveragedPerceptron model;
            model = new AveragedPerceptron(immutableFeatureMap);
             double[] total = new double[model.parameter.length];
             int[] timestamp = new int[model.parameter.length];
            int current = 0;
            for (int iter = 1; iter <= maxIteration; iter++)
            {
                Utility.shuffleArray(instances);
                for (Instance instance : instances)
                {
                    ++current;
                    int[] guessLabel = new int[instance.length()];
                    model.viterbiDecode(instance, guessLabel);
                    for (int i = 0; i < instance.length(); i++)
                    {
                        int[] featureVector = instance.getFeatureAt(i);
                        int[] goldFeature = new int[featureVector.length];
                        int[] predFeature = new int[featureVector.length];
                        for (int j = 0; j < featureVector.length - 1; j++)
                        {
                            goldFeature[j] = featureVector[j] * tagSet.size() + instance.tagArray[i];
                            predFeature[j] = featureVector[j] * tagSet.size() + guessLabel[i];
                        }
                        goldFeature[featureVector.length - 1] = (i == 0 ? tagSet.bosId() : instance.tagArray[i - 1]) * tagSet.size() + instance.tagArray[i];
                        predFeature[featureVector.length - 1] = (i == 0 ? tagSet.bosId() : guessLabel[i - 1]) * tagSet.size() + guessLabel[i];
                        model.update(goldFeature, predFeature, total, timestamp, current);
                    }
                }

                // 在开发集上校验
                accuracy = trainingFile.equals(developFile) ? IOUtility.evaluate(instances, model) : evaluate(developFile, model);
                _out.printf("Iter#%d - ", iter);
                printAccuracy(accuracy);
            }
            // 平均
            model.average(total, timestamp, current);
            accuracy = trainingFile.equals(developFile) ? IOUtility.evaluate(instances, model) : evaluate(developFile, model);
            _out.print("AP - ");
            printAccuracy(accuracy);
            logger.start("以压缩比 %.2f 保存模型到 %s ... ", compressRatio, modelFile);
            model.save(modelFile, immutableFeatureMap.featureIdMap.entrySet(), compressRatio);
            logger.finish(" 保存完毕\n");
            if (compressRatio == 0) return new Result(model, accuracy);
        }
        else
        {
            // 多线程用Structure Perceptron
            StructuredPerceptron[] models = new StructuredPerceptron[threadNum];
            for (int i = 0; i < models.length; i++)
            {
                models[i] = new StructuredPerceptron(immutableFeatureMap);
            }

            TrainingWorker[] workers = new TrainingWorker[threadNum];
            int job = instances.length / threadNum;
            for (int iter = 1; iter <= maxIteration; iter++)
            {
                Utility.shuffleArray(instances);
                try
                {
                    for (int i = 0; i < workers.length; i++)
                    {
                        workers[i] = new TrainingWorker(instances, i * job,
                                                        i == workers.length - 1 ? instances.length : (i + 1) * job,
                                                        models[i]);
                        workers[i].start();
                    }
                    for (TrainingWorker worker : workers)
                    {
                        worker.join();
                    }
                    for (int j = 0; j < models[0].parameter.length; j++)
                    {
                        for (int i = 1; i < models.length; i++)
                        {
                            models[0].parameter[j] += models[i].parameter[j];
                        }
                        models[0].parameter[j] /= threadNum;
                    }
                    accuracy = trainingFile.equals(developFile) ? IOUtility.evaluate(instances, models[0]) : evaluate(developFile, models[0]);
                    _out.printf("Iter#%d - ", iter);
                    printAccuracy(accuracy);
                }
                catch (InterruptedException e)
                {
                    err.printf("线程同步异常，训练失败\n");
                    e.printStackTrace();
                    return null;
                }
            }
            logger.start("以压缩比 %.2f 保存模型到 %s ... ", compressRatio, modelFile);
            models[0].save(modelFile, immutableFeatureMap.featureIdMap.entrySet(), compressRatio, HanLP.Config.DEBUG);
            logger.finish(" 保存完毕\n");
            if (compressRatio == 0) return new Result(models[0], accuracy);
        }

        LinearModel model = new LinearModel(modelFile);
        if (compressRatio > 0)
        {
            accuracy = evaluate(developFile, model);
            _out.printf("\n%.2f compressed model - ", compressRatio);
            printAccuracy(accuracy);
        }

        return new Result(model, accuracy);
    }

    private void printAccuracy(double[] accuracy)
    {
        if (accuracy.Length == 3)
        {
            _out.printf("P:%.2f R:%.2f F:%.2f\n", accuracy[0], accuracy[1], accuracy[2]);
        }
        else
        {
            _out.printf("P:%.2f\n", accuracy[0]);
        }
    }

    private class TrainingWorker : Thread
    {
        private Instance[] instances;
        private int start;
        private int end;
        private StructuredPerceptron model;

        public TrainingWorker(Instance[] instances, int start, int end, StructuredPerceptron model)
        {
            this.instances = instances;
            this.start = start;
            this.end = end;
            this.model = model;
        }

        //@Override
        public void run()
        {
            for (int s = start; s < end; ++s)
            {
                Instance instance = instances[s];
                model.update(instance);
            }
//            _out.printf("Finished [%d,%d)\n", start, end);
        }
    }

    protected Instance[] loadTrainInstances(string trainingFile,  MutableFeatureMap mutableFeatureMap) 
    {
        List<Instance> instanceList = new LinkedList<Instance>();
        IOUtility.loadInstance(trainingFile, new Ins());
        Instance[] instances = new Instance[instanceList.size()];
        instanceList.toArray(instances);
        return instances;
    }

    public class Ins: InstanceHandler
    {
        //@Override
        public bool process(Sentence sentence)
        {
            Utility.normalize(sentence);
            instanceList.add(PerceptronTrainer.this.createInstance(sentence, mutableFeatureMap));
            return false;
        }
    }

    private static DoubleArrayTrie<int> loadDictionary(string trainingFile, string dictionaryFile) 
    {
        FrequencyMap dictionaryMap = new FrequencyMap();
        if (dictionaryFile == null)
        {
            _out.printf("从训练文件%s中统计词库...\n", trainingFile);
            loadWordFromFile(trainingFile, dictionaryMap, true);
        }
        else
        {
            _out.printf("从外部词典%s中加载词库...\n", trainingFile);
            loadWordFromFile(dictionaryFile, dictionaryMap, false);
        }
        DoubleArrayTrie<int> dat = new DoubleArrayTrie<int>();
        dat.build(dictionaryMap);
        _out.printf("加载完毕，词库总词数：%d，总词频：%d\n", dictionaryMap.size(), dictionaryMap.totalFrequency);

        return dat;
    }

    public Result train(string trainingFile, string modelFile) 
    {
        return train(trainingFile, trainingFile, modelFile);
    }

    public Result train(string trainingFile, string developFile, string modelFile) 
    {
        return train(trainingFile, developFile, modelFile, 0.1, 50, Runtime.getRuntime().availableProcessors());
    }

    private static void loadWordFromFile(string path, FrequencyMap storage, bool segmented) 
    {
        BufferedReader br = IOUtility.newBufferedReader(path);
        string line;
        while ((line = br.readLine()) != null)
        {
            if (segmented)
            {
                for (string word : IOUtility.readLineToArray(line))
                {
                    storage.add(word);
                }
            }
            else
            {
                line = line.trim();
                if (line.length() != 0)
                {
                    storage.add(line);
                }
            }
        }
        br.close();
    }
}
