

using com.hankcs.hanlp.corpus.io;
/**
* MDAG is a Java library capable of constructing character-sequence-storing,
* directed acyclic graphs of minimal size.<br>
* hankcs implemented the unSimplify() method.
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
 * 最小环形图<br>
 * A minimalistic directed acyclical graph suitable for storing a set of Strings.
 *
 * @author Kevin
 */
public class MDAG : ICacheAble
{
    //MDAGNode from which all others in the structure are reachable (all manipulation and non-simplified MDAG search operations begin from this).
    /**
     * 根节点
     */
    protected MDAGNode sourceNode = new MDAGNode(false);

    //SimpleMDAGNode from which all others in the structure are reachable (will be defined if this MDAG is simplified)
    /**
     * 简化后的根节点（简化指的是用数组简化，而不是最小化，这个结构永远是最小化的）
     */
    protected SimpleMDAGNode simplifiedSourceNode;

    //HashMap which Contains the MDAGNodes collectively representing the all unique equivalence classes in the MDAG. 
    //Uniqueness is defined by the types of transitions allowed from, and number and type of nodes reachable
    //from the node of interest. Since there are no duplicate nodes in an MDAG, # of equivalence classes == # of nodes.
    /**
     * 等价类集合，相当于论文中的register
     */
    protected Dictionary<MDAGNode, MDAGNode> equivalenceClassMDAGNodeHashMap = new Dictionary<MDAGNode, MDAGNode>();

    //Array that will contain a space-saving version of the MDAG after a call to simplify().
    /**
     * 调用simplify()后填充此空间
     */
    protected SimpleMDAGNode[] mdagDataArray;

    //HashSet which will contain the set of unique characters used as _transition labels in the MDAG
    /**
     * 字母表
     */
    protected HashSet<char> charTreeSet = new HashSet<char>();

    //An int denoting the total number of transitions between the nodes of the MDAG
    /**
     * 所有边的数量
     */
    protected int transitionCount;

    //@Override
    public void save(Stream Out)
    {
        simplify();
        Out.writeInt(charTreeSet.Count);
        foreach (var character in charTreeSet)
        {
            Out.writeChar(character);
        }
        simplifiedSourceNode.save(Out);
        Out.writeInt(mdagDataArray.Length);
        foreach (SimpleMDAGNode simpleMDAGNode in mdagDataArray)
        {
            simpleMDAGNode.save(Out);
        }
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        int Length = byteArray.Next();
        for (int i = 0; i < Length; ++i)
        {
            charTreeSet.Add(byteArray.nextChar());
        }
        simplifiedSourceNode = new SimpleMDAGNode();
        simplifiedSourceNode.load(byteArray);
        Length = byteArray.Next();
        mdagDataArray = new SimpleMDAGNode[Length];
        for (int i = 0; i < Length; ++i)
        {
            mdagDataArray[i] = new SimpleMDAGNode();
            mdagDataArray[i].load(byteArray);
        }
        sourceNode = null;
        return true;
    }

    //Enum containing fields collectively denoting the set of all conditions that can be applied to a search on the MDAG

    /**
     * 几种搜索模式
     */
    public class SearchCondition
    {
        public static SearchCondition NO_SEARCH_CONDITION = new();
        public static SearchCondition PREFIX_SEARCH_CONDITION = new();
        public static SearchCondition SUBSTRING_SEARCH_CONDITION = new();
        public static SearchCondition SUFFIX_SEARCH_CONDITION = new();

        /**
         * 判断两个字符串是否满足关系<br>
         * Determines whether two Strings have a given type of relationship.
         *
         * @param str1 字符串1
         * @param str2 字符串2
         * @return 是否满足自己这种关系
         */
        public bool satisfiesCondition(string str1, string str2)
        {
            bool satisfiesSearchCondition;

            switch (this)
            {
                case PREFIX_SEARCH_CONDITION:
                    satisfiesSearchCondition = (str1.StartsWith(str2));
                    break;
                case SUBSTRING_SEARCH_CONDITION:
                    satisfiesSearchCondition = (str1.Contains(str2));
                    break;
                case SUFFIX_SEARCH_CONDITION:
                    satisfiesSearchCondition = (str1.EndsWith(str2));
                    break;
                default:
                    satisfiesSearchCondition = true;
                    break;
            }

            return satisfiesSearchCondition;
        }
    }
    /////

    /**
     * 从文件路径构造
     * @param path
     * @
     */
    public MDAG(string path) 
        : this(IOUtil.newBufferedReader(path))
    {
        ;
    }


    /**
     * 从一个文件建立MDAG<br>
     * Creates an MDAG from a newline delimited file containing the data of interest.
     *
     * @param dataFile a {@link java.io.File} representation of a file
     *                 containing the Strings that the MDAG will contain
     * @throws java.io.IOException if {@code datafile} cannot be opened, or a read operation on it cannot be carried Out
     */
    public MDAG(File dataFile)
        : this(IOUtil.newBufferedReader(dataFile.getPath()))
    {
       ;
    }

