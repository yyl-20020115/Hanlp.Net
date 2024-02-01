using com.hankcs.hanlp.algorithm.ahocorasick.interval;
using System.ComponentModel.DataAnnotations;

namespace com.hankcs.hanlp.algorithm.ahocorasick.trie;



/**
 * 基于 Aho-Corasick 白皮书, 贝尔实验室: ftp://163.13.200.222/assistant/bearhero/prog/%A8%E4%A5%A6/ac_bm.pdf
 *
 * @author Robert Bor
 */
public class Trie
{

    private TrieConfig trieConfig;

    private State rootState;

    /**
     * 是否建立了failure表
     */
    private bool failureStatesConstructed = false;

    /**
     * 构造一棵trie树
     */
    public Trie(TrieConfig trieConfig)
    {
        this.trieConfig = trieConfig;
        this.rootState = new State();
    }

    public Trie()
        : this(new TrieConfig())
    {
    }

    public Trie(ICollection<string> keywords)
        :this()
    {
        AddAllKeyword(keywords);
    }

    public Trie RemoveOverlaps()
    {
        this.trieConfig.        AllowOverlaps = false;
        return this;
    }

    /**
     * 只保留最长匹配
     * @return
     */
    public Trie RemainLongest()
    {
        this.trieConfig.remainLongest = true;
        return this;
    }

    public void AddKeyword(string keyword)
    {
        if (keyword == null || keyword.Length == 0)
        {
            return;
        }
        State currentState = this.rootState;
        foreach (char character in keyword.ToCharArray())
        {
            currentState = currentState.AddState(character);
        }
        currentState.AddEmit(keyword);
    }

    public void AddAllKeyword(ICollection<string> keywordSet)
    {
        foreach (string keyword in keywordSet)
        {
            AddKeyword(keyword);
        }
    }

    /**
     * 一个最长分词器
     *
     * @param text 待分词文本
     * @return
     */
    public ICollection<Token> Tokenize(string text)
    {

        List<Token> tokens = new ();

        var collectedEmits = ParseText(text);
        // 下面是最长分词的关键
        var intervalTree = new IntervalTree((List<Intervalable>)collectedEmits);
        intervalTree.RemoveOverlaps((List<Intervalable>) collectedEmits);
        // 移除结束

        int lastCollectedPosition = -1;
        foreach (Emit emit in collectedEmits)
        {
            if (emit.Start - lastCollectedPosition > 1)
            {
                tokens.Add(CreateFragment(emit, text, lastCollectedPosition));
            }
            tokens.Add(CreateMatch(emit, text));
            lastCollectedPosition = emit.End;
        }
        if (text.Length - lastCollectedPosition > 1)
        {
            tokens.Add(CreateFragment(null, text, lastCollectedPosition));
        }

        return tokens;
    }

    private Token CreateFragment(Emit emit, string text, int lastCollectedPosition)
    {
        return new FragmentToken(text[(lastCollectedPosition + 1)..(emit == null ? text.Length : emit.Start)]);
    }

    private Token CreateMatch(Emit emit, string text)
    {
        return new MatchToken(text[emit.Start.. (emit.End + 1)], emit);
    }

    /**
     * 模式匹配
     *
     * @param text 待匹配的文本
     * @return 匹配到的模式串
     */
    
    public ICollection<Emit> ParseText(string text)
    {
        CheckForConstructedFailureStates();

        int position = 0;
        State currentState = this.rootState;
        List<Emit> collectedEmits = new ();
        for (int i = 0; i < text.Length; ++i)
        {
            currentState = GetState(currentState, text[i]);
            StoreEmits(position, currentState, collectedEmits);
            ++position;
        }

        if (!trieConfig.AllowOverlaps)
        {
            var vs = collectedEmits.Cast<Intervalable>().ToList();
            IntervalTree intervalTree = new IntervalTree(vs);
            intervalTree.RemoveOverlaps(vs);
        }

        if (trieConfig.remainLongest)
        {
            RemainLongest(collectedEmits);
        }

        return collectedEmits;
    }

