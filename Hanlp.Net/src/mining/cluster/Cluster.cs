/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-12 7:11 PM</create-date>
 *
 * <copyright file="Cluster.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.mining.cluster;

using System.Collections.Generic;
/**
 * @author hankcs
 */
public class Cluster<K> : IComparable<Cluster<K>>
{
    public List<Document<K>> documents_;          ///< documents
    SparseVector composite_;                           ///< a composite SparseVector
    SparseVector centroid_;                            ///< a centroid SparseVector
    List<Cluster<K>> sectioned_clusters_;  ///< sectioned clusters
    double sectioned_gain_;                      ///< a sectioned gain
    Random random;

    public Cluster()
        : this(new List<Document<K>>())
    {
        ;
    }

    public Cluster(List<Document<K>> documents)
    {
        this.documents_ = documents;
        composite_ = new SparseVector();
        random = new Random();
    }

    /**
     * Add the vectors of all documents to a composite vector.
     */
    public void set_composite_vector()
    {
        composite_.Clear();
        foreach (Document<K> document in documents_)
        {
            composite_.add_vector(document.feature());
        }
    }

    /**
     * Clear status.
     */
    public void Clear()
    {
        documents_.Clear();
        composite_.Clear();
        if (centroid_ != null)
            centroid_.Clear();
        if (sectioned_clusters_ != null)
            sectioned_clusters_.Clear();
        sectioned_gain_ = 0.0;
    }


    /**
     * Get the size.
     *
     * @return the size of this cluster
     */
    int size()
    {
        return documents_.Count;
    }

    /**
     * Get the pointer of a centroid vector.
     *
     * @return the pointer of a centroid vector
     */
    SparseVector centroid_vector()
    {
        if (documents_.Count > 0 && composite_.Count == 0)
            set_composite_vector();
        centroid_ = (SparseVector) composite_vector().clone();
        centroid_.normalize();
        return centroid_;
    }

    /**
     * Get the pointer of a composite vector.
     *
     * @return the pointer of a composite vector
     */
    public SparseVector composite_vector()
    {
        return composite_;
    }

    /**
     * Get documents in this cluster.
     *
     * @return documents in this cluster
     */
    public List<Document<K>> documents()
    {
        return documents_;
    }

    /**
     * Add a document.
     *
     * @param doc the pointer of a document object
     */
    public void add_document(Document<K> doc)
    {
        doc.feature().normalize();
        documents_.Add(doc);
        composite_.add_vector(doc.feature());
    }

    /**
     * Remove a document from this cluster.
     *
     * @param index the index of vector container of documents
     */
    public void remove_document(int index)
    {
        var listIterator = documents_.listIterator(index);
        Document<K> document = listIterator.next();
        listIterator.set(null);
        composite_.sub_vector(document.feature());
    }

    /**
     * Remove a document from this cluster.
     *
     * @param doc the pointer of a document object
     */
    public void remove_document(Document<K> doc)
    {
        foreach (Document<K> document in documents_)
        {
            if (document.Equals(doc))
            {
                remove_document(doc);
                return;
            }
        }
    }


    /**
     * Delete removed documents from the internal container.
     */
    public void refresh()
    {
        var listIterator = documents_.GetEnumerator();
        while (listIterator.MoveNext())
        {
            if (listIterator.next() == null)
                listIterator.Remove();
        }
    }

    /**
     * Get a gain when this cluster sectioned.
     *
     * @return a gain
     */
    public double sectioned_gain()
    {
        return sectioned_gain_;
    }

    /**
     * Set a gain when the cluster sectioned.
     */
    public void set_sectioned_gain()
    {
        double gain = 0.0f;
        if (sectioned_gain_ == 0 && sectioned_clusters_.Count > 1)
        {
            foreach (Cluster<K> cluster in sectioned_clusters_)
            {
                gain += cluster.composite_vector().norm();
            }
            gain -= composite_.norm();
        }
        sectioned_gain_ = gain;
    }

    /**
     * Get sectioned clusters.
     *
     * @return sectioned clusters
     */
    public List<Cluster<K>> sectioned_clusters()
    {
        return sectioned_clusters_;
    }

//    /**
//     * Choose documents randomly.
//     */
//    void choose_randomly(int ndocs, List<Document > docs)
//{
//    HashMap<int, bool>.type choosed;
//    int siz = size();
//    init_hash_map(siz, choosed, ndocs);
//    if (siz < ndocs)
//        ndocs = siz;
//    int count = 0;
//    while (count < ndocs)
//    {
//        int index = myrand(seed_) % siz;
//        if (choosed.find(index) == choosed.end())
//        {
//            choosed.insert(std.pair<int, bool>(index, true));
//            docs.push_back(documents_[index]);
//            ++count;
//        }
//    }
//}

    /**
     * 选取初始质心
     *
     * @param ndocs 质心数量
     * @param docs  输出到该列表中
     */
    public void choose_smartly(int ndocs, List<Document<K>> docs)
    {
        int siz = size();
        double[] closest = new double[siz];
        if (siz < ndocs)
            ndocs = siz;
        int index, count = 0;

        index = random.Next(siz);  // initial center
        docs.Add(documents_[(index)]);
        ++count;
        double potential = 0.0;
        for (int i = 0; i < documents_.Count; i++)
        {
            double dist = 1.0 - SparseVector.inner_product(documents_[i].feature(), documents_[index].feature());
            potential += dist;
            closest[i] = dist;
        }

        // choose each center
        while (count < ndocs)
        {
            double randval = random.NextDouble() * potential;

            for (index = 0; index < documents_.Count; index++)
            {
                double dist = closest[index];
                if (randval <= dist)
                    break;
                randval -= dist;
            }
            if (index == documents_.Count)
                index--;
            docs.Add(documents_[(index)]);
            ++count;

            double new_potential = 0.0;
            for (int i = 0; i < documents_.Count; i++)
            {
                double dist = 1.0 - SparseVector.inner_product(documents_[i].feature(), documents_[index].feature());
                double min = closest[i];
                if (dist < min)
                {
                    closest[i] = dist;
                    min = dist;
                }
                new_potential += min;
            }
            potential = new_potential;
        }
    }

    /**
     * 将本簇划分为nclusters个簇
     *
     * @param nclusters
     */
    public void section(int nclusters)
    {
        if (size() < nclusters)
            return;

        sectioned_clusters_ = new (nclusters);
        List<Document<K>> centroids = new (nclusters);
        // choose_randomly(nclusters, centroids);
        choose_smartly(nclusters, centroids);
        for (int i = 0; i < centroids.Count; i++)
        {
            Cluster<K> cluster = new Cluster<K>();
            sectioned_clusters_.Add(cluster);
        }

        foreach (Document<K> d in documents_)
        {
            double max_similarity = -1.0;
            int max_index = 0;
            for (int j = 0; j < centroids.Count; j++)
            {
                double similarity = SparseVector.inner_product(d.feature(), centroids[j].feature());
                if (max_similarity < similarity)
                {
                    max_similarity = similarity;
                    max_index = j;
                }
            }
            sectioned_clusters_[(max_index)].add_document(d);
        }
    }

    //@Override
    public int CompareTo(Cluster<K> o)
    {
        return Double.compare(o.sectioned_gain(), sectioned_gain());
    }
}