    /**
     * 从一个打开的TextReader构造
     * @param dataFileBufferedReader
     * @
     */
    public MDAG(TextReader dataFileBufferedReader) 
    {
        string currentString = "";
        string previousString = "";

        //Read all the lines in dataFile and Add the string contained in each to the MDAG.
        while ((currentString = dataFileBufferedReader.ReadLine()) != null)
        {
            int mpsIndex = calculateMinimizationProcessingStartIndex(previousString, currentString);

            //If the _transition path of the previousString needs to be examined for minimization or
            //equivalence class representation after a certain point, call replaceOrRegister to do so.
            if (mpsIndex != -1)
            {
                string transitionSubstring = previousString.substring(0, mpsIndex);             // 公共前缀
                string minimizationProcessingSubstring = previousString.substring(mpsIndex);    // 不同后缀
                replaceOrRegister(sourceNode.transition(transitionSubstring), minimizationProcessingSubstring);
            }
            /////

            addStringInternal(currentString);
            previousString = currentString;
        }
        /////

        //Since we delay the minimization of the previously-added string
        //until after we read the next one, we need to have a seperate
        //statement to minimize the absolute last string.
        replaceOrRegister(sourceNode, previousString);
    }


    /**
     * Creates an MDAG from a collection of Strings.
     *
     * @param strCollection a {@link java.util.Collection} containing Strings that the MDAG will contain
     */
    public MDAG(ICollection<string> strCollection)
    {
        addStrings(strCollection);
    }

    /**
     * 空白图
     */
    public MDAG()
    {
    }

    /**
     * Adds a Collection of Strings to the MDAG.
     *
     * @param strCollection a {@link java.util.Collection} containing Strings to be added to the MDAG
     */
    public void addStrings(ICollection<string> strCollection)
    {
        if (sourceNode != null)
        {
            string previousString = "";

            //Add all the Strings in strCollection to the MDAG.
            foreach (string currentString in strCollection)
            {
                int mpsIndex = calculateMinimizationProcessingStartIndex(previousString, currentString);

                //If the _transition path of the previousString needs to be examined for minimization or
                //equivalence class representation after a certain point, call replaceOrRegister to do so.
                if (mpsIndex != -1)
                {

                    string transitionSubstring = previousString.substring(0, mpsIndex);
                    string minimizationProcessingSubString = previousString.substring(mpsIndex);
                    replaceOrRegister(sourceNode.transition(transitionSubstring), minimizationProcessingSubString);
                }
                /////

                addStringInternal(currentString);
                previousString = currentString;
            }
            /////

            //Since we delay the minimization of the previously-added string
            //until after we read the next one, we need to have a seperate
            //statement to minimize the absolute last string.
            replaceOrRegister(sourceNode, previousString);
        }
        else
        {
            unSimplify();
            addStrings(strCollection);
        }
    }


    /**
     * Adds a string to the MDAG.
     *
     * @param str the string to be added to the MDAG
     */
    public void addString(string str)
    {
        if (sourceNode != null)
        {
            addStringInternal(str);
            replaceOrRegister(sourceNode, str);
        }
        else
        {
            unSimplify();
            addString(str);
        }
    }


    private void splitTransitionPath(MDAGNode originNode, string storedStringSubstr)
    {
        var firstConfluenceNodeDataHashMap = getTransitionPathFirstConfluenceNodeData(originNode, storedStringSubstr);
        int toFirstConfluenceNodeTransitionCharIndex = (int) firstConfluenceNodeDataHashMap.get("toConfluenceNodeTransitionCharIndex");
        MDAGNode firstConfluenceNode = (MDAGNode) firstConfluenceNodeDataHashMap.get("confluenceNode");

        if (firstConfluenceNode != null)
        {
            MDAGNode firstConfluenceNodeParent = originNode.transition(storedStringSubstr.substring(0, toFirstConfluenceNodeTransitionCharIndex));

            MDAGNode firstConfluenceNodeClone = firstConfluenceNode.clone(firstConfluenceNodeParent, storedStringSubstr.charAt(toFirstConfluenceNodeTransitionCharIndex));

            transitionCount += firstConfluenceNodeClone.getOutgoingTransitionCount();

            string unprocessedSubString = storedStringSubstr.substring(toFirstConfluenceNodeTransitionCharIndex + 1);
            splitTransitionPath(firstConfluenceNodeClone, unprocessedSubString);
        }
    }


    /**
     * Calculates the Length of the the sub-path in a _transition path, that is used only by a given string.
     *
     * @param str a string corresponding to a _transition path from sourceNode
     * @return an int denoting the size of the sub-path in the _transition path
     * corresponding to {@code str} that is only used by {@code str}
     */
    private int calculateSoleTransitionPathLength(string str)
    {
        Stack<MDAGNode> transitionPathNodeStack = sourceNode.getTransitionPathNodes(str);
        transitionPathNodeStack.pop();  //The MDAGNode at the top of the stack is not needed
        //(we are processing the outgoing transitions of nodes inside str's _transition path,
        //the outgoing transitions of the MDAGNode at the top of the stack are outside this path)

        transitionPathNodeStack.trimToSize();

        //Process each node in transitionPathNodeStack, using each to determine whether the
        //_transition path corresponding to str is only used by str.  This is true if and only if
        //each node in the _transition path has a single outgoing _transition and is not an accept state.
        while (!transitionPathNodeStack.isEmpty())
        {
            MDAGNode currentNode = transitionPathNodeStack.peek();
            if (currentNode.getOutgoingTransitions().Count <= 1 && !currentNode.isAcceptNode())
                transitionPathNodeStack.pop();
            else
                break;
        }
        /////

        return (transitionPathNodeStack.capacity() - transitionPathNodeStack.Count);
    }