    /**
     * 只保留最长词
     * @param collectedEmits
     */
    private static void RemainLongest(List<Emit> collectedEmits)
    {
        if (collectedEmits.Count < 2) return;
        var emitMapStart = new Dictionary<int, Emit>();
        foreach (Emit emit in collectedEmits)
        {
            if (!emitMapStart.TryGetValue(emit.Start,out var pre) || (pre.Count < emit.Count))
            {
                emitMapStart.Add(emit.Start, emit);
            }
        }
        if (emitMapStart.Count < 2)
        {
            collectedEmits.Clear();
            collectedEmits.AddRange(emitMapStart.Values);
            return;
        }
        var emitMapEnd = new Dictionary<int, Emit>();
        foreach (Emit emit in emitMapStart.Values)
        {
            if (!emitMapEnd.TryGetValue(emit.End,out var pre) || pre.Count < emit.Count)
            {
                emitMapEnd.Add(emit.End, emit);
            }
        }

        collectedEmits.Clear();
        collectedEmits.AddRange(emitMapEnd.Values);
    }


    /**
     * 跳转到下一个状态
     *
     * @param currentState 当前状态
     * @param character    接受字符
     * @return 跳转结果
     */
    private static State GetState(State currentState, char character)
    {
        State newCurrentState = currentState.NextState(character);  // 先按success跳转
        while (newCurrentState == null) // 跳转失败的话，按failure跳转
        {
            currentState = currentState.Failure;
            newCurrentState = currentState.NextState(character);
        }
        return newCurrentState;
    }

    /**
     * 检查是否建立了failure表
     */
    private void CheckForConstructedFailureStates()
    {
        if (!this.failureStatesConstructed)
        {
            ConstructFailureStates();
        }
    }

    /**
     * 建立failure表
     */
    private void ConstructFailureStates()
    {
        Queue<State> queue = new ();

        // 第一步，将深度为1的节点的failure设为根节点
        foreach (State depthOneState in this.rootState.States)
        {
            depthOneState.            Failure = this.rootState;
            queue.Enqueue(depthOneState);
        }
        this.failureStatesConstructed = true;

        // 第二步，为深度 > 1 的节点建立failure表，这是一个bfs
        while (queue.Count>0)
        {
            State currentState = queue.Dequeue();

            foreach (char transition in currentState.Transitions)
            {
                State targetState = currentState.NextState(transition);
                queue.Enqueue(targetState);

                State traceFailureState = currentState.Failure;
                while (traceFailureState.NextState(transition) == null)
                {
                    traceFailureState = traceFailureState.Failure;
                }
                State newFailureState = traceFailureState.NextState(transition);
                targetState.                Failure = newFailureState;
                targetState.AddEmit(newFailureState.Emit());
            }
        }
    }

    public void DFS(IWalker walker)
    {
        CheckForConstructedFailureStates();
        DFS(rootState, "", walker);
    }

    private void DFS(State currentState, string path, IWalker walker)
    {
        walker.Meet(path, currentState);
        foreach (char transition in currentState.Transitions)
        {
            State targetState = currentState.NextState(transition);
            DFS(targetState, path + transition, walker);
        }
    }


    public interface IWalker
    {
        /**
         * 遇到了一个节点
         * @param path
         * @param state
         */
        void Meet(string path, State state);
    }

    /**
     * 保存匹配结果
     *
     * @param position       当前位置，也就是匹配到的模式串的结束位置+1
     * @param currentState   当前状态
     * @param collectedEmits 保存位置
     */
    private static void StoreEmits(int position, State currentState, List<Emit> collectedEmits)
    {
        var emits = currentState.Emit();
        if (emits != null && emits.Count>0)
        {
            foreach (string emit in emits)
            {
                collectedEmits.Add(new Emit(position - emit.Length + 1, position, emit));
            }
        }
    }

    /**
     * 文本是否包含任何模式
     *
     * @param text 待匹配的文本
     * @return 文本包含模式時回傳true
     */
    public bool HasKeyword(string text)
    {
        CheckForConstructedFailureStates();

        State currentState = this.rootState;
        for (int i = 0; i < text.Length; ++i)
        {
        	State nextState = GetState(currentState, text[(i)]);
            if (nextState != null && nextState != currentState && nextState.Emit().Count != 0) {
                return true;
            }
            currentState = nextState;
        }
        return false;
    }

}
