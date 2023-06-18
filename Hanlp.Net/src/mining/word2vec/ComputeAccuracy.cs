/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-07-15 PM2:20</create-date>
 *
 * <copyright file="ComputeAccuracy.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.mining.word2vec;


/**
 * @author hankcs
 */
public class ComputeAccuracy
{
    const int max_size = 2000;         // max Length of strings
    const int N = 1;                   // number of closest words
    const int max_w = 50;              // max Length of vocabulary entries

    public static void main(string[] argv) 
    {
        BufferedReader f;
        string st1 = null, st2, st3, st4;
        string[] bestw = new string[N];
        double dist, len;
        double[] bestd = new double[N];
        double[] vec = new double[max_size];
        int words = 0, size = 0, a, b, c, d, b1, b2, b3, threshold = 0;
        double[] M;
        string[] vocab;
        int TCN, CCN = 0, TACN = 0, CACN = 0, SECN = 0, SYCN = 0, SEAC = 0, SYAC = 0, QID = 0, TQ = 0, TQS = 0;
        if (argv == null || argv.Length != 3)
        {
            printf("Usage: ./compute-accuracy <FILE> <threshold> <QUESTION FILE>\nwhere FILE contains word projections, and threshold is used to reduce vocabulary of the model for fast approximate evaluation (0 = off, otherwise typical value is 30000). Question file contains questions and answers\n");
            return;
        }
        string file_name = argv[0];
        threshold = int.parseInt(argv[1]);
        try
        {
            f = new BufferedReader(new InputStreamReader(new FileInputStream(file_name), "UTF-8"));
        }
        catch (FileNotFoundException e)
        {
            printf("Input file not found\n");
            Environment.Exit(-1);
            return;
        }
        catch (UnsupportedEncodingException e)
        {
            e.printStackTrace();
            return;
        }

        try
        {
            string[] _params = f.readLine().Split("\\s");
            words = int.parseInt(_params[0]);
            if (words > threshold) words = threshold;
            size = int.parseInt(_params[1]);
            vocab = new string[words];
            M = new double[words * size];
            for (b = 0; b < words; b++)
            {
                _params = f.readLine().Split("\\s");
                vocab[b] = _params[0].toUpperCase();
                for (a = 0; a < size; a++)
                {
                    M[a + b * size] = Double.parseDouble(_params[1 + a]);
                }
                len = 0;
                for (a = 0; a < size; a++) len += M[a + b * size] * M[a + b * size];
                len = Math.Sqrt(len);
                for (a = 0; a < size; a++) M[a + b * size] /= len;
            }
            f.close();
        }
        catch (IOException e)
        {
            printf("IO error\n");
            System.exit(-2);
            return;
        }
        catch (OutOfMemoryError e)
        {
            printf("Cannot allocate memory: %lld MB\n", words * size * 8 / 1048576);
            System.exit(-3);
            return;
        }

        TCN = 0;
        BufferedReader stdin = null;
        try
        {
            stdin = new BufferedReader(new InputStreamReader(new FileInputStream(argv[2])));
        }
        catch (FileNotFoundException e)
        {
            printf("Question file %s not found\n", argv[2]);
        }
        while (true)
        {
            for (a = 0; a < N; a++) bestd[a] = 0;
            for (a = 0; a < N; a++) bestw[a] = null;
            string line = stdin.readLine();

            string[] param = null;
            if (line != null && line.Length > 0)
            {
                param = line.toUpperCase().Split("\\s");
                st1 = param[0];
            }
            if (line == null || line.Length == 0 || st1.Equals(":") || st1.Equals("EXIT"))
            {
                if (TCN == 0) TCN = 1;
                if (QID != 0)
                {
                    printf("ACCURACY TOP1: %.2f %%  (%d / %d)\n", CCN / (double) TCN * 100, CCN, TCN);
                    printf("Total accuracy: %.2f %%   Semantic accuracy: %.2f %%   Syntactic accuracy: %.2f %% \n", CACN / (double) TACN * 100, SEAC / (double) SECN * 100, SYAC / (double) SYCN * 100);
                }
                QID++;
                if (line == null || line.Length == 0) break;
                st1 = param[1];
                printf("%s:\n", st1);
                TCN = 0;
                CCN = 0;
                continue;
            }
            if ("EXIT".Equals(st1)) break;
            st2 = param[1];
            st3 = param[2];
            st4 = param[3];
            for (b = 0; b < words; b++) if (st1.Equals(vocab[b]))break;
            b1 = b;
            for (b = 0; b < words; b++) if (st2.Equals(vocab[b]))break;
            b2 = b;
            for (b = 0; b < words; b++) if (st3.Equals(vocab[b]))break;
            b3 = b;
            for (a = 0; a < N; a++) bestd[a] = 0;
            for (a = 0; a < N; a++) bestw[a] = null;
            TQ++;
            if (b1 == words) continue;
            if (b2 == words) continue;
            if (b3 == words) continue;
            for (b = 0; b < words; b++) if (st4.Equals(vocab[b]))break;
            if (b == words) continue;
            for (a = 0; a < size; a++) vec[a] = (M[a + b2 * size] - M[a + b1 * size]) + M[a + b3 * size];
            TQS++;
            for (c = 0; c < words; c++)
            {
                if (c == b1) continue;
                if (c == b2) continue;
                if (c == b3) continue;
                dist = 0;
                for (a = 0; a < size; a++) dist += vec[a] * M[a + c * size];
                for (a = 0; a < N; a++)
                {
                    if (dist > bestd[a])
                    {
                        for (d = N - 1; d > a; d--)
                        {
                            bestd[d] = bestd[d - 1];
                            bestw[d] = bestw[d - 1];
                        }
                        bestd[a] = dist;
                        bestw[a] = vocab[c];
                        break;
                    }
                }
            }
            if (st4.Equals(bestw[0]))
            {
                CCN++;
                CACN++;
                if (QID <= 5) SEAC++;
                else SYAC++;
            }
            if (QID <= 5) SECN++;
            else SYCN++;
            TCN++;
            TACN++;
        }
        printf("Questions seen / total: %d %d   %.2f %% \n", TQS, TQ, TQS / (double) TQ * 100);
    }

    private static void printf(string Format, params Object[] args)
    {
        System._out.printf(Format, args);
    }
}
