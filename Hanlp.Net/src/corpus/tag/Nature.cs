/*
 * <summary></summary>
 * <author>He Han</author>
 * <email>hankcs.cn@gmail.com</email>
 * <create-date>2014/5/10 14:34</create-date>
 *
 * <copyright file="Nature.java" company="上海林原信息科技有限公司">
 * Copyright (c) 2003-2014, 上海林原信息科技有限公司. All Right Reserved, http://www.linrunsoft.com/
 * This source is subject to the LinrunSpace License. Please contact 上海林原信息科技有限公司 to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.tag;


/**
 * 词性
 *
 * @author hankcs
 */
public class Nature
{
    /**
     * 区别语素
     */
    public static readonly Nature bg = new Nature("bg");

    /**
     * 数语素
     */
    public static readonly Nature mg = new Nature("mg");

    /**
     * 名词性惯用语
     */
    public static readonly Nature nl = new Nature("nl");

    /**
     * 字母专名
     */
    public static readonly Nature nx = new Nature("nx");

    /**
     * 量词语素
     */
    public static readonly Nature qg = new Nature("qg");

    /**
     * 助词
     */
    public static readonly Nature ud = new Nature("ud");

    /**
     * 助词
     */
    public static readonly Nature uj = new Nature("uj");

    /**
     * 着
     */
    public static readonly Nature uz = new Nature("uz");

    /**
     * 过
     */
    public static readonly Nature ug = new Nature("ug");

    /**
     * 连词
     */
    public static readonly Nature ul = new Nature("ul");

    /**
     * 连词
     */
    public static readonly Nature uv = new Nature("uv");

    /**
     * 语气语素
     */
    public static readonly Nature yg = new Nature("yg");

    /**
     * 状态词
     */
    public static readonly Nature zg = new Nature("zg");

    // 以上标签来自ICT，以下标签来自北大

    /**
     * 名词
     */
    public static readonly Nature n = new Nature("n");

    /**
     * 人名
     */
    public static readonly Nature nr = new Nature("nr");

    /**
     * 日语人名
     */
    public static readonly Nature nrj = new Nature("nrj");

    /**
     * 音译人名
     */
    public static readonly Nature nrf = new Nature("nrf");

    /**
     * 复姓
     */
    public static readonly Nature nr1 = new Nature("nr1");

    /**
     * 蒙古姓名
     */
    public static readonly Nature nr2 = new Nature("nr2");

    /**
     * 地名
     */
    public static readonly Nature ns = new Nature("ns");

    /**
     * 音译地名
     */
    public static readonly Nature nsf = new Nature("nsf");

    /**
     * 机构团体名
     */
    public static readonly Nature nt = new Nature("nt");

    /**
     * 公司名
     */
    public static readonly Nature ntc = new Nature("ntc");

    /**
     * 工厂
     */
    public static readonly Nature ntcf = new Nature("ntcf");

    /**
     * 银行
     */
    public static readonly Nature ntcb = new Nature("ntcb");

    /**
     * 酒店宾馆
     */
    public static readonly Nature ntch = new Nature("ntch");

    /**
     * 政府机构
     */
    public static readonly Nature nto = new Nature("nto");

    /**
     * 大学
     */
    public static readonly Nature ntu = new Nature("ntu");

    /**
     * 中小学
     */
    public static readonly Nature nts = new Nature("nts");

    /**
     * 医院
     */
    public static readonly Nature nth = new Nature("nth");

    /**
     * 医药疾病等健康相关名词
     */
    public static readonly Nature nh = new Nature("nh");

    /**
     * 药品
     */
    public static readonly Nature nhm = new Nature("nhm");

    /**
     * 疾病
     */
    public static readonly Nature nhd = new Nature("nhd");

    /**
     * 工作相关名词
     */
    public static readonly Nature nn = new Nature("nn");

    /**
     * 职务职称
     */
    public static readonly Nature nnt = new Nature("nnt");

    /**
     * 职业
     */
    public static readonly Nature nnd = new Nature("nnd");

    /**
     * 名词性语素
     */
    public static readonly Nature ng = new Nature("ng");

    /**
     * 食品，比如“薯片”
     */
    public static readonly Nature nf = new Nature("nf");