    /**
     * Removes a string from the MDAG.
     *
     * @param str the string to be removed from the MDAG
     */
    public void removeString(string str)
    {
        if (sourceNode != null)
        {
            //Split the _transition path corresponding to str to ensure that
            //any other _transition paths sharing nodes with it are not affected
            splitTransitionPath(sourceNode, str);

            //Remove from equivalenceClassMDAGNodeHashMap, the entries of all the nodes in the _transition path corresponding to str.
            removeTransitionPathRegisterEntries(str);

            //Get the last node in the _transition path corresponding to str
            MDAGNode strEndNode = sourceNode.transition(str);
            if (strEndNode == null) return;

            if (!strEndNode.hasTransitions())
            {
                int soleInternalTransitionPathLength = calculateSoleTransitionPathLength(str);
                int internalTransitionPathLength = str.Length - 1;

                if (soleInternalTransitionPathLength == internalTransitionPathLength)
                {
                    sourceNode.removeOutgoingTransition(str[0]);
                    transitionCount -= str.Length;
                }
                else
                {
                    //Remove the sub-path in str's _transition path that is only used by str
                    int toBeRemovedTransitionLabelCharIndex = (internalTransitionPathLength - soleInternalTransitionPathLength);
                    MDAGNode latestNonSoloTransitionPathNode = sourceNode.transition(str.substring(0, toBeRemovedTransitionLabelCharIndex));
                    latestNonSoloTransitionPathNode.removeOutgoingTransition(str.charAt(toBeRemovedTransitionLabelCharIndex));
                    transitionCount -= str.substring(toBeRemovedTransitionLabelCharIndex).Length;
                    /////

                    replaceOrRegister(sourceNode, str.substring(0, toBeRemovedTransitionLabelCharIndex));
                }

            }
            else
            {
                strEndNode.setAcceptStateStatus(false);
                replaceOrRegister(sourceNode, str);
            }
        }
        else
        {
            unSimplify();
        }
    }


    /**
     * 计算最小化的执行位置，其实是prevStr和currStr的第一个分叉点<br>
     * Determines the start index of the substring in the string most recently added to the MDAG
     * that corresponds to the _transition path that will be next up for minimization processing.
     * <p/>
     * The "minimization processing start index" is defined as the index in {@code prevStr} which starts the substring
     * corresponding to the _transition path that doesn't have its right language extended by {@code currStr}. The _transition path of
     * the substring before this point is not considered for minimization in order to limit the amount of times the
     * equivalence classes of its nodes will need to be reassigned during the processing of Strings which share prefixes.
     *
     * @param prevStr the string most recently added to the MDAG
     * @param currStr the string next to be added to the MDAG
     * @return an int of the index in {@code prevStr} that starts the substring corresponding
     * to the _transition path next up for minimization processing
     */
    private int calculateMinimizationProcessingStartIndex(string prevStr, string currStr)
    {
        int mpsIndex;

        if (!currStr.StartsWith(prevStr))
        {
            //Loop through the corresponding indices of both Strings in search of the first index containing differing characters.
            //The _transition path of the substring of prevStr from this point will need to be submitted for minimization processing.
            //The substring before this point, however, does not, since currStr will simply be extending the right languages of the 
            //nodes on its _transition path.
            int shortestStringLength = Math.Min(prevStr.Length, currStr.Length);
            for (mpsIndex = 0; mpsIndex < shortestStringLength && prevStr.charAt(mpsIndex) == currStr.charAt(mpsIndex); mpsIndex++)
            {
            }
            ;
            /////
        }
        else
            mpsIndex = -1;    //If the prevStr is a prefix of currStr, then currStr simply : the right language of the _transition path of prevStr.

        return mpsIndex;
    }


    /**
     * Determines the longest prefix of a given string that is
     * the prefix of another string previously added to the MDAG.
     *
     * @param str the string to be processed
     * @return a string of the longest prefix of {@code str}
     * that is also a prefix of a string contained in the MDAG
     */
    private string determineLongestPrefixInMDAG(string str)
    {
        MDAGNode currentNode = sourceNode;
        int numberOfChars = str.Length;
        int onePastPrefixEndIndex = 0;

        //Loop through the characters in str, using them in sequence to _transition
        //through the MDAG until the currently processing node doesn't have a _transition
        //labeled with the current processing char, or there are no more characters to process. 
        for (int i = 0; i < numberOfChars; i++, onePastPrefixEndIndex++)
        {
            char currentChar = str[i];
            if (currentNode.hasOutgoingTransition(currentChar))
                currentNode = currentNode.transition(currentChar);
            else
                break;
        }
        /////

        return str.substring(0, onePastPrefixEndIndex);
    }


