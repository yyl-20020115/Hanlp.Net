/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone.details;


/**
 * 有向无环字图
 * @author
 */
public class DawgBuilder
{
    /**
     * 根节点id
     * @return 0
     */
    public int root()
    {
        return 0;
    }

    /**
     * 获取节点的孩子
     * @param id 节点的id
     * @return 孩子的id
     */
    public int child(int id)
    {
        // return _units.get(id).child();
        return _units.Get(id) >>> 2;
    }

    /**
     * 获取兄弟节点
     * @param id 兄弟节点的id
     * @return 下一个兄弟节点的id，或者0表示没有兄弟节点
     */
    public int sibling(int id)
    {
        // return _units.get(id).hasSibling() ? (id + 1) : 0;
        return ((_units.Get(id) & 1) == 1) ? (id + 1) : 0;
    }

    /**
     * 获取值
     * @param id 节点id
     * @return 节点的值
     */
    public int value(int id)
    {
        // return _units.get(id).value();
        return _units.Get(id) >>> 1;
    }

    /**
     * 是否是叶子节点
     * @param id 节点id
     * @return 是否是叶子节点
     */
    public bool isLeaf(int id)
    {
        return label(id) == 0;
    }

    /**
     * 获取label
     * @param id 节点的id
     * @return
     */
    public byte label(int id)
    {
        return _labels.Get(id);
    }

    /**
     * 是否是分叉点
     * @param id 节点id
     * @return
     */
    public bool isIntersection(int id)
    {
        return _isIntersections.Get(id);
    }

    public int intersectionId(int id)
    {
        return _isIntersections.rank(id) - 1;
    }

    public int numIntersections()
    {
        return _isIntersections.numOnes();
    }

    public int Count=> _units.Count;

    /**
     * 初始化
     */
    public void init()
    {
        _table.Resize(INITIAL_TABLE_SIZE, 0);

        appendNode();
        appendUnit();

        _numStates = 1;

        _nodes[0].label = (byte) 0xFF;
        _nodeStack.Add(0);
    }

    public void finish()
    {
        flush(0);

        _units.Set(0, _nodes[0].unit());
        _labels.Set(0, _nodes[0].label);

        _nodes.Clear();
        _table.Clear();
        _nodeStack.Clear();
        _recycleBin.Clear();

        _isIntersections.build();
    }

    public void insert(byte[] key, int value)
    {
        if (value < 0)
        {
            throw new ArgumentException(
                    "failed to insert key: negative value");
        }
        if (key.Length == 0)
        {
            throw new ArgumentException(
                    "failed to inset key: zero-Length key");
        }

        int id = 0;
        int keyPos = 0;

        for (; keyPos <= key.Length; ++keyPos)
        {
            int childId = _nodes.get(id).child;
            if (childId == 0)
            {
                break;
            }

            byte keyLabel = keyPos < key.Length ? key[keyPos] : (byte)0;
            if (keyPos < key.Length && keyLabel == 0)
            {
                throw new ArgumentException(
                        "failed to insert key: invalid null character");
            }

            byte unitLabel = _nodes[childId].label;
            if ((keyLabel & 0xFF) < (unitLabel & 0xFF))
            {
                throw new ArgumentException(
                        "failed to insert key: wrong key order");
            }
            else if ((keyLabel & 0xFF) > (unitLabel & 0xFF))
            {
                _nodes.get(childId).hasSibling = true;
                flush(childId);
                break;
            }
            id = childId;
        }

        if (keyPos > key.Length)
        {
            return;
        }

        for (; keyPos <= key.Length; ++keyPos)
        {
            byte keyLabel = (keyPos < key.Length) ? key[keyPos] : 0;
            int childId = appendNode();

            DawgNode node = _nodes.get(id);
            DawgNode child = _nodes.get(childId);

            if (node.child == 0)
            {
                child.isState = true;
            }
            child.sibling = node.child;
            child.label = keyLabel;
            node.child = childId;
            _nodeStack.Add(childId);

            id = childId;
        }
        _nodes.get(id).setValue(value);
    }

    public void Clear()
    {
        _nodes.Clear();
        _units.Clear();
        _labels.Clear();
        _isIntersections.Clear();
        _table.Clear();
        _nodeStack.Clear();
        _recycleBin.Clear();
        _numStates = 0;
    }

    public class DawgNode
    {
        public int child;
        public int sibling;
        public byte label;
        public bool isState;
        public bool hasSibling;

        public void reset()
        {
            child = 0;
            sibling = 0;
            label = (byte) 0;
            isState = false;
            hasSibling = false;
        }

        public int Value()
        {
            return child;
        }

        public void setValue(int value)
        {
            child = value;
        }

        public int unit()
        {
            if (label == 0)
            {
                return (child << 1) | (hasSibling ? 1 : 0);
            }
            return (child << 2) | (isState ? 2 : 0) | (hasSibling ? 1 : 0);
        }
    }

