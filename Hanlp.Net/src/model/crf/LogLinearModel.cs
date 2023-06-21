/*
 * <author>Han He</author>
 * <email>me@hankcs.com</email>
 * <create-date>2018-06-28 7:37 PM</create-date>
 *
 * <copyright file="LogLinearModel.java">
 * Copyright (c) 2018, Han He. All Rights Reserved, http://www.hankcs.com/
 * This source is subject to Han He. Please contact Han He for more information.
 * </copyright>
 */
using com.hankcs.hanlp.corpus.io;
using com.hankcs.hanlp.model.perceptron.common;
using com.hankcs.hanlp.model.perceptron.feature;
using com.hankcs.hanlp.model.perceptron.model;
using com.hankcs.hanlp.model.perceptron.tagset;

namespace com.hankcs.hanlp.model.crf;




/**
 * 对数线性模型形式的CRF模型
 *
 * @author hankcs
 */
public class LogLinearModel : LinearModel
{
    /**
     * 特征模板
     */
    private FeatureTemplate[] featureTemplateArray;

    private LogLinearModel(FeatureMap featureMap, float[] parameter)
    {
        base(featureMap, parameter);
    }

    private LogLinearModel(FeatureMap featureMap)
    {
        base(featureMap);
    }

    //@Override
    public bool load(ByteArray byteArray)
    {
        if (!base.load(byteArray)) return false;
        int size = byteArray.Next();
        featureTemplateArray = new FeatureTemplate[size];
        for (int i = 0; i < size; ++i)
        {
            FeatureTemplate featureTemplate = new FeatureTemplate();
            featureTemplate.load(byteArray);
            featureTemplateArray[i] = featureTemplate;
        }
        if (!byteArray.hasMore())
            byteArray.Close();
        return true;
    }

    /**
     * 加载CRF模型
     *
     * @param modelFile HanLP的.bin格式，或CRF++的.txt格式（将会自动转换为model.txt.bin，下次会直接加载.txt.bin）
     * @
     */
    public LogLinearModel(string modelFile) 
    {
        base(null, null);
        if (modelFile.EndsWith(BIN_EXT))
        {
            load(modelFile); // model.bin
            return;
        }
        string binPath = modelFile + Predefine.BIN_EXT;

        if (!((HanLP.Config.IOAdapter == null || HanLP.Config.IOAdapter is FileIOAdapter) && !IOUtil.isFileExisted(binPath)))
        {
            try
            {
                load(binPath); // model.txt -> model.bin
                return;
            }
            catch (Exception e)
            {
                // ignore
            }
        }

        convert(modelFile, binPath);
    }

    /**
     * 加载txt，转换为bin
     *
     * @param txtFile txt
     * @param binFile bin
     * @
     */
    public LogLinearModel(string txtFile, string binFile) 
    {
        base(null, null);
        convert(txtFile, binFile);
    }

