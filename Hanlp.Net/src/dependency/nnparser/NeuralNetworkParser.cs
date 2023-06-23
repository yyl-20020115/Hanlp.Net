/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/10/30 20:00</create-date>
 *
 * <copyright file="NeuralNetworkParser.java" company="码农场">
 * Copyright (c) 2008-2015, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dependency.nnparser.action;
using com.hankcs.hanlp.dependency.nnparser.option;
using com.hankcs.hanlp.utility;
using Action = com.hankcs.hanlp.dependency.nnparser.action.Action;

namespace com.hankcs.hanlp.dependency.nnparser;



/**
 * @author hankcs
 */
public class NeuralNetworkParser : ICacheAble
{
    Matrix W1;
    Matrix W2;
    Matrix E;
    Matrix b1;
    Matrix saved;

    Alphabet forms_alphabet;
    Alphabet postags_alphabet;
    Alphabet deprels_alphabet;
    /**
     * 少量类目数的聚类
     */
    Alphabet cluster4_types_alphabet;
    /**
     * 中等类目数的聚类
     */
    Alphabet cluster6_types_alphabet;
    /**
     * 大量类目数的聚类
     */
    Alphabet cluster_types_alphabet;

    Dictionary<int, int> precomputation_id_encoder;
    /**
     * 将词映射到词聚类中的某个类
     */
    Dictionary<int, int> form_to_cluster4;
    Dictionary<int, int> form_to_cluster6;
    Dictionary<int, int> form_to_cluster;

    /**
     * 神经网络分类器
     */
    NeuralNetworkClassifier classifier;
    /**
     * 动作转移系统
     */
    TransitionSystem system;
    /**
     * 根节点词语
     */
    string root;

    /**
     * 语料库之外的词语的id
     */
    int kNilForm;
    /**
     * 语料库之外的词语的词性的id
     */
    int kNilPostag;
    /**
     * 语料库之外的依存关系名称的id
     */
    int kNilDeprel;
    int kNilDistance;
    int kNilValency;
    int kNilCluster4;
    int kNilCluster6;
    int kNilCluster;

    /**
     * 词语特征在特征空间中的起始位置
     */
    int kFormInFeaturespace;
    int kPostagInFeaturespace;
    int kDeprelInFeaturespace;
    int kDistanceInFeaturespace;
    int kValencyInFeaturespace;
    int kCluster4InFeaturespace;
    int kCluster6InFeaturespace;
    int kClusterInFeaturespace;
    int kFeatureSpaceEnd;

    int nr_feature_types;

    /**
     * 指定使用距离特征，具体参考Zhang and Nivre (2011)
     */
    bool use_distance;
    /**
     * 指定使用valency特征，具体参考Zhang and Nivre (2011)
     */
    bool use_valency;
    /**
     * 指定使用词聚类特征，具体参考Guo et. al, (2015)
     */
    bool use_cluster;

    static string model_header;

    /**
     * 加载parser模型
     * @param path
     * @return
     */
    public bool load(string path)
    {
        string binPath = path + Predefine.BIN_EXT;
        if (load(ByteArrayStream.createByteArrayStream(binPath))) return true;
        if (!loadTxt(path)) return false;
        try
        {
            logger.info("正在缓存" + binPath);
            Stream _out = new Stream(IOUtil.newOutputStream(binPath));
            save(_out);
            _out.Close();
        }
        catch (Exception e)
        {
            logger.warning("缓存" + binPath + "失败：\n" + TextUtility.exceptionToString(e));
        }

        return true;
    }