    /**
     * Determines and retrieves data related to the first confluence node
     * (defined as a node with two or more incoming transitions) of a
     * _transition path corresponding to a given string from a given node.
     *
     * @param originNode the MDAGNode from which the _transition path corresponding to str starts from
     * @param str        a string corresponding to a _transition path in the MDAG
     * @return a HashMap of Strings to Objects containing:
     * - an int denoting the Length of the path to the first confluence node in the _transition path of interest
     * - the MDAGNode which is the first confluence node in the _transition path of interest (or null if one does not exist)
     */
    private Dictionary<string, Object> getTransitionPathFirstConfluenceNodeData(MDAGNode originNode, string str)
    {
        int currentIndex = 0;
        int charCount = str.Length;
        MDAGNode currentNode = originNode;

        //Loop thorugh the characters in str, sequentially using them to _transition through the MDAG in search of
        //(and breaking upon reaching) the first node that is the target of two or more transitions. The loop is 
        //also broken from if the currently processing node doesn't have a _transition labeled with the currently processing char.
        for (; currentIndex < charCount; currentIndex++)
        {
            char currentChar = str.charAt(currentIndex);
            currentNode = (currentNode.hasOutgoingTransition(currentChar) ? currentNode.transition(currentChar) : null);

            if (currentNode == null || currentNode.isConfluenceNode())
                break;
        }
        /////

        bool noConfluenceNode = (currentNode == originNode || currentIndex == charCount);

        //Create a HashMap containing the index of the last char in the substring corresponding
        //to the transitoin path to the confluence node, as well as the actual confluence node
        var confluenceNodeDataHashMap = new Dictionary<string, Object>(2);
        confluenceNodeDataHashMap.Add("toConfluenceNodeTransitionCharIndex", (noConfluenceNode ? null : currentIndex));
        confluenceNodeDataHashMap.Add("confluenceNode", noConfluenceNode ? null : currentNode);
        /////

        return confluenceNodeDataHashMap;
    }


    /**
     * 在从给定节点开始的一段路径上执行最小化<br>
     * Performs minimization processing on a _transition path starting from a given node.
     * <p/>
     * This entails either replacing a node in the path with one that has an equivalent right language/equivalence class
     * (defined as set of _transition paths that can be traversed and nodes able to be reached from it), or making it
     * a representative of a right language/equivalence class if a such a node does not already exist.
     *
     * @param originNode the MDAGNode that the _transition path corresponding to str starts from
     * @param str        a string related to a _transition path
     */
    private void replaceOrRegister(MDAGNode originNode, string str)
    {
        char transitionLabelChar = str[0];
        MDAGNode relevantTargetNode = originNode.transition(transitionLabelChar);

        //If relevantTargetNode has transitions and there is at least one char left to process, recursively call 
        //this on the next char in order to further processing down the _transition path corresponding to str
        if (relevantTargetNode.hasTransitions() && !str.substring(1).isEmpty())
            replaceOrRegister(relevantTargetNode, str.substring(1));
        /////

        //Get the node representing the equivalence class that relevantTargetNode belongs to. MDAGNodes hash on the
        //transitions paths that can be traversed from them and nodes able to be reached from them;
        //nodes with the same equivalence classes will hash to the same bucket.
        MDAGNode equivalentNode = equivalenceClassMDAGNodeHashMap.get(relevantTargetNode);

        if (equivalentNode == null)  //if there is no node with the same right language as relevantTargetNode
            equivalenceClassMDAGNodeHashMap.Add(relevantTargetNode, relevantTargetNode);
        else if (equivalentNode != relevantTargetNode)   //if there is another node with the same right language as relevantTargetNode, reassign the
        {                                               //_transition between originNode and relevantTargetNode, to originNode and the node representing the equivalence class of interest
            relevantTargetNode.decrementTargetIncomingTransitionCounts();
            transitionCount -= relevantTargetNode.getOutgoingTransitionCount(); //Since this method is recursive, the outgoing transitions of all of relevantTargetNode's child nodes have already been reassigned, 
            //so we only need to decrement the _transition count by the relevantTargetNode's outgoing _transition count
            originNode.reassignOutgoingTransition(transitionLabelChar, relevantTargetNode, equivalentNode);
        }
    }


    /**
     * 给节点添加一个转移路径<br>
     * Adds a _transition path starting from a specific node in the MDAG.
     *
     * @param originNode the MDAGNode which will serve as the start point of the to-be-created _transition path
     * @param str        the string to be used to create a new _transition path from {@code originNode}
     */
    private void addTransitionPath(MDAGNode originNode, string str)
    {
        if (!str.isEmpty())
        {
            MDAGNode currentNode = originNode;
            int charCount = str.Length;

            //Loop through the characters in str, iteratevely adding
            // a _transition path corresponding to it from originNode
            for (int i = 0; i < charCount; i++, transitionCount++)
            {
                char currentChar = str[i];
                bool isLastChar = (i == charCount - 1);
                currentNode = currentNode.addOutgoingTransition(currentChar, isLastChar);

                charTreeSet.Add(currentChar);
            }
            /////
        }
        else
            originNode.setAcceptStateStatus(true);
    }


    /**
     * 从登记簿中移除路径对应的状态们<br>
     * Removes from equivalenceClassMDAGNodeHashmap the entries of all the nodes in a _transition path.
     *
     * @param str a string corresponding to a _transition path from sourceNode
     */
    private void removeTransitionPathRegisterEntries(string str)
    {
        MDAGNode currentNode = sourceNode;

        int charCount = str.Length;

        for (int i = 0; i < charCount; i++)
        {
            currentNode = currentNode.transition(str[i]);
            if (equivalenceClassMDAGNodeHashMap.get(currentNode) == currentNode)
                equivalenceClassMDAGNodeHashMap.Remove(currentNode);

            //The GetHashCode of an MDAGNode is cached the first time a hash is performed without a cache value present.
            //Since we just hashed currentNode, we must Clear this regardless of its presence in equivalenceClassMDAGNodeHashMap
            //since we're not actually declaring equivalence class representatives here.
            if (currentNode != null) currentNode.clearStoredHashCode();
        }
    }


