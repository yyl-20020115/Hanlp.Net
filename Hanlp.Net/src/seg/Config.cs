/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/10/30 10:06</create-date>
 *
 * <copyright file="Config.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.seg;

/**
 * 分词器配置项
 */
public class Config
{
    /**
     * 是否是索引分词（合理地最小分割），indexMode代表全切分词语的最小长度（包含）
     */
    public int indexMode = 0;
    /**
     * 是否识别中国人名
     */
    public bool nameRecognize = true;
    /**
     * 是否识别音译人名
     */
    public bool translatedNameRecognize = true;
    /**
     * 是否识别日本人名
     */
    public bool japaneseNameRecognize = false;
    /**
     * 是否识别地名
     */
    public bool placeRecognize = false;
    /**
     * 是否识别机构
     */
    public bool organizationRecognize = false;
    /**
     * 是否加载用户词典
     */
    public bool useCustomDictionary = true;
    /**
     * 用户词典高优先级
     */
    public bool forceCustomDictionary = false;
    /**
     * 词性标注
     */
    public bool speechTagging = false;
    /**
     * 命名实体识别是否至少有一项被激活
     */
    public bool ner = true;
    /**
     * 是否计算偏移量
     */
    public bool offset = false;
    /**
     * 是否识别数字和量词
     */
    public bool numberQuantifierRecognize = false;
    /**
     * 并行分词的线程数
     */
    public int threadNumber = 1;

    /**
     * 更新命名实体识别总开关
     */
    public void updateNerConfig()
    {
        ner = nameRecognize || translatedNameRecognize || japaneseNameRecognize || placeRecognize || organizationRecognize;
    }

    /**
     * 是否是索引模式
     *
     * @return
     */
    public bool isIndexMode()
    {
        return indexMode > 0;
    }
}
