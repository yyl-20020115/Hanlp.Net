

using System.Text;
/**
* MDAG is a Java library capable of constructing character-sequence-storing,
* directed acyclic graphs of minimal size. 
* 
*  Copyright (C) 2012 Kevin Lawson <Klawson88@gmail.com>
*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
namespace com.hankcs.hanlp.collection.MDAG;



/**
 * MDAG中的一个节点<br>
 * The class which represents a node in a MDAG.

 * @author Kevin
 */
public class MDAGNode
{
    //The bool denoting the accept state status of this node
    /**
     * 是否是终止状态
     */
    private bool isAcceptNode;
    
    //The Dictionary to contain entries that represent a _transition (label and target node)
    /**
     * 状态转移函数
     */
    private Dictionary<char, MDAGNode> outgoingTransitionTreeMap;

    //The int representing this node's incoming _transition node count
    /**
     * 入度
     */
    private int incomingTransitionCount = 0;
    
    //The int denoting index in a simplified mdag data array that this node's _transition set begins at
    /**
     * 在简化的MDAG中表示该节点的转移状态集合的起始位置
     */
    private int transitionSetBeginIndex = -1;
    
    //The int which will store this node's hash code after its been calculated (necessary due to how expensive the hashing calculation is)
    /**
     * 当它被计算后的hash值
     */
    private int? storedHashCode = null;
    
    
    
    /**
     * 建立一个节点<br>
     * Constructs an MDAGNode.
     
     * @param isAcceptNode     是否是终止状态 a bool denoting the accept state status of this node
     */
    public MDAGNode(bool isAcceptNode)
    {
        this.isAcceptNode = isAcceptNode;     
        outgoingTransitionTreeMap = new Dictionary<char, MDAGNode>();
    }

    
    
    /**
     * 克隆一个状态<br>
     * Constructs an MDAGNode possessing the same accept state status and outgoing transitions as another.
     
     * @param node      the MDAGNode possessing the accept state status and 
     *                  outgoing transitions that the to-be-created MDAGNode is to take on
     */
    private MDAGNode(MDAGNode node)
    {
        isAcceptNode = node.isAcceptNode;
        outgoingTransitionTreeMap = new Dictionary<char, MDAGNode>(node.outgoingTransitionTreeMap);
        
        //Loop through the nodes in this node's outgoing _transition set, incrementing the number of
        //incoming transitions of each by 1 (to account for this newly created node's outgoing transitions)
        foreach(Entry<char, MDAGNode> transitionKeyValuePair in outgoingTransitionTreeMap.entrySet())
            transitionKeyValuePair.Value.incomingTransitionCount++;
        /////
    }
    
    
    
    /**
     * 克隆一个状态<br>
     * Creates an MDAGNode possessing the same accept state status and outgoing transitions as this node.
     
     * @return      an MDAGNode possessing the same accept state status and outgoing transitions as this node
     */
    public MDAGNode clone()
    {
        return new MDAGNode(this);
    }
    
    
    
    /**
     * 克隆一个状态<br>
     * 原来soleParentNode转移到本状态，现在转移到克隆后的状态
     * Creates an MDAGNode possessing the same accept state status ant _transition set
     * (incoming & outgoing) as this node. outgoing transitions as this node.
     
     * @param soleParentNode                        the MDAGNode possessing the only _transition that targets this node
     * @param parentToCloneTransitionLabelChar      the char which labels the _transition from {@code soleParentNode} to this node
     * @return                                      an MDAGNode possessing the same accept state status and _transition set as this node.
     */
    public MDAGNode clone(MDAGNode soleParentNode, char parentToCloneTransitionLabelChar)
    {
        MDAGNode cloneNode = new MDAGNode(this);
        soleParentNode.reassignOutgoingTransition(parentToCloneTransitionLabelChar, this, cloneNode);
        
        return cloneNode;
    }
    
    

