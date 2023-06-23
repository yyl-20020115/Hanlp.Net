/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/9 21:00</create-date>
 *
 * <copyright file="FeatureTemplate.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using System.Text;
using System.Text.RegularExpressions;

namespace com.hankcs.hanlp.model.crf;



/**
 * 特征模板
 * @author hankcs
 */
public class FeatureTemplate : ICacheAble
{
    /**
     * 用来解析模板的正则表达式
     */
    static readonly Regex pattern = new("%x\\[(-?\\d*),(\\d*)]");
    string template;
    /**
     * 每个部分%x[-2,0]的位移，其中int[0]储存第一个数（-2），int[1]储存第二个数（0）
     */
    public List<int[]> offsetList;
    public List<string> delimiterList;

    public FeatureTemplate()
    {
    }

    public static FeatureTemplate create(string template)
    {
        FeatureTemplate featureTemplate = new FeatureTemplate();
        featureTemplate.delimiterList = new ();
        featureTemplate.offsetList = new (3);
        featureTemplate.template = template;
        var matcher = pattern.matcher(template);
        int start = 0;
        while (matcher.find())
        {
            featureTemplate.delimiterList.Add(template.substring(start, matcher.start()));
            start = matcher.end();
            featureTemplate.offsetList.Add(new int[]{int.parseInt(matcher.group(1)), 
                int.parseInt(matcher.group(2))});
        }
        return featureTemplate;
    }

    public char[] generateParameter(Table table, int current)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        foreach (string d in delimiterList)
        {
            sb.Append(d);
            int[] offset = offsetList[(i++)];
            sb.Append(table.get(current + offset[0], offset[1]));
        }

        char[] o = new char[sb.Length];
        sb.getChars(0, sb.Length, o, 0);

        return o;
    }

    //@Override
    public void save(Stream _out) 
    {
        _out.writeUTF(template);
        _out.writeInt(offsetList.Count);
        foreach (int[] offset in offsetList)
        {
            _out.writeInt(offset[0]);
            _out.writeInt(offset[1]);
        }
        _out.writeInt(delimiterList.Count);
        foreach (string s in delimiterList)
        {
            _out.writeUTF(s);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        template = byteArray.nextUTF();
        int size = byteArray.Next();
        offsetList = new (size);
        for (int i = 0; i < size; ++i)
        {
            offsetList.Add(new int[]{byteArray.Next(), byteArray.Next()});
        }
        size = byteArray.Next();
        delimiterList = new (size);
        for (int i = 0; i < size; ++i)
        {
            delimiterList.Add(byteArray.nextUTF());
        }
        return true;
    }

    //@Override
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("FeatureTemplate{");
        sb.Append("template='").Append(template).Append('\'');
        sb.Append(", delimiterList=").Append(delimiterList);
        sb.Append('}');
        return sb.ToString();
    }

    public string getTemplate()
    {
        return template;
    }
}
