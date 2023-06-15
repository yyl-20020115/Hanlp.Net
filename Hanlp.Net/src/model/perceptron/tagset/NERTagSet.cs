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
namespace com.hankcs.hanlp.model.perceptron.tagset;



/**
 * @author hankcs
 */
public class NERTagSet : TagSet
{
    public final string O_TAG = "O";
    public final char O_TAG_CHAR = 'O';
    public final string B_TAG_PREFIX = "B-";
    public final char B_TAG_CHAR = 'B';
    public final string M_TAG_PREFIX = "M-";
    public final string E_TAG_PREFIX = "E-";
    public final string S_TAG = "S";
    public final char S_TAG_CHAR = 'S';
    public final Set<string> nerLabels = new HashSet<string>();

    /**
     * 非NER
     */
    public final int O;

    public NERTagSet()
    {
        super(TaskType.NER);
        O = add(O_TAG);
    }

    public NERTagSet(int o, Collection<string> tags)
    {
        super(TaskType.NER);
        O = o;
        for (string tag : tags)
        {
            add(tag);
            string label = NERTagSet.posOf(tag);
            if (label.length() != tag.length())
                nerLabels.add(label);
        }
    }

    public static string posOf(string tag)
    {
        int index = tag.indexOf('-');
        if (index == -1)
        {
            return tag;
        }

        return tag.substring(index + 1);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        super.load(byteArray);
        nerLabels.clear();
        for (KeyValuePair<string, int> entry : this)
        {
            string tag = entry.getKey();
            int index = tag.indexOf('-');
            if (index != -1)
            {
                nerLabels.add(tag.substring(index + 1));
            }
        }

        return true;
    }
}
