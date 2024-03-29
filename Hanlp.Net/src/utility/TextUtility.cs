using com.hankcs.hanlp.corpus.document.sentence;
using com.hankcs.hanlp.corpus.document.sentence.word;
using com.hankcs.hanlp.dictionary.other;
using System.Text;

namespace com.hankcs.hanlp.utility;

/**
 * 文本工具类
 */
public class TextUtility
{

    public static int GetCharType(char c) => GetCharType(c.ToString());

    /**
     * 判断字符类型
     * @param str
     * @return
     */
    public static int GetCharType(string str)
    {
        if (str != null && str.Length > 0)
        {
            if (Predefine.CHINESE_NUMBERS.Contains(str)) return CharType.CT_CNUM;
            byte[] b;
            try
            {
                b = str.getBytes("GBK");
            }
            catch (Exception e)
            {
                b = str.ToCharArray()[0];
                //e.printStackTrace();
            }
            byte b1 = b[0];
            byte b2 = b.Length > 1 ? b[1] : (byte)0;
            int ub1 = getUnsigned(b1);
            int ub2 = getUnsigned(b2);
            if (ub1 < 128)
            {
                if (ub1 < 32) return CharType.CT_DELIMITER; // NON PRINTABLE CHARACTERS
                if (' ' == b1) return CharType.CT_OTHER;
                if ('\n' == b1) return CharType.CT_DELIMITER;
                if ("*\"!,.?()[]{}+=/\\;:|".IndexOf((char) b1) != -1)
                    return CharType.CT_DELIMITER;
                if ("0123456789".IndexOf((char)b1) != -1)
                    return CharType.CT_NUM;
                return CharType.CT_SINGLE;
            }
            else if (ub1 == 162)
                return CharType.CT_INDEX;
            else if (ub1 == 163 && ub2 > 175 && ub2 < 186)
                return CharType.CT_NUM;
            else if (ub1 == 163
                    && (ub2 >= 193 && ub2 <= 218 || ub2 >= 225
                    && ub2 <= 250))
                return CharType.CT_LETTER;
            else if (ub1 == 161 || ub1 == 163)
                return CharType.CT_DELIMITER;
            else if (ub1 >= 176 && ub1 <= 247)
                return CharType.CT_CHINESE;

        }
        return CharType.CT_OTHER;
    }

    /**
     * 是否全是中文
     * @param str
     * @return
     */
    public static bool isAllChinese(string str)
    {
        return str.Matches("[\\u4E00-\\u9FA5]+");
    }
    /**
     * 是否全部不是中文
     * @param sString
     * @return
     */
    public static bool isAllNonChinese(byte[] sString)
    {
        int nLen = sString.Length;
        int i = 0;

        while (i < nLen)
        {
            if (getUnsigned(sString[i]) < 248 && getUnsigned(sString[i]) > 175)
                return false;
            if (sString[i] < 0)
                i += 2;
            else
                i += 1;
        }
        return true;
    }

