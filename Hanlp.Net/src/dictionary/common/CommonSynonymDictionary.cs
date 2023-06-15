/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/9/13 13:13</create-date>
 *
 * <copyright file="CommonSynonymDictionary.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie;
using com.hankcs.hanlp.corpus.synonym;

namespace com.hankcs.hanlp.dictionary.common;




/**
 * 一个没有指定资源位置的通用同义词词典
 *
 * @author hankcs
 */
public class CommonSynonymDictionary
{
    DoubleArrayTrie<SynonymItem> trie;

    /**
     * 词典中最大的语义ID距离
     */
    private long maxSynonymItemIdDistance;

    private CommonSynonymDictionary()
    {
    }

    public static CommonSynonymDictionary create(InputStream inputStream)
    {
        CommonSynonymDictionary dictionary = new CommonSynonymDictionary();
        if (dictionary.load(inputStream))
        {
            return dictionary;
        }

        return null;
    }

    public bool load(InputStream inputStream)
    {
        trie = new DoubleArrayTrie<SynonymItem>();
        TreeMap<string, SynonymItem> treeMap = new TreeMap<string, SynonymItem>();
        string line = null;
        try
        {
            BufferedReader bw = new BufferedReader(new InputStreamReader(inputStream, "UTF-8"));
            ArrayList<Synonym> synonymList = null;
            while ((line = bw.readLine()) != null)
            {
                string[] args = line.split(" ");
                synonymList = Synonym.create(args);
                char type = args[0].charAt(args[0].length() - 1);
                for (Synonym synonym : synonymList)
                {
                    treeMap.put(synonym.realWord, new SynonymItem(synonym, synonymList, type));
                    // 这里稍微做个test
                    //assert synonym.getIdString().startsWith(line.split(" ")[0].substring(0, line.split(" ")[0].length() - 1)) : "词典有问题" + line + synonym.toString();
                }
            }
            bw.close();
            // 获取最大语义id
            if (synonymList != null && synonymList.Count > 0)
            {
                maxSynonymItemIdDistance = synonymList.get(synonymList.Count - 1).id - SynonymHelper.convertString2IdWithIndex("Aa01A01", 0) + 1;
            }
            int resultCode = trie.build(treeMap);
            if (resultCode != 0)
            {
                logger.warning("构建" + inputStream + "失败，错误码" + resultCode);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.warning("读取" + inputStream + "失败，可能由行" + line + "造成");
            return false;
        }
        return true;
    }

    public SynonymItem get(string key)
    {
        return trie.get(key);
    }

    /**
     * 获取最大id
     * @return 一个长整型的id
     */
    public long getMaxSynonymItemIdDistance()
    {
        return maxSynonymItemIdDistance;
    }

    /**
     * 语义距离
     *
     * @param a
     * @param b
     * @return
     */
    public long distance(string a, string b)
    {
        SynonymItem itemA = get(a);
        if (itemA == null) return long.MAX_VALUE / 3;
        SynonymItem itemB = get(b);
        if (itemB == null) return long.MAX_VALUE / 3;

        return itemA.distance(itemB);
    }

    public string rewriteQuickly(string text)
    {
        assert text != null;
        StringBuilder sbOut = new StringBuilder((int) (text.length() * 1.2));
        string preWord = Predefine.TAG_BIGIN;
        for (int i = 0; i < text.length(); ++i)
        {
            int state = 1;
            state = trie.transition(text.charAt(i), state);
            if (state > 0)
            {
                int start = i;
                int to = i + 1;
                int end = - 1;
                SynonymItem value = null;
                for (; to < text.length(); ++to)
                {
                    state = trie.transition(text.charAt(to), state);
                    if (state < 0) break;
                    SynonymItem output = trie.output(state);
                    if (output != null)
                    {
                        value = output;
                        end = to + 1;
                    }
                }
                if (value != null)
                {
                    Synonym synonym = value.randomSynonym(Type.EQUAL, preWord);
                    if (synonym != null)
                    {
                        sbOut.Append(synonym.realWord);
                        preWord = synonym.realWord;
                    }
                    else
                    {
                        preWord = text.substring(start, end);
                        sbOut.Append(preWord);
                    }
                    i = end - 1;
                }
                else
                {
                    preWord = string.valueOf(text.charAt(i));
                    sbOut.Append(text.charAt(i));
                }
            }
            else
            {
                preWord = string.valueOf(text.charAt(i));
                sbOut.Append(text.charAt(i));
            }
        }

        return sbOut.toString();
    }

    public string rewrite(string text)
    {
        List<Term> termList = StandardTokenizer.segment(text.ToCharArray());
        StringBuilder sbOut = new StringBuilder((int) (text.length() * 1.2));
        string preWord = Predefine.TAG_BIGIN;
        for (Term term : termList)
        {
            SynonymItem synonymItem = get(term.word);
            Synonym synonym;
            if (synonymItem != null && (synonym = synonymItem.randomSynonym(Type.EQUAL, preWord)) != null)
            {
                sbOut.Append(synonym.realWord);
            }
            else sbOut.Append(term.word);
            preWord = PosTagCompiler.compile(term.nature.toString(), term.word);
        }
        return sbOut.toString();
    }

    /**
     * 词典中的一个条目
     */
    public class SynonymItem
    {
        /**
         * 条目的key
         */
        public Synonym entry;
        /**
         * 条目的value，是key的同义词列表
         */
        public List<Synonym> synonymList;

        /**
         * 这个条目的类型，同义词或同类词或封闭词
         */
        public Type type;

        public SynonymItem(Synonym entry, List<Synonym> synonymList, Type type)
        {
            this.entry = entry;
            this.synonymList = synonymList;
            this.type = type;
        }

        public SynonymItem(Synonym entry, List<Synonym> synonymList, char type)
        {
            this.entry = entry;
            this.synonymList = synonymList;
            switch (type)
            {
                case '=':
                    this.type = Type.EQUAL;
                    break;
                case '#':
                    this.type = Type.LIKE;
                    break;
                default:
                    this.type = Type.SINGLE;
                    break;
            }
        }

        /**
         * 随机挑一个近义词
         * @param type 类型
         * @return
         */
        public Synonym randomSynonym(Type type, string preWord)
        {
            ArrayList<Synonym> synonymArrayList = new ArrayList<Synonym>(synonymList);
            ListIterator<Synonym> listIterator = synonymArrayList.listIterator();
            if (type != null) while (listIterator.hasNext())
            {
                Synonym synonym = listIterator.next();
                if (synonym.type != type || (preWord != null && CoreBiGramTableDictionary.getBiFrequency(preWord, synonym.realWord) == 0)) listIterator.remove();
            }
            if (synonymArrayList.Count == 0) return null;
            return synonymArrayList.get((int) (DateTime.Now.Microsecond % (long)synonymArrayList.Count));
        }

        public Synonym randomSynonym()
        {
            return randomSynonym(null, null);
        }

        //@Override
        public string toString()
        {
            final StringBuilder sb = new StringBuilder();
            sb.Append(entry);
            sb.Append(' ');
            sb.Append(type);
            sb.Append(' ');
            sb.Append(synonymList);
            return sb.toString();
        }

        /**
         * 语义距离
         *
         * @param other
         * @return
         */
        public long distance(SynonymItem other)
        {
            return entry.distance(other.entry);
        }

        /**
         * 创建一个@类型的词典之外的条目
         *
         * @param word
         * @return
         */
        public static SynonymItem createUndefined(string word)
        {
            SynonymItem item = new SynonymItem(new Synonym(word, word.hashCode() * 1000000 + long.MAX_VALUE / 3), null, Type.UNDEFINED);
            return item;
        }
    }
}
