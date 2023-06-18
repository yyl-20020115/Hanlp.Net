/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/30 19:29</create-date>
 *
 * <copyright file="Instance.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dependency.nnparser;


/**
 * @author hankcs
 */
public class Instance
{
    List<string> raw_forms; //! The original form.
    List<string> forms;     //! The converted form.
    List<string> lemmas;    //! The lemmas.
    List<string> postags;   //! The postags.
    List<string> cpostags;  //! The cpostags.

    List<int> heads;
    List<int> deprelsidx;
    List<string> deprels;
    List<int> predict_heads;
    List<int> predict_deprelsidx;
    List<string> predict_deprels;

    public Instance()
    {
        forms = new ();
        postags = new ();
    }

    int size()
    {
        return forms.size();
    }

    bool is_tree()
    {
        List<List<int>> tree = new (heads.size());
        int root = -1;
        for (int modifier = 0; modifier < heads.size(); ++modifier)
        {
            int head = heads.get(modifier);
            if (head == -1)
            {
                root = modifier;
            }
            else
            {
                tree.get(head).Add(modifier);
            }
        }
        bool[] visited = new bool[heads.size()];
        if (!is_tree_travel(root, tree, visited))
        {
            return false;
        }
        for (int i = 0; i < visited.Length; ++i)
        {
            bool visit = visited[i];
            if (!visit)
            {
                return false;
            }
        }
        return true;
    }

    bool is_tree_travel(int now, List<List<int>> tree, bool[] visited)
    {
        if (visited[now])
        {
            return false;
        }
        visited[now] = true;
        for (int c = 0; c < tree.get(now).size(); ++c)
        {
            int next = tree.get(now).get(c);
            if (!is_tree_travel(next, tree, visited))
            {
                return false;
            }
        }
        return true;
    }

    bool is_projective()
    {
        return !is_non_projective();
    }

    bool is_non_projective()
    {
        for (int modifier = 0; modifier < heads.size(); ++modifier)
        {
            int head = heads.get(modifier);
            if (head < modifier)
            {
                for (int from = head + 1; from < modifier; ++from)
                {
                    int to = heads.get(from);
                    if (to < head || to > modifier)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int from = modifier + 1; from < head; ++from)
                {
                    int to = heads.get(from);
                    if (to < modifier || to > head)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