    /**
     * 是否全是单字节
     * @param str
     * @return
     */
    public static bool isAllSingleByte(string str)
    {
        //assert str != null;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] >128)
            {
                return false;
            }
        }
        return true;
    }

    /**
     * 把表示数字含义的字符串转成整形
     *
     * @param str 要转换的字符串
     * @return 如果是有意义的整数，则返回此整数值。否则，返回-1。
     */
    public static int cint(string str)
    {
        if (str != null)
            try
            {
                int.TryParse(str, out var i);
                return i;
            }
            catch (NumberFormatException e)
            {

            }

        return -1;
    }
    /**
     * 是否全是数字
     * @param str
     * @return
     */
    public static bool isAllNum(string str)
    {
        if (str == null)
            return false;

        int i = 0;
        /** 判断开头是否是+-之类的符号 */
        if ("±+-＋－—".IndexOf(str[0]) != -1)
            i++;
        /** 如果是全角的０１２３４５６７８９ 字符* */
        while (i < str.Length && "０１２３４５６７８９".IndexOf(str[i]) != -1)
            i++;
        // Get middle delimiter such as .
        if (i > 0 && i < str.Length)
        {
            char ch = str[i];
            if ("·∶:，,．.／/".IndexOf(ch) != -1)
            {// 98．1％
                i++;
                while (i < str.Length && "０１２３４５６７８９".IndexOf(str[i]) != -1)
                    i++;
            }
        }
        if (i >= str.Length)
            return true;

        /** 如果是半角的0123456789字符* */
        while (i < str.Length && "0123456789".IndexOf(str[i]) != -1)
            i++;
        // Get middle delimiter such as .
        if (i > 0 && i < str.Length)
        {
            char ch = str[i];
            if (',' == ch || '.' == ch || '/' == ch  || ':' == ch || "∶·，．／".IndexOf(ch) != -1)
            {// 98．1％
                i++;
                while (i < str.Length && "0123456789".IndexOf(str[i]) != -1)
                    i++;
            }
        }

        if (i < str.Length)
        {
            if ("百千万亿佰仟%％‰".IndexOf(str[i]) != -1)
                i++;
        }
        if (i >= str.Length)
            return true;

        return false;
    }

    /**
     * 是否全是序号
     * @param sString
     * @return
     */
    public static bool isAllIndex(byte[] sString)
    {
        int nLen = sString.Length;
        int i = 0;

        while (i < nLen - 1 && getUnsigned(sString[i]) == 162)
        {
            i += 2;
        }
        if (i >= nLen)
            return true;
        while (i < nLen && (sString[i] > 'A' - 1 && sString[i] < 'Z' + 1)
                || (sString[i] > 'a' - 1 && sString[i] < 'z' + 1))
        {// single
            // byte
            // number
            // char
            i += 1;
        }

        if (i < nLen)
            return false;
        return true;

    }

    /**
     * 是否全为英文
     *
     * @param text
     * @return
     */
    public static bool isAllLetter(string text)
    {
        for (int i = 0; i < text.Length; ++i)
        {
            char c = text[i];
            if ((((c < 'a' || c > 'z')) && ((c < 'A' || c > 'Z'))))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * 是否全为英文或字母
     *
     * @param text
     * @return
     */
    public static bool isAllLetterOrNum(string text)
    {
        for (int i = 0; i < text.Length; ++i)
        {
            char c = text[i];
            if ((((c < 'a' || c > 'z')) && ((c < 'A' || c > 'Z')) && ((c < '0' || c > '9'))))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * 是否全是分隔符
     * @param sString
     * @return
     */
    public static bool isAllDelimiter(byte[] sString)
    {
        int nLen = sString.Length;
        int i = 0;

        while (i < nLen - 1 && (getUnsigned(sString[i]) == 161 || getUnsigned(sString[i]) == 163))
        {
            i += 2;
        }
        if (i < nLen)
            return false;
        return true;
    }

    /**
     * 是否全是中国数字
     * @param word
     * @return
     */
    public static bool isAllChineseNum(string word)
    {// 百分之五点六的人早上八点十八分起床

        string chineseNum = "零○一二两三四五六七八九十廿百千万亿壹贰叁肆伍陆柒捌玖拾佰仟∶·．／点";//
        string prefix = "几数上第";
        string surfix = "几多余来成倍";
        bool round = false;

        if (word == null)
            return false;

        char[] temp = word.ToCharArray();
        for (int i = 0; i < temp.Length; i++)
        {
            if (word.StartsWith("分之", i))// 百分之五
            {
                i += 1;
                continue;
            }
            char tchar = temp[i];
            if (i == 0 && prefix.IndexOf(tchar) != -1)
            {
                round = true;
            }
            else if (i == temp.Length-1 && !round && surfix.IndexOf(tchar) != -1)
            {
                round = true;
            }
            else if (chineseNum.IndexOf(tchar) == -1)
                return false;
        }
        return true;
    }


    /**
     * 得到字符集的字符在字符串中出现的次数
     *
     * @param charSet
     * @param word
     * @return
     */
    public static int getCharCount(string charSet, string word)
    {
        int nCount = 0;

        if (word != null)
        {
            string temp = word + " ";
            for (int i = 0; i < word.Length; i++)
            {
                string s = temp[i .. (i + 1)];
                if (charSet.IndexOf(s) != -1)
                    nCount++;
            }
        }

        return nCount;
    }


    /**
     * 获取字节对应的无符号整型数
     *
     * @param b
     * @return
     */
    public static int getUnsigned(byte b)
    {
        if (b > 0)
            return (int) b;
        else
            return (b & 0x7F + 128);
    }

    /**
     * 判断字符串是否是年份
     *
     * @param snum
     * @return
     */
    public static bool isYearTime(string snum)
    {
        if (snum != null)
        {
            int len = snum.Length;
            string first = snum.Substring(0, 1);

            // 1992年, 98年,06年
            if (isAllSingleByte(snum)
                    && (len == 4 || len == 2 && (cint(first) > 4 || cint(first) == 0)))
                return true;
            if (isAllNum(snum) && (len >= 3 || len == 2 && "０５６７８９".IndexOf(first) != -1))
                return true;
            if (getCharCount("零○一二三四五六七八九壹贰叁肆伍陆柒捌玖", snum) == len && len >= 2)
                return true;
            if (len == 4 && getCharCount("千仟零○", snum) == 2)// 二仟零二年
                return true;
            if (len == 1 && getCharCount("千仟", snum) == 1)
                return true;
            if (len == 2 && getCharCount("甲乙丙丁戊己庚辛壬癸", snum) == 1
                    && getCharCount("子丑寅卯辰巳午未申酉戌亥", snum.substring(1)) == 1)
                return true;
        }
        return false;
    }

    /**
     * 判断一个字符串的所有字符是否在另一个字符串集合中
     *
     * @param aggr 字符串集合
     * @param str  需要判断的字符串
     * @return
     */
    public static bool isInAggregate(string aggr, string str)
    {
        if (aggr != null && str != null)
        {
            str += "1";
            for (int i = 0; i < str.Length; i++)
            {
                string s = str.Substring(i, 1);
                if (aggr.IndexOf(s) == -1)
                    return false;
            }
            return true;
        }

        return false;
    }

    /**
     * 判断该字符串是否是半角字符
     *
     * @param str
     * @return
     */
    public static bool isDBCCase(string str)
    {
        if (str != null)
        {
            str += " ";
            for (int i = 0; i < str.Length; i++)
            {
                string s = str.substring(i, i + 1);
                int Length = 0;
                try
                {
                    Length = s.getBytes("GBK").Length;
                }
                catch (UnsupportedEncodingException e)
                {
                    //e.printStackTrace();
                    Length = s.getBytes().Length;
                }
                if (Length != 1)
                    return false;
            }

            return true;
        }

        return false;
    }

    /**
     * 判断该字符串是否是全角字符
     *
     * @param str
     * @return
     */
    public static bool isSBCCase(string str)
    {
        if (str != null)
        {
            str += " ";
            for (int i = 0; i < str.Length; i++)
            {
                string s = str.substring(i, i + 1);
                int Length = 0;
                try
                {
                    Length = s.getBytes("GBK").Length;
                }
                catch (UnsupportedEncodingException e)
                {
                    //e.printStackTrace();
                    Length = s.getBytes().Length;
                }
                if (Length != 2)
                    return false;
            }

            return true;
        }

        return false;
    }

    /**
     * 判断是否是一个连字符（分隔符）
     *
     * @param str
     * @return
     */
    public static bool isDelimiter(string str)
    {
        if (str != null && ("-".Equals(str) || "－".Equals(str)))
            return true;
        else
            return false;
    }

    public static bool isUnknownWord(string word)
    {
        if (word != null && word.IndexOf("未##") == 0)
            return true;
        else
            return false;
    }

    /**
     * 防止频率为0发生除零错误
     *
     * @param frequency
     * @return
     */
    public static double nonZero(double frequency)
    {
        if (frequency == 0) return 1e-3;

        return frequency;
    }

    /**
     * 转换long型为char数组
     *
     * @param x
     */
    public static char[] long2char(long x)
    {
        char[] c = new char[4];
        c[0] = (char) (x >> 48);
        c[1] = (char) (x >> 32);
        c[2] = (char) (x >> 16);
        c[3] = (char) (x);
        return c;
    }

    /**
     * 转换long类型为string
     *
     * @param x
     * @return
     */
    public static string long2String(long x)
    {
        char[] cArray = long2char(x);
        StringBuilder sbResult = new StringBuilder(cArray.Length);
        foreach (char c in cArray)
        {
            sbResult.Append(c);
        }
        return sbResult.ToString();
    }

    /**
     * 将异常转为字符串
     *
     * @param e
     * @return
     */
    public static string exceptionToString(Exception e)
    {
        StringWriter sw = new StringWriter();
        TextWriter pw = new TextWriter(sw);
        e.printStackTrace(pw);
        return sw.ToString();
    }

    /**
     * 判断某个字符是否为汉字
     *
     * @param c 需要判断的字符
     * @return 是汉字返回true，否则返回false
     */
    public static bool isChinese(char c)
    {
        string regex = "[\\u4e00-\\u9fa5]";
        return string.valueOf(c).matches(regex);
    }

    /**
     * 统计 keyword 在 srcText 中的出现次数
     *
     * @param keyword
     * @param srcText
     * @return
     */
    public static int count(string keyword, string srcText)
    {
        int count = 0;
        int leng = srcText.Length;
        int j = 0;
        for (int i = 0; i < leng; i++)
        {
            if (srcText[i] == keyword[j])
            {
                j++;
                if (j == keyword.Length)
                {
                    count++;
                    j = 0;
                }
            }
            else
            {
                i = i - j;// should rollback when not match
                j = 0;
            }
        }

        return count;
    }

    /**
     * 简单好用的写string方式
     *
     * @param s
     * @param Out
     * @
     */
    public static void writeString(string s, Stream Out) 
    {
        Out.writeInt(s.Length);
        foreach (char c in s.ToCharArray())
        {
            Out.writeChar(c);
        }
    }

    /**
     * 判断字符串是否为空（null和空格）
     *
     * @param cs
     * @return
     */
    public static bool isBlank(string cs)
    {
        int strLen;
        if (cs == null || (strLen = cs.Length) == 0)
        {
            return true;
        }
        for (int i = 0; i < strLen; i++)
        {
            if (!char.IsWhiteSpace(cs[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static string join(string delimiter, ICollection<string> stringCollection)
    {
        StringBuilder sb = new StringBuilder(stringCollection.Count * (16 + delimiter.Length));
        foreach (string str in stringCollection)
        {
            sb.Append(str).Append(delimiter);
        }

        return sb.ToString();
    }

    public static string combine(params string[] termArray)
    {
        StringBuilder sbSentence = new StringBuilder();
        foreach (string word in termArray)
        {
            sbSentence.Append(word);
        }
        return sbSentence.ToString();
    }

    public static string join(IEnumerator<string> s, string delimiter)
    {
        IEnumerator<char> iter = s.GetEnumerator();
        if (!iter.MoveNext()) return "";
        StringBuilder buffer = new StringBuilder(iter.Current);
        while (iter.MoveNext()) buffer.Append(delimiter).Append(iter.Current);
        return buffer.ToString();
    }

    public static string combine(Sentence sentence)
    {
        var sb = new StringBuilder(sentence.wordList.Count * 3);
        foreach (IWord word in sentence.wordList)
        {
            sb.Append(word.Value);
        }

        return sb.ToString();
    }

    public static string combine(List<Word> wordList)
    {
        var sb = new StringBuilder(wordList.Count * 3);
        foreach (IWord word in wordList)
        {
            sb.Append(word.Value);
        }

        return sb.ToString();
    }
}
