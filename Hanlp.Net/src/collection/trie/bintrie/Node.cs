/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/5/3 12:27</create-date>
 *
 * <copyright file="Node.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */

using com.hankcs.hanlp.collection.trie.bintrie.util;

namespace com.hankcs.hanlp.collection.trie.bintrie;



/**
 * 深度大于等于2的子节点
 *
 * @author He Han
 */
public class Node<V> : BaseNode<V>
{
    //@Override
    protected bool addChild(BaseNode<V> node)
    {
        bool Add = false;
        if (child == null)
        {
            child = new BaseNode<V>[0];
        }
        int index = ArrayTool.binarySearch(child, node);
        if (index >= 0)
        {
            BaseNode<V> target = child[index];
            switch (node.status)
            {
                case Status.UNDEFINED_0:
                    if (target.status != Status.NOT_WORD_1)
                    {
                        target.status = Status.NOT_WORD_1;
                        target.value = null;
                        Add = true;
                    }
                    break;
                case Status.NOT_WORD_1:
                    if (target.status == Status.WORD_END_3)
                    {
                        target.status = Status.WORD_MIDDLE_2;
                    }
                    break;
                case Status.WORD_END_3:
                    if (target.status != Status.WORD_END_3)
                    {
                        target.status = Status.WORD_MIDDLE_2;
                    }
                    if (target.Value == null)
                    {
                        Add = true;
                    }
                    target.setValue(node.Value);
                    break;
            }
        }
        else
        {
            BaseNode<V>[] newChild = new BaseNode<V>[child.Length + 1];
            int insert = -(index + 1);
            Array.Copy(child, 0, newChild, 0, insert);
            Array.Copy(child, insert, newChild, insert + 1, child.Length - insert);
            newChild[insert] = node;
            child = newChild;
            Add = true;
        }
        return Add;
    }

    /**
     * @param c      节点的字符
     * @param status 节点状态
     * @param value  值
     */
    public Node(char c, Status status, V value)
    {
        this.c = c;
        this.status = status;
        this.value = value;
    }

    public Node()
    {
    }

    //@Override
    public override BaseNode<V> getChild(char c)
    {
        if (child == null) return null;
        int index = Array.BinarySearch(child, c);
        if (index < 0) return null;

        return child[index];
    }
}