    /**
     * 机构相关（不是独立机构名）
     */
    public static readonly Nature ni = new Nature("ni");

    /**
     * 教育相关机构
     */
    public static readonly Nature nit = new Nature("nit");

    /**
     * 下属机构
     */
    public static readonly Nature nic = new Nature("nic");

    /**
     * 机构后缀
     */
    public static readonly Nature nis = new Nature("nis");

    /**
     * 物品名
     */
    public static readonly Nature nm = new Nature("nm");

    /**
     * 化学品名
     */
    public static readonly Nature nmc = new Nature("nmc");

    /**
     * 生物名
     */
    public static readonly Nature nb = new Nature("nb");

    /**
     * 动物名
     */
    public static readonly Nature nba = new Nature("nba");

    /**
     * 动物纲目
     */
    public static readonly Nature nbc = new Nature("nbc");

    /**
     * 植物名
     */
    public static readonly Nature nbp = new Nature("nbp");

    /**
     * 其他专名
     */
    public static readonly Nature nz = new Nature("nz");

    /**
     * 学术词汇
     */
    public static readonly Nature g = new Nature("g");

    /**
     * 数学相关词汇
     */
    public static readonly Nature gm = new Nature("gm");

    /**
     * 物理相关词汇
     */
    public static readonly Nature gp = new Nature("gp");

    /**
     * 化学相关词汇
     */
    public static readonly Nature gc = new Nature("gc");

    /**
     * 生物相关词汇
     */
    public static readonly Nature gb = new Nature("gb");

    /**
     * 生物类别
     */
    public static readonly Nature gbc = new Nature("gbc");

    /**
     * 地理地质相关词汇
     */
    public static readonly Nature gg = new Nature("gg");

    /**
     * 计算机相关词汇
     */
    public static readonly Nature gi = new Nature("gi");

    /**
     * 简称略语
     */
    public static readonly Nature j = new Nature("j");

    /**
     * 成语
     */
    public static readonly Nature i = new Nature("i");

    /**
     * 习用语
     */
    public static readonly Nature l = new Nature("l");

    /**
     * 时间词
     */
    public static readonly Nature t = new Nature("t");

    /**
     * 时间词性语素
     */
    public static readonly Nature tg = new Nature("tg");

    /**
     * 处所词
     */
    public static readonly Nature s = new Nature("s");

    /**
     * 方位词
     */
    public static readonly Nature f = new Nature("f");

    /**
     * 动词
     */
    public static readonly Nature v = new Nature("v");

    /**
     * 副动词
     */
    public static readonly Nature vd = new Nature("vd");

    /**
     * 名动词
     */
    public static readonly Nature vn = new Nature("vn");

    /**
     * 动词“是”
     */
    public static readonly Nature vshi = new Nature("vshi");

    /**
     * 动词“有”
     */
    public static readonly Nature vyou = new Nature("vyou");

    /**
     * 趋向动词
     */
    public static readonly Nature vf = new Nature("vf");

    /**
     * 形式动词
     */
    public static readonly Nature vx = new Nature("vx");

    /**
     * 不及物动词（内动词）
     */
    public static readonly Nature vi = new Nature("vi");

    /**
     * 动词性惯用语
     */
    public static readonly Nature vl = new Nature("vl");

    /**
     * 动词性语素
     */
    public static readonly Nature vg = new Nature("vg");

    /**
     * 形容词
     */
    public static readonly Nature a = new Nature("a");

    /**
     * 副形词
     */
    public static readonly Nature ad = new Nature("ad");

    /**
     * 名形词
     */
    public static readonly Nature an = new Nature("an");

    /**
     * 形容词性语素
     */
    public static readonly Nature ag = new Nature("ag");

    /**
     * 形容词性惯用语
     */
    public static readonly Nature al = new Nature("al");

    /**
     * 区别词
     */
    public static readonly Nature b = new Nature("b");

    /**
     * 区别词性惯用语
     */
    public static readonly Nature bl = new Nature("bl");

    /**
     * 状态词
     */
    public static readonly Nature z = new Nature("z");

    /**
     * 代词
     */
    public static readonly Nature r = new Nature("r");

    /**
     * 人称代词
     */
    public static readonly Nature rr = new Nature("rr");

    /**
     * 指示代词
     */
    public static readonly Nature rz = new Nature("rz");

