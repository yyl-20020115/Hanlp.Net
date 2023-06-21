/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/11/25 20:53</create-date>
 *
 * <copyright file="MaxEntDependencyModelMaker.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.dependency.CoNll;
using com.hankcs.hanlp.corpus.io;

namespace com.hankcs.hanlp.corpus.dependency.model;



/**
 * 最大熵模型构建工具，训练暂时不使用自己的代码，借用opennlp训练。本maker只生成训练文件
 *
 * @author hankcs
 */
public class MaxEntDependencyModelMaker
{
    public static bool makeModel(string corpusLoadPath, string modelSavePath) 
    {
        TextWriter bw = new TextWriter(new StreamWriter(IOUtil.newOutputStream(modelSavePath)));
        LinkedList<CoNLLSentence> sentenceList = CoNLLLoader.loadSentenceList(corpusLoadPath);
        int id = 1;
        foreach (CoNLLSentence sentence in sentenceList)
        {
            Console.WriteLine("%d / %d...", id++, sentenceList.size());
            string[][] edgeArray = sentence.getEdgeArray();
            CoNLLWord[] word = sentence.getWordArrayWithRoot();
            for (int i = 0; i < word.Length; ++i)
            {
                for (int j = 0; j < word.Length; ++j)
                {
                    if (i == j) continue;
                    // 这就是一个边的实例，从i出发，到j，当然它可能存在也可能不存在，不存在取null照样是一个实例
                    List<string> contextList = new ();
                    // 先生成i和j的原子特征
                    contextList.AddRange(generateSingleWordContext(word, i, "i"));
                    contextList.AddRange(generateSingleWordContext(word, j, "j"));
                    // 然后生成二元组的特征
                    contextList.AddRange(generateUniContext(word, i, j));
                    // 将特征字符串化
                    foreach (string f in contextList)
                    {
                        bw.write(f);
                        bw.write(' ');
                    }
                    // 事件名称为依存关系
                    bw.write("" + edgeArray[i][j]);
                    bw.newLine();
                }
            }
            Console.WriteLine("done.");
        }
        bw.Close();
        return true;
    }

    public static ICollection<string> generateSingleWordContext(CoNLLWord[] word, int index, string mark)
    {
        ICollection<string> context = new LinkedList<string>();
        for (int i = index - 2; i < index + 2 + 1; ++i)
        {
            CoNLLWord w = i >= 0 && i < word.Length ? word[i] : CoNLLWord.NULL;
            context.Add(w.NAME + mark + (i - index));      // 在尾巴上做个标记，不然特征冲突了
            context.Add(w.POSTAG + mark + (i - index));
        }

        return context;
    }

    public static ICollection<string> generateUniContext(CoNLLWord[] word, int i, int j)
    {
        var context = new List<string>();
        context.Add(word[i].NAME + '→' + word[j].NAME);
        context.Add(word[i].POSTAG + '→' + word[j].POSTAG);
        context.Add(word[i].NAME + '→' + word[j].NAME + (i - j));
        context.Add(word[i].POSTAG + '→' + word[j].POSTAG + (i - j));
        CoNLLWord wordBeforeI = i - 1 >= 0 ? word[i - 1] : CoNLLWord.NULL;
        CoNLLWord wordBeforeJ = j - 1 >= 0 ? word[j - 1] : CoNLLWord.NULL;
        context.Add(wordBeforeI.NAME + '@' + word[i].NAME + '→' + word[j].NAME);
        context.Add(word[i].NAME + '→' + wordBeforeJ.NAME + '@' + word[j].NAME);
        context.Add(wordBeforeI.POSTAG + '@' + word[i].POSTAG + '→' + word[j].POSTAG);
        context.Add(word[i].POSTAG + '→' + wordBeforeJ.POSTAG + '@' + word[j].POSTAG);
        return context;
    }
}