    /**
     * Retrieves the index in a simplified mdag data array that the SimpleMDAGNode
     * representation of this node's outgoing _transition set begins at.
     
     * @return      the index in a simplified mdag data array that this node's _transition set begins at,
     *              or -1 if its _transition set is not present in such an array
     */
    public int getTransitionSetBeginIndex()
    {
        return transitionSetBeginIndex;
    }
    
    
    
    /**
     * Retrieves this node's outgoing _transition count.
     
     * @return      an int representing this node's number of outgoing transitions
     */
    public int getOutgoingTransitionCount()
    {
        return outgoingTransitionTreeMap.Count;
    }
    
    
    
    /**
     * Retrieves this node's incoming _transition count
     
     * @return      an int representing this node's number of incoming transitions
     */
    public int getIncomingTransitionCount()
    {
        return incomingTransitionCount;
    }
    
    
    
    /**
     * Determines if this node is a confluence node
     * (defined as a node with two or more incoming transitions
     
     * @return      true if this node has two or more incoming transitions, false otherwise
     */
    public bool isConfluenceNode()
    {
        return (incomingTransitionCount > 1);
    }
    
    
    
    /**
     * Retrieves the accept state status of this node.
     
     * @return      true if this node is an accept state, false otherwise
     */
    public bool isAcceptNode()
    {
        return isAcceptNode;
    }
    
    
    
    /**
     * Sets this node's accept state status.
     * 
     * @param isAcceptNode     a bool representing the desired accept state status 
     */
    public void setAcceptStateStatus(bool isAcceptNode)
    {
        this.isAcceptNode = isAcceptNode;
    }
    
    
    
    /**
     * 转移状态在数组中的起始下标<br>
     * Records the index that this node's _transition set starts at
     * in an array containing this node's containing MDAG data (simplified MDAG).
     
     * @param transitionSetBeginIndex       a _transition set
     */
    public void setTransitionSetBeginIndex(int transitionSetBeginIndex)
    {
        this.transitionSetBeginIndex = transitionSetBeginIndex;
    }
    
    
    
    /**
     * Determines whether this node has an outgoing _transition with a given label.
     
     * @param letter        the char labeling the desired _transition
     * @return              true if this node possesses a _transition labeled with
     *                      {@code letter}, and false otherwise
     */
    public bool hasOutgoingTransition(char letter)
    {
        return outgoingTransitionTreeMap.ContainsKey(letter);
    }
    
    
    
    /**
     * Determines whether this node has any outgoing transitions.
     
     * @return      true if this node has at least one outgoing _transition, false otherwise
     */
    public bool hasTransitions()
    {
        return outgoingTransitionTreeMap.Count>0;
    }
    
    
    
    /**
     * Follows an outgoing _transition of this node labeled with a given char.
     
     * @param letter        the char representation of the desired _transition's label
     * @return              the MDAGNode that is the target of the _transition labeled with {@code letter},
     *                      or null if there is no such labeled _transition from this node
     */
    public MDAGNode transition(char letter)
    {
        return outgoingTransitionTreeMap.get(letter);
    }
    
    
    
    /**
     * 沿着一个路径转移<br>
     * Follows a _transition path starting from this node.
     
     * @param str               a string corresponding a _transition path in the MDAG
     * @return                  the MDAGNode at the end of the _transition path corresponding to
     *                          {@code str}, or null if such a _transition path is not present in the MDAG
     */
    public MDAGNode transition(string str)
    {
        int charCount = str.Length;
        MDAGNode currentNode = this;
        
        //Iteratively _transition through the MDAG using the chars in str
        for(int i = 0; i < charCount; i++)
        {
            currentNode = currentNode.transition(str.charAt(i));
            if(currentNode == null) break;
        }
        /////
        
        return currentNode;
    }

    public MDAGNode transition(char[] str)
    {
        int charCount = str.Length;
        MDAGNode currentNode = this;

        //Iteratively _transition through the MDAG using the chars in str
        for(int i = 0; i < charCount; ++i)
        {
            currentNode = currentNode.transition(str[i]);
            if(currentNode == null) break;
        }
        /////

        return currentNode;
    }

