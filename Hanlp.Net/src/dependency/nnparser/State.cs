/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/31 21:20</create-date>
 *
 * <copyright file="State.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.dependency.nnparser.action;
using com.hankcs.hanlp.dependency.nnparser.util;
using Action = com.hankcs.hanlp.dependency.nnparser.action.Action;

namespace com.hankcs.hanlp.dependency.nnparser;



/**
 * @author hankcs
 */
public class State
{
    //! The pointer to the previous state.
    /**
     * 栈
     */
    public List<int> stack;
    /**
     * 队列的队首元素（的下标）
     */
    public int buffer;               //! The front word in the buffer.
    /**
     * 上一个状态
     */
    public State previous;    //! The pointer to the previous state.
    public Dependency @ref;    //! The pointer to the dependency tree.
    public double score;             //! The score.
    /**
     * 上一次动作
     */
    public Action last_action;       //! The last action.

    /**
     * 栈顶元素
     */
    public int top0;                 //! The top word on the stack.
    /**
     * 栈顶元素的下一个元素（全栈第二个元素）
     */
    public int top1;                 //! The second top word on the stack.
    public List<int> heads;   //! Use to record the heads in current state.
    public List<int> deprels; //! The dependency relation cached in state.
    /**
     * 当前节点的左孩子数量
     */
    public List<int> nr_left_children;      //! The number of left children in this state.
    /**
     * 当前节点的右孩子数量
     */
    public List<int> nr_right_children;     //! The number of right children in this state.
    public List<int> left_most_child;       //! The left most child for each word in this state.
    public List<int> right_most_child;      //! The right most child for each word in this state.
    public List<int> left_2nd_most_child;   //! The left 2nd-most child for each word in this state.
    public List<int> right_2nd_most_child;  //! The right 2nd-most child for each word in this state.

    public State()
    {
    }

    public State(Dependency @ref)
    {
        this.@ref = @ref;
        stack = new List<int>(_ref.size());
        Clear();
        int L = @ref.size();
        heads = std.create(L, -1);
        deprels = std.create(L, 0);
        nr_left_children = std.create(L, 0);
        nr_right_children = std.create(L, 0);
        left_most_child = std.create(L, -1);
        right_most_child = std.create(L, -1);
        left_2nd_most_child = std.create(L, -1);
        right_2nd_most_child = std.create(L, -1);
    }

    public void Clear()
    {
        score = 0;
        previous = null;
        top0 = -1;
        top1 = -1;
        buffer = 0;
        stack.Clear();
        std.fill(heads, -1);
        std.fill(deprels, 0);
        std.fill(nr_left_children, 0);
        std.fill(nr_right_children, 0);
        std.fill(left_most_child, -1);
        std.fill(right_most_child, -1);
        std.fill(left_2nd_most_child, -1);
        std.fill(right_2nd_most_child, -1);
    }

    public bool can_shift()
    {
        return !buffer_empty();
    }

    public bool can_left_arc()
    {
        return stack_size() >= 2;
    }

    public bool can_right_arc()
    {
        return stack_size() >= 2;
    }

    /**
     * 克隆一个状态到自己
     * @param source 源状态
     */
    public void copy(State source)
    {
        this.@ref = source.@ref;
        this.score = source.score;
        this.previous = source.previous;
        this.buffer = source.buffer;
        this.top0 = source.top0;
        this.top1 = source.top1;
        this.stack = source.stack;
        this.last_action = source.last_action;
        this.heads = source.heads;
        this.deprels = source.deprels;
        this.left_most_child = source.left_most_child;
        this.right_most_child = source.right_most_child;
        this.left_2nd_most_child = source.left_2nd_most_child;
        this.right_2nd_most_child = source.right_2nd_most_child;
        this.nr_left_children = source.nr_left_children;
        this.nr_right_children = source.nr_right_children;
    }

    /**
     * 更新栈的信息
     */
    public void refresh_stack_information()
    {
        int sz = stack.Count;
        if (0 == sz)
        {
            top0 = -1;
            top1 = -1;
        }
        else if (1 == sz)
        {
            top0 = stack.get(sz - 1);
            top1 = -1;
        }
        else
        {
            top0 = stack.get(sz - 1);
            top1 = stack.get(sz - 2);
        }
    }

