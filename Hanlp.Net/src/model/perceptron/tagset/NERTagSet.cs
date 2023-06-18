/*
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2017-10-28 11:40</create-date>
 *
 * <copyright file="NERTagSet.java" company="码农场">
 * Copyright (c) 2017, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.common;

namespace com.hankcs.hanlp.model.perceptron.tagset;



/**
 * @author hankcs
 */
public class NERTagSet : TagSet
{
    public string O_TAG = "O";
    public char O_TAG_CHAR = 'O';
    public string B_TAG_PREFIX = "B-";
    public char B_TAG_CHAR = 'B';
    public string M_TAG_PREFIX = "M-";
    public string E_TAG_PREFIX = "E-";
    public string S_TAG = "S";
    public char S_TAG_CHAR = 'S';
    public HashSet<string> nerLabels = new HashSet<string>();

    /**
     * 非NER
     */
    public int O;

    public NERTagSet()
        : base(TaskType.NER)
    {
        ;
        O = Add(O_TAG);
    }

    public NERTagSet(int o, ICollection<string> tags)
        : base(TaskType.NER)
    {
        ;
        O = o;
        foreach (string tag in tags)
        {
            Add(tag);
            string label = NERTagSet.posOf(tag);
            if (label.Length != tag.Length)
                nerLabels.Add(label);
        }
    }

    public static string posOf(string tag)
    {
        int index = tag.IndexOf('-');
        if (index == -1)
        {
            return tag;
        }

        return tag.Substring(index + 1);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        base.load(byteArray);
        nerLabels.Clear();
        foreach (KeyValuePair<string, int> entry in this)
        {
            string tag = entry.Key;
            int index = tag.IndexOf('-');
            if (index != -1)
            {
                nerLabels.Add(tag.Substring(index + 1));
            }
        }

        return true;
    }
}
