/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-12 6:37 PM</create-date>
 *
 * <copyright file="ClusterAnalyzer.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.collection.trie.datrie;
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.dictionary.stopword;
using com.hankcs.hanlp.seg;
using com.hankcs.hanlp.seg.common;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.mining.cluster;




/**
 * 文本聚类
 *
 * @param <K> 文档的id类型
 * @author hankcs
 */
public class ClusterAnalyzer<K>
{
    protected Dictionary<K, Document<K>> documents_;
    protected Segment segment;
    protected MutableDoubleArrayTrieInteger vocabulary;
    const int NUM_REFINE_LOOP = 30;

    public ClusterAnalyzer()
    {
        documents_ = new ();
        segment = HanLP.newSegment();
        vocabulary = new MutableDoubleArrayTrieInteger();
    }

    protected int id(string word)
    {
        int id = vocabulary.get(word);
        if (id == -1)
        {
            id = vocabulary.size();
            vocabulary.Add(word, id);
        }
        return id;
    }

    /**
     * 重载此方法实现自己的预处理逻辑（预处理、分词、去除停用词）
     *
     * @param document 文档
     * @return 单词列表
     */
    protected List<string> preprocess(string document)
    {
        List<Term> termList = segment.seg(document);
        var listIterator = termList.GetEnumerator();
        while (listIterator.MoveNext())
        {
            Term term = listIterator.Current;
            if (CoreStopWordDictionary.Contains(term.word) ||
                term.nature.StartsWith("w")
                )
            {
                listIterator.Remove();
            }
        }
        List<string> wordList = new (termList.size());
        foreach (Term term in termList)
        {
            wordList.Add(term.word);
        }
        return wordList;
    }

    protected SparseVector toVector(List<string> wordList)
    {
        SparseVector vector = new SparseVector();
        foreach (string word in wordList)
        {
            int id = id(word);
            Double f = vector.get(id);
            if (f == null)
            {
                f = 1.;
                vector.Add(id, f);
            }
            else
            {
                vector.Add(id, ++f);
            }
        }
        return vector;
    }

    /**
     * 添加文档
     *
     * @param id       文档id
     * @param document 文档内容
     * @return 文档对象
     */
    public Document<K> addDocument(K id, string document)
    {
        return addDocument(id, preprocess(document));
    }

    /**
     * 添加文档
     *
     * @param id       文档id
     * @param document 文档内容
     * @return 文档对象
     */
    public Document<K> addDocument(K id, List<string> document)
    {
        SparseVector vector = toVector(document);
        Document<K> d = new Document<K>(id, vector);
        return documents_.Add(id, d);
    }

    /**
     * k-means聚类
     *
     * @param nclusters 簇的数量
     * @return 指定数量的簇（Set）构成的集合
     */
    public List<HashSet<K>> kmeans(int nclusters)
    {
        Cluster<K> cluster = new Cluster<K>();
        foreach (Document<K> document in documents_.values())
        {
            cluster.add_document(document);
        }
        cluster.section(nclusters);
        refine_clusters(cluster.sectioned_clusters());
        List<Cluster<K>> clusters_ = new (nclusters);
        foreach (Cluster<K> s in cluster.sectioned_clusters())
        {
            s.refresh();
            clusters_.Add(s);
        }
        return toResult(clusters_);
    }

    private List<HashSet<K>> toResult(List<Cluster<K>> clusters_)
    {
        List<HashSet<K>> result = new (clusters_.size());
        foreach (Cluster<K> c in clusters_)
        {
            HashSet<K> s = new HashSet<K>();
            foreach (Document<K> d in c.documents_)
            {
                s.Add(d.id_);
            }
            result.Add(s);
        }
        return result;
    }

    /**
     * repeated bisection 聚类
     *
     * @param nclusters 簇的数量
     * @return 指定数量的簇（Set）构成的集合
     */
    public List<HashSet<K>> repeatedBisection(int nclusters)
    {
        return repeatedBisection(nclusters, 0);
    }

    /**
     * repeated bisection 聚类
     *
     * @param limit_eval 准则函数增幅阈值
     * @return 指定数量的簇（Set）构成的集合
     */
    public List<HashSet<K>> repeatedBisection(double limit_eval)
    {
        return repeatedBisection(0, limit_eval);
    }