    /**
     * 从给点节点开始克隆一条路径<br>
     * Clones a _transition path from a given node.
     *
     * @param pivotConfluenceNode         the MDAGNode that the cloning operation is to be based from
     * @param transitionStringToPivotNode a string which corresponds with a _transition path from souceNode to {@code pivotConfluenceNode}
     * @param str                         a string which corresponds to the _transition path from {@code pivotConfluenceNode} that is to be cloned
     */
    private void cloneTransitionPath(MDAGNode pivotConfluenceNode, string transitionStringToPivotNode, string str)
    {
        MDAGNode lastTargetNode = pivotConfluenceNode.transition(str);      //Will store the last node which was used as the base of a cloning operation
        MDAGNode lastClonedNode = null;                                     //Will store the last cloned node
        char lastTransitionLabelChar = '\0';                                //Will store the char which labels the _transition to lastTargetNode from its parent node in the prefixString's _transition path

        //Loop backwards through the indices of str, using each as a boundary to create substrings of str of decreasing Length
        //which will be used to _transition to, and duplicate the nodes in the _transition path of str from pivotConfluenceNode.
        for (int i = str.Length; i >= 0; i--)
        {
            string currentTransitionString = (i > 0 ? str.substring(0, i) : null);
            MDAGNode currentTargetNode = (i > 0 ? pivotConfluenceNode.transition(currentTransitionString) : pivotConfluenceNode);
            MDAGNode clonedNode;

            if (i == 0)  //if we have reached pivotConfluenceNode
            {
                //Clone pivotConfluenceNode in a way that reassigns the _transition of its parent node (in transitionStringToConfluenceNode's path) to the clone.
                string transitionStringToPivotNodeParent = transitionStringToPivotNode.substring(0, transitionStringToPivotNode.Length - 1);
                char parentTransitionLabelChar = transitionStringToPivotNode.charAt(transitionStringToPivotNode.Length - 1);
                clonedNode = pivotConfluenceNode.clone(sourceNode.transition(transitionStringToPivotNodeParent), parentTransitionLabelChar);
                /////
            }
            else
                clonedNode = currentTargetNode.clone();     //simply clone curentTargetNode

            transitionCount += clonedNode.getOutgoingTransitionCount();

            //If this isn't the first node we've cloned, reassign clonedNode's _transition labeled
            //with the lastTransitionChar (which points to the last targetNode) to the last clone.
            if (lastClonedNode != null)
            {
                clonedNode.reassignOutgoingTransition(lastTransitionLabelChar, lastTargetNode, lastClonedNode);
                lastTargetNode = currentTargetNode;
            }

            //Store clonedNode and the char which labels the _transition between the node it was cloned from (currentTargetNode) and THAT node's parent.
            //These will be used to establish an equivalent _transition to clonedNode from the next clone to be created (it's clone parent).
            lastClonedNode = clonedNode;
            lastTransitionLabelChar = (i > 0 ? str.charAt(i - 1) : '\0');
            /////
        }
        /////
    }


    /**
     * Adds a string to the MDAG (called by addString to do actual MDAG manipulation).
     *
     * @param str the string to be added to the MDAG
     */
    private void addStringInternal(string str)
    {
        string prefixString = determineLongestPrefixInMDAG(str);
        string suffixString = str.substring(prefixString.Length);

        //Retrive the data related to the first confluence node (a node with two or more incoming transitions)
        //in the _transition path from sourceNode corresponding to prefixString.
        var firstConfluenceNodeDataHashMap = getTransitionPathFirstConfluenceNodeData(sourceNode, prefixString);
        MDAGNode firstConfluenceNodeInPrefix = (MDAGNode) firstConfluenceNodeDataHashMap.get("confluenceNode");
        int toFirstConfluenceNodeTransitionCharIndex = (int) firstConfluenceNodeDataHashMap.get("toConfluenceNodeTransitionCharIndex");
        /////

        //Remove the register entries of all the nodes in the prefixString _transition path up to the first confluence node
        //(those past the confluence node will not need to be removed since they will be cloned and unaffected by the 
        //addition of suffixString). If there is no confluence node in prefixString, then Remove the register entries in prefixString's entire _transition path
        removeTransitionPathRegisterEntries((toFirstConfluenceNodeTransitionCharIndex == null ? prefixString : prefixString.substring(0, toFirstConfluenceNodeTransitionCharIndex)));

        //If there is a confluence node in the prefix, we must duplicate the _transition path
        //of the prefix starting from that node, before we Add suffixString (to the duplicate path).
        //This ensures that we do not disturb the other _transition paths containing this node.
        if (firstConfluenceNodeInPrefix != null)
        {
            string transitionStringOfPathToFirstConfluenceNode = prefixString.substring(0, toFirstConfluenceNodeTransitionCharIndex + 1);
            string transitionStringOfToBeDuplicatedPath = prefixString.substring(toFirstConfluenceNodeTransitionCharIndex + 1);
            cloneTransitionPath(firstConfluenceNodeInPrefix, transitionStringOfPathToFirstConfluenceNode, transitionStringOfToBeDuplicatedPath);
        }
        /////

        //Add the _transition based on suffixString to the end of the (possibly duplicated) _transition path corresponding to prefixString
        addTransitionPath(sourceNode.transition(prefixString), suffixString);
    }