    /**
     * 时间指示代词
     */
    public static readonly Nature rzt = new Nature("rzt");

    /**
     * 处所指示代词
     */
    public static readonly Nature rzs = new Nature("rzs");

    /**
     * 谓词性指示代词
     */
    public static readonly Nature rzv = new Nature("rzv");

    /**
     * 疑问代词
     */
    public static readonly Nature ry = new Nature("ry");

    /**
     * 时间疑问代词
     */
    public static readonly Nature ryt = new Nature("ryt");

    /**
     * 处所疑问代词
     */
    public static readonly Nature rys = new Nature("rys");

    /**
     * 谓词性疑问代词
     */
    public static readonly Nature ryv = new Nature("ryv");

    /**
     * 代词性语素
     */
    public static readonly Nature rg = new Nature("rg");

    /**
     * 古汉语代词性语素
     */
    public static readonly Nature Rg = new Nature("Rg");

    /**
     * 数词
     */
    public static readonly Nature m = new Nature("m");

    /**
     * 数量词
     */
    public static readonly Nature mq = new Nature("mq");

    /**
     * 甲乙丙丁之类的数词
     */
    public static readonly Nature Mg = new Nature("Mg");

    /**
     * 量词
     */
    public static readonly Nature q = new Nature("q");

    /**
     * 动量词
     */
    public static readonly Nature qv = new Nature("qv");

    /**
     * 时量词
     */
    public static readonly Nature qt = new Nature("qt");

    /**
     * 副词
     */
    public static readonly Nature d = new Nature("d");

    /**
     * 辄,俱,复之类的副词
     */
    public static readonly Nature dg = new Nature("dg");

    /**
     * 连语
     */
    public static readonly Nature dl = new Nature("dl");

    /**
     * 介词
     */
    public static readonly Nature p = new Nature("p");

    /**
     * 介词“把”
     */
    public static readonly Nature pba = new Nature("pba");

    /**
     * 介词“被”
     */
    public static readonly Nature pbei = new Nature("pbei");

    /**
     * 连词
     */
    public static readonly Nature c = new Nature("c");

    /**
     * 并列连词
     */
    public static readonly Nature cc = new Nature("cc");

    /**
     * 助词
     */
    public static readonly Nature u = new Nature("u");

    /**
     * 着
     */
    public static readonly Nature uzhe = new Nature("uzhe");

    /**
     * 了 喽
     */
    public static readonly Nature ule = new Nature("ule");

    /**
     * 过
     */
    public static readonly Nature uguo = new Nature("uguo");

    /**
     * 的 底
     */
    public static readonly Nature ude1 = new Nature("ude1");

    /**
     * 地
     */
    public static readonly Nature ude2 = new Nature("ude2");

    /**
     * 得
     */
    public static readonly Nature ude3 = new Nature("ude3");

    /**
     * 所
     */
    public static readonly Nature usuo = new Nature("usuo");

    /**
     * 等 等等 云云
     */
    public static readonly Nature udeng = new Nature("udeng");

    /**
     * 一样 一般 似的 般
     */
    public static readonly Nature uyy = new Nature("uyy");

    /**
     * 的话
     */
    public static readonly Nature udh = new Nature("udh");

    /**
     * 来讲 来说 而言 说来
     */
    public static readonly Nature uls = new Nature("uls");

    /**
     * 之
     */
    public static readonly Nature uzhi = new Nature("uzhi");

    /**
     * 连 （“连小学生都会”）
     */
    public static readonly Nature ulian = new Nature("ulian");

    /**
     * 叹词
     */
    public static readonly Nature e = new Nature("e");

    /**
     * 语气词(delete yg)
     */
    public static readonly Nature y = new Nature("y");

    /**
     * 拟声词
     */
    public static readonly Nature o = new Nature("o");

    /**
     * 前缀
     */
    public static readonly Nature h = new Nature("h");

    /**
     * 后缀
     */
    public static readonly Nature k = new Nature("k");

    /**
     * 字符串
     */
    public static readonly Nature x = new Nature("x");

    /**
     * 非语素字
     */
    public static readonly Nature xx = new Nature("xx");

    /**
     * 网址URL
     */
    public static readonly Nature xu = new Nature("xu");