    /**
     * repeated bisection 聚类
     *
     * @param nclusters  簇的数量
     * @param limit_eval 准则函数增幅阈值
     * @return 指定数量的簇（Set）构成的集合
     */
    public List<HashSet<K>> repeatedBisection(int nclusters, double limit_eval)
    {
        Cluster<K> cluster = new Cluster<K>();
        List<Cluster<K>> clusters_ = new (nclusters > 0 ? nclusters : 16);
        foreach (Document<K> document in documents_.values())
        {
            cluster.add_document(document);
        }

        PriorityQueue<Cluster<K>> que = new PriorityQueue<Cluster<K>>();
        cluster.section(2);
        refine_clusters(cluster.sectioned_clusters());
        cluster.set_sectioned_gain();
        cluster.composite_vector().Clear();
        que.Add(cluster);

        while (!que.isEmpty())
        {
            if (nclusters > 0 && que.size() >= nclusters)
                break;
            cluster = que.peek();
            if (cluster.sectioned_clusters().size() < 1)
                break;
            if (limit_eval > 0 && cluster.sectioned_gain() < limit_eval)
                break;
            que.poll();
            List<Cluster<K>> sectioned = cluster.sectioned_clusters();

            foreach (Cluster<K> c in sectioned)
            {
                c.section(2);
                refine_clusters(c.sectioned_clusters());
                c.set_sectioned_gain();
                if (c.sectioned_gain() < limit_eval)
                {
                    foreach (Cluster<K> sub in c.sectioned_clusters())
                    {
                        sub.Clear();
                    }
                }
                c.composite_vector().Clear();
                que.Add(c);
            }
        }
        while (!que.isEmpty())
        {
            clusters_.Add(0, que.poll());
        }
        return toResult(clusters_);
    }

    /**
     * 根据k-means算法迭代优化聚类
     *
     * @param clusters 簇
     * @return 准则函数的值
     */
    double refine_clusters(List<Cluster<K>> clusters)
    {
        double[] norms = new double[clusters.Count];
        int offset = 0;
        foreach (Cluster<K> cluster in clusters)
        {
            norms[offset++] = cluster.composite_vector().norm();
        }

        double eval_cluster = 0.0;
        int loop_count = 0;
        while (loop_count++ < NUM_REFINE_LOOP)
        {
            List<int[]> items = new (documents_.Count);
            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = 0; j < clusters[(i)].documents().size(); j++)
                {
                    items.Add(new int[]{i, j});
                }
            }
            Collections.shuffle(items);

