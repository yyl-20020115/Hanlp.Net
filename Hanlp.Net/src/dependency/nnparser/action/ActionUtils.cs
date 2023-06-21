/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/31 20:52</create-date>
 *
 * <copyright file="ActionUtils.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dependency.nnparser.action;



/**
 * @author hankcs
 */
public class ActionUtils : ActionType
{
    public static bool is_shift(Action act)
    {
        return (act.name() == ActionType.kShift);
    }

    public static bool is_left_arc(Action act, int[] deprel)
    {
        if (act.name() == ActionType.kLeftArc)
        {
            deprel[0] = act.rel();
            return true;
        }
        deprel[0] = 0;
        return false;
    }

    public static bool is_right_arc(Action act, int[] deprel)
    {
        if (act.name() == ActionType.kRightArc)
        {
            deprel[0] = act.rel();
            return true;
        }
        deprel[0] = 0;
        return false;
    }

    void get_oracle_actions(List<int> heads,
                            List<int> deprels,
                            List<Action> actions)
    {
        // The oracle finding algorithm for arcstandard is using a in-order tree
        // searching.
        int N = heads.Count;
        int root = -1;
        List<List<int>> tree = new (N);

        actions.Clear();
        for (int i = 0; i < N; ++i)
        {
            int head = heads[(i)];
            if (head == -1)
            {
                if (root == -1)
                    Console.Error.WriteLine("error: there should be only one root.");
                root = i;
            }
            else
            {
                tree[(head)].Add(i);
            }
        }

        get_oracle_actions_travel(root, heads, deprels, tree, actions);
    }

    void get_oracle_actions_travel(int root,
                                   List<int> heads,
                                   List<int> deprels,
                                   List<List<int>> tree,
                                   List<Action> actions)
    {
        List<int> children = tree[(root)];

        int i;
        for (i = 0; i < children.Count && children[i] < root; ++i)
        {
            get_oracle_actions_travel(children[i], heads, deprels, tree, actions);
        }

        actions.Add(ActionFactory.make_shift());

        for (int j = i; j < children.Count; ++j)
        {
            int child = children[(j)];
            get_oracle_actions_travel(child, heads, deprels, tree, actions);
            actions.Add(ActionFactory.make_right_arc (deprels[(child)]));
        }

        for (int j = i - 1; j >= 0; --j)
        {
            int child = children[(j)];
            actions.Add(ActionFactory.make_left_arc (deprels[(child)]));
        }
    }

    void get_oracle_actions2( Dependency instance,
                        List<Action> actions)
    {
        get_oracle_actions2(instance.heads, instance.deprels, actions);
    }
    
    void get_oracle_actions2(List<int> heads,
                             List<int> deprels,
                             List<Action> actions) {
        actions.Clear();
        int len = heads.Count;
        List<int> sigma = new ();
        int beta = 0;
        List<int> output = new (len);
        for (int i = 0; i < len; i++)
        {
            output.Add(-1);
        }

        int step = 0;
        while (!(sigma.Count ==1 && beta == len))
        {
            int[] beta_reference = new int[]{beta};
            get_oracle_actions_onestep(heads, deprels, sigma, beta_reference, output, actions);
            beta = beta_reference[0];
        }
    }
    
    void get_oracle_actions_onestep(List<int> heads,
                                    List<int> deprels,
                                    List<int> sigma,
                                    int[] beta,
                                    List<int> output,
                                    List<Action> actions)
    {
        int top0 = (sigma.Count > 0 ? sigma[^1] : -1);
        int top1 = (sigma.Count > 1 ? sigma[^2] : -1);

        bool all_descendents_reduced = true;
        if (top0 >= 0)
        {
            for (int i = 0; i < heads.Count; ++i)
            {
                if (heads[(i)] == top0 && output[(i)] != top0)
                {
                    // _INFO << i << " " << output[i];
                    all_descendents_reduced = false;
                    break;
                }
            }
        }

        if (top1 >= 0 && heads[(top1)] == top0)
        {
            actions.Add(ActionFactory.make_left_arc(deprels[(top1)]));
            output[top1]=top0;
            sigma.Remove(sigma.Count - 1);
            sigma[^1]= (top0);
        }
        else if (top1 >= 0 && heads[(top0)] == top1 && all_descendents_reduced)
        {
            actions.Add(ActionFactory.make_right_arc(deprels[(top0)]));
            output[top0] = top1;
            sigma.Remove(sigma.Count - 1);
        }
        else if (beta[0] < heads.Count)
        {
            actions.Add(ActionFactory.make_shift ());
            sigma.Add(beta[0]);
            ++beta[0];
        }
    }
}