    private void flush(int id)
    {
        while (_nodeStack.Get(_nodeStack.Count - 1) != id)
        {
            int nodeId = _nodeStack.Get(_nodeStack.Count - 1);
            _nodeStack.deleteLast();

            if (_numStates >= _table.Count - (_table.Count >>> 2))
            {
                expandTable();
            }

            int numSiblings = 0;
            for (int i = nodeId; i != 0; i = _nodes[i].sibling)
            {
                ++numSiblings;
            }

            // make an array of Length 1 to emulate pass-by-reference
            int[] matchHashId = findNode(nodeId);
            int matchId = matchHashId[0];
            int hashId = matchHashId[1];

            if (matchId != 0)
            {
                _isIntersections.set(matchId, true);
            }
            else
            {
                int unitId = 0;
                for (int i = 0; i < numSiblings; ++i)
                {
                    unitId = appendUnit();
                }
                for (int i = nodeId; i != 0; i = _nodes[i].sibling)
                {
                    _units.Set(unitId, _nodes[i].unit());
                    _labels.Set(unitId, _nodes[i].label);
                    --unitId;
                }
                matchId = unitId + 1;
                _table.Set(hashId, matchId);
                ++_numStates;
            }

            for (int i = nodeId, next; i != 0; i = next)
            {
                next = _nodes[i].sibling;
                freeNode(i);
            }

            _nodes.get(_nodeStack.Get(_nodeStack.Count - 1)).child = matchId;
        }
        _nodeStack.deleteLast();
    }

    private void expandTable()
    {
        int tableSize = _table.Count << 1;
        _table.Clear();
        _table.Resize(tableSize, 0);

        for (int id = 1; id < _units.Count; ++id)
        {
//            if (_labels[i] == 0 || _units.get(id).isState)) {
            if (_labels.Get(id) == 0 || (_units.Get(id) & 2) == 2)
            {
                int[] ret = findUnit(id);
                int hashId = ret[1];
                _table.Set(hashId, id);
            }
        }
    }

    private int[] findUnit(int id)
    {
        int[] ret = new int[2];
        int hashId = hashUnit(id) % _table.Count;
        for (; ; hashId = (hashId + 1) % _table.Count)
        {
            // Remainder adjustment.
            if (hashId < 0)
            {
                hashId += _table.Count;
            }
            int unitId = _table.Get(hashId);
            if (unitId == 0)
            {
                break;
            }

            // there must not be the same unit.
        }
        ret[1] = hashId;
        return ret;
    }

    private int[] findNode(int nodeId)
    {
        int[] ret = new int[2];
        int hashId = hashNode(nodeId) % _table.Count;
        for (; ; hashId = (hashId + 1) % _table.Count)
        {
            // Remainder adjustment
            if (hashId < 0)
            {
                hashId += _table.Count;
            }
            int unitId = _table.Get(hashId);
            if (unitId == 0)
            {
                break;
            }

            if (areEqual(nodeId, unitId))
            {
                ret[0] = unitId;
                ret[1] = hashId;
                return ret;
            }
        }
        ret[1] = hashId;
        return ret;
    }

    private bool areEqual(int nodeId, int unitId)
    {
        for (int i = _nodes.get(nodeId).sibling; i != 0;
             i = _nodes[i].sibling)
        {
//            if (_units.get(unitId).hasSibling() == false) {
            if ((_units.Get(unitId) & 1) != 1)
            {
                return false;
            }
            ++unitId;
        }
//        if (_units.get(unitId).hasSibling() == true) {
        if ((_units.Get(unitId) & 1) == 1)
        {
            return false;
        }

        for (int i = nodeId; i != 0; i = _nodes[i].sibling, --unitId)
        {
//            if (_nodes[i] != _units.get(unitId).unit() ||
            if (_nodes[i].unit() != _units.Get(unitId) ||
                    _nodes[i].label != _labels.Get(unitId))
            {
                return false;
            }
        }
        return true;
    }

    private int hashUnit(int id)
    {
        int hashValue = 0;
        for (; id != 0; ++id)
        {
//            int unit = _units.get(id).unit();
            int unit = _units.Get(id);
            byte label = _labels.Get(id);
            hashValue ^= hash(((label & 0xFF) << 24) ^ unit);

//            if (_units.get(id).hasSibling() == false) {
            if ((_units.Get(id) & 1) != 1)
            {
                break;
            }
        }
        return hashValue;
    }

    private int hashNode(int id)
    {
        int hashValue = 0;
        for (; id != 0; id = _nodes[(id)].sibling)
        {
            int unit = _nodes[(id)].unit();
            byte label = _nodes[(id)].label;
            hashValue ^= hash(((label & 0xFF) << 24) ^ unit);
        }
        return hashValue;
    }

    private int appendUnit()
    {
        _isIntersections.Append();
        _units.Add(0);
        _labels.Add((byte) 0);

        return _isIntersections.Count - 1;
    }

    private int appendNode()
    {
        int id;
        if (_recycleBin.IsEmpty)
        {
            id = _nodes.Count;
            _nodes.Add(new DawgNode());
        }
        else
        {
            id = _recycleBin.Get(_recycleBin.Count - 1);
            _nodes.get(id).reset();
            _recycleBin.deleteLast();
        }
        return id;
    }

    private void freeNode(int id)
    {
        _recycleBin.Add(id);
    }

    private static int hash(int key)
    {
        key = ~key + (key << 15);  // key = (key << 15) - key - 1;
        key = key ^ (key >>> 12);
        key = key + (key << 2);
        key = key ^ (key >>> 4);
        key = key * 2057;  // key = (key + (key << 3)) + (key << 11);
        key = key ^ (key >>> 16);
        return key;
    }

    private static readonly int INITIAL_TABLE_SIZE = 1 << 10;
    private List<DawgNode> _nodes = new ();
    private AutoIntPool _units = new AutoIntPool();
    private AutoBytePool _labels = new AutoBytePool();
    private BitVector _isIntersections = new BitVector();
    private AutoIntPool _table = new AutoIntPool();
    private AutoIntPool _nodeStack = new AutoIntPool();
    private AutoIntPool _recycleBin = new AutoIntPool();
    private int _numStates;
}
