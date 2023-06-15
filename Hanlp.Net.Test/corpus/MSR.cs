/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-24 10:34 AM</create-date>
 *
 * <copyright file="MSR.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.corpus;


/**
 * @author hankcs
 */
[TestClass]
public class MSR
{
    public static readonly string TRAIN_PATH = "data/test/icwb2-data/training/msr_training.utf8";
    public static readonly string TEST_PATH = "data/test/icwb2-data/testing/msr_test.utf8";
    public static readonly string GOLD_PATH = "data/test/icwb2-data/gold/msr_test_gold.utf8";
    public static readonly string MODEL_PATH = "data/test/msr_cws";
    public static readonly string OUTPUT_PATH = "data/test/msr_output.txt";
    public static readonly string TRAIN_WORDS = "data/test/icwb2-data/gold/msr_training_words.utf8";
    public static String SIGHAN05_ROOT;

    static MSR()
    {
        SIGHAN05_ROOT = TestUtility.ensureTestData("icwb2-data", "http://sighan.cs.uchicago.edu/bakeoff2005/data/icwb2-data.zip");
        if (!IOUtil.isFileExisted(TRAIN_PATH))
        {
            System.err.println("请下载 http://sighan.cs.uchicago.edu/bakeoff2005/data/icwb2-data.zip 并解压为 data/test/icwb2-data");
            System.exit(1);
        }
    }
}
