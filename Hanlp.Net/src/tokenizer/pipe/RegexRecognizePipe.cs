/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-29 4:55 PM</create-date>
 *
 * <copyright file="URLRecognizePipe.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * See LICENSE file in the project root for full license information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.document.sentence.word;
using System.Text.RegularExpressions;

namespace com.hankcs.hanlp.tokenizer.pipe;



/**
 * 正则匹配管道
 *
 * @author hankcs
 */
public class RegexRecognizePipe : Pipe<List<IWord>, List<IWord>>
{
    /**
     * 正则表达式
     */
    protected Regex pattern;
    /**
     * 所属标签
     */
    protected string label;

    public RegexRecognizePipe(Regex pattern, string label)
    {
        this.pattern = pattern;
        this.label = label;
    }


    //@Override
    public List<IWord> flow(List<IWord> input)
    {
        ListIterator<IWord> listIterator = input.listIterator();
        while (listIterator.hasNext())
        {
            IWord wordOrSentence = listIterator.next();
            if (wordOrSentence.getLabel() != null)
                continue; // 这是别的管道已经处理过的单词，跳过
            listIterator.remove(); // 否则是句子
            string sentence = wordOrSentence.getValue();
            var matcher = pattern.matcher(sentence);
            int begin = 0;
            int end;
            while (matcher.find())
            {
                end = matcher.start();
                listIterator.add(new Word(sentence.substring(begin, end), null)); // 未拦截的部分
                listIterator.add(new Word(matcher.group(), label)); // 拦截到的部分
                begin = matcher.end();
            }
            if (begin < sentence.Length) listIterator.add(new Word(sentence.substring(begin), null));
        }
        return input;
    }
}