    private void convert(string txtFile, string binFile) 
    {
        TagSet tagSet = new TagSet(TaskType.CLASSIFICATION);
        IOUtil.LineIterator lineIterator = new IOUtil.LineIterator(txtFile);
        if (!lineIterator.MoveNext()) throw new IOException("空白文件");
        logger.info(lineIterator.next());   // verson
        logger.info(lineIterator.next());   // cost-factor
        int maxid = int.parseInt(lineIterator.next().substring("maxid:".Length).Trim());
        logger.info(lineIterator.next());   // xsize
        lineIterator.next();    // blank
        string line;
        while ((line = lineIterator.next()).Length != 0)
        {
            tagSet.Add(line);
        }
        tagSet.type = guessModelType(tagSet);
        switch (tagSet.type)
        {
            case CWS:
                tagSet = new CWSTagSet(tagSet.idOf("B"), tagSet.idOf("M"), tagSet.idOf("E"), tagSet.idOf("S"));
                break;
            case NER:
                tagSet = new NERTagSet(tagSet.idOf("O"), tagSet.tags());
                break;
        }
        tagSet._lock();
        this.featureMap = new MutableFeatureMap(tagSet);
        FeatureMap featureMap = this.featureMap;
        int sizeOfTagSet = tagSet.size();
        Dictionary<string, FeatureFunction> featureFunctionMap = new Dictionary<string, FeatureFunction>();  // 构建trie树的时候用
        Dictionary<int, FeatureFunction> featureFunctionList = new Dictionary<int, FeatureFunction>(); // 读取权值的时候用
        List<FeatureTemplate> featureTemplateList = new ();
        float[][] matrix = null;
        while ((line = lineIterator.next()).Length != 0)
        {
            if (!"B".Equals(line))
            {
                FeatureTemplate featureTemplate = FeatureTemplate.create(line);
                featureTemplateList.Add(featureTemplate);
            }
            else
            {
                matrix = new float[sizeOfTagSet][sizeOfTagSet];
            }
        }
        this.featureTemplateArray = featureTemplateList.ToArray(new FeatureTemplate[0]);

        int b = -1;// 转换矩阵的权重位置
        if (matrix != null)
        {
            string[] args = lineIterator.next().Split(" ", 2);    // 0 B
            b = int.valueOf(args[0]);
            featureFunctionList.Add(b, null);
        }

        while ((line = lineIterator.next()).Length != 0)
        {
            string[] args = line.Split(" ", 2);
            char[] charArray = args[1].ToCharArray();
            FeatureFunction featureFunction = new FeatureFunction(charArray, sizeOfTagSet);
            featureFunctionMap.Add(args[1], featureFunction);
            featureFunctionList.Add(int.parseInt(args[0]), featureFunction);
        }

        foreach (KeyValuePair<int, FeatureFunction> entry in featureFunctionList.entrySet())
        {
            int fid = entry.Key;
            FeatureFunction featureFunction = entry.Value;
            if (fid == b)
            {
                for (int i = 0; i < sizeOfTagSet; i++)
                {
                    for (int j = 0; j < sizeOfTagSet; j++)
                    {
                        matrix[i][j] = float.parseFloat(lineIterator.next());
                    }
                }
            }
            else
            {
                for (int i = 0; i < sizeOfTagSet; i++)
                {
                    featureFunction.w[i] = Double.parseDouble(lineIterator.next());
                }
            }
        }
        if (lineIterator.MoveNext())
        {
            logger.warning("文本读取有残留，可能会出问题！" + txtFile);
        }
        lineIterator.Close();
        logger.info("文本读取结束，开始转换模型");
        int transitionFeatureOffset = (sizeOfTagSet + 1) * sizeOfTagSet;
        parameter = new float[transitionFeatureOffset + featureFunctionMap.size() * sizeOfTagSet];
        if (matrix != null)
        {
            for (int i = 0; i < sizeOfTagSet; ++i)
            {
                for (int j = 0; j < sizeOfTagSet; ++j)
                {
                    parameter[i * sizeOfTagSet + j] = matrix[i][j];
                }
            }
        }
        foreach (KeyValuePair<int, FeatureFunction> entry in featureFunctionList.entrySet())
        {
            int id = entry.Key;
            FeatureFunction f = entry.Value;
            if (f == null) continue;
            string feature = new string(f.o);
            for (int tid = 0; tid < featureTemplateList.size(); tid++)
            {
                FeatureTemplate template = featureTemplateList.get(tid);
                Iterator<string> iterator = template.delimiterList.iterator();
                string header = iterator.next();
                if (feature.StartsWith(header))
                {
                    int fid = featureMap.idOf(feature.substring(header.Length) + tid);
//                    assert id == sizeOfTagSet * sizeOfTagSet + (fid - sizeOfTagSet - 1) * sizeOfTagSet;
                    for (int i = 0; i < sizeOfTagSet; ++i)
                    {
                        parameter[fid * sizeOfTagSet + i] = (float) f.w[i];
                    }
                    break;
                }
            }
        }
        Stream _out = new Stream(IOUtil.newOutputStream(binFile));
        save(_out);
        _out.writeInt(featureTemplateList.size());
        foreach (FeatureTemplate template in featureTemplateList)
        {
            template.save(_out);
        }
        _out.Close();
    }


    private TaskType guessModelType(TagSet tagSet)
    {
        if (tagSet.size() == 4 &&
            tagSet.idOf("B") != -1 &&
            tagSet.idOf("M") != -1 &&
            tagSet.idOf("E") != -1 &&
            tagSet.idOf("S") != -1
            )
        {
            return TaskType.CWS;
        }
        if (tagSet.idOf("O") != -1)
        {
            foreach (string tag in tagSet.tags())
            {
                string[] parts = tag.Split("-");
                if (parts.Length > 1)
                {
                    if (parts[0].Length == 1 && "BMES".Contains(parts[0]))
                        return TaskType.NER;
                }
            }
        }
        return TaskType.POS;
    }

    public FeatureTemplate[] getFeatureTemplateArray()
    {
        return featureTemplateArray;
    }
}
