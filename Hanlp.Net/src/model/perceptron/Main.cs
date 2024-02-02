/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-09-11 PM3:53</create-date>
 *
 * <copyright file="Main.java" company="码农场">
 * Copyright (c) 2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.model.perceptron.common;

namespace com.hankcs.hanlp.model.perceptron;




/**
 * @author hankcs
 */
public class Main
{
    public class Option
    {
        //@Argument(description = "任务类型:CWS|POS|NER")
        public TaskType task = TaskType.CWS;

        //@Argument(description = "执行训练任务")
        public bool train;

        //@Argument(description = "执行预测任务")
        public bool test;

        //@Argument(description = "执行评估任务")
        public bool evaluate;

        //@Argument(description = "模型文件路径")
        public string[] model = new string[]{HanLP.Config.PerceptronCWSModelPath, HanLP.Config.PerceptronPOSModelPath, HanLP.Config.PerceptronNERModelPath};

        //@Argument(description = "输入文本路径")
        public string input;

        //@Argument(description = "结果保存路径")
        public string result;

        //@Argument(description = "标准分词语料")
        public string gold;

        //@Argument(description = "训练集")
        public string reference;

        //@Argument(description = "开发集")
        public string development;

        //@Argument(description = "迭代次数")
        public int iter = 5;

        //@Argument(description = "模型压缩比率")
        Double compressRatio = 0.0;

        //@Argument(description = "线程数")
        public int thread = Environment.ProcessorCount;
    }

    public static void main(string[] args)
    {
        // nohup time java -jar averaged-perceptron-segment-1.0.jar -train -model 2014_2w.bin -reference 2014_blank.txt -development 2014_1k.txt > log.txt
        Option option = new Option();
        try
        {
            Args.parse(option, args);
            PerceptronTrainer trainer = null;
            switch (option.task)
            {
                case CWS:
                    trainer = new CWSTrainer();
                    break;
                case POS:
                    trainer = new POSTrainer();
                    break;
                case NER:
                    trainer = new NERTrainer();
                    break;
            }
            if (option.train)
            {
                trainer.Train(option.reference, option.development, option.model[0], option.compressRatio,
                              option.iter, option.thread);
            }
            else if (option.evaluate)
            {
                double[] prf = trainer.evaluate(option.gold, option.model[0]);
                Out.printf("Performance - P:%.2f R:%.2f F:%.2f\n", prf[0], prf[1], prf[2]);
            }
            else
            {
                PerceptronLexicalAnalyzer analyzer;
                string[] models = option.model;
                switch (models.Length)
                {
                    case 1:
                        analyzer = new PerceptronLexicalAnalyzer(models[0]);
                        break;
                    case 2:
                        analyzer = new PerceptronLexicalAnalyzer(models[0], models[1]);
                        break;
                    case 3:
                        analyzer = new PerceptronLexicalAnalyzer(models[0], models[1], models[2]);
                        break;
                    default:
                        Console.Error.WriteLine("最多支持载入3个模型，然而传入了多于3个: %s", Arrays.ToString(models));
                        return;
                }

                TextWriter printer;
                if (option.result == null)
                {
                    printer = new TextWriter(System.Out);
                }
                else
                {
                    printer = new TextWriter(new string(option.result), "utf-8");
                }
                Scanner scanner;
                if (option.input == null)
                {
                    scanner = new Scanner(System.@in);
//                    Console.Error.WriteLine("请输入文本：");
                }
                else
                {
                    scanner = new Scanner(new string(option.input), "utf-8");
                }
                string line;
                string lineSeparator = System.getProperty("line.separator");
                while (scanner.MoveNext() && (line = scanner.nextLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    Sentence sentence = analyzer.Analyze(line);
                    printer.Write(sentence.ToString());
                    printer.Write(lineSeparator);
                    if (option.result == null)
                    {
                        printer.flush();
                    }
                }
                printer.Close();
                scanner.Close();
            }
        }
        catch (ArgumentException e)
        {
            Console.Error.WriteLine(e.getMessage());
            Args.usage(option);
        }
        catch (IOException e)
        {
            Console.Error.WriteLine("发生了IO异常，请检查文件路径");
            //e.printStackTrace();
        }
    }
}
