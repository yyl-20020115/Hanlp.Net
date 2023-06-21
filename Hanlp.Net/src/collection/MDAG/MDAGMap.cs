/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/12/21 18:59</create-date>
 *
 * <copyright file="MDAGMap.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.collection.MDAG;



/**
 * 最好不要把MDAG当map用，现在的实现在key后面放一个int，导致右语言全部不同，退化为bintrie
 * @author hankcs
 */
public class MDAGMap<V> : AbstractMap<string, V>
{
    List<V> valueList = new ();
    MDAGForMap mdag = new MDAGForMap();

    //@Override
    public V Add(string key, V value)
    {
        V origin = get(key);
        if (origin == null)
        {
            valueList.Add(value);
            char[] twoChar = ByteUtil.convertIntToTwoChar(valueList.size() - 1);
            mdag.addString(key + MDAGForMap.DELIMITER + twoChar[0] + twoChar[1]);
        }
        return origin;
    }

    //@Override
    public V get(Object key)
    {
        int valueIndex = mdag.getValueIndex(key.ToString());
        if (valueIndex != -1)
        {
            return valueList.get(valueIndex);
        }
        return null;
    }

    public V get(string key)
    {
        int valueIndex = mdag.getValueIndex(key);
        if (valueIndex != -1)
        {
            return valueList.get(valueIndex);
        }
        return null;
    }

    //@Override
    public HashSet<KeyValuePair<string, V>> entrySet()
    {
        HashSet<string> keySet = mdag.getAllStrings();
        return null;
    }

    //@Override
    public HashSet<string> Keys()
    {
        HashSet<string> stringSet = mdag.getAllStrings();
        List<string> keySet = new List<string>();
        IEnumerator<string> iterator = stringSet.iterator();
        while (iterator.MoveNext())
        {
            string key = iterator.next();
            keySet.Add(key.substring(0, key.Length - 3));
        }
        return keySet;
    }

    /**
     * 前缀查询
     * @param key
     * @param begin
     * @return
     */
    public LinkedList<KeyValuePair<string, V>> commonPrefixSearchWithValue(char[] key, int begin)
    {
        LinkedList<KeyValuePair<string, int>> valueIndex = mdag.commonPrefixSearchWithValueIndex(key, begin);
        LinkedList<KeyValuePair<string, V>> entryList = new ();
        foreach (KeyValuePair<string, int> entry in valueIndex)
        {
            entryList.Add(new SimpleEntry<string, V>(entry.Key, valueList.get(entry.Value)));
        }

        return entryList;
    }

    /**
     * 前缀查询
     * @param key
     * @return
     */
    public LinkedList<KeyValuePair<string, V>> commonPrefixSearchWithValue(string key)
    {
        return commonPrefixSearchWithValue(key.ToCharArray(), 0);
    }

    /**
     * 进一步降低内存，提高查询速度<br>
     *     副作用是下次插入速度会变慢
     */
    public void simplify()
    {
        mdag.simplify();
    }

    public void unSimplify()
    {
        mdag.unSimplify();
    }

    public class MDAGForMap : MDAG
    {
        static readonly char DELIMITER = char.MinValue;

        public int getValueIndex(string key)
        {
            if (sourceNode != null)      //if the MDAG hasn't been simplified
            {
                MDAGNode currentNode = sourceNode.transition(key.ToCharArray());
                if (currentNode == null) return -1;
                return getValueIndex(currentNode);

            }
            else
            {
                SimpleMDAGNode currentNode = simplifiedSourceNode.transition(mdagDataArray, key.ToCharArray());
                if (currentNode == null) return -1;
                return getValueIndex(currentNode);
            }

        }

        private int getValueIndex(SimpleMDAGNode currentNode)
        {
            SimpleMDAGNode targetNode = currentNode.transition(mdagDataArray, DELIMITER);
            if (targetNode == null) return -1;
            // 接下来应该是一条单链路
            int transitionSetBeginIndex = targetNode.getTransitionSetBeginIndex();
            //assert targetNode.getOutgoingTransitionSetSize() == 1 : "不是单链！";
            char high = mdagDataArray[transitionSetBeginIndex].getLetter();
            targetNode = targetNode.transition(mdagDataArray, high);
            //assert targetNode.getOutgoingTransitionSetSize() == 1 : "不是单链！";
            transitionSetBeginIndex = targetNode.getTransitionSetBeginIndex();
            char low = mdagDataArray[transitionSetBeginIndex].getLetter();
            return ByteUtil.convertTwoCharToInt(high, low);
        }

        private int getValueIndex(MDAGNode currentNode)
        {
            MDAGNode targetNode = currentNode.transition(DELIMITER);
            if (targetNode == null) return -1;
            // 接下来应该是一条单链路
            Dictionary<char, MDAGNode> outgoingTransitions = targetNode.getOutgoingTransitions();
            //assert outgoingTransitions.size() == 1 : "不是单链！";
            char high = outgoingTransitions.Keys.iterator().next();
            targetNode = targetNode.transition(high);
            outgoingTransitions = targetNode.getOutgoingTransitions();
            //assert outgoingTransitions.size() == 1 : "不是单链！";
            char low = outgoingTransitions.Keys.iterator().next();
            return ByteUtil.convertTwoCharToInt(high, low);
        }

        public LinkedList<KeyValuePair<string, int>> commonPrefixSearchWithValueIndex(char[] key, int begin)
        {
            LinkedList<KeyValuePair<string, int>> result = new LinkedList<KeyValuePair<string, int>>();
            if (sourceNode != null)
            {
                int charCount = key.Length;
                MDAGNode currentNode = sourceNode;
                for (int i = 0; i < charCount; ++i)
                {
                    currentNode = currentNode.transition(key[begin + i]);
                    if (currentNode == null) break;
                    {
                        int index = getValueIndex(currentNode);
                        if (index != -1) result.Add(new SimpleEntry<string, int>(new string(key, begin, i + 1), index));
                    }
                }
            }
            else
            {
                int charCount = key.Length;
                SimpleMDAGNode currentNode = simplifiedSourceNode;
                for (int i = 0; i < charCount; ++i)
                {
                    currentNode = currentNode.transition(mdagDataArray, key[begin + i]);
                    if (currentNode == null) break;
                    {
                        int index = getValueIndex(currentNode);
                        if (index != -1) result.Add(new SimpleEntry<string, int>(new string(key, begin, i + 1), index));
                    }
                }
            }

            return result;
        }
    }
}