            bool changed = false;
            for (int[] item : items)
            {
                int cluster_id = item[0];
                int item_id = item[1];
                Cluster<K> cluster = clusters.get(cluster_id);
                Document<K> doc = cluster.documents().get(item_id);
                double value_base = refined_vector_value(cluster.composite_vector(), doc.feature(), -1);
                double norm_base_moved = Math.Pow(norms[cluster_id], 2) + value_base;
                norm_base_moved = norm_base_moved > 0 ? Math.Sqrt(norm_base_moved) : 0.0;

                double eval_max = -1.0;
                double norm_max = 0.0;
                int max_index = 0;
                for (int j = 0; j < clusters.size(); j++)
                {
                    if (cluster_id == j)
                        continue;
                    Cluster<K> other = clusters.get(j);
                    double value_target = refined_vector_value(other.composite_vector(), doc.feature(), 1);
                    double norm_target_moved = Math.Pow(norms[j], 2) + value_target;
                    norm_target_moved = norm_target_moved > 0 ? Math.Sqrt(norm_target_moved) : 0.0;
                    double eval_moved = norm_base_moved + norm_target_moved - norms[cluster_id] - norms[j];
                    if (eval_max < eval_moved)
                    {
                        eval_max = eval_moved;
                        norm_max = norm_target_moved;
                        max_index = j;
                    }
                }
                if (eval_max > 0)
                {
                    eval_cluster += eval_max;
                    clusters.get(max_index).add_document(doc);
                    clusters.get(cluster_id).remove_document(item_id);
                    norms[cluster_id] = norm_base_moved;
                    norms[max_index] = norm_max;
                    changed = true;
                }
            }
            if (!changed)
                break;
            foreach (Cluster<K> cluster in clusters)
            {
                cluster.refresh();
            }
        }
        return eval_cluster;
    }

    /**
     * c^2 - 2c(a + c) + d^2 - 2d(b + d)
     *
     * @param composite (a+c,b+d)
     * @param vec       (c,d)
     * @param sign
     * @return
     */
    double refined_vector_value(SparseVector composite, SparseVector vec, int sign)
    {
        double sum = 0.0;
        foreach (KeyValuePair<int, Double> entry in vec)
        {
            sum += Math.Pow(entry.Value, 2) + sign * 2 * composite.get(entry.Key) * entry.Value;
        }
        return sum;
    }

    /**
     * 训练模型
     *
     * @param folderPath 分类语料的根目录.目录必须满足如下结构:<br>
     *                   根目录<br>
     *                   ├── 分类A<br>
     *                   │   └── 1.txt<br>
     *                   │   └── 2.txt<br>
     *                   │   └── 3.txt<br>
     *                   ├── 分类B<br>
     *                   │   └── 1.txt<br>
     *                   │   └── ...<br>
     *                   └── ...<br>
     *                   文件不一定需要用数字命名,也不需要以txt作为后缀名,但一定需要是文本文件.
     * @param algorithm  kmeans 或 repeated bisection
     * @ 任何可能的IO异常
     */
    public static double evaluate(string folderPath, string algorithm)
    {
        if (folderPath == null) throw new ArgumentException("参数 folderPath == null");
        File root = new File(folderPath);
        if (!root.exists()) throw new ArgumentException(string.Format("目录 %s 不存在", root));
        if (!root.isDirectory())
            throw new ArgumentException(string.Format("目录 %s 不是一个目录", root));

        ClusterAnalyzer<string> analyzer = new ClusterAnalyzer<string>();
        File[] folders = root.listFiles();
        if (folders == null) return 1.;
        logger.start("根目录:%s\n加载中...\n", folderPath);
        int docSize = 0;
        int[] ni = new int[folders.Length];
        string[] cat = new string[folders.Length];
        int offset = 0;
        foreach (File folder in folders)
        {
            if (folder.isFile()) continue;
            File[] files = folder.listFiles();
            if (files == null) continue;
            string category = folder.getName();
            cat[offset] = category;
            logger._out("[%s]...", category);
            int b = 0;
            int e = files.Length;

            int logEvery = (int) Math.Ceiling((e - b) / 10000f);
            for (int i = b; i < e; i++)
            {
                analyzer.addDocument(folder.getName() + " " + files[i].getName(), IOUtil.readTxt(files[i]));
                if (i % logEvery == 0)
                {
                    logger._out("%c[%s]...%.2f%%", 13, category, MathUtility.percentage(i - b + 1, e - b));
                }
                ++docSize;
                ++ni[offset];
            }
            logger._out(" %d 篇文档\n", e - b);
            ++offset;
        }
        logger.finish(" 加载了 %d 个类目,共 %d 篇文档\n", folders.Length, docSize);
        logger.start(algorithm + "聚类中...");
        List<HashSet<string>> clusterList = algorithm.Replace("[-\\s]", "").ToLower().Equals("kmeans") ?
            analyzer.kmeans(ni.Length) : analyzer.repeatedBisection(ni.Length);
        logger.finish(" 完毕。\n");
        double[] fi = new double[ni.Length];
        for (int i = 0; i < ni.Length; i++)
        {
            foreach (HashSet<string> j in clusterList)
            {
                int nij = 0;
                foreach (string d in j)
                {
                    if (d.StartsWith(cat[i]))
                        ++nij;
                }
                if (nij == 0) continue;
                double p = nij / (double) (j.Count);
                double r = nij / (double) (ni[i]);
                double fx = 2 * p * r / (p + r);
                fi[i] = Math.Max(fi[i], fx);
            }
        }
        double f = 0;
        for (int i = 0; i < fi.Length; i++)
        {
            f += fi[i] * ni[i] / docSize;
        }
        return f;
    }
}