    /**
     * 不建立依存关系，只转移句法分析的焦点，即原来的右焦点词变为新的左焦点词（本状态），依此类推。
     * @param source 右焦点词
     * @return 是否shift成功
     */
    public bool shift(State source)
    {
        if (!source.can_shift())
        {
            return false;
        }

        this.copy(source);
        stack.Add(this.buffer);
        refresh_stack_information();
        ++this.buffer;

        this.last_action = ActionFactory.make_shift();
        this.previous = source;
        return true;
    }

    public bool left_arc(State source, int deprel)
    {
        if (!source.can_left_arc())
        {
            return false;
        }

        this.copy(source);
        stack.Remove(stack.size() - 1);
        stack.set(stack.size() - 1, top0);

        heads.set(top1, top0);
        deprels.set(top1, deprel);

        if (-1 == left_most_child.get(top0))
        {
            // TP0 is left-isolate node.
            left_most_child.set(top0, top1);
        }
        else if (top1 < left_most_child.get(top0))
        {
            // (TP1, LM0, TP0)
            left_2nd_most_child.set(top0, left_most_child.get(top0));
            left_most_child.set(top0, top1);
        }
        else if (top1 < left_2nd_most_child.get(top0))
        {
            // (LM0, TP1, TP0)
            left_2nd_most_child.set(top0, top1);
        }

        nr_left_children.set(top0, nr_left_children.get(top0) + 1);
        refresh_stack_information();
        this.last_action = ActionFactory.make_left_arc(deprel);
        this.previous = source;
        return true;
    }

    public bool right_arc(State source, int deprel)
    {
        if (!source.can_right_arc())
        {
            return false;
        }

        this.copy(source);
        std.pop_back(stack);
        heads.set(top0, top1);
        deprels.set(top0, deprel);

        if (-1 == right_most_child.get(top1))
        {
            // TP1 is right-isolate node.
            right_most_child.set(top1, top0);
        }
        else if (right_most_child.get(top1) < top0)
        {
            right_2nd_most_child.set(top1, right_most_child.get(top1));
            right_most_child.set(top1, top0);
        }
        else if (right_2nd_most_child.get(top1) < top0)
        {
            right_2nd_most_child.set(top1, top0);
        }
        nr_right_children.set(top1, nr_right_children.get(top1) + 1);
        refresh_stack_information();
        this.last_action = ActionFactory.make_right_arc(deprel);
        this.previous = source;
        return true;
    }