    public MDAGNode transition(char[] str, int offset)
    {
        int charCount = str.Length - offset;
        MDAGNode currentNode = this;

        //Iteratively _transition through the MDAG using the chars in str
        for(int i = 0; i < charCount; ++i)
        {
            currentNode = currentNode.transition(str[i + offset]);
            if(currentNode == null) break;
        }
        /////

        return currentNode;
    }

    /**
     * 获取一个字符串路径上经过的节点<br>
     * Retrieves the nodes in the _transition path starting
     * from this node corresponding to a given string .
     
     * @param str       a string corresponding to a _transition path starting from this node
     * @return          a Stack of MDAGNodes containing the nodes in the _transition path
     *                  denoted by {@code str}, in the order they are encountered in during transitioning
     */
    public Stack<MDAGNode> getTransitionPathNodes(string str)
    {
        Stack<MDAGNode> nodeStack = new Stack<MDAGNode>();
        
        MDAGNode currentNode = this;
        int numberOfChars = str.Length;
        
        //Iteratively _transition through the MDAG using the chars in str,
        //putting each encountered node in nodeStack
        for(int i = 0; i < numberOfChars && currentNode != null; i++)
        {
            currentNode = currentNode.transition(str.charAt(i));
            nodeStack.Push(currentNode);
        }
        /////
         
        return nodeStack;
    }

    
    
    /**
     * Retrieves this node's outgoing transitions.
     
     * @return      a Dictionary containing entries collectively representing
     *              all of this node's outgoing transitions
     */
    public Dictionary<char, MDAGNode> getOutgoingTransitions()
    {
        return outgoingTransitionTreeMap;
    }
    
    
    
    /**
     * 本状态的目标状态们的入度减一
     * Decrements (by 1) the incoming _transition counts of all of the nodes
     * that are targets of outgoing transitions from this node.
     */
    public void decrementTargetIncomingTransitionCounts()
    {
        foreach(var transitionKeyValuePair in  outgoingTransitionTreeMap)
            transitionKeyValuePair.Value.incomingTransitionCount--;
    }
    
    
    
    /**
     * 重新设置转移状态函数的目标
     * Reassigns the target node of one of this node's outgoing transitions.
     
     * @param letter            the char which labels the outgoing _transition of interest
     * @param oldTargetNode     the MDAGNode that is currently the target of the _transition of interest
     * @param newTargetNode     the MDAGNode that is to be the target of the _transition of interest
     */
    public void reassignOutgoingTransition(char letter, MDAGNode oldTargetNode, MDAGNode newTargetNode)
    {
        oldTargetNode.incomingTransitionCount--;
        newTargetNode.incomingTransitionCount++;
        
        outgoingTransitionTreeMap.Add(letter, newTargetNode);
    }
    
    
    
    /**
     * 新建一个转移目标<br>
     * Creates an outgoing _transition labeled with a
     * given char that has a new node as its target.
     
     * @param letter                        a char representing the desired label of the _transition
     * @param targetAcceptStateStatus       a bool representing to-be-created _transition target node's accept status
     * @return                              the (newly created) MDAGNode that is the target of the created _transition
     */
    public MDAGNode addOutgoingTransition(char letter, bool targetAcceptStateStatus)
    {
        MDAGNode newTargetNode = new MDAGNode(targetAcceptStateStatus);
        newTargetNode.incomingTransitionCount++;
        
        outgoingTransitionTreeMap.Add(letter, newTargetNode);
        return newTargetNode;
    }

    /**
     * 建立一条边（起点是自己）
     * @param letter 边上的字符串
     * @param newTargetNode 边的重点
     * @return 终点
     */
    public MDAGNode addOutgoingTransition(char letter, MDAGNode newTargetNode)
    {
        newTargetNode.incomingTransitionCount++;

        outgoingTransitionTreeMap.Add(letter, newTargetNode);
        return newTargetNode;
    }
    
    
    
