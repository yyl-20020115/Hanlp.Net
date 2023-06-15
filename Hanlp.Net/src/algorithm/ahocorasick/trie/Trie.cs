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

    public Trie(ICollection<String> keywords)
        :this()
    {
        addAllKeyword(keywords);
    }

    public Trie removeOverlaps()
    {
        this.trieConfig.setAllowOverlaps(false);
        return this;
    }

    /**
     * 只保留最长匹配
     * @return
     */
    public Trie remainLongest()
    {
        this.trieConfig.remainLongest = true;
        return this;
    }

    public void addKeyword(String keyword)
    {
        if (keyword == null || keyword.Length == 0)
        {
            return;
        }
        State currentState = this.rootState;
        foreach (char character in keyword.ToCharArray())
        {
            currentState = currentState.addState(character);
        }
        currentState.addEmit(keyword);
    }

    public void addAllKeyword(ICollection<String> keywordSet)
    {
        foreach (String keyword in keywordSet)
        {
            addKeyword(keyword);
        }
    }

    /**
     * 一个最长分词器
     *
     * @param text 待分词文本
     * @return
     */
    public ICollection<Token> tokenize(String text)
    {

        List<Token> tokens = new ();

        var collectedEmits = parseText(text);
        // 下面是最长分词的关键
        var intervalTree = new IntervalTree((List<Intervalable>)collectedEmits);
        intervalTree.removeOverlaps((List<Intervalable>) collectedEmits);
        // 移除结束

        int lastCollectedPosition = -1;
        foreach (Emit emit in collectedEmits)
        {
            if (emit.getStart() - lastCollectedPosition > 1)
            {
                tokens.Add(createFragment(emit, text, lastCollectedPosition));
            }
            tokens.Add(createMatch(emit, text));
            lastCollectedPosition = emit.getEnd();
        }
        if (text.Length - lastCollectedPosition > 1)
        {
            tokens.Add(createFragment(null, text, lastCollectedPosition));
        }

        return tokens;
    }

    private Token createFragment(Emit emit, String text, int lastCollectedPosition)
    {
        return new FragmentToken(text[(lastCollectedPosition + 1)..(emit == null ? text.Length : emit.getStart())]);
    }

    private Token createMatch(Emit emit, String text)
    {
        return new MatchToken(text[emit.getStart().. (emit.getEnd() + 1)], emit);
    }

    /**
     * 模式匹配
     *
     * @param text 待匹配的文本
     * @return 匹配到的模式串
     */
    
    public ICollection<Emit> parseText(String text)
    {
        checkForConstructedFailureStates();

        int position = 0;
        State currentState = this.rootState;
        List<Emit> collectedEmits = new ();
        for (int i = 0; i < text.Length; ++i)
        {
            currentState = getState(currentState, text[i]);
            storeEmits(position, currentState, collectedEmits);
            ++position;
        }

        if (!trieConfig.isAllowOverlaps())
        {
            var vs = collectedEmits.Cast<Intervalable>().ToList();
            IntervalTree intervalTree = new IntervalTree(vs);
            intervalTree.removeOverlaps(vs);
        }

        if (trieConfig.remainLongest)
        {
            remainLongest(collectedEmits);
        }

        return collectedEmits;
    }

    /**
     * 只保留最长词
     * @param collectedEmits
     */
    private static void remainLongest(List<Emit> collectedEmits)
    {
        if (collectedEmits.Count < 2) return;
        var emitMapStart = new Dictionary<int, Emit>();
        foreach (Emit emit in collectedEmits)
        {
            if (!emitMapStart.TryGetValue(emit.getStart(),out var pre) || (pre.size() < emit.size()))
            {
                emitMapStart.Add(emit.getStart(), emit);
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
            if (!emitMapEnd.TryGetValue(emit.getEnd(),out var pre) || pre.size() < emit.size())
            {
                emitMapEnd.Add(emit.getEnd(), emit);
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
    private static State getState(State currentState, char character)
    {
        State newCurrentState = currentState.nextState(character);  // 先按success跳转
        while (newCurrentState == null) // 跳转失败的话，按failure跳转
        {
            currentState = currentState.failure();
            newCurrentState = currentState.nextState(character);
        }
        return newCurrentState;
    }

    /**
     * 检查是否建立了failure表
     */
    private void checkForConstructedFailureStates()
    {
        if (!this.failureStatesConstructed)
        {
            constructFailureStates();
        }
    }

    /**
     * 建立failure表
     */
    private void constructFailureStates()
    {
        Queue<State> queue = new ();

        // 第一步，将深度为1的节点的failure设为根节点
        foreach (State depthOneState in this.rootState.getStates())
        {
            depthOneState.setFailure(this.rootState);
            queue.Enqueue(depthOneState);
        }
        this.failureStatesConstructed = true;

        // 第二步，为深度 > 1 的节点建立failure表，这是一个bfs
        while (queue.Count>0)
        {
            State currentState = queue.Dequeue();

            foreach (char transition in currentState.getTransitions())
            {
                State targetState = currentState.nextState(transition);
                queue.Enqueue(targetState);

                State traceFailureState = currentState.failure();
                while (traceFailureState.nextState(transition) == null)
                {
                    traceFailureState = traceFailureState.failure();
                }
                State newFailureState = traceFailureState.nextState(transition);
                targetState.setFailure(newFailureState);
                targetState.addEmit(newFailureState.emit());
            }
        }
    }

    public void dfs(IWalker walker)
    {
        checkForConstructedFailureStates();
        dfs(rootState, "", walker);
    }

    private void dfs(State currentState, String path, IWalker walker)
    {
        walker.meet(path, currentState);
        foreach (char transition in currentState.getTransitions())
        {
            State targetState = currentState.nextState(transition);
            dfs(targetState, path + transition, walker);
        }
    }


    public interface IWalker
    {
        /**
         * 遇到了一个节点
         * @param path
         * @param state
         */
        void meet(String path, State state);
    }

    /**
     * 保存匹配结果
     *
     * @param position       当前位置，也就是匹配到的模式串的结束位置+1
     * @param currentState   当前状态
     * @param collectedEmits 保存位置
     */
    private static void storeEmits(int position, State currentState, List<Emit> collectedEmits)
    {
        var emits = currentState.emit();
        if (emits != null && emits.Count>0)
        {
            foreach (String emit in emits)
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
    public bool hasKeyword(String text)
    {
        checkForConstructedFailureStates();

        State currentState = this.rootState;
        for (int i = 0; i < text.Length; ++i)
        {
        	State nextState = getState(currentState, text[(i)]);
            if (nextState != null && nextState != currentState && nextState.emit().Count != 0) {
                return true;
            }
            currentState = nextState;
        }
        return false;
    }

}
