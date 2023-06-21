/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>16/2/10 PM4:24</create-date>
 *
 * <copyright file="Lexicon.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;

namespace com.hankcs.hanlp.classification.corpus;




/**
 * 词表
 *
 * @author hankcs
 */
public class Lexicon
{
    public BinTrie<int> wordId;
    public List<string> idWord;

    public Lexicon()
    {
        wordId = new ();
        idWord = new ();
    }

    public Lexicon(BinTrie<int> wordIdTrie)
    {
        wordId = wordIdTrie;
    }

    public int addWord(string word)
    {
        //assert word != null;
        char[] charArray = word.ToCharArray();
        int id = wordId.get(charArray);
        if (id == null)
        {
            id = wordId.size();
            wordId.Add(charArray, id);
            idWord.Add(word);
            //assert idWord.size() == wordId.size();
        }

        return id;
    }

    public int getId(string word)
    {
        return wordId.get(word);
    }

    public string getWord(int id)
    {
        //assert 0 <= id;
        //assert id <= idWord.size();
        return idWord.get(id);
    }

    public int size()
    {
        return idWord.size();
    }

    public string[] getWordIdArray()
    {
        string[] wordIdArray = new string[idWord.size()];
        if (idWord.isEmpty()) return wordIdArray;
        int p = -1;
        IEnumerator<string> iterator = idWord.iterator();
        while (iterator.MoveNext())
        {
            wordIdArray[++p] = iterator.next();
        }

        return wordIdArray;
    }
}