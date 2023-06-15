/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/7 20:14</create-date>
 *
 * <copyright file="DemoPosTagging.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp;
using com.hankcs.hanlp.corpus.dependency.CoNll;
using com.hankcs.hanlp.utility;

namespace com.hankcs.demo;


/**
 * 依存句法分析（CRF句法模型需要-Xms512m -Xmx512m -Xmn256m，MaxEnt和神经网络句法模型需要-Xms1g -Xmx1g -Xmn512m）
 * @author hankcs
 */
public class DemoDependencyParser : TestUtility
{
    public static void Main(String[] args)
    {
        CoNLLSentence sentence = HanLP.parseDependency("徐先生还具体帮助他确定了把画雄鹰、松鼠和麻雀作为主攻目标。");
        Console.WriteLine(sentence);
        // 可以方便地遍历它
        foreach (CoNLLWord word in sentence)
        {
            Console.WriteLine("{0} --({1})--> {2}\n", word.LEMMA, word.DEPREL, word.HEAD.LEMMA);
        }
        // 也可以直接拿到数组，任意顺序或逆序遍历
        CoNLLWord[] wordArray = sentence.getWordArray();
        for (int i = wordArray.Length - 1; i >= 0; i--)
        {
            CoNLLWord word = wordArray[i];
            Console.WriteLine("{0} --({1})--> {2}\n", word.LEMMA, word.DEPREL, word.HEAD.LEMMA);
        }
        // 还可以直接遍历子树，从某棵子树的某个节点一路遍历到虚根
        CoNLLWord head = wordArray[12];
        while ((head = head.HEAD) != null)
        {
            if (head == CoNLLWord.ROOT) Console.WriteLine(head.LEMMA);
            else Console.WriteLine("{0} --({1})--> ", head.LEMMA, head.DEPREL);
        }
    }
}