    public int cost(List<int> gold_heads,
             List<int> gold_deprels)
    {
        List<List<int>> tree = new (gold_heads.size());
        for (int i = 0; i < gold_heads.size(); ++i)
        {
            int h = gold_heads.get(i);
            if (h >= 0)
            {
                tree.get(h).Add(i);
            }
        }

        List<int> sigma_l = stack;
        List<int> sigma_r = new ();
        sigma_r.Add(stack.get(stack.size() - 1));

        bool[] sigma_l_mask = new bool[gold_heads.size()];
        bool[] sigma_r_mask = new bool[gold_heads.size()];
        for (int s = 0; s < sigma_l.size(); ++s)
        {
            sigma_l_mask[sigma_l.get(s)] = true;
        }

        for (int i = buffer; i < @ref.size(); ++i)
        {
            if (gold_heads.get(i) < buffer)
            {
                sigma_r.Add(i);
                sigma_r_mask[i] = true;
                continue;
            }

            List<int> node = tree.get(i);
            for (int d = 0; d < node.size(); ++d)
            {
                if (sigma_l_mask[node.get(d)] || sigma_r_mask[node.get(d)])
                {
                    sigma_r.Add(i);
                    sigma_r_mask[i] = true;
                    break;
                }
            }
        }

        int len_l = sigma_l.size();
        int len_r = sigma_r.size();

        // typedef boost.multi_array<int, 3> array_t;
        // array_t T(boost.extents[len_l][len_r][len_l+len_r-1]);
        // std.fill( T.origin(), T.origin()+ T.num_elements(), 1024);
        int[][][] T = new int[len_l][len_r][len_l + len_r - 1];
        foreach (int[][] one in T)
        {
            foreach (int[] two in one)
            {
                for (int i = 0; i < two.Length; i++)
                {
                    two[i] = 1024;
                }
            }
        }

        T[0][0][len_l - 1] = 0;
        for (int d = 0; d < len_l + len_r - 1; ++d)
        {
            for (int j = Math.Max(0, d - len_l + 1); j < Math.Min(d + 1, len_r); ++j)
            {
                int i = d - j;
                if (i < len_l - 1)
                {
                    int i_1 = sigma_l.get(len_l - i - 2);
                    int i_1_rank = len_l - i - 2;
                    for (int rank = len_l - i - 1; rank < len_l; ++rank)
                    {
                        int h = sigma_l.get(rank);
                        int h_rank = rank;
                        T[i + 1][j][h_rank] = Math.Min(T[i + 1][j][h_rank],
                                                       T[i][j][h_rank] + (gold_heads.get(i_1) == h ? 0 : 2));
                        T[i + 1][j][i_1_rank] = Math.Min(T[i + 1][j][i_1_rank],
                                                         T[i][j][h_rank] + (gold_heads.get(h) == i_1 ? 0 : 2));
                    }
                    for (int rank = 1; rank < j + 1; ++rank)
                    {
                        int h = sigma_r.get(rank);
                        int h_rank = len_l + rank - 1;
                        T[i + 1][j][h_rank] = Math.Min(T[i + 1][j][h_rank],
                                                       T[i][j][h_rank] + (gold_heads.get(i_1) == h ? 0 : 2));
                        T[i + 1][j][i_1_rank] = Math.Min(T[i + 1][j][i_1_rank],
                                                         T[i][j][h_rank] + (gold_heads.get(h) == i_1 ? 0 : 2));
                    }
                }
                if (j < len_r - 1)
                {
                    int j_1 = sigma_r.get(j + 1);
                    int j_1_rank = len_l + j;
                    for (int rank = len_l - i - 1; rank < len_l; ++rank)
                    {
                        int h = sigma_l.get(rank);
                        int h_rank = rank;
                        T[i][j + 1][h_rank] = Math.Min(T[i][j + 1][h_rank],
                                                       T[i][j][h_rank] + (gold_heads.get(j_1) == h ? 0 : 2));
                        T[i][j + 1][j_1_rank] = Math.Min(T[i][j + 1][j_1_rank],
                                                         T[i][j][h_rank] + (gold_heads.get(h) == j_1 ? 0 : 2));
                    }
                    for (int rank = 1; rank < j + 1; ++rank)
                    {
                        int h = sigma_r.get(rank);
                        int h_rank = len_l + rank - 1;
                        T[i][j + 1][h_rank] = Math.Min(T[i][j + 1][h_rank],
                                                       T[i][j][h_rank] + (gold_heads.get(j_1) == h ? 0 : 2));
                        T[i][j + 1][j_1_rank] = Math.Min(T[i][j + 1][j_1_rank],
                                                         T[i][j][h_rank] + (gold_heads.get(h) == j_1 ? 0 : 2));
                    }
                }
            }
        }
        int penalty = 0;
        for (int i = 0; i < buffer; ++i)
        {
            if (heads.get(i) != -1)
            {
                if (heads.get(i) != gold_heads.get(i))
                {
                    penalty += 2;
                }
                else if (deprels.get(i) != gold_deprels.get(i))
                {
                    penalty += 1;
                }
            }
        }
        return T[len_l - 1][len_r - 1][0] + penalty;
    }

    /**
     * 队列是否为空
     * @return
     */
    public bool buffer_empty()
    {
        return (this.buffer == this.@ref.size());
    }

    /**
     * 栈的大小
     * @return
     */
    public int stack_size()
    {
        return (this.stack.Count);
    }
}