    /**
     * 标点符号
     */
    public static readonly Nature w = new Nature("w");

    /**
     * 左括号，全角：（ 〔  ［  ｛  《 【  〖 〈   半角：( [ { <
     */
    public static readonly Nature wkz = new Nature("wkz");

    /**
     * 右括号，全角：） 〕  ］ ｝ 》  】 〗 〉 半角： ) ] { >
     */
    public static readonly Nature wky = new Nature("wky");

    /**
     * 左引号，全角：“ ‘ 『
     */
    public static readonly Nature wyz = new Nature("wyz");

    /**
     * 右引号，全角：” ’ 』
     */
    public static readonly Nature wyy = new Nature("wyy");

    /**
     * 句号，全角：。
     */
    public static readonly Nature wj = new Nature("wj");

    /**
     * 问号，全角：？ 半角：?
     */
    public static readonly Nature ww = new Nature("ww");

    /**
     * 叹号，全角：！ 半角：!
     */
    public static readonly Nature wt = new Nature("wt");

    /**
     * 逗号，全角：， 半角：,
     */
    public static readonly Nature wd = new Nature("wd");

    /**
     * 分号，全角：； 半角： ;
     */
    public static readonly Nature wf = new Nature("wf");

    /**
     * 顿号，全角：、
     */
    public static readonly Nature wn = new Nature("wn");

    /**
     * 冒号，全角：： 半角： :
     */
    public static readonly Nature wm = new Nature("wm");

    /**
     * 省略号，全角：……  …
     */
    public static readonly Nature ws = new Nature("ws");

    /**
     * 破折号，全角：——   －－   ——－   半角：---  ----
     */
    public static readonly Nature wp = new Nature("wp");

    /**
     * 百分号千分号，全角：％ ‰   半角：%
     */
    public static readonly Nature wb = new Nature("wb");

    /**
     * 单位符号，全角：￥ ＄ ￡  °  ℃  半角：$
     */
    public static readonly Nature wh = new Nature("wh");

    /**
     * 仅用于终##终，不会出现在分词结果中
     */
    public static readonly Nature end = new Nature("end");

    /**
     * 仅用于始##始，不会出现在分词结果中
     */
    public static readonly Nature begin = new Nature("begin");

    private static Dictionary<string, int> idMap = new Dictionary<string, int>();
    public static List<Nature> values = new();
    private int ordinal;
    private string name;
    private Nature(string name)
    {
        if (idMap == null) idMap = new ();
        //assert !idMap.ContainsKey(name);
        this.name = name;
        ordinal = idMap.Count;
        idMap.Add(name, ordinal);
        values.Add(this);
        //Nature[] extended = new Nature[idMap.Count];
        //if (values != null)
        //    Array.Copy(values, 0, extended, 0, values.Length);
        //extended[ordinal] = this;
        //values = extended;
    }

    /**
     * 词性是否以该前缀开头<br>
     * 词性根据开头的几个字母可以判断大的类别
     *
     * @param prefix 前缀
     * @return 是否以该前缀开头
     */
    public bool StartsWith(string prefix) => name.StartsWith(prefix);

    /**
     * 词性是否以该前缀开头<br>
     * 词性根据开头的几个字母可以判断大的类别
     *
     * @param prefix 前缀
     * @return 是否以该前缀开头
     */
    public bool StartsWith(char prefix) => name[0] == prefix;

    /**
     * 词性的首字母<br>
     * 词性根据开头的几个字母可以判断大的类别
     *
     * @return
     */
    public char FirstChar => name[0];

    /**
     * 安全地将字符串类型的词性转为Enum类型，如果未定义该词性，则返回null
     *
     * @param name 字符串词性
     * @return Enum词性
     */
    public static Nature fromString(string name)
    {
        int id = idMap[name];
        if (id == null)
            return null;
        return values[id];
    }

    /**
     * 创建自定义词性,如果已有该对应词性,则直接返回已有的词性
     *
     * @param name 字符串词性
     * @return Enum词性
     */
    public static Nature create(string name)
    {
        Nature nature = fromString(name);
        if (nature == null)
            return new Nature(name);
        return nature;
    }
    
    //@Override
    public override string ToString()
    {
        return name;
    }

    public int Ordinal => ordinal;

    public static List<Nature> Values => values;
}
