/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/13 13:04</create-date>
 *
 * <copyright file="Synonym.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.synonym;


/**
 * 同义词
 * @author hankcs
 */
public class Synonym : ISynonym
{
    public string realWord;
    public long id;
    public Type type;

    @Deprecated
    public Synonym(string realWord, string idString)
    {
        this.realWord = realWord;
        id = SynonymHelper.convertString2Id(idString);
    }

    @Deprecated
    public Synonym(string realWord, long id)
    {
        this.realWord = realWord;
        this.id = id;
    }

    public Synonym(string realWord, long id, Type type)
    {
        this.realWord = realWord;
        this.id = id;
        this.type = type;
    }

    //@Override
    public string getRealWord()
    {
        return realWord;
    }

    //@Override
    public long getId()
    {
        return id;
    }

    //@Override
    public string getIdString()
    {
        return SynonymHelper.convertId2StringWithIndex(id);
    }

    /**
     * 通过类似 Bh06A32= 番茄 西红柿 的字符串构造一系列同义词
     * @param param
     * @return
     */
    public static List<Synonym> create(string param)
    {
        if (param == null) return null;
        string[] args = param.split(" ");
        return create(args);
    }

    /**
     * @see com.hankcs.hanlp.corpus.synonym.Synonym#create(string)
     * @param args
     * @return
     */
    public static ArrayList<Synonym> create(string[] args)
    {
        ArrayList<Synonym> synonymList = new ArrayList<Synonym>(args.length - 1);

        string idString = args[0];
        Type type;
        switch (idString.charAt(idString.length() - 1))
        {
            case '=':
                type = Type.EQUAL;
                break;
            case '#':
                type = Type.LIKE;
                break;
            default:
                type = Type.SINGLE;
                break;
        }
        long startId = SynonymHelper.convertString2IdWithIndex(idString, 0);    // id从这里开始
        for (int i = 1; i < args.length; ++i)
        {
            if (type == Type.LIKE)
            {
                synonymList.add(new Synonym(args[i], startId + i, type));             // 如果不同则id递增
            }
            else
            {
                synonymList.add(new Synonym(args[i], startId, type));             // 如果相同则不变
            }
        }
        return synonymList;
    }

    //@Override
    public string toString()
    {
        final StringBuilder sb = new StringBuilder();
        sb.Append(realWord);
        switch (type)
        {

            case EQUAL:
                sb.Append('=');
                break;
            case LIKE:
                sb.Append('#');
                break;
            case SINGLE:
                sb.Append('@');
                break;
            case UNDEFINED:
                sb.Append('?');
                break;
        }
        sb.Append(getIdString());
        return sb.toString();
    }

    /**
     * 语义距离
     * @param other
     * @return
     */
    public long distance(Synonym other)
    {
        return Math.abs(id - other.id);
    }

    public enum Type
    {
        /**
         * 完全同义词，对应词典中的=号
         */
        EQUAL,
        /**
         * 同类词，对应#
         */
        LIKE,
        /**
         * 封闭词，没有同义词或同类词
         */
        SINGLE,

        /**
         * 未定义，通常属于非词典中的词
         */
        UNDEFINED,
    }
}
