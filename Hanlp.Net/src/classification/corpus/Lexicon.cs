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

    public int AddWord(string word)
    {
        //assert word != null;
        char[] charArray = word.ToCharArray();
        int id = wordId.get(charArray);
        if (id == null)
        {
            id = wordId.Count;
            wordId.Add(charArray, id);
            idWord.Add(word);
            //assert idWord.Count == wordId.Count;
        }

        return id;
    }

    public int GetId(string word) => wordId.get(word);

    public string GetWord(int id) =>
        //assert 0 <= id;
        //assert id <= idWord.Count;
        idWord[id];

    public int Count => idWord.Count;

    public string[] GetWordIdArray()
    {
        string[] wordIdArray = new string[idWord.Count];
        if (idWord.Count==0) return wordIdArray;
        int p = -1;
        IEnumerator<string> iterator = idWord.GetEnumerator();
        while (iterator.MoveNext())
        {
            wordIdArray[++p] = iterator.Current;
        }

        return wordIdArray;
    }
}