    /**
     * 从txt加载
     * @param path
     * @return
     */
    public bool loadTxt(string path)
    {
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(path);
        model_header = lineIterator.next();
        if (model_header == null) return false;
        root = lineIterator.next();
        use_distance = "1".Equals(lineIterator.next());
        use_valency = "1".Equals(lineIterator.next());
        use_cluster = "1".Equals(lineIterator.next());

        W1 = read_matrix(lineIterator);
        W2 = read_matrix(lineIterator);
        E = read_matrix(lineIterator);
        b1 = read_vector(lineIterator);
        saved = read_matrix(lineIterator);

        forms_alphabet = read_alphabet(lineIterator);
        postags_alphabet = read_alphabet(lineIterator);
        deprels_alphabet = read_alphabet(lineIterator);

        precomputation_id_encoder = read_map(lineIterator);

        if (use_cluster)
        {
            cluster4_types_alphabet = read_alphabet(lineIterator);
            cluster6_types_alphabet = read_alphabet(lineIterator);
            cluster_types_alphabet = read_alphabet(lineIterator);

            form_to_cluster4 = read_map(lineIterator);
            form_to_cluster6 = read_map(lineIterator);
            form_to_cluster = read_map(lineIterator);
        }

        //assert !lineIterator.MoveNext() : "文件有残留，可能是读取逻辑不对";

        classifier = new NeuralNetworkClassifier(W1, W2, E, b1, saved, precomputation_id_encoder);
        classifier.canonical();

        return true;
    }

    /**
     * 保存到磁盘
     * @param _out
     * @throws Exception
     */
    public void save(Stream _out)
    {
        TextUtility.writeString(model_header, _out);
        TextUtility.writeString(root, _out);

        _out.writeInt(use_distance ? 1 : 0);
        _out.writeInt(use_valency ? 1 : 0);
        _out.writeInt(use_cluster ? 1 : 0);

        W1.save(_out);
        W2.save(_out);
        E.save(_out);
        b1.save(_out);
        saved.save(_out);

        forms_alphabet.save(_out);
        postags_alphabet.save(_out);
        deprels_alphabet.save(_out);

        save_map(precomputation_id_encoder, _out);

        if (use_cluster)
        {
            cluster4_types_alphabet.save(_out);
            cluster6_types_alphabet.save(_out);
            cluster_types_alphabet.save(_out);

            save_map(form_to_cluster4, _out);
            save_map(form_to_cluster6 , _out);
            save_map(form_to_cluster , _out);
        }
    }

    /**
     * 从bin加载
     * @param byteArray
     * @return
     */
    public bool load(ByteArray byteArray)
    {
        if (byteArray == null) return false;
        model_header = byteArray.nextString();
        root = byteArray.nextString();

        use_distance = byteArray.Next() == 1;
        use_valency = byteArray.Next() == 1;
        use_cluster = byteArray.Next() == 1;

        W1 = new Matrix();
        W1.load(byteArray);
        W2 = new Matrix();
        W2.load(byteArray);
        E = new Matrix();
        E .load(byteArray);
        b1 = new Matrix();
        b1 .load(byteArray);
        saved = new Matrix();
        saved .load(byteArray);

        forms_alphabet = new Alphabet();
        forms_alphabet .load(byteArray);
        postags_alphabet = new Alphabet();
        postags_alphabet .load(byteArray);
        deprels_alphabet = new Alphabet();
        deprels_alphabet .load(byteArray);

        precomputation_id_encoder = read_map(byteArray);

        if (use_cluster)
        {
            cluster4_types_alphabet = new Alphabet();
            cluster4_types_alphabet.load(byteArray);
            cluster6_types_alphabet = new Alphabet();
            cluster6_types_alphabet .load(byteArray);
            cluster_types_alphabet = new Alphabet();
            cluster_types_alphabet .load(byteArray);

            form_to_cluster4 = read_map(byteArray);
            form_to_cluster6 = read_map(byteArray);
            form_to_cluster = read_map(byteArray);
        }

        //assert !byteArray.hasMore() : "文件有残留，可能是读取逻辑不对";

        classifier = new NeuralNetworkClassifier(W1, W2, E, b1, saved, precomputation_id_encoder);
        classifier.canonical();

        return true;
    }

    private static Matrix read_matrix(IOUtil.LineIterator lineIterator)
    {
        string[] rc = lineIterator.next().Split("\t");
        int rows = int.valueOf(rc[0]);
        int cols = int.valueOf(rc[1]);
        double[][] valueArray = new double[rows][cols];
        foreach (double[] valueRow in valueArray)
        {
            string[] args = lineIterator.next().Split("\t");
            for (int i = 0; i < valueRow.Length; i++)
            {
                valueRow[i] = Double.valueOf(args[i]);
            }
        }

        return new Matrix(valueArray);
    }

