/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-08-12 6:40 PM</create-date>
 *
 * <copyright file="SparseVector.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.mining.cluster;


/**
 * @author hankcs
 */
public class SparseVector : Dictionary<int, double>
{
    //@Override
    public Double get(Object key)
    {
        Double v = base.get(key);
        if (v == null) return 0.0f;
        return v;
    }

    /**
     * Normalize a vector.
     */
    public void normalize()
    {
        double nrm = norm();
        foreach (KeyValuePair<int, Double> d in this)
        {
            d.setValue(d.Value / nrm);
        }
    }

    /**
     * Calculate a squared norm.
     */
    double norm_squared()
    {
        double sum = 0;
        foreach (Double point in Values)
        {
            sum += point * point;
        }
        return sum;
    }

    /**
     * Calculate a norm.
     */
    public double norm()
    {
        return (double) Math.Sqrt(norm_squared());
    }

    /**
     * Multiply each value of  avector by a constant value.
     */
    public void multiply_constant(double x)
    {
        foreach (KeyValuePair<int, Double> entry in this)
        {
            entry.setValue(entry.Value * x);
        }
    }

    /**
     * Add other vector.
     */
    public void add_vector(SparseVector vec)
    {

        foreach (KeyValuePair<int, Double> entry in vec)
        {
            if(!this.TryGetValue(entry.Key,out var v)) v = 0.0;

            Add(entry.Key, v + entry.Value);
        }
    }

    /**
     * Subtract other vector.
     */
    public void sub_vector(SparseVector vec)
    {

        foreach (KeyValuePair<int, Double> entry in vec)
        {
            Double v = get(entry.Key);
            if (v == null)
                v = 0.0;
            Add(entry.Key, v - entry.Value);
        }
    }

    //    /**
    //     * Calculate the squared euclid distance between vectors.
    //     */
    //    double euclid_distance_squared(const Vector &vec1, const Vector &vec2)
    //{
    //    HashMap<VecKey, bool>::type done;
    //    init_hash_map(VECTOR_EMPTY_KEY, done, vec1.Count);
    //    VecHashMap::const_iterator it1, it2;
    //    double dist = 0;
    //    for (it1 = vec1.hash_map()->begin(); it1 != vec1.hash_map()->end(); ++it1)
    //    {
    //        double val = vec2.get(it1->first);
    //        dist += (it1->second - val) * (it1->second - val);
    //        done[it1->first] = true;
    //    }
    //    for (it2 = vec2.hash_map()->begin(); it2 != vec2.hash_map()->end(); ++it2)
    //    {
    //        if (done.find(it2->first) == done.end())
    //        {
    //            double val = vec1.get(it2->first);
    //            dist += (it2->second - val) * (it2->second - val);
    //        }
    //    }
    //    return dist;
    //}
    //
    //    /**
    //     * Calculate the euclid distance between vectors.
    //     */
    //    double euclid_distance(const Vector &vec1, const Vector &vec2)
    //{
    //    return sqrt(euclid_distance_squared(vec1, vec2));
    //}

    /**
     * Calculate the inner product value between vectors.
     */
    public static double inner_product(SparseVector vec1, SparseVector vec2)
    {
        SparseVector other;
        IEnumerator<KeyValuePair<int, Double>> it;
        if (vec1.Count < vec2.Count)
        {
            it = vec1.GetEnumerator();
            other = vec2;
        }
        else
        {
            it = vec2.GetEnumerator();
            other = vec1;
        }
        double prod = 0;
        while (it.MoveNext())
        {
            KeyValuePair<int, double> entry = it.Current;
            prod += entry.Value * other.get(entry.Key);
        }
        return prod;
    }

    /**
     * Calculate the cosine value between vectors.
     */
    public double cosine(SparseVector vec1, SparseVector vec2)
    {
        double norm1 = vec1.norm();
        double norm2 = vec2.norm();
        double result = 0.0f;
        if (norm1 == 0 && norm2 == 0)
        {
            return result;
        }
        else
        {
            double prod = inner_product(vec1, vec2);
            result = prod / (norm1 * norm2);
            return Double.IsNaN(result) ? 0.0f : result;
        }
    }

//    /**
//     * Calculate the Jaccard coefficient value between vectors.
//     */
//    double jaccard(const Vector &vec1, const Vector &vec2)
//{
//    double norm1 = vec1.norm();
//    double norm2 = vec2.norm();
//    double prod = inner_product(vec1, vec2);
//    double denom = norm1 + norm2 - prod;
//    double result = 0.0;
//    if (!denom)
//    {
//        return result;
//    }
//    else
//    {
//        result = prod / denom;
//        return isnan(result) ? 0.0 : result;
//    }
//}
}
