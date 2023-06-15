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
namespace com.hankcs.hanlp.collection.MDAG;



/**
 * 最好不要把MDAG当map用，现在的实现在key后面放一个int，导致右语言全部不同，退化为bintrie
 * @author hankcs
 */
public class MDAGMap<V> : AbstractMap<string, V>
{
    ArrayList<V> valueList = new ArrayList<V>();
    MDAGForMap mdag = new MDAGForMap();

    //@Override
    public V put(string key, V value)
    {
        V origin = get(key);
        if (origin == null)
        {
            valueList.add(value);
            char[] twoChar = ByteUtil.convertIntToTwoChar(valueList.size() - 1);
            mdag.addString(key + MDAGForMap.DELIMITER + twoChar[0] + twoChar[1]);
        }
        return origin;
    }

    //@Override
    public V get(Object key)
    {
        int valueIndex = mdag.getValueIndex(key.toString());
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
    public Set<Entry<string, V>> entrySet()
    {
        HashSet<string> keySet = mdag.getAllStrings();
        return null;
    }

    //@Override
    public Set<string> keySet()
    {
        HashSet<string> stringSet = mdag.getAllStrings();
        LinkedHashSet<string> keySet = new LinkedHashSet<string>();
        Iterator<string> iterator = stringSet.iterator();
        while (iterator.hasNext())
        {
            string key = iterator.next();
            keySet.add(key.substring(0, key.length() - 3));
        }
        return keySet;
    }

    /**
     * 前缀查询
     * @param key
     * @param begin
     * @return
     */
    public LinkedList<Entry<string, V>> commonPrefixSearchWithValue(char[] key, int begin)
    {
        LinkedList<Entry<string, int>> valueIndex = mdag.commonPrefixSearchWithValueIndex(key, begin);
        LinkedList<Entry<string, V>> entryList = new LinkedList<Entry<string, V>>();
        for (Entry<string, int> entry : valueIndex)
        {
            entryList.add(new SimpleEntry<string, V>(entry.getKey(), valueList.get(entry.getValue())));
        }

        return entryList;
    }

    /**
     * 前缀查询
     * @param key
     * @return
     */
    public LinkedList<Entry<string, V>> commonPrefixSearchWithValue(string key)
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

    static class MDAGForMap : MDAG
    {
        static readonly char DELIMITER = Character.MIN_VALUE;

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
            assert targetNode.getOutgoingTransitionSetSize() == 1 : "不是单链！";
            char high = mdagDataArray[transitionSetBeginIndex].getLetter();
            targetNode = targetNode.transition(mdagDataArray, high);
            assert targetNode.getOutgoingTransitionSetSize() == 1 : "不是单链！";
            transitionSetBeginIndex = targetNode.getTransitionSetBeginIndex();
            char low = mdagDataArray[transitionSetBeginIndex].getLetter();
            return ByteUtil.convertTwoCharToInt(high, low);
        }

        private int getValueIndex(MDAGNode currentNode)
        {
            MDAGNode targetNode = currentNode.transition(DELIMITER);
            if (targetNode == null) return -1;
            // 接下来应该是一条单链路
            TreeMap<Character, MDAGNode> outgoingTransitions = targetNode.getOutgoingTransitions();
            assert outgoingTransitions.size() == 1 : "不是单链！";
            char high = outgoingTransitions.keySet().iterator().next();
            targetNode = targetNode.transition(high);
            outgoingTransitions = targetNode.getOutgoingTransitions();
            assert outgoingTransitions.size() == 1 : "不是单链！";
            char low = outgoingTransitions.keySet().iterator().next();
            return ByteUtil.convertTwoCharToInt(high, low);
        }

        public LinkedList<Entry<string, int>> commonPrefixSearchWithValueIndex(char[] key, int begin)
        {
            LinkedList<KeyValuePair<string, int>> result = new LinkedList<KeyValuePair<string, int>>();
            if (sourceNode != null)
            {
                int charCount = key.length;
                MDAGNode currentNode = sourceNode;
                for (int i = 0; i < charCount; ++i)
                {
                    currentNode = currentNode.transition(key[begin + i]);
                    if (currentNode == null) break;
                    {
                        int index = getValueIndex(currentNode);
                        if (index != -1) result.add(new SimpleEntry<string, int>(new string(key, begin, i + 1), index));
                    }
                }
            }
            else
            {
                int charCount = key.length;
                SimpleMDAGNode currentNode = simplifiedSourceNode;
                for (int i = 0; i < charCount; ++i)
                {
                    currentNode = currentNode.transition(mdagDataArray, key[begin + i]);
                    if (currentNode == null) break;
                    {
                        int index = getValueIndex(currentNode);
                        if (index != -1) result.add(new SimpleEntry<string, int>(new string(key, begin, i + 1), index));
                    }
                }
            }

            return result;
        }
    }
}