    private static Matrix read_vector(IOUtil.LineIterator lineIterator)
    {
        int rows = int.valueOf(lineIterator.next());
        double[][] valueArray = new double[rows][1];
        string[] args = lineIterator.next().Split("\t");
        for (int i = 0; i < rows; i++)
        {
            valueArray[i][0] = Double.valueOf(args[i]);
        }

        return new Matrix(valueArray);
    }

    private static Alphabet read_alphabet(IOUtil.LineIterator lineIterator)
    {
        int size = int.valueOf(lineIterator.next());
        Dictionary<string, int> map = new Dictionary<string, int>();
        for (int i = 0; i < size; i++)
        {
            string[] args = lineIterator.next().Split("\t");
            map.Add(args[0], int.valueOf(args[1]));
        }

        Alphabet trie = new Alphabet();
        trie.build(map);

        return trie;
    }

    private static Dictionary<int, int> read_map(IOUtil.LineIterator lineIterator)
    {
        int size = int.valueOf(lineIterator.next());
        Dictionary<int, int> map = new ();
        for (int i = 0; i < size; i++)
        {
            string[] args = lineIterator.next().Split("\t");
            map.Add(int.valueOf(args[0]), int.valueOf(args[1]));
        }

        return map;
    }

    private static Dictionary<int, int> read_map(ByteArray byteArray)
    {
        int size = byteArray.Next();
        Dictionary<int, int> map = new ();
        for (int i = 0; i < size; i++)
        {
            map.Add(byteArray.Next(), byteArray.Next());
        }

        return map;
    }

    private static void save_map(Dictionary<int, int> map, Stream _out) 
    {
        _out.writeInt(map.Count);
        foreach (KeyValuePair<int, int> entry in map)
        {
            _out.writeInt(entry.Key);
            _out.writeInt(entry.Value);
        }
    }

    /**
     * 初始化
     */
    void setup_system()
    {
        system = new TransitionSystem();
        system.set_root_relation(deprels_alphabet.idOf(root));
        system.set_number_of_relations(deprels_alphabet.Count - 2);
    }

    /**
     * 初始化特征空间的长度等信息
     */
    void build_feature_space()
    {
        kFormInFeaturespace = 0;
        kNilForm = forms_alphabet.idOf(SpecialOption.NIL);
        kFeatureSpaceEnd = forms_alphabet.Count;

        kPostagInFeaturespace = kFeatureSpaceEnd;
        kNilPostag = kFeatureSpaceEnd + postags_alphabet.idOf(SpecialOption.NIL);
        kFeatureSpaceEnd += postags_alphabet.Count;

        kDeprelInFeaturespace = kFeatureSpaceEnd;
        kNilDeprel = kFeatureSpaceEnd + deprels_alphabet.idOf(SpecialOption.NIL);
        kFeatureSpaceEnd += deprels_alphabet.Count;

        kDistanceInFeaturespace = kFeatureSpaceEnd;
        kNilDistance = kFeatureSpaceEnd + (use_distance ? 8 : 0);
        kFeatureSpaceEnd += (use_distance ? 9 : 0);

        kValencyInFeaturespace = kFeatureSpaceEnd;
        kNilValency = kFeatureSpaceEnd + (use_valency ? 8 : 0);
        kFeatureSpaceEnd += (use_valency ? 9 : 0);

        kCluster4InFeaturespace = kFeatureSpaceEnd;
        if (use_cluster)
        {
            kNilCluster4 = kFeatureSpaceEnd + cluster4_types_alphabet.idOf(SpecialOption.NIL);
            kFeatureSpaceEnd += cluster4_types_alphabet.Count;
        }
        else
        {
            kNilCluster4 = kFeatureSpaceEnd;
        }

        kCluster6InFeaturespace = kFeatureSpaceEnd;
        if (use_cluster)
        {
            kNilCluster6 = kFeatureSpaceEnd + cluster6_types_alphabet.idOf(SpecialOption.NIL);
            kFeatureSpaceEnd += cluster6_types_alphabet.Count;
        }
        else
        {
            kNilCluster6 = kFeatureSpaceEnd;
        }

        kClusterInFeaturespace = kFeatureSpaceEnd;
        if (use_cluster)
        {
            kNilCluster = kFeatureSpaceEnd + cluster_types_alphabet.idOf(SpecialOption.NIL);
            kFeatureSpaceEnd += cluster_types_alphabet.Count;
        }
        else
        {
            kNilCluster = kFeatureSpaceEnd;
        }

    }

