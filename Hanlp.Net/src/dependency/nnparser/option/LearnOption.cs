/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>me@hankcs.com</email>
 * <create-date>2015/11/1 20:13</create-date>
 *
 * <copyright file="LearnOption.java" company="��ũ��">
 * Copyright (c) 2008-2015, ��ũ��. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.dependency.nnparser.option;

/**
 * @author hankcs
 */
public class LearnOption : BasicOption
{
    public double ada_eps;             //! Eps used in AdaGrad
    public double ada_alpha;           //! Alpha used in AdaGrad
    public double lambda;              //! TODO not known.
    public double dropout_probability; //! The probability for dropout.

    public int hidden_layer_size;    //! Size for hidden layer.
    public int embedding_size;       //! Size for embedding.

    string reference_file;   //! The path to the reference file.
    string devel_file;       //! The path to the devel file.
    string embedding_file;   //! The path to the embedding.
    string cluster_file;     //! The path to the cluster file, actived in use-cluster.
    string oracle;           //! The oracle type, can be [static, nondet, explore]
    int word_cutoff;              //! The frequency of rare word, word lower than that
    //! will be cut off.
    int max_iter;                 //! The maximum iteration.
    public double init_range;            //!
    public int batch_size;               //! The Size of batch.
    int nr_precomputed;           //! The number of precomputed features
    int evaluation_stops;         //!
    int clear_gradient_per_iter;  //! Clear gradient each iteration.
    bool save_intermediate;       //! Save model whenever see an improved UAS.
    public bool fix_embeddings;          //! Not tune the embedding when learning the parameters
    bool use_distance;            //! Specify to use distance feature.
    bool use_valency;             //! Specify to use valency feature.
    bool use_cluster;             //! Specify to use cluster feature.
}