    /**
     * 移除一个转移目标<br>
     * Removes a _transition labeled with a given char. This only removes the connection
     * between this node and the _transition's target node; the target node is not deleted.
     
     * @param letter        the char labeling the _transition of interest
     */
    public void removeOutgoingTransition(char letter)
    {
        outgoingTransitionTreeMap.Remove(letter);
    }


    
    /**
     * 是否含有相同的转移函数
     * @param node1
     * @param node2
     * @return
     */
    public static bool haveSameTransitions(MDAGNode node1, MDAGNode node2)
    {
        Dictionary<char, MDAGNode> outgoingTransitionTreeMap1 = node1.outgoingTransitionTreeMap;
        Dictionary<char, MDAGNode> outgoingTransitionTreeMap2 = node2.outgoingTransitionTreeMap;
        
        if(outgoingTransitionTreeMap1.Count == outgoingTransitionTreeMap2.Count)
        {
            //For each _transition in outgoingTransitionTreeMap1, get the identically lableed _transition
            //in outgoingTransitionTreeMap2 (if present), and test the equality of the transitions' target nodes
            foreach (var transitionKeyValuePair in outgoingTransitionTreeMap1)
            {
                char currentCharKey = transitionKeyValuePair.Key;
                MDAGNode currentTargetNode = transitionKeyValuePair.Value;
                
                if(!outgoingTransitionTreeMap2.ContainsKey(currentCharKey) || !outgoingTransitionTreeMap2.get(currentCharKey).Equals(currentTargetNode))
                    return false;
            }
            /////
        }
        else
            return false;
        
        return true;
    }
    
    
    
    /**
     * Clears this node's stored hash value
     */
    public void clearStoredHashCode()
    {
        storedHashCode = null;
    }
    
    
    
    /**
     * 两个状态是否等价，只有状态转移函数完全一致才算相等<br>
     * Evaluates the equality of this node with another object.
     * This node is equal to obj if and only if obj is also an MDAGNode,
     * and the set of transitions paths from this node and obj are equivalent.
     
     * @param obj       an object
     * @return          true of {@code obj} is an MDAGNode and the set of 
     *                  _transition paths from this node and obj are equivalent
     */
    //@Override
    public bool Equals(Object obj)
    {
        bool areEqual = (this == obj);
        
        if(!areEqual && obj != null && obj.getClass().Equals(MDAGNode.s))
        {
            MDAGNode node = (MDAGNode)obj;
            areEqual = (isAcceptNode == node.isAcceptNode && haveSameTransitions(this, node));
        }
       
        return areEqual;
    }

    
    
    /**
     * Hashes this node using its accept state status and set of outgoing _transition paths.
     * This is an expensive operation, so the result is cached and only cleared when necessary.
    
     * @return      an int of this node's hash code
     */
    //@Override
    public int GetHashCode() {
        
        if(storedHashCode == null)
        {
            int hash = 7;
            hash = 53 * hash + (this.isAcceptNode ? 1 : 0);
            hash = 53 * hash + (this.outgoingTransitionTreeMap != null ? this.outgoingTransitionTreeMap.GetHashCode() : 0);    //recursively hashes the nodes in all the 
                                                                                                                                //_transition paths stemming from this node
            storedHashCode = hash;
            return hash;
        }
        else
            return storedHashCode.Value;
    }

    //@Override
    public override string ToString()
    {
         StringBuilder sb = new StringBuilder("MDAGNode{");
        sb.Append("isAcceptNode=").Append(isAcceptNode);
        sb.Append(", outgoingTransitionTreeMap=").Append(outgoingTransitionTreeMap.keySet());
        sb.Append(", incomingTransitionCount=").Append(incomingTransitionCount);
//        sb.Append(", transitionSetBeginIndex=").Append(transitionSetBeginIndex);
//        sb.Append(", storedHashCode=").Append(storedHashCode);
        sb.Append('}');
        return sb.ToString();
    }
}