    /**
     * 将实例转为依存树
     * @param data 实例
     * @param dependency 输出的依存树
     * @param with_dependencies 是否输出依存关系（仅在解析后才有意义）
     */
    void transduce_instance_to_dependency(Instance data,
                                          Dependency dependency, bool with_dependencies)
    {
        int L = data.forms.Count;
        for (int i = 0; i < L; ++i)
        {
            int form = forms_alphabet.idOf(data.forms[i]);
            if (form == null)
            {
                form = forms_alphabet.idOf(SpecialOption.UNKNOWN);
            }
            int postag = postags_alphabet.idOf(data.postags[i]);
            if (postag == null) postag = postags_alphabet.idOf(SpecialOption.UNKNOWN);
            int deprel = (with_dependencies ? deprels_alphabet.idOf(data.deprels[i]) : -1);

            dependency.forms.Add(form);
            dependency.postags.Add(postag);
            dependency.heads.Add(with_dependencies ? data.heads[i] : -1);
            dependency.deprels.Add(with_dependencies ? deprel : -1);
        }
    }

    /**
     * 获取词聚类特征
     * @param data 输入数据
     * @param cluster4
     * @param cluster6
     * @param cluster
     */
    void get_cluster_from_dependency(Dependency data,
                                     List<int> cluster4,
                                     List<int> cluster6,
                                     List<int> cluster)
    {
        if (use_cluster)
        {
            int L = data.forms.Count;
            for (int i = 0; i < L; ++i)
            {
                int form = data.forms[i];
                cluster4.Add(i == 0 ?
                                     cluster4_types_alphabet.idOf(SpecialOption.ROOT) : form_to_cluster4.get(form));
                cluster6.Add(i == 0 ?
                                     cluster6_types_alphabet.idOf(SpecialOption.ROOT) : form_to_cluster6.get(form));
                cluster.Add(i == 0 ?
                                    cluster_types_alphabet.idOf(SpecialOption.ROOT) : form_to_cluster.get(form));
            }
        }
    }