    /**
     * Creates a SimpleMDAGNode version of an MDAGNode's outgoing _transition set in mdagDataArray.
     *
     * @param node                                 the MDAGNode containing the _transition set to be inserted in to {@code mdagDataArray}
     * @param mdagDataArray                        an array of SimpleMDAGNodes containing a subset of the data of the MDAG
     * @param onePastLastCreatedTransitionSetIndex an int of the index in {@code mdagDataArray} that the outgoing _transition set of {@code node} is to start from
     * @return an int of one past the end of the _transition set located farthest in {@code mdagDataArray}
     */
    private int createSimpleMDAGTransitionSet(MDAGNode node, SimpleMDAGNode[] mdagDataArray, int onePastLastCreatedTransitionSetIndex)
    {
        int pivotIndex = onePastLastCreatedTransitionSetIndex;  // node自己的位置
        node.setTransitionSetBeginIndex(pivotIndex);

        onePastLastCreatedTransitionSetIndex += node.getOutgoingTransitionCount();  // 这个参数代表id的消耗

        //Create a SimpleMDAGNode representing each _transition label/target combo in transitionTreeMap, recursively calling this method (if necessary)
        //to set indices in these SimpleMDAGNodes that the set of transitions emitting from their respective _transition targets starts from.
        var transitionTreeMap = node.getOutgoingTransitions();
        foreach (var transitionKeyValuePair in transitionTreeMap.entrySet())
        {
            //Use the current _transition's label and target node to create a SimpleMDAGNode
            //(which is a space-saving representation of the _transition), and insert it in to mdagDataArray
            char transitionLabelChar = transitionKeyValuePair.Key;
            MDAGNode transitionTargetNode = transitionKeyValuePair.Value;
            mdagDataArray[pivotIndex] = new SimpleMDAGNode(transitionLabelChar, transitionTargetNode.isAcceptNode(), transitionTargetNode.getOutgoingTransitionCount());
            /////

            //If targetTransitionNode's outgoing _transition set hasn't been inserted in to mdagDataArray yet, call this method on it to do so.
            //After this call returns, transitionTargetNode will contain the index in mdagDataArray that its _transition set starts from
            if (transitionTargetNode.getTransitionSetBeginIndex() == -1)
                onePastLastCreatedTransitionSetIndex = createSimpleMDAGTransitionSet(transitionTargetNode, mdagDataArray, onePastLastCreatedTransitionSetIndex);

            mdagDataArray[pivotIndex++].setTransitionSetBeginIndex(transitionTargetNode.getTransitionSetBeginIndex());
        }
        /////

        return onePastLastCreatedTransitionSetIndex;
    }


    /**
     * 固化自己<br>
     * Creates a space-saving version of the MDAG in the form of an array.
     * Once the MDAG is simplified, Strings can no longer be added to or removed from it.
     */
    public void simplify()
    {
        if (sourceNode != null)
        {
            mdagDataArray = new SimpleMDAGNode[transitionCount];
            createSimpleMDAGTransitionSet(sourceNode, mdagDataArray, 0);
            simplifiedSourceNode = new SimpleMDAGNode('\0', false, sourceNode.getOutgoingTransitionCount());

            //Mark the previous MDAG data structure and equivalenceClassMDAGNodeHashMap
            //for garbage collection since they are no longer needed.
            sourceNode = null;
            equivalenceClassMDAGNodeHashMap = null;
            /////
        }
    }

    /**
     * 解压缩
     */
    public void unSimplify()
    {
        if (sourceNode == null)
        {
            sourceNode = new MDAGNode(false);
            equivalenceClassMDAGNodeHashMap = new ();
            MDAGNode[] toNodeArray = new MDAGNode[mdagDataArray.Length];
            createMDAGNode(simplifiedSourceNode, -1, toNodeArray, new MDAGNode[mdagDataArray.Length]);
            // 构建注册表
            foreach (MDAGNode mdagNode in toNodeArray)
            {
                equivalenceClassMDAGNodeHashMap.Add(mdagNode, mdagNode);
            }
            // 扔掉垃圾
            simplifiedSourceNode = null;
        }
    }

    /**
     * 递归创建节点<br>
     * @param current 当前简易节点
     * @param fromIndex 起点下标
     * @param toNodeArray 终点数组
     * @param fromNodeArray 起点数组，它们两个按照下标一一对应
     */
    private void createMDAGNode(SimpleMDAGNode current, int fromIndex, MDAGNode[] toNodeArray, MDAGNode[] fromNodeArray)
    {
        MDAGNode from = (fromIndex == -1 ? sourceNode : toNodeArray[fromIndex]);
        int transitionSetBegin = current.getTransitionSetBeginIndex();
        int onePastTransitionSetEnd = transitionSetBegin + current.getOutgoingTransitionSetSize();

        for (int i = transitionSetBegin; i < onePastTransitionSetEnd; i++)
        {
            SimpleMDAGNode targetNode = mdagDataArray[i];
            if (toNodeArray[i] != null)
            {
                fromNodeArray[fromIndex].addOutgoingTransition(current.getLetter(), fromNodeArray[i]);
                toNodeArray[fromIndex] = fromNodeArray[i];
                continue;
            }
            toNodeArray[i] = from.addOutgoingTransition(targetNode.getLetter(), targetNode.isAcceptNode());
            fromNodeArray[i] = from;
            createMDAGNode(targetNode, i, toNodeArray, fromNodeArray);
        }
    }



