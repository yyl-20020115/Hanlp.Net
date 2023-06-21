/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-13 2:26 PM</create-date>
 *
 * <copyright file="Vocabulary.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.bintrie;
using com.hankcs.hanlp.model.perceptron.common;

namespace com.hankcs.hanlp.model.hmm;


/**
 * @author hankcs
 */
public class Vocabulary : IStringIdMap
{
    private BinTrie<int> trie;
    bool mutable;
    private static readonly int UNK = 0;

    public Vocabulary(BinTrie<int> trie, bool mutable)
    {
        this.trie = trie;
        this.mutable = mutable;
    }

    public Vocabulary()
        : this(new BinTrie<int>(), true)

    {
        trie.Add("\t", UNK);
    }

    //@Override
    public int idOf(string s)
    {
        int id = trie.get(s);
        if (id == null)
        {
            if (mutable)
            {
                id = trie.size();
                trie.Add(s, id);
            }
            else
                id = UNK;
        }
        return id;
    }
}