    /**
     * 依存分析
     * @param data 实例
     * @param heads 依存指向的储存位置
     * @param deprels 依存关系的储存位置
     */
    void predict(Instance data, List<int> heads,
                 List<string> deprels)
    {
        Dependency dependency = new Dependency();
        List<int> cluster = new (), cluster4 = new (), cluster6 = new ();
        transduce_instance_to_dependency(data, dependency, false);
        get_cluster_from_dependency(dependency, cluster4, cluster6, cluster);

        int L = data.forms.Count;
        State[] states = new State[L * 2];
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = new State();
        }
        states[0].copy(new State(dependency));
        system.transit(states[0], ActionFactory.make_shift(), states[1]);
        for (int step = 1; step < L * 2 - 1; ++step)
        {
            List<int> attributes = new ();
            if (use_cluster)
            {
                get_features(states[step], cluster4, cluster6, cluster, attributes);
            }
            else
            {
                get_features(states[step], attributes);
            }

            List<Double> scores = new (system.number_of_transitions());
            classifier.score(attributes, scores);

            List<Action> possible_actions = new ();
            system.get_possible_actions(states[step], possible_actions);

            int best = -1;
            for (int j = 0; j < possible_actions.Count; ++j)
            {
                int l = system.transform(possible_actions.get(j));
                if (best == -1 || scores.get(best) < scores.get(l))
                {
                    best = l;
                }
            }

            Action act = system.transform(best);
            system.transit(states[step], act, states[step + 1]);
        }

//        heads.resize(L);
//        deprels.resize(L);
        for (int i = 0; i < L; ++i)
        {
            heads.Add(states[L * 2 - 1].heads[(i)]);
            deprels.Add(deprels_alphabet.labelOf(states[L * 2 - 1].deprels[(i)]));
        }
    }

    /**
     * 获取某个状态的上下文
     * @param s 状态
     * @param ctx 上下文
     */
    void get_context(State s, Context ctx)
    {
        ctx.S0 = (s.stack.Count > 0 ? s.stack.get(s.stack.Count - 1) : -1);
        ctx.S1 = (s.stack.Count > 1 ? s.stack.get(s.stack.Count - 2) : -1);
        ctx.S2 = (s.stack.Count > 2 ? s.stack.get(s.stack.Count - 3) : -1);
        ctx.N0 = (s.buffer < s.@ref.Count ? s.buffer : -1);
        ctx.N1 = (s.buffer + 1 < s.@ref.Count ? s.buffer + 1 : -1);
        ctx.N2 = (s.buffer + 2 < s.@ref.Count ? s.buffer + 2 : -1);

        ctx.S0L = (ctx.S0 >= 0 ? s.left_most_child.get(ctx.S0) : -1);
        ctx.S0R = (ctx.S0 >= 0 ? s.right_most_child.get(ctx.S0) : -1);
        ctx.S0L2 = (ctx.S0 >= 0 ? s.left_2nd_most_child.get(ctx.S0) : -1);
        ctx.S0R2 = (ctx.S0 >= 0 ? s.right_2nd_most_child.get(ctx.S0) : -1);
        ctx.S0LL = (ctx.S0L >= 0 ? s.left_most_child.get(ctx.S0L) : -1);
        ctx.S0RR = (ctx.S0R >= 0 ? s.right_most_child.get(ctx.S0R) : -1);

        ctx.S1L = (ctx.S1 >= 0 ? s.left_most_child.get(ctx.S1) : -1);
        ctx.S1R = (ctx.S1 >= 0 ? s.right_most_child.get(ctx.S1) : -1);
        ctx.S1L2 = (ctx.S1 >= 0 ? s.left_2nd_most_child.get(ctx.S1) : -1);
        ctx.S1R2 = (ctx.S1 >= 0 ? s.right_2nd_most_child.get(ctx.S1) : -1);
        ctx.S1LL = (ctx.S1L >= 0 ? s.left_most_child.get(ctx.S1L) : -1);
        ctx.S1RR = (ctx.S1R >= 0 ? s.right_most_child.get(ctx.S1R) : -1);
    }

    void get_features(State s,
                      List<int> features)
    {
        Context ctx = new Context();
        get_context(s, ctx);
        get_basic_features(ctx, s.@ref.forms, s.@ref.postags, s.deprels, features);
        get_distance_features(ctx, features);
        get_valency_features(ctx, s.nr_left_children, s.nr_right_children, features);
    }

    /**
     * 生成特征
     * @param s 当前状态
     * @param cluster4
     * @param cluster6
     * @param cluster
     * @param features 输出特征
     */
    void get_features(State s,
                      List<int> cluster4,
                      List<int> cluster6,
                      List<int> cluster,
                      List<int> features)
    {
        Context ctx = new Context();
        get_context(s, ctx);
        get_basic_features(ctx, s.@ref.forms, s.@ref.postags, s.deprels, features);
        get_distance_features(ctx, features);
        get_valency_features(ctx, s.nr_left_children, s.nr_right_children, features);
        get_cluster_features(ctx, cluster4, cluster6, cluster, features);
    }

    /**
     * 获取单词
     * @param forms 单词列表
     * @param id 单词下标
     * @return 单词
     */
    int FORM(List<int> forms, int id)
    {
        return ((id != -1) ? (forms.get(id)) : kNilForm);
    }

    /**
     * 获取词性
     * @param postags 词性列表
     * @param id 词性下标
     * @return 词性
     */
    int POSTAG(List<int> postags, int id)
    {
        return ((id != -1) ? (postags.get(id) + kPostagInFeaturespace) : kNilPostag);
    }

    /**
     * 获取依存
     * @param deprels 依存列表
     * @param id 依存下标
     * @return 依存
     */
    int DEPREL(List<int> deprels, int id)
    {
        return ((id != -1) ? (deprels.get(id) + kDeprelInFeaturespace) : kNilDeprel);
    }

    /**
     * 添加特征
     * @param features 输出特征的储存位置
     * @param feat 特征
     */
    void PUSH(List<int> features, int feat)
    {
        features.Add(feat);
    }

    /**
     * 获取基本特征
     * @param ctx 上下文
     * @param forms 单词
     * @param postags 词性
     * @param deprels 依存
     * @param features 输出特征的储存位置
     */
    void get_basic_features(Context ctx,
                            List<int> forms,
                            List<int> postags,
                            List<int> deprels,
                            List<int> features)
    {
        PUSH(features, FORM(forms, ctx.S0));
        PUSH(features, POSTAG(postags, ctx.S0));
        PUSH(features, FORM(forms, ctx.S1));
        PUSH(features, POSTAG(postags, ctx.S1));
        PUSH(features, FORM(forms, ctx.S2));
        PUSH(features, POSTAG(postags, ctx.S2));
        PUSH(features, FORM(forms, ctx.N0));
        PUSH(features, POSTAG(postags, ctx.N0));
        PUSH(features, FORM(forms, ctx.N1));
        PUSH(features, POSTAG(postags, ctx.N1));
        PUSH(features, FORM(forms, ctx.N2));
        PUSH(features, POSTAG(postags, ctx.N2));
        PUSH(features, FORM(forms, ctx.S0L));
        PUSH(features, POSTAG(postags, ctx.S0L));
        PUSH(features, DEPREL(deprels, ctx.S0L));
        PUSH(features, FORM(forms, ctx.S0R));
        PUSH(features, POSTAG(postags, ctx.S0R));
        PUSH(features, DEPREL(deprels, ctx.S0R));
        PUSH(features, FORM(forms, ctx.S0L2));
        PUSH(features, POSTAG(postags, ctx.S0L2));
        PUSH(features, DEPREL(deprels, ctx.S0L2));
        PUSH(features, FORM(forms, ctx.S0R2));
        PUSH(features, POSTAG(postags, ctx.S0R2));
        PUSH(features, DEPREL(deprels, ctx.S0R2));
        PUSH(features, FORM(forms, ctx.S0LL));
        PUSH(features, POSTAG(postags, ctx.S0LL));
        PUSH(features, DEPREL(deprels, ctx.S0LL));
        PUSH(features, FORM(forms, ctx.S0RR));
        PUSH(features, POSTAG(postags, ctx.S0RR));
        PUSH(features, DEPREL(deprels, ctx.S0RR));
        PUSH(features, FORM(forms, ctx.S1L));
        PUSH(features, POSTAG(postags, ctx.S1L));
        PUSH(features, DEPREL(deprels, ctx.S1L));
        PUSH(features, FORM(forms, ctx.S1R));
        PUSH(features, POSTAG(postags, ctx.S1R));
        PUSH(features, DEPREL(deprels, ctx.S1R));
        PUSH(features, FORM(forms, ctx.S1L2));
        PUSH(features, POSTAG(postags, ctx.S1L2));
        PUSH(features, DEPREL(deprels, ctx.S1L2));
        PUSH(features, FORM(forms, ctx.S1R2));
        PUSH(features, POSTAG(postags, ctx.S1R2));
        PUSH(features, DEPREL(deprels, ctx.S1R2));
        PUSH(features, FORM(forms, ctx.S1LL));
        PUSH(features, POSTAG(postags, ctx.S1LL));
        PUSH(features, DEPREL(deprels, ctx.S1LL));
        PUSH(features, FORM(forms, ctx.S1RR));
        PUSH(features, POSTAG(postags, ctx.S1RR));
        PUSH(features, DEPREL(deprels, ctx.S1RR));
    }

    /**
     * 获取距离特征
     * @param ctx 当前特征
     * @param features 输出特征
     */
    void get_distance_features(Context ctx,
                               List<int> features)
    {
        if (!use_distance)
        {
            return;
        }

        int dist = 8;
        if (ctx.S0 >= 0 && ctx.S1 >= 0)
        {
            dist = math.binned_1_2_3_4_5_6_10[ctx.S0 - ctx.S1];
            if (dist == 10)
            {
                dist = 7;
            }
        }
        features.Add(dist + kDistanceInFeaturespace);
    }

    /**
     * 获取(S0和S1的)配价特征
     * @param ctx 上下文
     * @param nr_left_children 左孩子数量列表
     * @param nr_right_children 右孩子数量列表
     * @param features 输出特征
     */
    void get_valency_features(Context ctx,
                              List<int> nr_left_children,
                              List<int> nr_right_children,
                              List<int> features)
    {
        if (!use_valency)
        {
            return;
        }

        int lvc = 8;
        int rvc = 8;
        if (ctx.S0 >= 0)
        {
            lvc = math.binned_1_2_3_4_5_6_10[nr_left_children.get(ctx.S0)];
            rvc = math.binned_1_2_3_4_5_6_10[nr_right_children.get(ctx.S0)];
            if (lvc == 10)
            {
                lvc = 7;
            }
            if (rvc == 10)
            {
                rvc = 7;
            }
        }
        features.Add(lvc + kValencyInFeaturespace);
        features.Add(rvc + kValencyInFeaturespace);

        lvc = 8;
        rvc = 8;
        if (ctx.S1 >= 0)
        {
            lvc = math.binned_1_2_3_4_5_6_10[nr_left_children.get(ctx.S1)];
            rvc = math.binned_1_2_3_4_5_6_10[nr_right_children.get(ctx.S1)];
            if (lvc == 10)
            {
                lvc = 7;
            }
            if (rvc == 10)
            {
                rvc = 7;
            }
        }
        features.Add(lvc + kValencyInFeaturespace);
        features.Add(rvc + kValencyInFeaturespace);
    }

    int CLUSTER(List<int> cluster, int id)
    {
        return (id >= 0 ? (cluster.get(id) + kClusterInFeaturespace) : kNilCluster);
    }

    int CLUSTER4(List<int> cluster4, int id)
    {
        return (id >= 0 ? (cluster4.get(id) + kCluster4InFeaturespace) : kNilCluster4);
    }

    int CLUSTER6(List<int> cluster6, int id)
    {
        return (id >= 0 ? (cluster6.get(id) + kCluster6InFeaturespace) : kNilCluster6);
    }

    /**
     * 获取词聚类特征
     * @param ctx 上下文
     * @param cluster4
     * @param cluster6
     * @param cluster
     * @param features 输出特征
     */
    void get_cluster_features(Context ctx,
                              List<int> cluster4,
                              List<int> cluster6,
                              List<int> cluster,
                              List<int> features)
    {
        if (!use_cluster)
        {
            return;
        }

        PUSH(features, CLUSTER(cluster, ctx.S0));
        PUSH(features, CLUSTER4(cluster4, ctx.S0));
        PUSH(features, CLUSTER6(cluster6, ctx.S0));
        PUSH(features, CLUSTER(cluster, ctx.S1));
        PUSH(features, CLUSTER(cluster, ctx.S2));
        PUSH(features, CLUSTER(cluster, ctx.N0));
        PUSH(features, CLUSTER4(cluster4, ctx.N0));
        PUSH(features, CLUSTER6(cluster6, ctx.N0));
        PUSH(features, CLUSTER(cluster, ctx.N1));
        PUSH(features, CLUSTER(cluster, ctx.N2));
        PUSH(features, CLUSTER(cluster, ctx.S0L));
        PUSH(features, CLUSTER(cluster, ctx.S0R));
        PUSH(features, CLUSTER(cluster, ctx.S0L2));
        PUSH(features, CLUSTER(cluster, ctx.S0R2));
        PUSH(features, CLUSTER(cluster, ctx.S0LL));
        PUSH(features, CLUSTER(cluster, ctx.S0RR));
        PUSH(features, CLUSTER(cluster, ctx.S1L));
        PUSH(features, CLUSTER(cluster, ctx.S1R));
        PUSH(features, CLUSTER(cluster, ctx.S1L2));
        PUSH(features, CLUSTER(cluster, ctx.S1R2));
        PUSH(features, CLUSTER(cluster, ctx.S1LL));
        PUSH(features, CLUSTER(cluster, ctx.S1RR));
    }

}