    /**
     * 是否包含<br>
     * Determines whether a string is present in the MDAG.
     *
     * @param str the string to be searched for
     * @return true if {@code str} is present in the MDAG, and false otherwise
     */
    public bool Contains(string str)
    {
        if (sourceNode != null)      //if the MDAG hasn't been simplified
        {
            MDAGNode targetNode = sourceNode.transition(str.ToCharArray());
            return (targetNode != null && targetNode.isAcceptNode());
        }
        else
        {
            SimpleMDAGNode targetNode = simplifiedSourceNode.transition(mdagDataArray, str.ToCharArray());
            return (targetNode != null && targetNode.isAcceptNode());
        }
    }


    /**
     * Retrieves Strings corresponding to all valid _transition paths from a given node that satisfy a given condition.
     *
     * @param strHashSet            a HashSet of Strings to contain all those in the MDAG satisfying
     *                              {@code searchCondition} with {@code conditionString}
     * @param searchCondition       the SearchCondition enum field describing the type of relationship that Strings contained in the MDAG
     *                              must have with {@code conditionString} in order to be included in the result set
     * @param searchConditionString the string that all Strings in the MDAG must be related with in the fashion denoted
     *                              by {@code searchCondition} in order to be included in the result set
     * @param prefixString          the string corresponding to the currently traversed _transition path
     * @param transitionTreeMap     a Dictionary of Characters to MDAGNodes collectively representing an MDAGNode's _transition set
     */
    private void getStrings(HashSet<string> strHashSet, SearchCondition searchCondition, string searchConditionString, string prefixString, Dictionary<char, MDAGNode> transitionTreeMap)
    {
        //Traverse all the valid _transition paths beginning from each _transition in transitionTreeMap, inserting the
        //corresponding Strings in to strHashSet that have the relationship with conditionString denoted by searchCondition
        foreach (var transitionKeyValuePair in transitionTreeMap.entrySet())
        {
            string newPrefixString = prefixString + transitionKeyValuePair.Key;
            MDAGNode currentNode = transitionKeyValuePair.Value;

            if (currentNode.isAcceptNode() && searchCondition.satisfiesCondition(newPrefixString, searchConditionString))
                strHashSet.Add(newPrefixString);

            //Recursively call this to traverse all the valid _transition paths from currentNode
            getStrings(strHashSet, searchCondition, searchConditionString, newPrefixString, currentNode.getOutgoingTransitions());
        }
        /////
    }


    /**
     * Retrieves Strings corresponding to all valid _transition paths from a given node that satisfy a given condition.
     *
     * @param strHashSet              a HashSet of Strings to contain all those in the MDAG satisfying
     *                                {@code searchCondition} with {@code conditionString}
     * @param searchCondition         the SearchCondition enum field describing the type of relationship that Strings contained in the MDAG
     *                                must have with {@code conditionString} in order to be included in the result set
     * @param searchConditionString   the string that all Strings in the MDAG must be related with in the fashion denoted
     *                                by {@code searchCondition} in order to be included in the result set
     * @param prefixString            the string corresponding to the currently traversed _transition path
     * @param node                    an int denoting the starting index of a SimpleMDAGNode's _transition set in mdagDataArray
     */
    private void getStrings(HashSet<string> strHashSet, SearchCondition searchCondition, string searchConditionString, string prefixString, SimpleMDAGNode node)
    {
        int transitionSetBegin = node.getTransitionSetBeginIndex();
        int onePastTransitionSetEnd = transitionSetBegin + node.getOutgoingTransitionSetSize();

        //Traverse all the valid _transition paths beginning from each _transition in transitionTreeMap, inserting the
        //corresponding Strings in to strHashSet that have the relationship with conditionString denoted by searchCondition
        for (int i = transitionSetBegin; i < onePastTransitionSetEnd; i++)
        {
            SimpleMDAGNode currentNode = mdagDataArray[i];
            string newPrefixString = prefixString + currentNode.getLetter();

            if (currentNode.isAcceptNode() && searchCondition.satisfiesCondition(newPrefixString, searchConditionString))
                strHashSet.Add(newPrefixString);

            //Recursively call this to traverse all the valid _transition paths from currentNode
            getStrings(strHashSet, searchCondition, searchConditionString, newPrefixString, currentNode);
        }
        /////
    }


    /**
     * 取出所有key<br>
     * Retrieves all the valid Strings that have been inserted in to the MDAG.
     *
     * @return a HashSet containing all the Strings that have been inserted into the MDAG
     */
    public HashSet<string> getAllStrings()
    {
        HashSet<string> strHashSet = new ();

        if (sourceNode != null)
            getStrings(strHashSet, SearchCondition.NO_SEARCH_CONDITION, null, "", sourceNode.getOutgoingTransitions());
        else
            getStrings(strHashSet, SearchCondition.NO_SEARCH_CONDITION, null, "", simplifiedSourceNode);

        return strHashSet;
    }


    /**
     * 前缀查询<br>
     * Retrieves all the Strings in the MDAG that begin with a given string.
     *
     * @param prefixStr a string that is the prefix for all the desired Strings
     * @return a HashSet containing all the Strings present in the MDAG that begin with {@code prefixString}
     */
    public HashSet<string> getStringsStartingWith(string prefixStr)
    {
        HashSet<string> strHashSet = new HashSet<string>();

        if (sourceNode != null)      //if the MDAG hasn't been simplified
        {
            MDAGNode originNode = sourceNode.transition(prefixStr);  //attempt to _transition down the path denoted by prefixStr

            if (originNode != null) //if there a _transition path corresponding to prefixString (one or more stored Strings begin with prefixString)
            {
                if (originNode.isAcceptNode()) strHashSet.Add(prefixStr);
                getStrings(strHashSet, SearchCondition.PREFIX_SEARCH_CONDITION, prefixStr, prefixStr, originNode.getOutgoingTransitions());   //retrieve all Strings that extend the _transition path denoted by prefixStr
            }
        }
        else
        {
            SimpleMDAGNode originNode = SimpleMDAGNode.traverseMDAG(mdagDataArray, simplifiedSourceNode, prefixStr);      //attempt to _transition down the path denoted by prefixStr

            if (originNode != null)      //if there a _transition path corresponding to prefixString (one or more stored Strings begin with prefixStr)
            {
                if (originNode.isAcceptNode()) strHashSet.Add(prefixStr);
                getStrings(strHashSet, SearchCondition.PREFIX_SEARCH_CONDITION, prefixStr, prefixStr, originNode);        //retrieve all Strings that extend the _transition path denoted by prefixString
            }
        }

        return strHashSet;
    }


    /**
     * 返回包含字串的key<br>
     * Retrieves all the Strings in the MDAG that contain a given string.
     *
     * @param str a string that is contained in all the desired Strings
     * @return a HashSet containing all the Strings present in the MDAG that begin with {@code prefixString}
     */
    public HashSet<string> getStringsWithSubstring(string str)
    {
        HashSet<string> strHashSet = new HashSet<string>();

        if (sourceNode != null)      //if the MDAG hasn't been simplified
            getStrings(strHashSet, SearchCondition.SUBSTRING_SEARCH_CONDITION, str, "", sourceNode.getOutgoingTransitions());
        else
            getStrings(strHashSet, SearchCondition.SUBSTRING_SEARCH_CONDITION, str, "", simplifiedSourceNode);

        return strHashSet;
    }


    /**
     * 后缀查询<br>
     * Retrieves all the Strings in the MDAG that begin with a given string.
     *
     * @param suffixStr a string that is the suffix for all the desired Strings
     * @return a HashSet containing all the Strings present in the MDAG that end with {@code suffixStr}
     */
    public HashSet<string> getStringsEndingWith(string suffixStr)
    {
        HashSet<string> strHashSet = new HashSet<string>();

        if (sourceNode != null)      //if the MDAG hasn't been simplified
            getStrings(strHashSet, SearchCondition.SUFFIX_SEARCH_CONDITION, suffixStr, "", sourceNode.getOutgoingTransitions());
        else
            getStrings(strHashSet, SearchCondition.SUFFIX_SEARCH_CONDITION, suffixStr, "", simplifiedSourceNode);

        return strHashSet;
    }


    /**
     * 获取MDAG的根节点<br>
     * Returns the MDAG's source node.
     *
     * @return the MDAGNode or SimpleMDAGNode functioning as the MDAG's source node.
     */
    private Object getSourceNode()
    {
        return (sourceNode != null ? sourceNode : simplifiedSourceNode);
    }


    /**
     * 获取简化后的状态Array<br>
     * Returns the array of SimpleMDAGNodes collectively containing the
     * data of this MDAG, or null if it hasn't been simplified yet.
     *
     * @return the array of SimpleMDAGNodes collectively containing the data of this MDAG
     * if this MDAG has been simplified, or null if it has not
     */
    public SimpleMDAGNode[] getSimpleMDAGArray()
    {
        return mdagDataArray;
    }


    /**
     * Procures the set of characters which collectively label the MDAG's transitions.
     *
     * @return a TreeSet of chars which collectively label all the transitions in the MDAG
     */
    private HashSet<char> getTransitionLabelSet()
    {
        return charTreeSet;
    }


    /**
     * Determines if a child node object is accepting.
     *
     * @param nodeObj an Object
     * @return if {@code nodeObj} is either an MDAGNode or a SimplifiedMDAGNode,
     * true if the node is accepting, false otherwise
     *       if {@code nodeObj} is not an MDAGNode or SimplifiedMDAGNode
     */
    private static bool isAcceptNode(Object nodeObj)
    {
        if (nodeObj != null)
        {
            Type nodeObjClass = nodeObj.getClass();

            if (nodeObjClass.Equals(MDAGNode.c))
                return ((MDAGNode) nodeObj).isAcceptNode();
            else if (nodeObjClass.Equals(SimpleMDAGNode.c))
                return ((SimpleMDAGNode) nodeObj).isAcceptNode();

        }

        throw new ArgumentException("Argument is not an MDAGNode or SimpleMDAGNode");
    }

//    //@Override
//    public override string ToString()
//    {
//        StringBuilder sb = new StringBuilder("MDAG{");
//        sb.Append("sourceNode=").Append(sourceNode);
//        sb.Append(", simplifiedSourceNode=").Append(simplifiedSourceNode);
//        sb.Append(", equivalenceClassMDAGNodeHashMap=").Append(equivalenceClassMDAGNodeHashMap);
//        sb.Append(", mdagDataArray=").Append(Arrays.ToString(mdagDataArray));
//        sb.Append(", charTreeSet=").Append(charTreeSet);
//        sb.Append(", transitionCount=").Append(transitionCount);
//        sb.Append('}');
//        return sb.ToString();
//    }

    /**
     * 调试用
     * @return
     */
    public Dictionary<MDAGNode, MDAGNode> _getEquivalenceClassMDAGNodeHashMap()
    {
        return new Dictionary<MDAGNode, MDAGNode>(equivalenceClassMDAGNodeHashMap);
    }
}
